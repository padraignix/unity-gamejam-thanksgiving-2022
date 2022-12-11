using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof(StatusC))]
[RequireComponent (typeof(CharacterMotorC))]

public class AIenemyC : MonoBehaviour {

	public enum AIState { Moving = 0, Pausing = 1 , Idle = 2 , Patrol = 3 }
	
	private GameObject mainModel;
	public bool useMecanim = false;
	public Animator animator; //For Mecanim
	public Transform followTarget;
	public float approachDistance = 2.0f;
	public float detectRange = 15.0f;
	public float lostSight = 100.0f;
	public float speed = 4.0f;
	public AnimationClip movingAnimation;
	public AnimationClip idleAnimation;
	public AnimationClip attackAnimation;
	public AnimationClip hurtAnimation;
	
	[HideInInspector]
	public bool flinch = false;
	
	public bool freeze = false;
	
	public Transform bulletPrefab;
	public Transform attackPoint;
	
	public float attackCast = 0.3f;
	public float attackDelay = 0.5f;
	
	public AIState followState = AIState.Idle;
	private float distance = 0.0f;
	private int atk = 0;
	private int matk = 0;

	[HideInInspector]
	public bool cancelAttack = false;

	private bool attacking = false;
	private bool castSkill = false;
	private GameObject[] gos; 
	
	public AudioClip attackVoice;
	public AudioClip hurtVoice;

	public bool useNavMeshAgent = false; //Require Nav Mesh Agent
	public bool notAttackBehindWall = false;

	void Start(){
		gameObject.tag = "Enemy"; 

		if(!attackPoint){
			attackPoint = this.transform;
		}
		mainModel = GetComponent<StatusC>().mainModel;

		if(!mainModel){
			mainModel = this.gameObject;
		}

		GetComponent<StatusC>().useMecanim = useMecanim;
		//Assign MainModel in Status Script
		GetComponent<StatusC>().mainModel = mainModel;
		//Set ATK = Monster's Status
		atk = GetComponent<StatusC>().atk;
		matk = GetComponent<StatusC>().matk;
		
		followState = AIState.Idle;
		
		if(!useMecanim){
			//If using Legacy Animation
			mainModel.GetComponent<Animation>().Play(idleAnimation.name);
			mainModel.GetComponent<Animation>()[hurtAnimation.name].layer = 10;
			GetComponent<StatusC>().hurt = hurtAnimation;
		}else{
			//If using Mecanim Animation
			if(!animator){
				animator = mainModel.GetComponent<Animator>();
			}
		}
		
		if(hurtVoice){
			GetComponent<StatusC>().hurtVoice = hurtVoice;
		}
	}
	
	public Vector3 GetDestination(){
		Vector3 destination = followTarget.position;
		destination.y = transform.position.y;
		return destination;
	}
	
	void Update(){
		StatusC stat = GetComponent<StatusC>();
		CharacterController controller = GetComponent<CharacterController>();
		
		gos = GameObject.FindGameObjectsWithTag("Player"); 

		if(gos.Length > 0){
			followTarget = FindClosest().transform;
		}

		if(useMecanim){
			animator.SetBool("hurt" , stat.flinch);
		}
		
		if(stat.flinch){
			cancelAttack = true;
			Vector3 knock = transform.TransformDirection(Vector3.back);

			controller.Move(knock * 5* Time.deltaTime);
			followState = AIState.Moving;
			return;
		}
		
		if(freeze || stat.freeze){
			return;
		}
		
		if(!followTarget){
			return;
		}

		//-----------------------------------
		
		if(followState == AIState.Moving){
			//mainModel.animation.CrossFade(movingAnimation.name, 0.2f);
			if(!useMecanim){
				//If using Legacy Animation
				mainModel.GetComponent<Animation>().CrossFade(movingAnimation.name, 0.2f);
			}else{
				animator.SetBool("run" , true);
			}

			if((followTarget.position - transform.position).magnitude <= approachDistance){
				followState = AIState.Pausing;
				//----Attack----
				StartCoroutine(Attack());
			}else if((followTarget.position - transform.position).magnitude >= lostSight){ //Lost Sight
				GetComponent<StatusC>().health = GetComponent<StatusC>().maxHealth;
				//mainModel.animation.CrossFade(idleAnimation.name, 0.2f);
				if(!useMecanim){
					//If using Legacy Animation
					mainModel.GetComponent<Animation>().CrossFade(idleAnimation.name, 0.2f); 
				}else{
					animator.SetBool("run" , false);
				}
				followState = AIState.Idle;
			}else{
				if(useNavMeshAgent){
					PathFinding(followTarget.position);
				}else{
					Vector3 forward = transform.TransformDirection(Vector3.forward);
					controller.Move(forward * speed * Time.deltaTime);
					
					Vector3 destiny = followTarget.position;
					destiny.y = transform.position.y;
					transform.LookAt(destiny);
				}
			}
		}else if(followState == AIState.Pausing){
			//mainModel.animation.CrossFade(idleAnimation.name, 0.2f);
			if(!useMecanim){
				//If using Legacy Animation
				mainModel.GetComponent<Animation>().CrossFade(idleAnimation.name, 0.2f); 
			}else{
				animator.SetBool("run" , false);
			}

			Vector3 destinya = followTarget.position;
			destinya.y = transform.position.y;
			transform.LookAt(destinya);
			
			distance = (transform.position - GetDestination()).magnitude;

			if(distance > approachDistance){
				followState = AIState.Moving;
			}
		}else if(followState == AIState.Idle){ //----------------Idle Mode--------------
			//mainModel.animation.CrossFade(idleAnimation.name, 0.2f);
			Vector3 destinyheight = followTarget.position;
			destinyheight.y = transform.position.y - destinyheight.y;
			int getHealth = GetComponent<StatusC>().maxHealth - GetComponent<StatusC>().health;
			
			distance = (transform.position - GetDestination()).magnitude;

			if(distance < detectRange && Mathf.Abs(destinyheight.y) <= 4 || getHealth > 0){
				followState = AIState.Moving;
			}
		}
		//-----------------------------------
	}
	
