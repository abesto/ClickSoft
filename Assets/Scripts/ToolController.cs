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
	public abstract ToolName name { get; }
}

public class WorkTool: Tool {
	override public ToolName name {
		get { return ToolName.WORK; }
	}

	override public void Act (ThingController thing) {
		thing.commitWork ();
	}
}

public class AddDevTool: Tool {
	override public ToolName name {
		get { return ToolName.ADD_DEV; }
	}

	override public void Act (ThingController thing) {
		GameObject workForce = GameObject.FindGameObjectWithTag ("WorkForce");
		Developer newDev = workForce.AddComponent<Developer> ();
		newDev.target = thing;
	}
}

public class RemoveDevTool: Tool {
	override public ToolName name {
		get { return ToolName.REMOVE_DEV; }
	}
	
	override public void Act (ThingController thing) {
		GameObject workForce = GameObject.FindGameObjectWithTag ("WorkForce");
		foreach (Developer dev in workForce.GetComponents<Developer>()) {
			if (dev.target == thing) {
				dev.target = null;
				return;
			}
		}
	}
}


public class ToolManager: Singleton<ToolManager> {
	public Tool tool;

	public void ActivateTool(string strName) {
		ToolName newName = (ToolName) Enum.Parse (typeof(ToolName), strName);
		if (tool != null && newName == tool.name) {
			return;
		}
		Debug.LogFormat("Activating tool {0} (replaces {1} as the active tool)", newName, (tool == null ? "nothing" : tool.name.ToString()));
		switch(newName) {
		case ToolName.WORK:
			ToolManager.Instance.tool = new WorkTool();
			break;
		case ToolName.ADD_DEV:
			ToolManager.Instance.tool = new AddDevTool();
			break;
		case ToolName.REMOVE_DEV:
			ToolManager.Instance.tool = new RemoveDevTool();
			break;
		default:
			Debug.LogErrorFormat("Unknown tool name {0}", newName);
			break;
		}
	}
}

public class ToolController : MonoBehaviour {

	void Start () {
		foreach (Toggle _toggle in GetComponentsInChildren<Toggle>()) {
			Toggle toggle = _toggle;  // Because loops, references and closures lead to sadness
			Debug.LogFormat ("Found tool {0} in toolbar", toggle.name);
			toggle.onValueChanged.AddListener((activated) => {
				if (activated) {
					ToolManager.Instance.ActivateTool(toggle.name);
				}
			});
			if (toggle.isOn) {
				Debug.LogFormat ("UI Toggle for tool {0} is on at startup, activating", toggle.name);
				ToolManager.Instance.ActivateTool(toggle.name);
			}
		}
	}
}
