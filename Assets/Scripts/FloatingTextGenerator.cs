using UnityEngine;
using System.Collections;

public class FloatingTextGenerator : MonoBehaviour {
	public GameObject textPrefab;
	public float scrollPerFrame = 0.01f;
	
	public void CreateAndStart(GameObject floatAbove, string str, float duration) {
		GameObject text = Instantiate (textPrefab, floatAbove.transform.position, textPrefab.transform.rotation) as GameObject;
		TextMesh tm = text.GetComponent<TextMesh> ();
		tm.text = str;
		text.transform.SetParent (floatAbove.transform);	
		text.transform.Translate (Vector3.up * floatAbove.transform.localScale.y / 2 + 
		                          Vector3.up * floatAbove.transform.Find ("ProgressText").GetComponent<MeshRenderer>().bounds.size.y,
		                          Space.World);
		tm.name = "(Floating text) " + str;
		StartCoroutine (Tick (text, duration));
	}
	
	private IEnumerator Tick(GameObject text, float fullDuration) {
		float durationLeft = fullDuration;
		while (durationLeft > 0) {
			durationLeft -= Time.deltaTime;
			TextMesh tm = text.GetComponent<TextMesh> ();
			Color c = tm.color;
			c.a = durationLeft / fullDuration;
			tm.color = c;
			text.transform.Translate (Vector3.up * scrollPerFrame, Space.World);
			yield return null;
		}
		Object.Destroy ((Object) text);
	}
}