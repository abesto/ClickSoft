using UnityEngine;
using System.Collections;


public class ThingController : MonoBehaviour {	
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
			controller.SetColorPreservingAlpha (controller.initialColor);
			controller.progressText.color = Color.white;
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
			controller.SetColorPreservingAlpha (Color.cyan);
			controller.progressText.color = Color.cyan;
		}

		override public ThingState Act () {
			if (!IsHovered()) {
				return new NormalState (controller);
			}
			if (!Input.GetMouseButtonDown (0)) {
				return this;
			}
			ToolManager.Instance.tool.Act (controller);
			if (controller.IsDone ()) {
				return new DoneState (controller);
			} else {
				return this;
			}
		}
	}

	class DoneState: ThingState {
		public DoneState(ThingController controller): base(controller) {}

		override protected void Enter() {
			controller.SetColorPreservingAlpha (Color.green);
			controller.progressText.color = Color.green;
			controller.EmitDoneParticles ();
		}
	}

	public ParticleSystem prefabDoneSparks;
	public TextMesh prefabProgressText;
	public byte progressOnClick = 10;
	public byte initialProgress = 10;

	private ParticleSystem myDoneSparks;
	private TextMesh progressText;
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
			SetAlpha (value / 100f);
			SetProgressText (value);
		}
	}

	private Color color {
		get { return thing.materials [0].color; }
		set { thing.materials [0].color = value; }
	}
	
	bool IsDone() {
		return progress == 100;
	}

	void EmitDoneParticles() {
		myDoneSparks.Play ();
	}

	void SetAlpha(float alpha) {
		Color c = color;
		c.a = alpha;
		color = c;
	}

	void SetProgressText(int value) {
		progressText.text = value.ToString() + "%";
	}

	void SetColorPreservingAlpha(Color to) {
		to.a = color.a;
		color = to;
	}

	void Start () {
		thing = GetComponent<MeshRenderer> ();
		// Initialize "Done" sparks
		myDoneSparks = Instantiate (prefabDoneSparks, transform.position, transform.rotation) as ParticleSystem;
		myDoneSparks.transform.SetParent (transform);
		myDoneSparks.transform.localScale = new Vector3 (1, 1, 1);
		// Initialize Progress text
		progressText = Instantiate (prefabProgressText, transform.position, prefabProgressText.transform.rotation) as TextMesh;
		progressText.transform.SetParent (transform);
		progressText.transform.Translate (Vector3.up * transform.localScale.z / 2, Space.World);
		// Set initial progress
		progress = initialProgress;
		initialColor = color;
		state = new NormalState (this);
	}

	void Update () {
		state = state.Act ();
	}
}
