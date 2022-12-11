using UnityEngine;
using System.Collections;

[RequireComponent (typeof(AIenemyC))]

public class PatrollingAiC : MonoBehaviour {
	
	public float speed = 4.0f;
	private AIenemyC ai;
	private int state = 0; //0 = Idle , 1 = Moving.
	private AnimationClip movingAnimation;
	private AnimationClip idleAnimation;
	private GameObject mainModel;
	
	public float idleDuration = 2.0f;
	public float moveDuration = 3.0f;
	
	private float wait = 0f;
	private bool useMecanim = false;
	private Animator animator; //For Mecanim
	
	void Start() {
		ai = GetComponent<AIenemyC>();
		mainModel = GetComponent<StatusC>().mainModel;
		useMecanim = ai.useMecanim;
		if(!mainModel){
			mainModel = this.gameObject;
		}
		movingAnimation = ai.movingAnimation;
		idleAnimation = ai.idleAnimation;
		//-------Check for Mecanim Animator-----------
		if(useMecanim){
			animator = ai.animator;
			if(!animator){
				animator = mainModel.GetComponent<Animator>();
			}
		}
	}
	
	void Update() {
		if(ai.followState == AIenemyC.AIState.Idle){
			if(state == 1){//Moving
				CharacterController controller = GetComponent<CharacterController>();
				Vector3 forward = transform.TransformDirection(Vector3.forward);
				controller.Move(forward * speed * Time.deltaTime);
			}
			//----------------------------
			if(wait >= idleDuration && state == 0){
				//Set to Moving Mode.
				RandomTurning();
			}
			if(wait >= moveDuration && state == 1){
				//Set to Idle Mode.
				if(idleAnimation && !useMecanim){
					//For Legacy Animation
					mainModel.GetComponent<Animation>().CrossFade(idleAnimation.name, 0.2f);
				}else if(useMecanim){
					//For Mecanim Animation
					animator.SetBool("run" , false);
				}
				wait = 0;
				state = 0;
			}
			wait += Time.deltaTime;
			//-----------------------------
		}
		
	}
	
	public void RandomTurning() {
		float dir = Random.Range(0 , 360);
		Vector3 tempEuelerAngles = transform.eulerAngles;
		tempEuelerAngles.y = dir;
		transform.eulerAngles = tempEuelerAngles;
		if(movingAnimation && !useMecanim){
			//For Legacy Animation
			mainModel.GetComponent<Animation>().CrossFade(movingAnimation.name, 0.2f);
		}else if(useMecanim){
			//For Mecanim Animation
			animator.SetBool("run" , true);
		}
		wait = 0; // Reset wait time.
		state = 1; // Change State to Move.
		
	}
	
	
}