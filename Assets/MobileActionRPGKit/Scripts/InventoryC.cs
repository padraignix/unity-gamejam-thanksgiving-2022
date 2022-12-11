using UnityEngine;
using System.Collections;

public class InventoryC : MonoBehaviour {

	private bool menu = false;
	private bool itemMenu = true;
	private bool equipMenu = false;
	
	public int[] itemSlot = new int[15];
	public int[] itemQuantity = new int[15];
	public int[] equipment = new int[12];
	
	public int weaponEquip = 0;
	public bool allowWeaponUnequip = false;
	public int armorEquip = 0;
	public bool allowArmorUnequip = true;
	
	public GameObject[] weapon = new GameObject[1];
	
	public GameObject player;
	public GameObject database;
	public GameObject fistPrefab;
	
	public int cash = 500;
	
	public GUISkin skin;
	public Rect windowRect = new Rect (360 ,140 ,480 ,550);
	private Rect originalRect;
	
	public int itemPageMultiply = 5;
	public int equipmentPageMultiply = 4;
	private int page = 0;
	
	public GUIStyle itemNameText;
	public GUIStyle itemDescriptionText;
	public GUIStyle itemQuantityText;
	public bool useLegacyUi = false;
	
	void Start() {
		if(!player) {
			player = this.gameObject;
		}
		//ItemDataC dataItem = database.GetComponent<ItemDataC>();
		originalRect = windowRect;
		SetEquipmentStatus();
	}
	
	public void SetEquipmentStatus() {
		ItemDataC dataItem = database.GetComponent<ItemDataC>();
		//Reset Power of Current Weapon & Armor
		player.GetComponent<StatusC>().addAtk = 0;
		player.GetComponent<StatusC>().addDef = 0;
		player.GetComponent<StatusC>().addMatk = 0;
		player.GetComponent<StatusC>().addMdef = 0;
		player.GetComponent<StatusC>().weaponAtk = 0;
		player.GetComponent<StatusC>().weaponMatk = 0;
		player.GetComponent<StatusC>().addHPpercent = 0;
		player.GetComponent<StatusC>().addMPpercent = 0;
		//Set New Variable of Weapon
		player.GetComponent<StatusC>().weaponAtk += dataItem.equipment[weaponEquip].attack;
		player.GetComponent<StatusC>().addDef += dataItem.equipment[weaponEquip].defense;
		player.GetComponent<StatusC>().weaponMatk += dataItem.equipment[weaponEquip].magicAttack;
		player.GetComponent<StatusC>().addMdef += dataItem.equipment[weaponEquip].magicDefense;
		//Set New Variable of Armor
		player.GetComponent<StatusC>().weaponAtk += dataItem.equipment[armorEquip].attack;
		player.GetComponent<StatusC>().addDef += dataItem.equipment[armorEquip].defense;
		player.GetComponent<StatusC>().weaponMatk += dataItem.equipment[armorEquip].magicAttack;
		player.GetComponent<StatusC>().addMdef += dataItem.equipment[armorEquip].magicDefense;
		
		player.GetComponent<StatusC>().CalculateStatus();
	}
	
	void Update(){
		if(Input.GetKeyDown("i") || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) {
			if(useLegacyUi){
				OnOffMenu();
			}
			//AutoSortItem();
		}
	}
	
	public void UseItem(int slot){
		ItemDataC dataItem = database.GetComponent<ItemDataC>();
		int id = itemSlot[slot];
		if(dataItem.usableItem[id].unusable){
			return;
		}
		GetComponent<StatusC>().Heal(dataItem.usableItem[id].hpRecover , dataItem.usableItem[id].mpRecover);
		GetComponent<StatusC>().atk += dataItem.usableItem[id].atkPlus;
		GetComponent<StatusC>().def += dataItem.usableItem[id].defPlus;
		GetComponent<StatusC>().matk += dataItem.usableItem[id].matkPlus;
		GetComponent<StatusC>().mdef += dataItem.usableItem[id].mdefPlus;

		itemQuantity[slot]--;
		if(itemQuantity[slot] <= 0){
			itemSlot[slot] = 0;
			itemQuantity[slot] = 0;
		}
		
		AutoSortItem();
	}
	
