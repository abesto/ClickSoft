using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class Developer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    const float MIN_COMMITS_PER_SECOND = 0.01f;
    const float MAX_COMMITS_PER_SECOND = 2f;
    public float commitsPerSecond = 0.5f;
	public ThingController target = null;
  float timeSinceLastWork = 0f;
    float nextCommit = 0f;

    // Use this for initialization
    void Start ()
	{
		commitsPerSecond = Random.Range(MIN_COMMITS_PER_SECOND, MAX_COMMITS_PER_SECOND);
        determineNextCommit();
        Debug.Log (string.Format("Developer created with {0} efficiency", commitsPerSecond.ToString()));
		transform.Find ("Efficiency").GetComponent<Text> ().text = commitsPerSecond.ToString ();
		MoniesController.Instance.deltaCash -= commitsPerSecond * 100;
	}

	// Update is called once per frame
	void Update ()
	{
		if (target != null) {
            timeSinceLastWork += Time.deltaTime;
            if (timeSinceLastWork >= nextCommit) {
				target.commitWork ();
                timeSinceLastWork = 0f;
                determineNextCommit();
            }
		}
	}

		void determineNextCommit() {
        nextCommit = Random.Range(0f, 2f) / commitsPerSecond;
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
