using UnityEngine;
using System.Collections;

[System.Serializable]
public class SkilAtk {
	public Texture2D icon;
	public Transform skillPrefab;
	public AnimationClip skillAnimation;
	public float skillAnimationSpeed = 1.0f;
	public int manaCost = 10;
	public bool cantStack = false;
}

[System.Serializable]
public class LockOn{
	public bool  enable = true;
	public float radius = 5.0f;  //this is radius to checks for other objects
	public float lockOnRange = 4.0f; //this is how far it checks for other objects
	[HideInInspector]
	public Transform lockTarget;
	[HideInInspector]
	public GameObject target;
}

[System.Serializable]
public class AtkSound{
	public AudioClip[] attackComboVoice = new AudioClip[3];
	public AudioClip magicCastVoice;
	public AudioClip hurtVoice;
}

public enum whileAtk{
	MeleeFwd = 0,
	Immobile = 1,
	WalkFree = 2
}

[RequireComponent (typeof(StatusC))]
//[RequireComponent (typeof(StatusWindowC))]
//[RequireComponent (typeof(HealthBarC))]
[RequireComponent (typeof(CharacterMotorC))]
[RequireComponent (typeof(InventoryC))]
[RequireComponent (typeof(QuestStatC))]
[RequireComponent (typeof(SkillWindowC))]
[RequireComponent (typeof(SaveLoadC))]
[RequireComponent (typeof(DontDestroyOnloadC))]
[RequireComponent (typeof(SpawnPartnerC))]
[RequireComponent (typeof(ShowEnemyHealthC))]

public class AttackTriggerC : MonoBehaviour {
	
	[HideInInspector]
	public GameObject mainModel;

	public bool  useMecanim = false;
	public Transform attackPoint;
	public Transform attackPrefab;

	public whileAtk whileAttack = whileAtk.MeleeFwd;
	
	public LockOn autoLockTarget;
	
	public SkilAtk[] skill = new SkilAtk[3];

	private bool atkDelay = false;
	public bool freeze = false;
	
	public float attackSpeed = 0.15f;
	private float nextFire = 0.0f;
	public float atkDelay1 = 0.1f;
	public float skillDelay = 0.3f;
	
	public AnimationClip[] attackCombo = new AnimationClip[3];
	public float attackAnimationSpeed = 1.0f;
	
	private bool meleefwd = false;
	[HideInInspector]
	public bool isCasting = false;
	[HideInInspector]
	public bool useSkill = false;

	[HideInInspector]
	public int c = 0;
	private int conCombo = 0;
	
	[HideInInspector]
	public Transform Maincam;
	public GameObject MaincamPrefab;
	public GameObject attackPointPrefab;
	
	private int str = 0;
	private int matk = 0;

	public AtkSound sound;
	private Transform cantStackPrefab;
	
	void Awake(){
		gameObject.tag = "Player";
		mainModel = GetComponent<StatusC>().mainModel;
		if(!mainModel){
			mainModel = this.gameObject;
		}
		GetComponent<StatusC>().useMecanim = useMecanim;
		if(useMecanim){
			animator = mainModel.GetComponent<Animator>();
		}
		
		GameObject[] oldcam = GameObject.FindGameObjectsWithTag("MainCamera");
		foreach(GameObject o in oldcam){
			Destroy(o);
		}
		GameObject newCam = Instantiate(MaincamPrefab, transform.position , MaincamPrefab.transform.rotation) as GameObject;
		Maincam = newCam.transform;

		str = GetComponent<StatusC>().addAtk;
		matk = GetComponent<StatusC>().addMatk;
		//Set All Attack Animation'sLayer to 15
		int animationSize = attackCombo.Length;
		int a = 0;
		if(animationSize > 0 && !useMecanim  && mainModel.GetComponent<Animation>()){
			while(a < animationSize && attackCombo[a]){
				mainModel.GetComponent<Animation>()[attackCombo[a].name].layer = 15;
				a++;
			}
		}
		
		//--------------------------------
		//Spawn new Attack Point if you didn't assign it.
		if(!attackPoint){
			if(!attackPointPrefab){
				print("Please assign Attack Point");
				freeze = true;
				return;
			}
			GameObject newAtkPoint = Instantiate(attackPointPrefab, transform.position , transform.rotation) as GameObject;
			newAtkPoint.transform.parent = this.transform;
			attackPoint = newAtkPoint.transform;	
		}
		if(sound.hurtVoice){
			GetComponent<StatusC>().hurtVoice = sound.hurtVoice;
		}
	}