	public void EquipItem(int id, int slot) {
		if (id == 0) {
			return;
		}
		if (!player) {
			player = this.gameObject;
		}

		GameObject wea;

		ItemDataC dataItem = database.GetComponent<ItemDataC>();
		//Backup Your Current Equipment before Unequip
		int tempEquipment = 0;
		
		if(dataItem.equipment[id].EquipmentType == EqType.Weapon) {
			//Weapon Type
			tempEquipment = weaponEquip;
			weaponEquip = id;
			if(dataItem.equipment[id].attackPrefab) {
				player.GetComponent<AttackTriggerC>().attackPrefab = dataItem.equipment[id].attackPrefab.transform;
			}
			if(GetComponent<CharacterMouseMoveC>()){
				GetComponent<CharacterMouseMoveC>().targetApproachDistance = dataItem.equipment[id].range;
			}
			//Change Weapon Mesh
			if (dataItem.equipment[id].model && weapon.Length > 0 && weapon[0]) {
				int allWeapon = weapon.Length;
				int a = 0;
				if (allWeapon > 0 && dataItem.equipment[id].assignAllWeapon) {
					while (a < allWeapon && weapon[a]) {
						//weapon[a].SetActiveRecursively(true);
						weapon[a].SetActive(true);
						wea = (GameObject)Instantiate(dataItem.equipment[id].model,weapon[a].transform.position,weapon[a].transform.rotation);
						wea.transform.parent = weapon[a].transform.parent;
						Destroy(weapon[a].gameObject);
						weapon[a] = wea;
						a++;
					}
				} else if(allWeapon > 0) {
					while (a < allWeapon && weapon[a]) {
						if (a == 0) {
							//weapon[a].SetActiveRecursively(true);
							weapon[a].SetActive(true);
							wea = (GameObject)Instantiate(dataItem.equipment[id].model,weapon[a].transform.position,weapon[a].transform.rotation);
							wea.transform.parent = weapon[a].transform.parent;
							Destroy(weapon[a].gameObject);
							weapon[a] = wea;
						}else{
							//weapon[a].SetActiveRecursively(false);
							weapon[a].SetActive(false);
						}
						a++;
					}
				}
			}
		} else {
			//Armor Type
			tempEquipment = armorEquip;
			armorEquip = id;
		}
		if (slot <= equipment.Length) {
			equipment[slot] = 0;
		}
		//Assign Weapon Animation to PlayerAnimation Script
		AssignWeaponAnimation(id);
		//Reset Power of Current Weapon & Armor
		SetEquipmentStatus();
		
		AutoSortEquipment();
		AddEquipment(tempEquipment);		
	}
	
	public void RemoveWeaponMesh() {
		if (weapon.Length > 0 && weapon[0]) { 
			int allWeapon = weapon.Length;
			int a = 0;
			if (allWeapon > 0) {
				while (a < allWeapon && weapon[a]) {
					//weapon[a].SetActiveRecursively(false);
					weapon[a].SetActive(false);
					//Destroy(weapon[a].gameObject);
					a++;
				}
			}
		}
	}
	
	public void UnEquip(int id) {
		ItemDataC dataItem = database.GetComponent<ItemDataC>();
		bool full = false;

		if (!player) {
			player = this.gameObject;
		}
		if (dataItem.equipment[id].model && weapon.Length > 0) {
			full = AddEquipment(weaponEquip);
		} else {
			full = AddEquipment(armorEquip);
		}
		if (!full) {
			if (dataItem.equipment[id].model && weapon.Length > 0) {
				weaponEquip = 0;
				player.GetComponent<AttackTriggerC>().attackPrefab = fistPrefab.transform;
				if (weapon.Length > 0 && weapon[0]) {
					int allWeapon = weapon.Length;
					int a = 0;
					if (allWeapon > 0) {
						while (a < allWeapon && weapon[a]) {
							//weapon[a].SetActiveRecursively(false);
							weapon[a].SetActive(false);
							//Destroy(weapon[a].gameObject);
							a++;
						}
					}
				}
			} else {
				armorEquip = 0;
			}
			SetEquipmentStatus();
		}
	}
	
