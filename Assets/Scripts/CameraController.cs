using UnityEngine;
using System.Collections;

// Originally from 

// Camera Controller
// Revision 2
// Allows the camera to move left, right, up and down along a fixed axis.
// Attach to a camera GameObject (e.g MainCamera) for functionality.
public class CameraController : MonoBehaviour {
	
	// How fast the camera moves
	public const float cameraVelocity = 50f;
	
	// Use this for initialization
	void Start () {
		
		// Set the initial position of the camera.
		// Right now we don't actually need to set up any other variables as
		// we will start with the initial position of the camera in the scene editor
		// If you want to create cameras dynamically this will be the place to
		// set the initial transform.positiom.x/y/z
	}
	
	// Update is called once per frame
	void Update () {
		float horizontalInput = Input.GetAxis ("Horizontal");
		float verticalInput = Input.GetAxis ("Vertical");
		Vector3 target = transform.right * horizontalInput + transform.up * verticalInput;
		transform.Translate (target * cameraVelocity * Time.deltaTime, Space.World);

		GameObject background = GameObject.FindGameObjectWithTag ("Background");
		Ray ray = new Ray (transform.position, background.transform.position - transform.position);
		RaycastHit hitInfo = new RaycastHit();
		if (Physics.Raycast (transform.position, background.transform.position - transform.position , out hitInfo, 100f)) {
			transform.RotateAround(hitInfo.point, background.transform.up, Input.GetAxis("Circular"));
		}
	}
}
