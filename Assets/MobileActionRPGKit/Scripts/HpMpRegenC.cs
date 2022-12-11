using UnityEngine;
using System.Collections;

public class HpMpRegenC : MonoBehaviour {
	public int hpRegen = 0;
	public int mpRegen = 3;
	public float hpRegenDelay = 3.0f;
	public float mpRegenDelay = 3.0f;
	
	private float hpTime = 0.0f;
	private float mpTime = 0.0f;
	private StatusC stat;
	
	void Start(){
		stat= GetComponent<StatusC>();
	}
	
	void Update(){
		if(hpRegen > 0 && stat.health < stat.maxHealth){
			if(hpTime >= hpRegenDelay){
				HPRecovery();
			}else{
				hpTime += Time.deltaTime;
			}
		}
		//----------------------------------------------------
		if(mpRegen > 0 && stat.mana < stat.maxMana){
			if(mpTime >= mpRegenDelay){
				MPRecovery();
			}else{
				mpTime += Time.deltaTime;
			}
		}
	}
	
	void HPRecovery(){
		int amount = stat.maxHealth * hpRegen / 100;
		if(amount <= 1){
			amount = 1;
		}
		stat.health += amount;
		hpTime = 0.0f;
		if(stat.health >= stat.maxHealth){
			stat.health = stat.maxHealth;
		}
	}
	
	void MPRecovery(){
		int amount = stat.maxMana * mpRegen / 100;
		if(amount <= 1){
			amount = 1;
		}
		stat.mana += amount;
		mpTime = 0.0f;
		if(stat.mana >= stat.maxMana){
			stat.mana = stat.maxMana;
		}
	}
	
	
}