	void OnGUI() {
		GUI.skin = skin;
		if (menu && itemMenu) {
			windowRect = GUI.Window (1, windowRect, ItemWindow, "Items");
		}
		if (menu && equipMenu) {
			windowRect = GUI.Window (1, windowRect, ItemWindow, "Equipment");
		}
		
		if (menu) {
			if (GUI.Button ( new Rect(windowRect.x -50, windowRect.y +105,50,100), "Item")) {
				//Switch to Item Tab
				page = 0;
				itemMenu = true;
				equipMenu = false;
			}
			if (GUI.Button ( new Rect(windowRect.x -50, windowRect.y +225,50,100), "Equip")) {
				//Switch to Equipment Tab
				page = 0;
				equipMenu = true;
				itemMenu = false;	
			}
		}
	}
	
	//-----------Item Window-------------
	public void ItemWindow(int windowID) {
		ItemDataC dataItem = database.GetComponent<ItemDataC>();
		if (menu && itemMenu) {
			//GUI.Box ( new Rect(260,140,280,385), "Items");
			//Close Window Button
			if (GUI.Button ( new Rect(390,8,70,70), "X")) {
				OnOffMenu();
			}
			//Items Slot
			if (GUI.Button ( new Rect(30,30,75,75),dataItem.usableItem[itemSlot[0 + page]].icon)){
				if(!dataItem.usableItem[itemSlot[0 + page]].unusable){
					UseItem(0 + page);
				}
			}
			GUI.Label ( new Rect(125, 40, 320, 75), dataItem.usableItem[itemSlot[0 + page]].itemName.ToString() , itemNameText); //Item Name
			GUI.Label ( new Rect(125, 65, 320, 75), dataItem.usableItem[itemSlot[0 + page]].description.ToString() , itemDescriptionText); //Item Description
			if(itemQuantity[0 + page] > 0){
				GUI.Label ( new Rect(88, 88, 40, 30), itemQuantity[0 + page].ToString() , itemQuantityText); //Quantity
			}
			//------------------------------------------------------
			if (GUI.Button ( new Rect(30,120,75,75),dataItem.usableItem[itemSlot[1 + page]].icon)){
				if(!dataItem.usableItem[itemSlot[1 + page]].unusable){
					UseItem(1 + page);
				}
			}
			GUI.Label ( new Rect(125, 130, 320, 75), dataItem.usableItem[itemSlot[1 + page]].itemName.ToString() , itemNameText); //Item Name
			GUI.Label ( new Rect(125, 155, 320, 75), dataItem.usableItem[itemSlot[1 + page]].description.ToString() , itemDescriptionText); //Item Description
			if(itemQuantity[1 + page] > 0){
				GUI.Label ( new Rect(88, 178, 40, 30), itemQuantity[1 + page].ToString() , itemQuantityText); //Quantity
			}
			//------------------------------------------------------
			if (GUI.Button ( new Rect(30,210,75,75),dataItem.usableItem[itemSlot[2 + page]].icon)){
				if(!dataItem.usableItem[itemSlot[2 + page]].unusable){
					UseItem(2 + page);
				}
			}
			GUI.Label ( new Rect(125, 220, 320, 75), dataItem.usableItem[itemSlot[2 + page]].itemName.ToString() , itemNameText); //Item Name
			GUI.Label ( new Rect(125, 245, 320, 75), dataItem.usableItem[itemSlot[2 + page]].description.ToString() , itemDescriptionText); //Item Description
			if(itemQuantity[2 + page] > 0){
				GUI.Label ( new Rect(88, 268, 40, 30), itemQuantity[2 + page].ToString() , itemQuantityText); //Quantity
			}
			//------------------------------------------------------
			if (GUI.Button ( new Rect(30,300,75,75),dataItem.usableItem[itemSlot[3 + page]].icon)){
				if(!dataItem.usableItem[itemSlot[3 + page]].unusable){
					UseItem(3 + page);
				}
			}
			GUI.Label ( new Rect(125, 310, 320, 75), dataItem.usableItem[itemSlot[3 + page]].itemName.ToString() , itemNameText); //Item Name
			GUI.Label ( new Rect(125, 335, 320, 75), dataItem.usableItem[itemSlot[3 + page]].description.ToString() , itemDescriptionText); //Item Description
			if(itemQuantity[3 + page] > 0){
				GUI.Label ( new Rect(88, 358, 40, 30), itemQuantity[3 + page].ToString() , itemQuantityText); //Quantity
			}
			//------------------------------------------------------
			if (GUI.Button ( new Rect(30,390,75,75),dataItem.usableItem[itemSlot[4 + page]].icon)){
				if(!dataItem.usableItem[itemSlot[4 + page]].unusable){
					UseItem(4 + page);
				}
			}
			GUI.Label ( new Rect(125, 400, 320, 75), dataItem.usableItem[itemSlot[4 + page]].itemName.ToString() , itemNameText); //Item Name
			GUI.Label ( new Rect(125, 425, 320, 75), dataItem.usableItem[itemSlot[4 + page]].description.ToString() , itemDescriptionText); //Item Description
			if(itemQuantity[4 + page] > 0){
				GUI.Label ( new Rect(88, 448, 40, 30), itemQuantity[4 + page].ToString() , itemQuantityText); //Quantity
			}
			//------------------------------------------------------
			
			if(GUI.Button (new Rect (220,485,50,52), "1")){
				page = 0;
			}
			if(GUI.Button (new Rect (290,485,50,52), "2")){
				page = itemPageMultiply;
			}
			if(GUI.Button (new Rect (360,485,50,52), "3")){
				page = itemPageMultiply *2;
			}
			
			GUI.Label ( new Rect(20, 505, 150, 50), "$ " + cash.ToString() , itemDescriptionText);
			//---------------------------
		}
		
		//---------------Equipment Tab----------------------------
		if (menu && equipMenu) {
			//Close Window Button
			if (GUI.Button ( new Rect(390,8,70,70), "X")) {
				OnOffMenu();
			}
			//Weapon
			GUI.Label ( new Rect(20, 60, 150, 50), "Weapon");			
			if (GUI.Button ( new Rect(100,30,70,70), dataItem.equipment[weaponEquip].icon)){
				if (!allowWeaponUnequip || weaponEquip == 0) {
					return;
				}
				UnEquip(weaponEquip);
			}
			//Armor
			GUI.Label ( new Rect(200, 60, 150, 50), "Armor");
			if (GUI.Button ( new Rect(260,30,70,70), dataItem.equipment[armorEquip].icon)){
				if (!allowArmorUnequip || armorEquip == 0) {
					return;
				}
				UnEquip(armorEquip);
				
			}
			
			
			//--------Equipment Slot---------
			if (GUI.Button ( new Rect(30,130,75,75),dataItem.equipment[equipment[0 + page]].icon)){
				EquipItem(equipment[0 + page] , 0 + page);
			}
			GUI.Label ( new Rect(125, 140, 320, 75), dataItem.equipment[equipment[0 + page]].itemName.ToString() , itemNameText); //Item Name
			GUI.Label ( new Rect(125, 165, 320, 75), dataItem.equipment[equipment[0 + page]].description.ToString() , itemDescriptionText); //Item Description
			//------------------------------------------------------
			if (GUI.Button ( new Rect(30,220,75,75),dataItem.equipment[equipment[1 + page]].icon)){
				EquipItem(equipment[1 + page] , 1 + page);
			}
			GUI.Label ( new Rect(125, 230, 320, 75), dataItem.equipment[equipment[1 + page]].itemName.ToString() , itemNameText); //Item Name
			GUI.Label ( new Rect(125, 255, 320, 75), dataItem.equipment[equipment[1 + page]].description.ToString() , itemDescriptionText); //Item Description
			//------------------------------------------------------
			if (GUI.Button ( new Rect(30,310,75,75),dataItem.equipment[equipment[2 + page]].icon)){
				EquipItem(equipment[2 + page] , 2 + page);
			}
			GUI.Label ( new Rect(125, 320, 320, 75), dataItem.equipment[equipment[2 + page]].itemName.ToString() , itemNameText); //Item Name
			GUI.Label ( new Rect(125, 345, 320, 75), dataItem.equipment[equipment[2 + page]].description.ToString() , itemDescriptionText); //Item Description
			//------------------------------------------------------
			if (GUI.Button ( new Rect(30,400,75,75),dataItem.equipment[equipment[3 + page]].icon)){
				EquipItem(equipment[3 + page] , 3 + page);
			}
			GUI.Label ( new Rect(125, 410, 320, 75), dataItem.equipment[equipment[3 + page]].itemName.ToString() , itemNameText); //Item Name
			GUI.Label ( new Rect(125, 435, 320, 75), dataItem.equipment[equipment[3 + page]].description.ToString() , itemDescriptionText); //Item Description
			//------------------------------------------------------
			
			if (GUI.Button (new Rect (220,485,50,52), "1")) {
				page = 0;
			}
			if (GUI.Button (new Rect (290,485,50,52), "2")) {
				page = equipmentPageMultiply;
			}
			if (GUI.Button (new Rect (360,485,50,52), "3")) {
				page = equipmentPageMultiply *2;
			}
			
			GUI.Label ( new Rect(20, 505, 150, 50), "$ " + cash.ToString() , itemDescriptionText);
			
		}
		GUI.DragWindow (new Rect (0,0,10000,10000)); 
	}
	
