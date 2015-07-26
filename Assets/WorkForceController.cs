using UnityEngine;
using System.Collections;

public class WorkForceController : MonoBehaviour {

	public GameObject developer;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void addDeveloper (ThingController thing) {
		GameObject newDev = Instantiate (developer);
		newDev.transform.SetParent (transform, true);
		newDev.GetComponent<Developer> ().target = thing;
	}
}