	public IEnumerator Attack(){
		RaycastHit hit;
		if(Physics.Linecast (transform.position, followTarget.position , out hit)) {
			if(hit.transform.tag == "Wall" && notAttackBehindWall){
				if(useNavMeshAgent){
					PathFinding(followTarget.position);
				}
				followState = AIState.Pausing;
				yield break;
			}
		}
		cancelAttack = false;

		if(!GetComponent<StatusC>().flinch && !GetComponent<StatusC>().freeze && !freeze && !attacking) {
			freeze = true;
			attacking = true;
			
			if(!useMecanim) {
				//If using Legacy Animation
				mainModel.GetComponent<Animation>().Play(attackAnimation.name);
			}else{
				animator.Play(attackAnimation.name);
			}
			
			yield return new WaitForSeconds(attackCast);
			//attackPoint.transform.LookAt(followTarget);
			
			if(!cancelAttack) {
				if(attackVoice){
					GetComponent<AudioSource>().clip = attackVoice;
					GetComponent<AudioSource>().Play();
				}
				Transform bulletShootout = (Transform)Instantiate(bulletPrefab, attackPoint.transform.position , attackPoint.transform.rotation);
				
				bulletShootout.GetComponent<BulletStatusC>().Setting(atk , matk , "Enemy" , this.gameObject);
			}
			
			yield return new WaitForSeconds(attackDelay);
			freeze = false;
			attacking = false;
			//mainModel.animation.CrossFade(movingAnimation.name, 0.2f);
			CheckDistance();
		}
			
	}
	
	public void CheckDistance(){
		if(!followTarget) {
			if(!useMecanim) {
				//If using Legacy Animation
				mainModel.GetComponent<Animation>().CrossFade(idleAnimation.name, 0.2f);  
			}else{
				animator.SetBool("run" , false);
			}
			followState = AIState.Idle;
			return;
		}

		float distancea = (followTarget.position - transform.position).magnitude;

		if (distancea <= approachDistance) {
			Vector3 destinya = followTarget.position;
			destinya.y = transform.position.y;
			transform.LookAt(destinya);
			StartCoroutine(Attack());
		} else {
			followState = AIState.Moving;
			if (!useMecanim) {
				//If using Legacy Animation
				mainModel.GetComponent<Animation>().CrossFade(movingAnimation.name, 0.2f);
			} else {
				animator.SetBool("run" , true);
			}
		}
	}

	public GameObject FindClosest(){ 
		// Find Closest Player   
		//gos = GameObject.FindGameObjectsWithTag("Player");

		List<GameObject> gosList = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
		gosList.AddRange(GameObject.FindGameObjectsWithTag("Ally"));
		
		gos = gosList.ToArray() as GameObject[];

		if(gos.Length == 0){
			return null;
		}

		GameObject closest = null; 
		
		float distance = Mathf.Infinity; 
		Vector3 position = transform.position; 
		
		foreach(GameObject go in gos) { 
			Vector3 diff = (go.transform.position - position); 
			float curDistance = diff.sqrMagnitude; 

			if (curDistance < distance) { 
				//------------
				closest = go; 
				distance = curDistance; 
			} 
		} 
		//  var target = closest;
		return closest; 
	}
	
	public IEnumerator UseSkill(Transform skill, float castTime, float delay, string anim, float dist){
		cancelAttack = false;

		if(flinch || !followTarget || (followTarget.position - transform.position).magnitude >= dist || GetComponent<StatusC>().silence || GetComponent<StatusC>().freeze  || castSkill) {
			//Do Nothing
		}else{
			freeze = true;
			castSkill = true;
			if(!useMecanim) {
				//If using Legacy Animation
				mainModel.GetComponent<Animation>().Play(anim);
			}else{
				animator.Play(anim);
			}
			
			yield return new WaitForSeconds(castTime);
			
			//attackPoint.transform.LookAt(followTarget);
			if(!cancelAttack) {
				Transform bulletShootout = (Transform)Instantiate(skill, attackPoint.transform.position , attackPoint.transform.rotation);
				bulletShootout.GetComponent<BulletStatusC>().Setting(atk , matk , "Enemy" , this.gameObject);
			}
			
			yield return new WaitForSeconds(delay);
			freeze = false;
			castSkill = false;
			
			if(!useMecanim){
				//If using Legacy Animation
				mainModel.GetComponent<Animation>().CrossFade(movingAnimation.name, 0.2f);
			}else{
				animator.SetBool("run" , true);
			}
		}
	}

	void PathFinding(Vector3 target){
		//Require Nav Mesh Agent.
		GetComponent<UnityEngine.AI.NavMeshAgent>().SetDestination(target);
	}
}