	public bool AddItem(int id, int quan) {
		bool full = false;
		bool geta = false;
		
		int pt = 0;
		while (pt < itemSlot.Length && !geta) {
			if (itemSlot[pt] == id) {
				itemQuantity[pt] += quan;
				geta = true;
			} else if(itemSlot[pt] == 0) {
				itemSlot[pt] = id;
				itemQuantity[pt] = quan;
				geta = true;
			} else {
				pt++;
				if (pt >= itemSlot.Length) {
					full = true;
					print("Full");
				}
			}			
		}
		
		return full;
		
	}
	
	public bool AddEquipment(int id){
		bool full = false;
		bool geta = false;
				
		int pt = 0;
		while (pt < equipment.Length && !geta){
			if (equipment[pt] == 0) {
				equipment[pt] = id;
				geta = true;
			}else{
				pt++;
				if(pt >= equipment.Length){
					full = true;
					print("Full");
				}
			}
		}
		return full;		
	}
	//------------AutoSort----------

	public void AutoSortItem(){
		int pt = 0;
		int nextp = 0;
		bool clearr = false;
		while(pt < itemSlot.Length){
			if(itemSlot[pt] == 0){
				nextp = pt + 1;
				while(nextp < itemSlot.Length && !clearr){
					if(itemSlot[nextp] > 0) {
						//Fine Next Item and Set
						itemSlot[pt] = itemSlot[nextp];
						itemQuantity[pt] = itemQuantity[nextp];
						itemSlot[nextp] = 0;
						itemQuantity[nextp] = 0;
						clearr = true;
					}else{
						nextp++;
					}
				}
				//Continue New Loop
				clearr = false;
				pt++;
			}else{
				pt++;
			}
		}		
	}
	
