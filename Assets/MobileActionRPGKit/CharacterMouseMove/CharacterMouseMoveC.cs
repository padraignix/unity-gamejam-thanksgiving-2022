using UnityEngine;
using System.Collections;

public class CharacterMouseMoveC : MonoBehaviour {
	
	public GameObject mainModel;
	public float moveSpeed = 6.0f;
	public Texture2D attackCursor;
	public float targetApproachDistance = 2.0f;
	public GameObject clickEffect;
	private Vector3 moveDestination;
	public Transform lockOnTarget;
	private bool moving = false;
	private bool movingAnim = false;
	private float dist = 0.1f;
	private bool showIcon = false;
	public AnimMode animationType = AnimMode.Legacy;
	public LegacyAnim legacyAnimationSet;
	private Animator animator;
	public float npcInteractRange = 1.2f;
	public string sendMsgNpc = "Activate";

	public bool useNavMeshAgent = false; //Require Nav Mesh Agent
	[HideInInspector]
	public bool onUi = false;

	void Start(){
		if(!mainModel){
			mainModel = GetComponent<StatusC>().mainModel;
		}
		moveDestination = transform.position;
		moving = false;
		gameObject.tag = "Player";
		lockOnTarget = null;
		SetAnimationSpeed();
		if(animationType == AnimMode.Mecanim){
			animator = mainModel.GetComponent<Animator>();
			GetComponent<AttackTriggerC>().SetMecanim();
		}
	}

	public void SetAnimationSpeed(){
		if(animationType == AnimMode.Legacy){
			if(!mainModel){
				mainModel = GetComponent<StatusC>().mainModel;
			}
			mainModel.GetComponent<Animation>()[legacyAnimationSet.run.name].speed = legacyAnimationSet.runAnimationSpeed;
		
			if(legacyAnimationSet.hurt){
				GetComponent<StatusC>().hurt = legacyAnimationSet.hurt;
				mainModel.GetComponent<Animation>()[legacyAnimationSet.hurt.name].layer = 5;
			}
		}
	}

	public void StopMoving(){
		moveDestination = transform.position;
		moving = false;
		movingAnim = false;
		if(useNavMeshAgent){
			GetComponent<UnityEngine.AI.NavMeshAgent>().ResetPath();
		}
		//Play Animation
		//Legacy
		if(animationType == AnimMode.Legacy){
			mainModel.GetComponent<Animation>().CrossFade(legacyAnimationSet.idle.name);
		}
		
		//Mecanim
		if(animationType == AnimMode.Mecanim){
			animator.SetFloat("horizontal" , 0);
		}
	}

	void Update(){
		if(GetComponent<StatusC>().freeze && GetComponent<UnityEngine.AI.NavMeshAgent>()){
			GetComponent<UnityEngine.AI.NavMeshAgent>().SetDestination(transform.position);
			StopMoving();
		}
		if(Time.timeScale == 0.0f || GetComponent<StatusC>().freeze){
			StopMoving();
			return;
		}

		if(GetComponent<AttackTriggerC>().useSkill){
			LookMouse();
			StopMoving();
		}

		if(Input.GetButton("Fire1") && !lockOnTarget && !onUi){
			//Prevent Hold Left Mouse Botton when attacking target
			CharacterMoveTowards();
		}
		
		if(Input.GetButtonDown("Fire1") && lockOnTarget){
			//Cancel lock on target
			CharacterMoveTowards();
		}
		
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if(Physics.Raycast(ray,out hit)){
			if(clickEffect && Input.GetButtonDown("Fire1")){
				Instantiate(clickEffect, hit.point, clickEffect.transform.rotation);
			}
			if(attackCursor){
				if(hit.transform.tag == "Enemy"){
					showIcon = true;
				}else{
					showIcon = false;
				}
			}
		}
		
		
		if(lockOnTarget){
			Vector3 look = new Vector3(lockOnTarget.position.x , transform.position.y , lockOnTarget.position.z);
			transform.LookAt(look);
			moveDestination = lockOnTarget.position;
		}
		if(moving && GetComponent<CharacterMotorC>().canControl){
			MoveTowardsTarget(moveDestination);
		}
		
		//Play Animation
		//Legacy
		if(animationType == AnimMode.Legacy){
			if(movingAnim){
				mainModel.GetComponent<Animation>().CrossFade(legacyAnimationSet.run.name);
			}else{
				mainModel.GetComponent<Animation>().CrossFade(legacyAnimationSet.idle.name);
			}
		}
		
		//Mecanim
		if(animationType == AnimMode.Mecanim){
			bool flinch = GetComponent<StatusC>().flinch;
			//Set Hurt Animation
			animator.SetBool("hurt" , flinch);

			if(movingAnim){
				animator.SetFloat("horizontal" , 1);
			}else{
				animator.SetFloat("horizontal" , 0);
			}
		}
	}
	
