using UnityEngine;
using System.Collections;

abstract class ThingState {
	protected ThingController controller;
	public ThingState(ThingController controller) {
		this.controller = controller;
		Enter ();  // Not nice having this here, but this is a throw-away state machine, so it's fine
	}
	
	protected bool IsHovered() {
		return HoverUtil.Instance.IsHovered (controller.gameObject);
	}
	
	virtual protected void Enter() {}
	
	virtual public ThingState Act() { return this; }
}

class NormalState: ThingState {
	public NormalState(ThingController controller): base(controller) {}
	
	override protected void Enter() {
		controller.ResetColor ();
		controller.progressTextColor = Color.white;
	}
	
	override public ThingState Act () {
		if (IsHovered()) {
			return new HighlightedState (controller);
		} else {
			return this;
		}
	}
}

class HighlightedState: ThingState {
	public HighlightedState(ThingController controller): base(controller) {}
	
	override protected void Enter() {
		controller.colorPreservingAlpha = Color.cyan;
		controller.progressTextColor = Color.cyan;
	}
	
	override public ThingState Act () {
		if (!IsHovered()) {
			return new NormalState (controller);
		}
		if (!Input.GetMouseButtonDown (0)) {
			return this;
		}
		ToolManager.Instance.tool.Act (controller);
		if (controller.done) {
			return new DoneState (controller);
		} else {
			return this;
		}
	}
}

class DoneState: ThingState {
	public DoneState(ThingController controller): base(controller) {}
	
	override protected void Enter() {
		controller.color = Color.green;
		controller.progressTextColor = Color.green;
		controller.EmitDoneParticles ();
	}
}

public class ThingController : MonoBehaviour {	
	public ParticleSystem prefabDoneSparks;
	public TextMesh prefabProgressText;
	public byte progressOnClick = 10;
	public byte initialProgress = 10;

	private ParticleSystem myDoneSparks;
	private TextMesh progressTextMesh;
	private MeshRenderer thing;
	private Color initialColor;
	private ThingState state;

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

	void Start () {
		thing = GetComponent<MeshRenderer> ();
		// Initialize "Done" sparks
		myDoneSparks = Instantiate (prefabDoneSparks, transform.position, transform.rotation) as ParticleSystem;
		myDoneSparks.transform.SetParent (transform);
		myDoneSparks.transform.localScale = new Vector3 (1, 1, 1);
		// Initialize Progress text
		progressTextMesh = Instantiate (prefabProgressText, transform.position, prefabProgressText.transform.rotation) as TextMesh;
		progressTextMesh.transform.SetParent (transform);
		progressTextMesh.transform.Translate (Vector3.up * transform.localScale.z / 2, Space.World);
		// Set initial progress
		progress = initialProgress;
		initialColor = color;
		state = new NormalState (this);
	}

	void Update () {
		state = state.Act ();
	}
}