	public void AutoSortEquipment(){
		int pt = 0;
		int nextp = 0;
		bool clearr = false;
		while(pt < equipment.Length){
			if(equipment[pt] == 0){
				nextp = pt + 1;
				while(nextp < equipment.Length && !clearr){
					if(equipment[nextp] > 0) {
						//Fine Next Item and Set
						equipment[pt] = equipment[nextp];
						equipment[nextp] = 0;
						clearr = true;
					}else{
						nextp++;
					}
				}
				//Continue New Loop
				clearr = false;
				pt++;
			}else{
				pt++;
			}			
		}		
	}
	
	public void OnOffMenu() {
		//Freeze Time Scale to 0 if Window is Showing
		if(!menu && Time.timeScale != 0.0f){
			menu = true;
			Time.timeScale = 0.0f;
			ResetPosition();
			//Screen.lockCursor = false;
		}else if(menu){
			menu = false;
			Time.timeScale = 1.0f;
			//Screen.lockCursor = true;
		}
		
	}
	
	public void AssignWeaponAnimation(int id){
		ItemDataC dataItem = database.GetComponent<ItemDataC>();
		PlayerAnimationC playerAnim = player.GetComponent<PlayerAnimationC>();

		if(GetComponent<CharacterMouseMoveC>()){
			//If use CharacterMouseMove Controller
			AssignMouseMoveAnimation(id);
			return;
		}
		if(GetComponent<PlayerMecanimAnimationC>()){
			//If use Mecanim
			AssignMecanimAnimation(id);
			return;
		}
		if(!playerAnim){
			return;
		}
		
		//Assign All Attack Combo Animation of the weapon from Database
		if(dataItem.equipment[id].attackCombo.Length > 0 && dataItem.equipment[id].attackCombo[0] && dataItem.equipment[id].EquipmentType == EqType.Weapon){
			int allPrefab = dataItem.equipment[id].attackCombo.Length;
			player.GetComponent<AttackTriggerC>().attackCombo = new AnimationClip[allPrefab];
			player.GetComponent<AttackTriggerC>().c = 0;
			
			int a = 0;
			if(allPrefab > 0){
				while(a < allPrefab){
					player.GetComponent<AttackTriggerC>().attackCombo[a] = dataItem.equipment[id].attackCombo[a];
					player.GetComponent<AttackTriggerC>().mainModel.GetComponent<Animation>()[dataItem.equipment[id].attackCombo[a].name].layer = 15;
					a++;
				}
			}
			player.GetComponent<AttackTriggerC>().whileAttack = dataItem.equipment[id].whileAttack;
			//Assign Attack Speed
			player.GetComponent<AttackTriggerC>().attackSpeed = dataItem.equipment[id].attackSpeed;
			player.GetComponent<AttackTriggerC>().atkDelay1 = dataItem.equipment[id].attackDelay;
		}
		
		if(dataItem.equipment[id].idleAnimation){
			playerAnim.idle = dataItem.equipment[id].idleAnimation;
		}
		if(dataItem.equipment[id].runAnimation){
			playerAnim.run = dataItem.equipment[id].runAnimation;
		}
		if(dataItem.equipment[id].rightAnimation){
			playerAnim.right = dataItem.equipment[id].rightAnimation;
		}
		if(dataItem.equipment[id].leftAnimation){
			playerAnim.left = dataItem.equipment[id].leftAnimation;
		}
		if(dataItem.equipment[id].backAnimation){
			playerAnim.back = dataItem.equipment[id].backAnimation;
		}
		if(dataItem.equipment[id].jumpAnimation){
			playerAnim.jump = dataItem.equipment[id].jumpAnimation;
		}
		//playerAnim.AnimationSpeedSet();
		
	}
	