	void CharacterMoveTowards(){
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		if(Physics.Raycast(ray,out hit)){
			moving = true;
			//print(hit.transform.name);
			if(hit.transform.tag == "NPC"){
				lockOnTarget = hit.transform;
				dist = npcInteractRange;
			}else if(hit.transform.tag == "Enemy"){
				//Vector3 destination = hit.point;
				lockOnTarget = hit.transform;
				dist = targetApproachDistance;
			}else{
				Vector3 destination = hit.point;
				destination.y = transform.position.y;
				transform.LookAt(destination);
				lockOnTarget = null;
				moveDestination = destination;
				dist = 0.15f;
			}
		}
	}
	
	void SpawnFromRayCast(){
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		if(Physics.Raycast(ray,out hit)) {
			//bulletShootout = Instantiate (skill[skillID].skillPrefab, hit.point, transform.rotation);
		}
	}
	
	void MoveTowardsTarget(Vector3 target){
		CharacterController cc = GetComponent<CharacterController>();
		target.y = transform.position.y;
		Vector3 offset = target - transform.position;

		//RaycastHit hit;
		//Vector3 fwd = transform.TransformDirection (Vector3.forward);
		
		if(offset.magnitude > dist) {
			if(useNavMeshAgent){
				GetComponent<UnityEngine.AI.NavMeshAgent>().SetDestination(target);
			}else{
				offset = offset.normalized * moveSpeed;
				cc.Move(offset * Time.deltaTime);
			}
			movingAnim = true;
		}else{
			movingAnim = false;
			if(lockOnTarget && lockOnTarget.tag == "Enemy" && !GetComponent<AttackTriggerC>().isCasting){
				GetComponent<AttackTriggerC>().TriggerAttack();
				if(GetComponent<UnityEngine.AI.NavMeshAgent>()){
					GetComponent<UnityEngine.AI.NavMeshAgent>().ResetPath();
				}

			}

			if(lockOnTarget && lockOnTarget.tag == "NPC"){
				lockOnTarget.SendMessage(sendMsgNpc , SendMessageOptions.DontRequireReceiver);
			}
			
			if(!lockOnTarget || lockOnTarget.tag != "Enemy"){
				moving = false;
			}
		}
	}
	
	void OnGUI(){
		if(attackCursor && showIcon){
			GUI.DrawTexture(new Rect(Event.current.mousePosition.x - 2, Event.current.mousePosition.y - 3, 30, 30), attackCursor);
		}
		
	}

	public void SetWeaponType(int val , string idle){
		//For Mecanim
		if(!mainModel){
			mainModel = GetComponent<StatusC>().mainModel;
		}
		animator = mainModel.GetComponent<Animator>();
		animator.SetInteger("weaponType" , val);
		animator.Play(idle);
	}

	void LookMouse(){
		if(onUi){
			return;
		}
		Plane playerPlane = new Plane(Vector3.up, transform.position);
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		
		float hitdist = 0.0f;
		
		if (playerPlane.Raycast (ray, out hitdist)) {
			// Get the point along the ray that hits the calculated distance.
			Vector3 targetPoint = ray.GetPoint(hitdist);
			Quaternion targetRotation = Quaternion.LookRotation(targetPoint - transform.position);
			//transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speed * Time.deltaTime);
			transform.rotation = targetRotation;
		}
	}
}

[System.Serializable]
public class LegacyAnim{
	public AnimationClip idle;
	public AnimationClip run;
	public AnimationClip hurt;
	public float runAnimationSpeed = 1.2f;
}

public enum AnimMode{
	Legacy = 0,
	Mecanim = 1
}