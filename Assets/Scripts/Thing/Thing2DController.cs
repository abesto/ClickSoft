using UnityEngine;
using System.Collections;

public class Thing2DController : MonoBehaviour {
	public SpriteRenderer greenInsideRenderer;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		float cutoff = Mathf.InverseLerp (0, Screen.width, Input.mousePosition.x);
		greenInsideRenderer.material.SetFloat("_Cutoff", cutoff); 
	}
}
