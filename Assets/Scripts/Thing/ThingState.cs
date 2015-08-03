using UnityEngine;

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
