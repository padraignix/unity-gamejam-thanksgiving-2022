using UnityEngine;
using System.Collections;

public class OrbitCameraC : MonoBehaviour {
	public Transform target;
	public float targetHeight = 1.2f;
	public float distance = 4.0f;
	public float maxDistance = 6;
	public float minDistance = 1.0f;
	public float xSpeed = 250.0f;
	public float ySpeed = 120.0f;
	public float yMinLimit = -10;
	public float yMaxLimit = 70;
	public float zoomRate = 80;
	public float rotationDampening = 3.0f;
	public float x = 20.0f;
	public float y = 32.0f;
	public bool  lockOn = false;
	public bool  freeze = false;
	
	//Transform attackPoint;
	public Transform targetBody;
	public JoystickCamera joyStick;// For Mobile

	
	void  Start (){
		if(!target){
			target = GameObject.FindWithTag ("Player").transform;
		}
		Vector3 angles = transform.eulerAngles;
		x = angles.y;
		y = 32.0f;
		
		if (GetComponent<Rigidbody>())
			GetComponent<Rigidbody>().freezeRotation = true;
	
		//if(!joyStick){
		//	joyStick = GameObject.FindWithTag("JoyStick").GetComponent<JoystickCamera>();
		//}
	}
	
	void  LateUpdate (){
		if(!target)
			return;
		
		if(!targetBody){
			targetBody = target;
		}
		if (Time.timeScale == 0.0f){
			return;
		}
		if(Input.GetButton("Fire2")){
			x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
			y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
		}

		distance -= (Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime) * zoomRate * Mathf.Abs(distance);
		distance = Mathf.Clamp(distance, minDistance, maxDistance);
		
		y = ClampAngle(y, yMinLimit, yMaxLimit);
		
		// Rotate Camera
		Quaternion rotation = Quaternion.Euler(y, x, 0);
		transform.rotation = rotation;
		
		//StatusC stat = targetBody.GetComponent<StatusC>();
		
		//Camera Position
		Vector3 position = target.position - (rotation * Vector3.forward * distance + new Vector3(0 ,-targetHeight,0));
		transform.position = position;
		
		RaycastHit hit;
		Vector3 trueTargetPosition = target.transform.position - new Vector3(0,-targetHeight,0);
		if (Physics.Linecast (trueTargetPosition, transform.position, out hit)) {  
			if(hit.transform.tag == "Wall"){
				float tempDistance = Vector3.Distance (trueTargetPosition, hit.point) - 0.28f;
				
				position = target.position - (rotation * Vector3.forward * tempDistance + new Vector3(0,-targetHeight,0));
				transform.position = position;
			}
		}
	}
	
	static float ClampAngle ( float angle ,   float min ,   float max  ){
		if (angle < -360)
			angle += 360;
		if (angle > 360)
			angle -= 360;
		return Mathf.Clamp (angle, min, max);
		
	}
}