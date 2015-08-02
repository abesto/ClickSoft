using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class Developer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

	public float efficiency = 0.002f;
	public ThingController target = null;

	// Use this for initialization
	void Start ()
	{
		efficiency = Random.value * 0.004f;
		Debug.Log (string.Format("Developer created with {0} efficiency", efficiency.ToString()));
		transform.Find ("Efficiency").GetComponent<Text> ().text = efficiency.ToString ();
		MoniesController.Instance.deltaCash -= efficiency * 100;
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

	public void OnPointerEnter (PointerEventData eventData)
	{
		if (target != null) {
			target.HighlightOn(ThingHighlightSource.MENU);
		}
	}

	public void OnPointerExit (PointerEventData eventData)
	{
		if (target != null) {
			target.HighlightOff(ThingHighlightSource.MENU);
		}
	}
}