	public void AssignMecanimAnimation(int id){
		ItemDataC dataItem = database.GetComponent<ItemDataC>();
		if(dataItem.equipment[id].EquipmentType == EqType.Weapon){
			player.GetComponent<AttackTriggerC>().whileAttack = dataItem.equipment[id].whileAttack;
			//Assign Attack Speed
			player.GetComponent<AttackTriggerC>().attackSpeed = dataItem.equipment[id].attackSpeed;
			player.GetComponent<AttackTriggerC>().atkDelay1 = dataItem.equipment[id].attackDelay;
			//Set Weapon Type ID to Mecanim Animator and Set New Idle
			player.GetComponent<PlayerMecanimAnimationC>().SetWeaponType(dataItem.equipment[id].weaponType , dataItem.equipment[id].idleAnimation.name);
			
			int allPrefab = dataItem.equipment[id].attackCombo.Length;
			player.GetComponent<AttackTriggerC>().attackCombo = new AnimationClip[allPrefab];
			
			//Set Attack Animation
			int a = 0;
			if(allPrefab > 0){
				while(a < allPrefab){
					player.GetComponent<AttackTriggerC>().attackCombo[a] = dataItem.equipment[id].attackCombo[a];
					a++;
				}
			}
		}
		
	}

	public void AssignMouseMoveAnimation(int id){
		ItemDataC dataItem = database.GetComponent<ItemDataC>();
		//Assign All Attack Combo Animation of the weapon from Database
		if(dataItem.equipment[id].attackCombo.Length > 0 && dataItem.equipment[id].attackCombo[0] && dataItem.equipment[id].EquipmentType == EqType.Weapon){
			int allPrefab = dataItem.equipment[id].attackCombo.Length;
			GetComponent<AttackTriggerC>().attackCombo = new AnimationClip[allPrefab];
			GetComponent<AttackTriggerC>().c = 0;
			
			int a = 0;
			if(allPrefab > 0){
				while(a < allPrefab){
					GetComponent<AttackTriggerC>().attackCombo[a] = dataItem.equipment[id].attackCombo[a];
					if(GetComponent<CharacterMouseMoveC>().animationType == AnimMode.Legacy)
						GetComponent<AttackTriggerC>().mainModel.GetComponent<Animation>()[dataItem.equipment[id].attackCombo[a].name].layer = 15;
					a++;
				}
			}
			player.GetComponent<AttackTriggerC>().whileAttack = dataItem.equipment[id].whileAttack;
			//Assign Attack Speed
			GetComponent<AttackTriggerC>().attackSpeed = dataItem.equipment[id].attackSpeed;
			GetComponent<AttackTriggerC>().atkDelay1 = dataItem.equipment[id].attackDelay;

			//For Mecanim(Weapon Animation)
			if(GetComponent<CharacterMouseMoveC>().animationType == AnimMode.Mecanim)
				GetComponent<CharacterMouseMoveC>().SetWeaponType(dataItem.equipment[id].weaponType , dataItem.equipment[id].idleAnimation.name);
		}
		
		if(GetComponent<CharacterMouseMoveC>().animationType == AnimMode.Legacy){
			if(dataItem.equipment[id].idleAnimation){
				GetComponent<CharacterMouseMoveC>().legacyAnimationSet.idle = dataItem.equipment[id].idleAnimation;
			}
			if(dataItem.equipment[id].runAnimation){
				GetComponent<CharacterMouseMoveC>().legacyAnimationSet.run = dataItem.equipment[id].runAnimation;
			}
			GetComponent<CharacterMouseMoveC>().SetAnimationSpeed();
		}
	}
	
