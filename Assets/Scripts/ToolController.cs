using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public enum ToolName {
	WORK,
	ADD_DEV,
	REMOVE_DEV
}

public abstract class Tool {
	public abstract void Act (ThingController thing);
}

public class WorkTool: Tool {
	override public void Act (ThingController thing) {
		thing.progress += thing.progressOnClick;
	}
}

public class NoopTool: Tool {
	override public void Act (ThingController thing) {
	}
}

public class ToolManager: Singleton<ToolManager> {
	public Tool tool;
}

public class ToolController : MonoBehaviour {
	public void ActivateTool(string name) {
		ToolName n = (ToolName) Enum.Parse (typeof(ToolName), name);
		switch(n) {
		case ToolName.WORK:
			ToolManager.Instance.tool = new WorkTool();
			break;
		default:
			ToolManager.Instance.tool = new NoopTool();
			break;
		}
		Debug.LogFormat("Activated tool {0}", n);
	}

	void Start () {
		foreach (Toggle _toggle in GetComponentsInChildren<Toggle>()) {
			Toggle toggle = _toggle;  // Because loops, references and closures lead to sadness
			Debug.LogFormat ("Found tool {0} in toolbar", toggle.name);
			toggle.onValueChanged.AddListener((activated) => {
				if (activated) {
					ActivateTool(toggle.name);
				} else {
					Debug.LogFormat("Deactivated tool {0}", Enum.Parse (typeof(ToolName), toggle.name));
				}
			});
			if (toggle.isOn) {
				Debug.LogFormat ("Toggle for tool {0} is on, activating", toggle.name);
				ActivateTool(toggle.name);
			}
		}
	}
}
