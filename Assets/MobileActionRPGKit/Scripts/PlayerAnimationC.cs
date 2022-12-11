using UnityEngine;
using System.Collections;


//[RequireComponent (typeof(AttackTriggerC))]
[RequireComponent (typeof(TopDownControllerC))]

public class PlayerAnimationC : MonoBehaviour {

	public float runMaxAnimationSpeed = 1.0f;
	public float backMaxAnimationSpeed = 1.0f;
	public float sprintAnimationSpeed = 1.5f;
	
	private GameObject player;
	private GameObject mainModel;
	
	//public string idle = "idle";
	public AnimationClip idle;
	public AnimationClip run;
	public AnimationClip right;
	public AnimationClip left;
	public AnimationClip back;
	public AnimationClip jump;
	public AnimationClip hurt;
	public AnimationClip death;
	public AnimationClip talk;
	public JoystickCanvas joyStick;// For Mobile
	
	void Start() {
		if(!player){
			player = this.gameObject;
		}
		mainModel = GetComponent<StatusC>().mainModel;
		if(!mainModel){
			mainModel = this.gameObject;
		}
		
		mainModel.GetComponent<Animation>()[run.name].speed = runMaxAnimationSpeed;
		mainModel.GetComponent<Animation>()[right.name].speed = runMaxAnimationSpeed;
		mainModel.GetComponent<Animation>()[left.name].speed = runMaxAnimationSpeed;
		mainModel.GetComponent<Animation>()[back.name].speed = backMaxAnimationSpeed;
		
		if(jump){
			mainModel.GetComponent<Animation>()[jump.name].wrapMode  = WrapMode.ClampForever;
		}
		
		if(hurt){
			GetComponent<StatusC>().hurt = hurt;
			mainModel.GetComponent<Animation>()[hurt.name].layer = 5;
		}
		if(!joyStick){
			joyStick = GameObject.FindWithTag("JoyStick").GetComponent<JoystickCanvas>();
		}
	}
	
	void Update() {

		float moveHorizontal = 0.0f;
		float moveVertical = 0.0f;

		if(Input.GetButton("Horizontal") || Input.GetButton("Vertical")){
			moveHorizontal = Input.GetAxis("Horizontal");
			moveVertical = Input.GetAxis("Vertical");
		}else if(joyStick){
			moveHorizontal = joyStick.position.x;
			moveVertical = joyStick.position.y;
		}
		
		CharacterController controller = player.GetComponent<CharacterController>();
		if ((controller.collisionFlags & CollisionFlags.Below) != 0){
			if (moveHorizontal > 0.1)
				mainModel.GetComponent<Animation>().CrossFade(right.name);
			else if (moveHorizontal < -0.1)
				mainModel.GetComponent<Animation>().CrossFade(left.name);
			else if (moveVertical > 0.1)
				mainModel.GetComponent<Animation>().CrossFade(run.name);
			else if (moveVertical < -0.1)
				mainModel.GetComponent<Animation>().CrossFade(back.name);
			else
				mainModel.GetComponent<Animation>().CrossFade(idle.name);
		}else{
			if(jump){
				mainModel.GetComponent<Animation>().CrossFade(jump.name);
			}
			
		}
	}

	public void DoDeath()
	{
		mainModel.GetComponent<Animation>().CrossFade(death.name);
	}
	/*
	public void AnimationSpeedSet() {
		mainModel = GetComponent<AttackTriggerC>().mainModel;
		if(!mainModel){
			mainModel = this.gameObject;
		}
		mainModel.GetComponent<Animation>()[run.name].speed = runMaxAnimationSpeed;
		mainModel.GetComponent<Animation>()[right.name].speed = runMaxAnimationSpeed;
		mainModel.GetComponent<Animation>()[left.name].speed = runMaxAnimationSpeed;
		mainModel.GetComponent<Animation>()[back.name].speed = backMaxAnimationSpeed;
	}*/

}