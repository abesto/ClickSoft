using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Developer : MonoBehaviour
{

	public float efficiency = 0.002f;
	public ThingController target = null;

	// Use this for initialization
	void Start ()
	{
		efficiency = Random.value * 0.004f;
		Debug.Log (string.Format("Developer created with {0} efficiency", efficiency.ToString()));
		transform.Find ("Efficiency").GetComponent<Text> ().text = efficiency.ToString ();
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

