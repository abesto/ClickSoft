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

public abstract class Tool {}

public class ToolController : MonoBehaviour {
	private Tool tool;

	public void ActivateTool(string name) {
		Debug.LogFormat("Activated tool {0}", Enum.Parse (typeof(ToolName), name));
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
