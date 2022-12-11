using UnityEngine;
using System.Collections;

[RequireComponent (typeof(AIenemyC))]

public class MonsterSkillC : MonoBehaviour {
	
	public GameObject mainModel;
	public float skillDistance = 4.5f;
	public float delay = 2.0f;
	private bool begin = false;
	private bool onSkill = false;
	
	private float wait = 0;
	
	[System.Serializable]
	public class SkillSetting {
		public Transform skillPrefab;
		public AnimationClip skillAnimation;
		public GameObject castEffect;
		public float castTime = 0.5f;
		public float delayTime = 1.5f;
	}
	
	public SkillSetting[] skillSet = new SkillSetting[1];
	
	IEnumerator Start(){
		if(!mainModel){
			mainModel = this.gameObject;
		}
		yield return new WaitForSeconds(1.5f);
		begin = true;
	}
	
	void Update(){
		if(begin && !onSkill){
			if(wait >= delay){
				StartCoroutine(UseSkill());
				wait = 0;
			}else{
				wait += Time.deltaTime;
			}
		}
	}
	
	public IEnumerator UseSkill() {
		if(!GetComponent<StatusC>().freeze){
			GameObject eff = null;
			
			AIenemyC ai = GetComponent<AIenemyC>();
			int c = 0;
			if(skillSet.Length > 1){
				c = Random.Range(0 , skillSet.Length);
			}
			onSkill = true;
			//Cast Effect
			if(skillSet[c].castEffect){
				eff = (GameObject)Instantiate(skillSet[c].castEffect , mainModel.transform.position , mainModel.transform.rotation);
				eff.transform.parent = this.transform;
			}
			//Call UseSkill Function in AIenemy Script.
			StartCoroutine(ai.UseSkill(skillSet[c].skillPrefab ,skillSet[c].castTime, skillSet[c].delayTime , skillSet[c].skillAnimation.name , skillDistance));
			yield return new WaitForSeconds(skillSet[c].castTime);
			if(eff){
				Destroy(eff);
			}
			yield return new WaitForSeconds(skillSet[c].delayTime);
			onSkill = false;
		}
	}
}