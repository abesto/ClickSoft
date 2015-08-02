using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;

public class ThingController : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IHighlightable {	
	public ParticleSystem prefabDoneSparks;
	public TextMesh prefabProgressText;
	public byte progressOnClick = 10;
	public byte initialProgress = 10;

	private ParticleSystem myDoneSparks;
	private TextMesh progressTextMesh;
	private MeshRenderer thing;
	private Color initialColor;
	private ThingState state;

	public ThingHighlighter highlighter;

	private byte _progress;
	public byte progress {
		get { return _progress; }
		set {
			if (value > 100) {
				value = 100;
			}
			byte before = _progress;
			_progress = value;
			alpha = (float)(value) / 100f;
			progressText = value;
			if (ProgressChanged != null) {
				ProgressChanged (this, new ProgressEventArgs (before, value));
			}
		}
	}

	public event EventHandler<ProgressEventArgs> ProgressChanged;
		
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
		this.UpdateState ();
	}

	void Start () {
		thing = GetComponent<MeshRenderer> ();
		highlighter = GetComponent<ThingHighlighter> ();
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

	private void UpdateState () {
		ThingState next = state.Act ();
		if (next != state) {
			Debug.LogFormat ("{0}: {1} -> {2}", name, state.GetType ().Name, next, GetType ().Name);
			state = state.Act ();
		}
	}
	
	public void OnHighlight() {
		colorPreservingAlpha = Color.cyan;
		progressTextColor = Color.cyan;
	}

	public void OnDehighlight() {
		color = state.Color();
		progressTextColor = state.ProgressTextColor();
	}

	public void OnPointerClick (PointerEventData eventData)
	{
		ToolManager.Instance.tool.Act (this);
	}

	public void OnPointerEnter (PointerEventData eventData)
	{
		highlighter.On (ThingHighlightSource.MOUSE);
	}

	public void OnPointerExit (PointerEventData eventData)
	{
		highlighter.Off (ThingHighlightSource.MOUSE);
	}
}

public class ProgressEventArgs : EventArgs
{
	public byte before;
	public byte after;

	public ProgressEventArgs(byte before, byte after) {
		this.before = before;
		this.after = after;
	}
}