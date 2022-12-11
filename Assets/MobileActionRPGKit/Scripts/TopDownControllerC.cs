using UnityEngine;
using System.Collections;

// Require a character controller to be attached to the same game object
[RequireComponent (typeof(CharacterMotorC))]

public class TopDownControllerC : MonoBehaviour {

	private CharacterMotorC motor;
	//private float moveDir = 0.0f;

	public JoystickCanvas joyStick;// For Mobile
	public JoystickCamera joyStick2;// For Mobile
	public AudioClip walkingSound;
	public GameObject orbitcam;
	private float x = 20.0f;
	private float y = 32.0f;
	public float xSpeed = 250.0f;
	public float ySpeed = 120.0f;
	public Transform target;
	public float yMinLimit = -10;
	public float yMaxLimit = 70;
	public float targetHeight = 1.2f;
	public float distance = 4.0f;
	// Use this for initialization
	void Awake() {
		motor = GetComponent<CharacterMotorC>();
		if(!joyStick){
			joyStick = GameObject.FindWithTag("JoyStick").GetComponent<JoystickCanvas>();
		}
		//if(!joyStick2){
		//	joyStick2 = GameObject.FindWithTag("JoyStick-Camera").GetComponent<JoystickCamera>();
		//}		
	}

	void LateUpdate(){
		/*
		float movecamHorizontal = 0.0f;
		float movecamVertical = 0.0f;
		bool pressed = false;

		if(!orbitcam){
			orbitcam = GameObject.FindWithTag("MainCamera");
			x = orbitcam.GetComponent<OrbitCameraC>().x;
			y = orbitcam.GetComponent<OrbitCameraC>().y;
		}
		if(joyStick2){
			movecamHorizontal = joyStick2.position.x;
			movecamVertical = joyStick2.position.y;
			pressed = joyStick2.GetComponent<JoystickCamera>().press;
			//print(pressed);
		}

		//x = orbitcam.transform.rotation.x;
		//y = orbitcam.transform.rotation.y;

		if((movecamHorizontal != 0 || movecamVertical != 0) && pressed){
			//Debug.Log("Camera change joystick noticed");
			x += movecamHorizontal * xSpeed * 0.02f;
			y -= movecamVertical * ySpeed * 0.02f;
			//print("X:" + x);
			//print("Y:" + y);
			y = ClampAngle(y, yMinLimit, yMaxLimit);
			
			// Rotate Camera
			Quaternion rotation = Quaternion.Euler(y, x, 0);
			orbitcam.transform.rotation = rotation;
			Vector3 position = target.position - (rotation * Vector3.forward * distance + new Vector3(0 ,-targetHeight,0));
			orbitcam.transform.position = position;
			
			RaycastHit hit;
			Vector3 trueTargetPosition = target.transform.position - new Vector3(0,-targetHeight,0);
			if (Physics.Linecast (trueTargetPosition, orbitcam.transform.position, out hit)) {  
				if(hit.transform.tag == "Wall"){
					float tempDistance = Vector3.Distance (trueTargetPosition, hit.point) - 0.28f;
					
					position = target.position - (rotation * Vector3.forward * tempDistance + new Vector3(0,-targetHeight,0));
					orbitcam.transform.position = position;
				}
			}
		}

		orbitcam.GetComponent<OrbitCameraC>().x = x;
		orbitcam.GetComponent<OrbitCameraC>().y = y;
		*/
	}
	
	// Update is called once per frame
	void Update() {
		StatusC stat = GetComponent<StatusC>();
		float moveHorizontal = 0.0f;
		float moveVertical = 0.0f;

		if(stat.freeze || stat.flinch){
			motor.inputMoveDirection = new Vector3(0,0,0);
			return;
		}
		if(Input.GetButton("Horizontal") || Input.GetButton("Vertical")){
			moveHorizontal = Input.GetAxis("Horizontal");
			moveVertical = Input.GetAxis("Vertical");
		}else if(joyStick){
			moveHorizontal = joyStick.position.x;
			moveVertical = joyStick.position.y;
		}
		
		Transform cameraTransform = Camera.main.transform;
		Vector3 forward = cameraTransform.TransformDirection(Vector3.forward);
		forward.y = 0;
		forward = forward.normalized;
		Vector3 right = new Vector3(forward.z, 0, -forward.x);
		Vector3 targetDirection = moveHorizontal * right + moveVertical * forward;
		

		//----------------------------------
		if(moveHorizontal != 0 || moveVertical != 0){
			//transform.eulerAngles = new Vector3(transform.eulerAngles.x, Mathf.Atan2(moveHorizontal , moveVertical) * Mathf.Rad2Deg, transform.eulerAngles.z);
			transform.rotation = Quaternion.LookRotation(targetDirection.normalized);
			
		}
		//-----------------------------------------------------------------------------
		if(moveVertical != 0 && walkingSound && !GetComponent<AudioSource>().isPlaying|| moveHorizontal != 0 && walkingSound && !GetComponent<AudioSource>().isPlaying){
			GetComponent<AudioSource>().PlayOneShot(walkingSound);
		}
		
		motor.inputMoveDirection = targetDirection.normalized;
		motor.inputJump = Input.GetButton("Jump");
	}

	static float ClampAngle ( float angle ,   float min ,   float max  ){
		if (angle < -360)
			angle += 360;
		if (angle > 360)
			angle -= 360;
		return Mathf.Clamp (angle, min, max);
	}

}

