using UnityEngine;
using System.Collections;

public class TrapShotC : MonoBehaviour {

	public Transform trapPrefab;
	public int damage = 50;
	private float wait = 0f;
	public float delay = 1.0f;
	
	void Update() {
		if(wait >= delay){
			//Shoot
			Transform bulletShootout = (Transform)Instantiate(trapPrefab, transform.position , transform.rotation);
			bulletShootout.GetComponent<BulletStatusC>().Setting(damage , damage , "Enemy" , this.gameObject);
			wait = 0;
		}else{
			wait += Time.deltaTime;
		}
		
	}
}