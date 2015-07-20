using UnityEngine;
using System.Collections;

// Do at most one ray-casting to the mouse per frame for detecting which object is hovered
public class HoverUtil : Singleton<HoverUtil> {
	protected HoverUtil() {}

	private int lastCheckedFrame;
	private bool checkedThisFrame {
		get {
			return lastCheckedFrame == Time.frameCount;
		}
	}

	private GameObject hoveredCache;

	public GameObject hovered {
		get {
			if (!checkedThisFrame) {
				RaycastHit hit;
				if (Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit)) {
					hoveredCache = hit.transform.gameObject;
				} else {
					hoveredCache = null;
				}
				lastCheckedFrame = Time.frameCount;
			}
			return hoveredCache;
		}
	}

	public bool IsHovered(GameObject o) {
		return o == hovered;
	}
}
