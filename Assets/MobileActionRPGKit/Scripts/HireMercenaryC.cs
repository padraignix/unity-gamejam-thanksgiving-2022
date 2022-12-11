using UnityEngine;
using System.Collections;

public class HireMercenaryC : MonoBehaviour {

	public bool startAfterTalk = false;
	private bool begin = false;
	private bool noCash = false;
	
	private GameObject player;
	private bool enter = false;

	[System.Serializable]
	public class MercenaryInfo {
		public string className = "";
		public GameObject mercenaryPrefab;
		public int level = 1;
		public int atk = 1;
		public int def = 1;
		public int matk = 1;
		public int mdef = 1;
		public int hp = 200;
		public int price = 500;
	}

	public MercenaryInfo[] mercenariesInfo = new MercenaryInfo[9];
	
	void Update(){
		if(Input.GetKeyDown("e") && enter && !begin && !startAfterTalk){
			begin = true;
		}
		if(startAfterTalk && enter && !begin){
			begin = GetComponent<DialogueC>().talkFinish;
		}
		if(startAfterTalk && begin){
			GetComponent<DialogueC>().talkFinish = false;
		}
	}
	
	void OnGUI(){
		if (begin && !noCash && player) {
			GUI.Box (new Rect (Screen.width /2 - 320 ,120, 680, 400), "Mercenaries List");
			if (GUI.Button (new Rect (Screen.width /2 + 160 , 460, 120, 50), "Cancel")) {
				CloseWindow();
			}
			GUI.Label (new Rect (Screen.width /2 - 290 , 490, 200, 30), "$ " + player.GetComponent<InventoryC>().cash);
			//---------------------------------------
			if (GUI.Button (new Rect (Screen.width /2 - 285 , 140, 200, 90), mercenariesInfo[0].className + " : LV " + mercenariesInfo[0].level + "\n Pay :" + mercenariesInfo[0].price)) {
				GetMercenary(0);
				CloseWindow();
			}
			if (GUI.Button (new Rect (Screen.width /2 - 75 , 140, 200, 90), mercenariesInfo[1].className + " : LV " + mercenariesInfo[1].level + "\n Pay :" + mercenariesInfo[1].price)) {
				GetMercenary(1);
				CloseWindow();
			}
			if (GUI.Button (new Rect (Screen.width /2 + 135 , 140, 200, 90), mercenariesInfo[2].className + " : LV " + mercenariesInfo[2].level + "\n Pay :" + mercenariesInfo[2].price)) {
				GetMercenary(2);
				CloseWindow();
			}
			//----------------------
			if (GUI.Button (new Rect (Screen.width /2 - 285 , 250, 200, 90), mercenariesInfo[3].className + " : LV " + mercenariesInfo[3].level + "\n Pay :" + mercenariesInfo[3].price)) {
				GetMercenary(3);
				CloseWindow();
			}
			if (GUI.Button (new Rect (Screen.width /2 - 75 , 250, 200, 90), mercenariesInfo[4].className + " : LV " + mercenariesInfo[4].level + "\n Pay :" + mercenariesInfo[4].price)) {
				GetMercenary(4);
				CloseWindow();
			}
			if (GUI.Button (new Rect (Screen.width /2 + 135 , 250, 200, 90), mercenariesInfo[5].className + " : LV " + mercenariesInfo[5].level + "\n Pay :" + mercenariesInfo[5].price)) {
				GetMercenary(5);
				CloseWindow();
			}
			//--------------------------
			if (GUI.Button (new Rect (Screen.width /2 - 285 , 360, 200, 90), mercenariesInfo[6].className + " : LV " + mercenariesInfo[6].level + "\n Pay :" + mercenariesInfo[6].price)) {
				GetMercenary(6);
				CloseWindow();
			}
			if (GUI.Button (new Rect (Screen.width /2 - 75 , 360, 200, 90), mercenariesInfo[7].className + " : LV " + mercenariesInfo[7].level + "\n Pay :" + mercenariesInfo[7].price)) {
				GetMercenary(7);
				CloseWindow();
			}
			if (GUI.Button (new Rect (Screen.width /2 + 135 , 360, 200, 90), mercenariesInfo[8].className + " : LV " + mercenariesInfo[8].level + "\n Pay :" + mercenariesInfo[8].price)) {
				GetMercenary(8);
				CloseWindow();
			}
		}
		
		if(noCash){
			GUI.Box (new Rect (Screen.width /2 - 125 ,220, 250, 120), "Not Enough Cash!!");
			if (GUI.Button (new Rect (Screen.width /2 - 75 , 255, 150, 60), "OK")) {
				noCash = false;
			}
		}
		
	}
	
	void OnTriggerEnter(Collider other){
		if (other.tag == "Player") {
			player = other.gameObject;
			enter = true;
			begin = false;
		}
	}
	
	void OnTriggerExit(Collider other){
		if (other.tag == "Player") {
			enter = false;
			CloseWindow();
		}		
	}
	
	public void CloseWindow(){
		if(startAfterTalk){
			GetComponent<DialogueC>().talkFinish = false;
		}
		//enter = false;
		begin = false;
	}
	
	public void GetMercenary(int id){
		if (player.GetComponent<InventoryC>().cash >= mercenariesInfo[id].price){
			//Get You Mercenary.
			player.GetComponent<InventoryC>().cash -= mercenariesInfo[id].price;
			if(!player.GetComponent<SpawnPartnerC>()){
				player.AddComponent<SpawnPartnerC>();
			}
			if(mercenariesInfo[id].mercenaryPrefab) {
				//Spawn Mercenary
				Vector3 pos = player.transform.position;
				pos.y += 0.2f;
				pos.x += Random.Range(-1.5f , 1.5f);
				pos.z += Random.Range(-1.5f , 1.5f);

				GameObject m = (GameObject)Instantiate(mercenariesInfo[id].mercenaryPrefab , pos , player.transform.rotation);
				m.GetComponent<AIfriendC>().master = player.transform;
				//Apply Mercenary's Status
				m.GetComponent<StatusC>().level = mercenariesInfo[id].level;
				m.GetComponent<StatusC>().atk = mercenariesInfo[id].atk;
				m.GetComponent<StatusC>().def = mercenariesInfo[id].def;
				m.GetComponent<StatusC>().matk = mercenariesInfo[id].matk;
				m.GetComponent<StatusC>().mdef = mercenariesInfo[id].mdef;
				m.GetComponent<StatusC>().maxHealth = mercenariesInfo[id].hp;
				m.GetComponent<StatusC>().health = mercenariesInfo[id].hp;

				bool full = player.GetComponent<SpawnPartnerC>().AddPartner(m);
				if(full){
					Destroy(player.GetComponent<SpawnPartnerC>().currentPartner[0]);
					player.GetComponent<SpawnPartnerC>().Sort();
					player.GetComponent<SpawnPartnerC>().AddPartner(m);
				}
			}else{
				print("Please Assign Mercenary Prefab");
			}
		}else{
			//When you have not enough cash to hire.
			noCash = true;
		}
	}
}