	public void SetMecanim(){
		useMecanim = true;
		animator = mainModel.GetComponent<Animator>();
		GetComponent<StatusC>().useMecanim = useMecanim;
	}
	
	void Update(){
		StatusC stat = GetComponent<StatusC>();
		if(freeze || atkDelay || Time.timeScale == 0.0f || stat.freeze){
			return;
		}
		CharacterController controller = GetComponent<CharacterController>();
		if (stat.flinch){
			controller.Move(stat.knock * 6* Time.deltaTime);
			return;
		}
		
		if (meleefwd){
			Vector3 lui = transform.TransformDirection(Vector3.forward);
			controller.Move(lui * 5 * Time.deltaTime);
		}
		//----------------------------
		//Normal Trigger
		if (Input.GetKey("j") && Time.time > nextFire && !isCasting) {
			TriggerAttack();
		}
		//Magic
		if(Input.GetKeyDown("1") && !isCasting && skill[0].skillPrefab){
			StartCoroutine(MagicSkill(0));
		}
		if(Input.GetKeyDown("2") && !isCasting && skill[1].skillPrefab){
			StartCoroutine(MagicSkill(1));
		}
		if(Input.GetKeyDown("3") && !isCasting && skill[2].skillPrefab){
			StartCoroutine(MagicSkill(2));
		}
		
	}
	
	public void  TriggerAttack (){
		if(freeze || atkDelay || Time.timeScale == 0.0f || GetComponent<StatusC>().freeze){
			return;
		}
		if (Time.time > nextFire && !isCasting) {
			if(Time.time > (nextFire + 0.5f)){
				c = 0;
			}
			//Attack Combo
			if(attackCombo.Length >= 1){
				conCombo++;
				StartCoroutine(AttackCombo());
			}
		}
	}
	
	public void  TriggerSkill ( int sk  ){
		if(freeze || atkDelay || Time.timeScale == 0.0f || GetComponent<StatusC>().freeze){
			return;
		}
		if (Time.time > nextFire && !isCasting && skill[sk].skillPrefab) {
			StartCoroutine(MagicSkill(sk));
		}
		
	}
	
	IEnumerator AttackCombo(){
		float wait = 0.0f;
		if(attackCombo[c]){
			if(autoLockTarget.enable){
				LockOnEnemy();
			}
			str = GetComponent<StatusC>().addAtk;
			matk = GetComponent<StatusC>().addMatk;
			isCasting = true;
			// If Melee Dash
			if(whileAttack == whileAtk.MeleeFwd){
				GetComponent<CharacterMotorC>().canControl = false;
				StartCoroutine(MeleeDash());
			}
			// If Immobile
			if(whileAttack == whileAtk.Immobile){
				GetComponent<CharacterMotorC>().canControl = false;
			}
			if(sound.attackComboVoice.Length > c && sound.attackComboVoice[c]){
				GetComponent<AudioSource>().clip = sound.attackComboVoice[c];
				GetComponent<AudioSource>().Play();
			}
			while(conCombo > 0){
				if(!useMecanim){
					//For Legacy Animation
					mainModel.GetComponent<Animation>().PlayQueued(attackCombo[c].name, QueueMode.PlayNow).speed = attackAnimationSpeed;
					wait = mainModel.GetComponent<Animation>()[attackCombo[c].name].length;
				}else{
					//For Mecanim Animation
					//GetComponent<PlayerMecanimAnimationC>().AttackAnimation(attackCombo[c].name);
					AttackAnimation(attackCombo[c].name);
					float clip = animator.GetCurrentAnimatorClipInfo(0).Length;
					wait = clip - 0.3f;
				}
				
				yield return new WaitForSeconds(atkDelay1);
				c++;
				
				nextFire = Time.time + attackSpeed;
				Transform bulletShootout = Instantiate(attackPrefab, attackPoint.transform.position , attackPoint.transform.rotation) as Transform;
				bulletShootout.GetComponent<BulletStatusC>().Setting(str , matk , "Player" , this.gameObject);
				conCombo -= 1;
				
				if(c >= attackCombo.Length){
					c = 0;
					atkDelay = true;
					yield return new WaitForSeconds(wait);
					atkDelay = false;
				}else{
					yield return new WaitForSeconds(attackSpeed);
				}
				
			}
			
			//yield return new WaitForSeconds(attackSpeed);
			isCasting = false;
			GetComponent<CharacterMotorC>().canControl = true;

		}else{
			print("Please assign attack animation in Attack Combo");
		}

	}
	
