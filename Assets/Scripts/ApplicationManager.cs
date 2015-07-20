using UnityEngine;
using System.Collections;

public class ApplicationManager : MonoBehaviour {
	

	public void Quit () 
	{
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#else
		Application.Quit();
		#endif
	}

	public void NewGame ()
	{
		Application.LoadLevel("scene1");
	}

	public void Update ()
	{
		if (Input.GetAxis ("Cancel") > 0) {
			if (Application.loadedLevelName != "menu") {
				Application.LoadLevel("menu");
			}
		}
	}

}
