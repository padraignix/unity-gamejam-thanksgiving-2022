using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SaveLoadC : MonoBehaviour {
	
	public bool autoLoad = false;
	public GameObject player;
	private bool menu = false;
	private Vector3 lastPosition;
	//private Transform mainCam;
	public GameObject oldPlayer;
	
	private int saveSlot = 0;
	
	void Start() {
		if(!player){
			player = this.gameObject;
		}
		saveSlot = PlayerPrefs.GetInt("SaveSlot");
		//If PlayerPrefs Loadgame = 10 That mean You Start with Load Game Menu.
		//If You Set Autoload = true It will LoadGame when you start.
		if(PlayerPrefs.GetInt("Loadgame") == 10 || autoLoad){
			LoadGame();
			if(!autoLoad){
				//If You didn't Set autoLoad then reset PlayerPrefs Loadgame to 0 after LoadGame.
				PlayerPrefs.SetInt("Loadgame", 0);
			}
		}
		
	}
	
	void Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			//Open Save Load Menu
			OnOffMenu();
		}
		
	}
	
	public void OnOffMenu(){
		//Freeze Time Scale to 0 if Window is Showing
		if(!menu && Time.timeScale != 0.0){
			menu = true;
			Time.timeScale = 0.0f;
			//Screen.lockCursor = false;
		}else if(menu){
			menu = false;
			Time.timeScale = 1.0f;
			//Screen.lockCursor = true;
		}
	}
	
	void OnGUI(){
		if(menu){
			GUI.Box (new Rect (Screen.width / 2 - 180,190,360,380), "Menu");
			if (GUI.Button (new Rect (Screen.width / 2 - 110,265,220,80), "Save Game")) {
				SaveData();
				OnOffMenu();
			}
			
			if (GUI.Button (new Rect (Screen.width / 2 - 110,355,220,80), "Load Game")) {
				LoadData();
				OnOffMenu();
			}
			
			if (GUI.Button (new Rect (Screen.width / 2 - 110,445,220,80), "Quit Game")) {
				/*var cam : GameObject = GameObject.FindWithTag ("MainCamera");
			Destroy(cam);
			Destroy(player);
			Time.timeScale = 1.0f;
			Application.LoadLevel ("Title");*/
				Application.Quit();
			}
			
			if (GUI.Button (new Rect (Screen.width / 2 + 110,195,60,60), "X")) {
				OnOffMenu();
			}
		}
		
	}
	
	
	public void SaveData() {
		PlayerPrefs.SetInt("PreviousSave" +saveSlot.ToString(), 10);
		PlayerPrefs.SetString("Name" +saveSlot.ToString(), player.GetComponent<StatusC>().characterName);
		PlayerPrefs.SetFloat("PlayerX", player.transform.position.x);
		PlayerPrefs.SetFloat("PlayerY", player.transform.position.y);
		PlayerPrefs.SetFloat("PlayerZ", player.transform.position.z);
		PlayerPrefs.SetInt("ID" +saveSlot.ToString(), player.GetComponent<StatusC>().playerID);
		PlayerPrefs.SetInt("PlayerLevel" +saveSlot.ToString(), player.GetComponent<StatusC>().level);
		PlayerPrefs.SetInt("PlayerATK" +saveSlot.ToString(), player.GetComponent<StatusC>().atk);
		PlayerPrefs.SetInt("PlayerDEF" +saveSlot.ToString(), player.GetComponent<StatusC>().def);
		PlayerPrefs.SetInt("PlayerMATK" +saveSlot.ToString(), player.GetComponent<StatusC>().matk);
		PlayerPrefs.SetInt("PlayerMDEF" +saveSlot.ToString(), player.GetComponent<StatusC>().mdef);
		PlayerPrefs.SetInt("PlayerEXP" +saveSlot.ToString(), player.GetComponent<StatusC>().exp);
		PlayerPrefs.SetInt("PlayerMaxEXP" +saveSlot.ToString(), player.GetComponent<StatusC>().maxExp);
		PlayerPrefs.SetInt("PlayerMaxHP" +saveSlot.ToString(), player.GetComponent<StatusC>().maxHealth);
		PlayerPrefs.SetInt("PlayerHP" +saveSlot.ToString(), player.GetComponent<StatusC>().health);
		PlayerPrefs.SetInt("PlayerMaxMP" +saveSlot.ToString(), player.GetComponent<StatusC>().maxMana);
		//	PlayerPrefs.SetInt("PlayerMP", player.GetComponent<StatusC>().mana);
		PlayerPrefs.SetInt("PlayerSTP" +saveSlot.ToString(), player.GetComponent<StatusC>().statusPoint);
		PlayerPrefs.SetInt("PlayerSKP" +saveSlot.ToString(), player.GetComponent<StatusC>().skillPoint);

		PlayerPrefs.SetInt("Cash" +saveSlot.ToString(), player.GetComponent<InventoryC>().cash);
		int itemSize = player.GetComponent<InventoryC>().itemSlot.Length;
		int a = 0;
		if(itemSize > 0){
			while(a < itemSize){
				PlayerPrefs.SetInt("Item" + a.ToString() +saveSlot.ToString(), player.GetComponent<InventoryC>().itemSlot[a]);
				PlayerPrefs.SetInt("ItemQty" + a.ToString() +saveSlot.ToString(), player.GetComponent<InventoryC>().itemQuantity[a]);
				a++;
			}
		}
		
		int equipSize = player.GetComponent<InventoryC>().equipment.Length;
		a = 0;
		if(equipSize > 0){
			while(a < equipSize){
				PlayerPrefs.SetInt("Equipm" + a.ToString() +saveSlot.ToString(), player.GetComponent<InventoryC>().equipment[a]);
				a++;
			}
		}
		PlayerPrefs.SetInt("WeaEquip" +saveSlot.ToString(), player.GetComponent<InventoryC>().weaponEquip);
		PlayerPrefs.SetInt("ArmoEquip" +saveSlot.ToString(), player.GetComponent<InventoryC>().armorEquip);
		//Save Quest
		int questSize = QuestStatC.questProgress.Length;
		PlayerPrefs.SetInt("QuestSize" +saveSlot.ToString(), questSize);
		a = 0;
		if(questSize > 0){
			while(a < questSize){
				PlayerPrefs.SetInt("Questp" + a.ToString() +saveSlot.ToString(), QuestStatC.questProgress[a]);
				a++;
			}
		}
		int questSlotSize = player.GetComponent<QuestStatC>().questSlot.Length;
		PlayerPrefs.SetInt("QuestSlotSize" +saveSlot.ToString(), questSlotSize);
		a = 0;
		if(questSlotSize > 0){
			while(a < questSlotSize){
				PlayerPrefs.SetInt("Questslot" + a.ToString() +saveSlot.ToString(), player.GetComponent<QuestStatC>().questSlot[a]);
				a++;
			}
		}
		//Save Skill Slot
		a = 0;
		while(a <= 2){
			PlayerPrefs.SetInt("Skill" + a.ToString() +saveSlot.ToString(), player.GetComponent<SkillWindowC>().skill[a]);
			a++;
		}
		//Skill List Slot
		a = 0;
		while(a < player.GetComponent<SkillWindowC>().skillListSlot.Length){
			PlayerPrefs.SetInt("SkillList" + a.ToString() +saveSlot.ToString(), player.GetComponent<SkillWindowC>().skillListSlot[a]);
			a++;
		}

		a = 0;
		while(a < QuestStatC.eventId.Length){
			PlayerPrefs.SetInt("EventID" + a.ToString() +saveSlot.ToString(), QuestStatC.eventId[a]);
			a++;
		}

		//PlayerPrefs.SetInt("Scene" +saveSlot.ToString(), Application.loadedLevel);
		PlayerPrefs.SetInt("Scene" +saveSlot.ToString(), SceneManager.GetActiveScene().buildIndex);

		PlayerPrefs.SetFloat("PlayerX" +saveSlot.ToString(), player.transform.position.x);
		PlayerPrefs.SetFloat("PlayerY" +saveSlot.ToString(), player.transform.position.y);
		PlayerPrefs.SetFloat("PlayerZ" +saveSlot.ToString(), player.transform.position.z);
		
		print("Saved");
	}
	
	public void LoadData() {
		//oldPlayer = GameObject.FindWithTag ("Player");
		GameObject respawn = GameObject.FindWithTag ("Player");
		
		respawn.GetComponent<StatusC>().characterName = PlayerPrefs.GetString("Name" +saveSlot.ToString());
		lastPosition.x = PlayerPrefs.GetFloat("PlayerX");
		lastPosition.y = PlayerPrefs.GetFloat("PlayerY");
		lastPosition.z = PlayerPrefs.GetFloat("PlayerZ");
		respawn.transform.position = lastPosition;
		//GameObject respawn = Instantiate(player, lastPosition , transform.rotation);
		respawn.GetComponent<StatusC>().playerID = PlayerPrefs.GetInt("ID" +saveSlot.ToString());
		respawn.GetComponent<StatusC>().level = PlayerPrefs.GetInt("PlayerLevel" +saveSlot.ToString());
		respawn.GetComponent<StatusC>().atk = PlayerPrefs.GetInt("PlayerATK" +saveSlot.ToString());
		respawn.GetComponent<StatusC>().def = PlayerPrefs.GetInt("PlayerDEF" +saveSlot.ToString());
		respawn.GetComponent<StatusC>().matk = PlayerPrefs.GetInt("PlayerMATK" +saveSlot.ToString());
		respawn.GetComponent<StatusC>().mdef = PlayerPrefs.GetInt("PlayerMDEF" +saveSlot.ToString());
		respawn.GetComponent<StatusC>().mdef = PlayerPrefs.GetInt("PlayerMDEF" +saveSlot.ToString());
		respawn.GetComponent<StatusC>().exp = PlayerPrefs.GetInt("PlayerEXP" +saveSlot.ToString());
		respawn.GetComponent<StatusC>().maxExp = PlayerPrefs.GetInt("PlayerMaxEXP" +saveSlot.ToString());
		respawn.GetComponent<StatusC>().maxHealth = PlayerPrefs.GetInt("PlayerMaxHP" +saveSlot.ToString());
		respawn.GetComponent<StatusC>().health = PlayerPrefs.GetInt("PlayerHP" +saveSlot.ToString());
		//respawn.GetComponent<StatusC>().health = PlayerPrefs.GetInt("PlayerMaxHP");
		respawn.GetComponent<StatusC>().maxMana = PlayerPrefs.GetInt("PlayerMaxMP" +saveSlot.ToString());
		respawn.GetComponent<StatusC>().mana = PlayerPrefs.GetInt("PlayerMaxMP" +saveSlot.ToString());
		respawn.GetComponent<StatusC>().statusPoint = PlayerPrefs.GetInt("PlayerSTP" +saveSlot.ToString());
		respawn.GetComponent<StatusC>().skillPoint = PlayerPrefs.GetInt("PlayerSKP" +saveSlot.ToString());
		//mainCam = GameObject.FindWithTag ("MainCamera").transform;
		//mainCam.GetComponent(ARPGcamera).target = respawn.transform;
		//-------------------------------
		respawn.GetComponent<InventoryC>().cash = PlayerPrefs.GetInt("Cash" +saveSlot.ToString());
		int itemSize = player.GetComponent<InventoryC>().itemSlot.Length;
		int a = 0;
		if(itemSize > 0){
			while(a < itemSize){
				respawn.GetComponent<InventoryC>().itemSlot[a] = PlayerPrefs.GetInt("Item" + a.ToString() +saveSlot.ToString());
				respawn.GetComponent<InventoryC>().itemQuantity[a] = PlayerPrefs.GetInt("ItemQty" + a.ToString() +saveSlot.ToString());
				//-------
				a++;
			}
		}
		
		int equipSize = player.GetComponent<InventoryC>().equipment.Length;
		a = 0;
		if(equipSize > 0){
			while(a < equipSize){
				respawn.GetComponent<InventoryC>().equipment[a] = PlayerPrefs.GetInt("Equipm" + a.ToString() +saveSlot.ToString());
				a++;
			}
		}
		respawn.GetComponent<InventoryC>().weaponEquip = 0;
		respawn.GetComponent<InventoryC>().armorEquip = PlayerPrefs.GetInt("ArmoEquip" +saveSlot.ToString());
		if(PlayerPrefs.GetInt("WeaEquip" +saveSlot.ToString()) == 0){
			respawn.GetComponent<InventoryC>().RemoveWeaponMesh();
		}else{
			respawn.GetComponent<InventoryC>().EquipItem(PlayerPrefs.GetInt("WeaEquip" +saveSlot.ToString()) , respawn.GetComponent<InventoryC>().equipment.Length + 5);
		}
		//----------------------------------
		//Screen.lockCursor = true;

		//Load Quest
		QuestStatC.questProgress = new int[PlayerPrefs.GetInt("QuestSize" +saveSlot.ToString())];
		int questSize = QuestStatC.questProgress.Length;
		a = 0;
		if(questSize > 0){
			while(a < questSize){
				QuestStatC.questProgress[a] = PlayerPrefs.GetInt("Questp" + a.ToString() +saveSlot.ToString());
				a++;
			}
		}
		
		respawn.GetComponent<QuestStatC>().questSlot = new int[PlayerPrefs.GetInt("QuestSlotSize" +saveSlot.ToString())];
		int questSlotSize = respawn.GetComponent<QuestStatC>().questSlot.Length;
		a = 0;
		if(questSlotSize > 0){
			while(a < questSlotSize){
				respawn.GetComponent<QuestStatC>().questSlot[a] = PlayerPrefs.GetInt("Questslot" + a.ToString() +saveSlot.ToString());
				a++;
			}
		}
		
		//Load Skill Slot
		a = 0;
		while(a <= 2){
			respawn.GetComponent<SkillWindowC>().skill[a] = PlayerPrefs.GetInt("Skill" + a.ToString() +saveSlot.ToString());
			a++;
		}
		//Skill List Slot
		a = 0;
		while(a < player.GetComponent<SkillWindowC>().skillListSlot.Length){
			player.GetComponent<SkillWindowC>().skillListSlot[a] = PlayerPrefs.GetInt("SkillList" + a.ToString() +saveSlot.ToString());
			a++;
		}
		respawn.GetComponent<SkillWindowC>().AssignAllSkill();
		//---------------Set Target to Minimap--------------
		GameObject minimap = GameObject.FindWithTag("Minimap");
		if(minimap){
			GameObject mapcam = minimap.GetComponent<MinimapOnOffC>().minimapCam;
			mapcam.GetComponent<MinimapCameraC>().target = respawn.transform;
		}

		a = 0;
		while(a < QuestStatC.eventId.Length){
			QuestStatC.eventId[a] = PlayerPrefs.GetInt("EventID" + a.ToString() +saveSlot.ToString());
			a++;
		}

		lastPosition.x = PlayerPrefs.GetFloat("PlayerX" +saveSlot.ToString());
		lastPosition.y = PlayerPrefs.GetFloat("PlayerY" +saveSlot.ToString()) + 0.5f;
		lastPosition.z = PlayerPrefs.GetFloat("PlayerZ" +saveSlot.ToString());
		respawn.transform.position = lastPosition;
		int sceneLoad = PlayerPrefs.GetInt("Scene" +saveSlot.ToString());
		//Application.LoadLevel(sceneLoad);
		SceneManager.LoadScene(sceneLoad, LoadSceneMode.Single);
		//player = GameObject.FindWithTag ("Player");
		/*if(oldPlayer){
			Destroy(gameObject);
		}*/
	}
	
	//Function LoadGame is unlike the Function LoadData.
	//This Function will not spawn new Player.
	public void LoadGame(){
		player.GetComponent<StatusC>().characterName = PlayerPrefs.GetString("Name" +saveSlot.ToString());
		player.GetComponent<StatusC>().playerID = PlayerPrefs.GetInt("ID" +saveSlot.ToString());
		player.GetComponent<StatusC>().level = PlayerPrefs.GetInt("PlayerLevel" +saveSlot.ToString());
		player.GetComponent<StatusC>().atk = PlayerPrefs.GetInt("PlayerATK" +saveSlot.ToString());
		player.GetComponent<StatusC>().def = PlayerPrefs.GetInt("PlayerDEF" +saveSlot.ToString());
		player.GetComponent<StatusC>().matk = PlayerPrefs.GetInt("PlayerMATK" +saveSlot.ToString());
		player.GetComponent<StatusC>().mdef = PlayerPrefs.GetInt("PlayerMDEF" +saveSlot.ToString());
		player.GetComponent<StatusC>().mdef = PlayerPrefs.GetInt("PlayerMDEF" +saveSlot.ToString());
		player.GetComponent<StatusC>().exp = PlayerPrefs.GetInt("PlayerEXP" +saveSlot.ToString());
		player.GetComponent<StatusC>().maxExp = PlayerPrefs.GetInt("PlayerMaxEXP" +saveSlot.ToString());
		player.GetComponent<StatusC>().maxHealth = PlayerPrefs.GetInt("PlayerMaxHP" +saveSlot.ToString());
		player.GetComponent<StatusC>().health = PlayerPrefs.GetInt("PlayerMaxHP" +saveSlot.ToString());
		player.GetComponent<StatusC>().maxMana = PlayerPrefs.GetInt("PlayerMaxMP" +saveSlot.ToString());
		player.GetComponent<StatusC>().mana = PlayerPrefs.GetInt("PlayerMaxMP" +saveSlot.ToString());	
		player.GetComponent<StatusC>().statusPoint = PlayerPrefs.GetInt("PlayerSTP" +saveSlot.ToString());
		player.GetComponent<StatusC>().skillPoint = PlayerPrefs.GetInt("PlayerSKP" +saveSlot.ToString());
		//mainCam = GameObject.FindWithTag ("MainCamera").transform;
		//mainCam.GetComponent(ARPGcamera).target = respawn.transform;
		//-------------------------------
		player.GetComponent<InventoryC>().cash = PlayerPrefs.GetInt("Cash" +saveSlot.ToString());
		int itemSize = player.GetComponent<InventoryC>().itemSlot.Length;
		int a = 0;
		if(itemSize > 0){
			while(a < itemSize){
				player.GetComponent<InventoryC>().itemSlot[a] = PlayerPrefs.GetInt("Item" + a.ToString() +saveSlot.ToString());
				player.GetComponent<InventoryC>().itemQuantity[a] = PlayerPrefs.GetInt("ItemQty" + a.ToString() +saveSlot.ToString());
				//-------
				a++;
			}
		}
		
		int equipSize = player.GetComponent<InventoryC>().equipment.Length;
		a = 0;
		if(equipSize > 0){
			while(a < equipSize){
				player.GetComponent<InventoryC>().equipment[a] = PlayerPrefs.GetInt("Equipm" + a.ToString() +saveSlot.ToString());
				a++;
			}
		}
		player.GetComponent<InventoryC>().weaponEquip = 0;
		player.GetComponent<InventoryC>().armorEquip = PlayerPrefs.GetInt("ArmoEquip" +saveSlot.ToString());
		if(PlayerPrefs.GetInt("WeaEquip" +saveSlot.ToString()) == 0){
			player.GetComponent<InventoryC>().RemoveWeaponMesh();
		}else{
			player.GetComponent<InventoryC>().EquipItem(PlayerPrefs.GetInt("WeaEquip" +saveSlot.ToString()) , player.GetComponent<InventoryC>().equipment.Length + 5);
		}
		//----------------------------------
		//Screen.lockCursor = true;

		//Load Quest
		QuestStatC.questProgress = new int[PlayerPrefs.GetInt("QuestSize" +saveSlot.ToString())];
		int questSize = QuestStatC.questProgress.Length;
		a = 0;
		if(questSize > 0){
			while(a < questSize){
				QuestStatC.questProgress[a] = PlayerPrefs.GetInt("Questp" + a.ToString() +saveSlot.ToString());
				a++;
			}
		}
		
		player.GetComponent<QuestStatC>().questSlot = new int[PlayerPrefs.GetInt("QuestSlotSize" +saveSlot.ToString())];
		int questSlotSize = player.GetComponent<QuestStatC>().questSlot.Length;
		a = 0;
		if(questSlotSize > 0){
			while(a < questSlotSize){
				player.GetComponent<QuestStatC>().questSlot[a] = PlayerPrefs.GetInt("Questslot" + a.ToString() +saveSlot.ToString());
				a++;
			}
		}
		
		//Load Skill Slot
		a = 0;
		while(a <= 2){
			player.GetComponent<SkillWindowC>().skill[a] = PlayerPrefs.GetInt("Skill" + a.ToString() +saveSlot.ToString());
			a++;
		}
		//Skill List Slot
		a = 0;
		while(a < player.GetComponent<SkillWindowC>().skillListSlot.Length){
			player.GetComponent<SkillWindowC>().skillListSlot[a] = PlayerPrefs.GetInt("SkillList" + a.ToString() +saveSlot.ToString());
			a++;
		}
		player.GetComponent<SkillWindowC>().AssignAllSkill();

		a = 0;
		while(a < QuestStatC.eventId.Length){
			QuestStatC.eventId[a] = PlayerPrefs.GetInt("EventID" + a.ToString() +saveSlot.ToString());
			a++;
		}

		lastPosition.x = PlayerPrefs.GetFloat("PlayerX" +saveSlot.ToString());
		lastPosition.y = PlayerPrefs.GetFloat("PlayerY" +saveSlot.ToString()) + 0.5f;
		lastPosition.z = PlayerPrefs.GetFloat("PlayerZ" +saveSlot.ToString());
		//var respawn : GameObject = Instantiate(player, lastPosition , transform.rotation);
		
		player.transform.position = lastPosition;
		int sceneLoad = PlayerPrefs.GetInt("Scene" +saveSlot.ToString());
		//Application.LoadLevel(sceneLoad);
		SceneManager.LoadScene(sceneLoad, LoadSceneMode.Single);
		player.transform.position = lastPosition;
		
		print("Loaded");
		
	}
}