	public void ResetPosition() {
		//Reset GUI Position when it out of Screen.
		if(windowRect.x >= Screen.width -30 || windowRect.y >= Screen.height -30 || windowRect.x <= -70 || windowRect.y <= -70 ){
			windowRect = originalRect;
		}
	}

	public bool CheckItem(int id , int type, int qty){
		bool having = false;
		bool geta = false;
		//type 0 = Usable , 1 = Equipment
		
		int pt = 0;
		
		//================Usable==================
		if(type == 0){
			while(pt < itemSlot.Length && !geta){
				if(itemSlot[pt] == id){
					if(itemQuantity[pt] >= qty){
						having = true;
					}
					geta = true;
				}else{
					pt++;
				}
				//--------------------------
			}
		}
		//=================Equipment=================
		if(type == 1){
			while(pt < equipment.Length && !geta){
				if(equipment[pt] == id){
					having = true;
					geta = true;
				}else{
					pt++;
				}
				//--------------------------
			}
		}
		
		return having;
		
	}
	
	
	public int FindItemSlot(int id) {
		bool geta = false;
		int pt = 0;
		while(pt < itemSlot.Length && !geta){
			if(itemSlot[pt] == id){
				geta = true;
			}else{
				pt++;
				if(pt >= itemSlot.Length){
					pt = itemSlot.Length + 50;//No Item
					print("No Item");
				}
			}
			
		}
		
		return pt;
		
	}
	
	public int FindEquipmentSlot(int id){
		bool geta = false;
		int pt = 0;
		while(pt < equipment.Length && !geta){
			if(equipment[pt] == id){
				geta = true;
			}else{
				pt++;
				if(pt >= equipment.Length){
					pt = equipment.Length + 50;//No Item
					print("No Item");
				}
			}
			
		}
		
		return pt;
		
	}
	
	public bool RemoveItem(int id , int amount){
		bool haveItem = false;
		int slot = FindItemSlot(id);
		if(slot < itemSlot.Length){
			if(itemQuantity[slot] > 0){
				itemQuantity[slot] -= amount;
				haveItem = true;
			}
			if(itemQuantity[slot] <= 0){
				itemSlot[slot] = 0;
				itemQuantity[slot] = 0;
				AutoSortItem();
			}
		}
		return haveItem;
	}
	
	public bool RemoveEquipment(int id){
		bool haveItem = false;
		int slot = FindEquipmentSlot(id);
		if(slot < equipment.Length){
			equipment[slot] = 0;
			AutoSortEquipment();
			haveItem = true;
		}
		return haveItem;
	}
}