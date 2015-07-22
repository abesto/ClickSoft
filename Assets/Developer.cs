using UnityEngine;
using System.Collections;

public class Developer : MonoBehaviour
{

	public float efficiency = 0.002f;
	public ThingController target = null;

	// Use this for initialization
	void Start ()
	{
		Debug.Log ("Developer created");
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (target != null) {
			if (Random.value <= efficiency) {
				target.commitWork ();
			}
		}
	}
}

