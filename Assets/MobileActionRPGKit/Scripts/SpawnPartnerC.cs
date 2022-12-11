using UnityEngine;
using System.Collections;

public class SpawnPartnerC : MonoBehaviour {

	public GameObject[] mercenariesPrefab = new GameObject[2];
	public int spawnId = 0;

	//[HideInInspector]
	public GameObject[] currentPartner = new GameObject[3];
	
	void Start() {
		Vector3 pos = transform.position;
		pos += Vector3.back * 3;
		if(mercenariesPrefab[spawnId]){
			GameObject m = (GameObject)Instantiate(mercenariesPrefab[spawnId] , pos , transform.rotation);
			m.GetComponent<AIfriendC>().master = this.transform;
			currentPartner[0] = m;
		}
		
	}
	
	public void MoveToMaster(){
		for(int a = 0; a < currentPartner.Length; a++){
			if(currentPartner[a]){
				Physics.IgnoreCollision(GetComponent<Collider>(), currentPartner[a].GetComponent<Collider>());
				Vector3 pos = transform.position;
				pos.y += 0.2f;
				if(a > 0){
					pos.x += Random.Range(-1.5f , 1.5f);
					pos.z += Random.Range(-1.5f , 1.5f);
				}
				currentPartner[a].transform.position = pos;
			}
		}
	}


	public bool AddPartner(GameObject p){
		bool full = false;
		bool geta = false;
		
		int pt = 0;
		while(pt < currentPartner.Length && !geta){
			if(!currentPartner[pt]) {
				currentPartner[pt] = p;
				p.GetComponent<AIfriendC>().master = this.transform;
				DontDestroyOnLoad(p.gameObject);
				geta = true;
			}else{
				pt++;
				if(pt >= currentPartner.Length){
					full = true;
					print("Full");
				}
			}
		}
		return full;		
	}

	public void Sort(){
		int pt = 0;
		int nextp = 0;
		bool clearr = false;
		while(pt < currentPartner.Length){
			if(!currentPartner[pt]){
				nextp = pt + 1;
				while(nextp < currentPartner.Length && !clearr){
					if(currentPartner[nextp]) {
						currentPartner[pt] = currentPartner[nextp];
						currentPartner[nextp] = null;
						clearr = true;
					}else{
						nextp++;
					}
				}
				clearr = false;
				pt++;
			}else{
				pt++;
			}			
		}		
	}
}