	IEnumerator MeleeDash (){
		meleefwd = true;
		yield return new WaitForSeconds(0.2f);
		meleefwd = false;
		
	}

	private Animator animator;
	[HideInInspector]
	public string jumpState = "jump";
	void AttackAnimation(string anim){
		animator.SetBool(jumpState , false);
		animator.Play(anim);
	}
	
	//---------------------
	//-------
	IEnumerator MagicSkill(int skillID){
		float wait = 0.0f;
		if(skill[skillID].skillAnimation){
			if(autoLockTarget.enable){
				LockOnEnemy();
			}
			str = GetComponent<StatusC>().addAtk;
			matk = GetComponent<StatusC>().addMatk;
			
			if(GetComponent<StatusC>().mana >= skill[skillID].manaCost && !GetComponent<StatusC>().silence){
				if(sound.magicCastVoice){
					GetComponent<AudioSource>().clip = sound.magicCastVoice;
					GetComponent<AudioSource>().Play();
				}
				isCasting = true;
				useSkill = true;
				GetComponent<CharacterMotorC>().canControl = false;
				
				if(!useMecanim){
					//For Legacy Animation
					mainModel.GetComponent<Animation>()[skill[skillID].skillAnimation.name].layer = 16;
					mainModel.GetComponent<Animation>()[skill[skillID].skillAnimation.name].speed = skill[skillID].skillAnimationSpeed;
					mainModel.GetComponent<Animation>().Play(skill[skillID].skillAnimation.name);
					wait = mainModel.GetComponent<Animation>()[skill[skillID].skillAnimation.name].length -0.3f;
				}else{
					//For Mecanim Animation
					//GetComponent<PlayerMecanimAnimationC>().AttackAnimation(skill[skillID].skillAnimation.name);
					AttackAnimation(skill[skillID].skillAnimation.name);
					float clip = animator.GetCurrentAnimatorClipInfo(0).Length;
					wait = clip - 0.3f;
				}
				
				nextFire = Time.time + skillDelay;
				//Maincam.GetComponent<ARPGcamera>().lockOn = true;
				yield return new WaitForSeconds(wait);
				//Maincam.GetComponent<ARPGcamera>().lockOn = false;
				Transform bulletShootout = Instantiate(skill[skillID].skillPrefab, attackPoint.transform.position , attackPoint.transform.rotation) as Transform;
				bulletShootout.GetComponent<BulletStatusC>().Setting(str , matk , "Player" , this.gameObject);

				if(skill[skillID].cantStack){
					if(cantStackPrefab){
						Destroy(cantStackPrefab.gameObject);
					}
					cantStackPrefab = bulletShootout;
				}

				yield return new WaitForSeconds(skillDelay);
				isCasting = false;
				useSkill = false;
				GetComponent<CharacterMotorC>().canControl = true;
				GetComponent<StatusC>().mana -= skill[skillID].manaCost;
			}

		}else{
			print("Please assign skill animation in Skill Animation");
		}

	}
	
	// Lock On the closest enemy 
	public void  LockOnEnemy (){ 
		Vector3 checkPos = transform.position + transform.forward * autoLockTarget.lockOnRange;
		GameObject closest; 
		
		float distance = Mathf.Infinity; 
		Vector3 position = transform.position; 
		autoLockTarget.lockTarget = null; // Reset Lock On Target
		Collider[] objectsAroundMe = Physics.OverlapSphere(checkPos , autoLockTarget.radius);
		foreach(Collider obj in objectsAroundMe){
			if(obj.CompareTag("Enemy")){
				Vector3 diff = (obj.transform.position - position); 
				float curDistance = diff.sqrMagnitude; 
				if (curDistance < distance) { 
					//------------
					closest = obj.gameObject; 
					distance = curDistance;
					autoLockTarget.target = closest;
					autoLockTarget.lockTarget = closest.transform;
				} 
			}
		}
		//Face to the target
		if(autoLockTarget.lockTarget){
			Vector3 lookOn = autoLockTarget.lockTarget.position;
			lookOn.y = transform.position.y;
			transform.LookAt(lookOn);
		}
		
	}
	

}

