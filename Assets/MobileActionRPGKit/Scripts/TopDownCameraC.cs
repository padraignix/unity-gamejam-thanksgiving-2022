using UnityEngine;
using System.Collections;

public class TopDownCameraC : MonoBehaviour {

	// The target we are following
	public Transform target;
	// The distance in the x-z plane to the target
	public float distance = 10.0f;
	// the height we want the camera to be above the target
	public float height = 5.0f;
	// How much we 
	public float heightDamping = 2.0f;
	public float rotationDamping = 0.0f;
	
	public float zoomRate = 80f;
	public float maxDistance = 7.9f;
	public float minDistance = 3.5f;
	
	public float maxHeight = 5.9f;
	public float minHeight = 1.5f;
	
	void Start() {
		if(!target){
			target = GameObject.FindWithTag("Player").transform;
		}
	}
	
	void LateUpdate() {
		// Early out if we don't have a target
		if (!target)
			return;
		
		height -= (Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime) * zoomRate * Mathf.Abs(height);
		height = Mathf.Clamp(height, minHeight, maxHeight);
		
		// Calculate the current rotation angles
		float wantedRotationAngle = target.eulerAngles.y;
		float wantedHeight = target.position.y + height;
		
		float currentRotationAngle = transform.eulerAngles.y;
		float currentHeight = transform.position.y;
		
		// Damp the rotation around the y-axis
		currentRotationAngle = Mathf.LerpAngle (currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);
		
		// Damp the height
		currentHeight = Mathf.Lerp (currentHeight, wantedHeight, heightDamping * Time.deltaTime);
		
		// Convert the angle into a rotation
		Quaternion currentRotation = Quaternion.Euler (0, currentRotationAngle, 0);
		
		distance -= (Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime) * zoomRate * Mathf.Abs(distance);
		distance = Mathf.Clamp(distance, minDistance, maxDistance);
		
		// Set the position of the camera on the x-z plane to:
		// distance meters behind the target
		transform.position = target.position;
		transform.position -= currentRotation * Vector3.forward * distance;
		
		transform.position = new Vector3(transform.position.x , currentHeight , transform.position.z);
		
		// Always look at the target
		transform.LookAt (target);
		
		Quaternion aRotation = Quaternion.Euler(0, 0, 0);
		
		RaycastHit hit;
		Vector3 trueTargetPosition = target.transform.position - new Vector3(0,-height,0);
		// Cast the line to check:
		if (Physics.Linecast (trueTargetPosition, transform.position, out hit)) { 
			if(hit.transform.tag == "Wall"){
				float tempDistance = Vector3.Distance (trueTargetPosition, hit.point) - 0.28f;
				Vector3 aPosition  = target.position - (aRotation * Vector3.forward * tempDistance + new Vector3(0,-height,0));
				
				transform.position = aPosition;
			}
		}
	}

}