using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

abstract class ThingState {
	protected ThingController controller;
	public ThingState(ThingController controller) {
		this.controller = controller;
		Enter ();  // Not nice having this here, but this is a throw-away state machine, so it's fine
	}
	
	virtual protected void Enter() {
		controller.progressTextColor = ProgressTextColor ();
		controller.color = Color ();
	}
	
	virtual public ThingState Act() { return this; }
	abstract public Color ProgressTextColor ();
	abstract public Color Color ();
}

class NormalState: ThingState {
	public NormalState(ThingController controller): base(controller) {}
	
	override public ThingState Act () {
		if (controller.done) {
			return new DoneState (controller);
		} else {
			return this;
		}
	}

	public override UnityEngine.Color ProgressTextColor ()
	{
		return UnityEngine.Color.white;
	}

	public override UnityEngine.Color Color ()
	{
		return UnityEngine.Color.blue;
	}
}

class DoneState: ThingState {
	public DoneState(ThingController controller): base(controller) {}
	
	override protected void Enter() {
		controller.EmitDoneParticles ();
		base.Enter ();
	}

	public override UnityEngine.Color ProgressTextColor ()
	{
		return UnityEngine.Color.green;
	}

	public override UnityEngine.Color Color ()
	{
		return UnityEngine.Color.green;
	}
}

public class ThingController : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {	
	public ParticleSystem prefabDoneSparks;
	public TextMesh prefabProgressText;
	public byte progressOnClick = 10;
	public byte initialProgress = 10;

	private ParticleSystem myDoneSparks;
	private TextMesh progressTextMesh;
	private MeshRenderer thing;
	private Color initialColor;
	private ThingState state;

	public bool mouseHighlight = false;
	public bool menuHighlight = false;

	private byte _progress;
	public byte progress {
		get { return _progress; }
		set {
			if (value > 100) {
				value = 100;
			}
			_progress = value;
			alpha = (float)(value) / 100f;
			progressText = value;
		}
	}
		
	private int progressText {
		set { progressTextMesh.text = value.ToString() + "%"; }
	}
	
	private float alpha {
		get { return color.a; }
		set {
			Color c = color;
			c.a = value;
			color = c;
		}
	}

	public Color color {
		get { return thing.materials [0].color; }
		set { 
			thing.materials [0].color = value; 
		}
	}

	public Color colorPreservingAlpha {
		set { 
			value.a = color.a;
			color = value;
		}
	}

	public void ResetColor() {
		color = initialColor;
	}

	public Color progressTextColor {
		get { return progressTextMesh.color; }
		set { progressTextMesh.color = value; }
	}

	public bool done {
		get { return progress == 100; }
	}

	
	public void EmitDoneParticles() {
		myDoneSparks.Play ();
	}

	public void commitWork() {
		if (done) {
			return;
		}
		this.progress += this.progressOnClick;
		this.GetComponentInParent<FloatingTextGenerator> ().CreateAndStart (this.gameObject, CommitMessages.GetRandom (), 3f);
	}

	void Start () {
		thing = GetComponent<MeshRenderer> ();
		// Initialize "Done" sparks
		myDoneSparks = Instantiate (prefabDoneSparks, transform.position, prefabDoneSparks.transform.rotation) as ParticleSystem;
		myDoneSparks.name = "DoneSparks";
		myDoneSparks.transform.SetParent (transform);
		myDoneSparks.transform.Translate (Vector3.up * GetComponent<MeshRenderer>().bounds.extents.y, Space.World);
		// Initialize Progress text
		progressTextMesh = Instantiate (prefabProgressText, transform.position, prefabProgressText.transform.rotation) as TextMesh;
		progressTextMesh.name = "ProgressText";
		progressTextMesh.transform.SetParent (transform);
		progressTextMesh.transform.Translate (Vector3.up * GetComponent<MeshRenderer>().bounds.extents.y, Space.World);
		// Set initial progress
		progress = initialProgress;
		initialColor = color;
		state = new NormalState (this);
	}

	void Update () {
		state = state.Act ();
		updateHighlight ();
	}

	void updateHighlight () {
		if (mouseHighlight || menuHighlight) {
			colorPreservingAlpha = Color.cyan;
			progressTextColor = Color.cyan;
		} else {
			color = state.Color();
			progressTextColor = state.ProgressTextColor();
		}
	}

	public void OnPointerClick (PointerEventData eventData)
	{
		ToolManager.Instance.tool.Act (this);
	}

	public void OnPointerEnter (PointerEventData eventData)
	{
		mouseHighlight = true;
	}

	public void OnPointerExit (PointerEventData eventData)
	{
		mouseHighlight = false;
	}
}
