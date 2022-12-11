using UnityEngine;
using System.Collections;

public class ShopC : MonoBehaviour {

	public int[] itemShopSlot = new int[10];
	public int[] equipmentShopSlot = new int[10];
	public Texture2D button;
	public GameObject database;
	private GameObject player;
	
	private bool menu = false;
	private bool shopMain = false;
	private bool shopItem = false;
	private bool shopEquip = false;
	private bool itemInven = false;
	private bool equipInven = false;
	private bool sellwindow = false;
	private bool buywindow = false;
	private bool buyerror = false;
	private string buyErrorLog = "Not Enough Cash";
	
	private bool enter = false;
	private int select = 0;
	
	private int num = 1;
	private string text = "1";
	
	public GUISkin skin1;
	public Rect windowRect = new Rect (360 ,140 ,480 ,550);
	private Rect originalRect;
	//public Texture2D selectedIcon;
	
	private ItemDataC dataItem;
	private int[] itemQuantity;
	private int cash;
	private int[] itemSlot;
	private int[] equipment;
	
	public int pageMultiply = 5;
	private int page = 0;
	
	public GUIStyle itemNameText;
	public GUIStyle itemDescriptionText;
	public GUIStyle itemQuantityText;
	public GUIStyle textStyle;
	
	void Start() {
		originalRect = windowRect;
		dataItem = database.GetComponent<ItemDataC>();
		/*itemQuantity = player.GetComponent<Inventory>().itemQuantity;
	cash = player.GetComponent<Inventory>().cash;
	itemSlot = player.GetComponent<Inventory>().itemSlot;
	equipment = player.GetComponent<Inventory>().equipment;*/
	}
	
	void Update() {
		if(Input.GetKeyDown("e") && enter){
			shopMain = true;
			OnOffMenu();
			
		}
		
	}
	
	void Activate(){
		if(enter){
			shopMain = true;
			OnOffMenu();
		}
	}
	
	public void ShopBuy(int id, int slot, int price, int quan) {
		if(player.GetComponent<InventoryC>().cash < price){
			//If not enough cash
			print(price);
			buyErrorLog = "Not Enough Cash";
			buyerror = true;
			return;
		}

		bool full;

		if(shopItem){
			//Buy Usable Item	
			full = player.GetComponent<InventoryC>().AddItem(id , quan);
			if(full){
				buyErrorLog = "Inventory Full";
				buyerror = true;
				return;
			}
			
		}else{
			//Buy Equipment
			full = player.GetComponent<InventoryC>().AddEquipment(id);
			if(full){
				buyErrorLog = "Inventory Full";
				buyerror = true;
				return;
			}
			
		}
		
		//Remove Cash
		player.GetComponent<InventoryC>().cash -= price;
		
		
	}
	
	public void ShopSell(int id, int slot, int price, int quan){
		if(itemInven){
			//Sell Usable Item
			if(quan >= player.GetComponent<InventoryC>().itemQuantity[slot]){
				quan = player.GetComponent<InventoryC>().itemQuantity[slot];
			}
			player.GetComponent<InventoryC>().itemQuantity[slot]-= quan;
			if(player.GetComponent<InventoryC>().itemQuantity[slot] <= 0){
				player.GetComponent<InventoryC>().itemSlot[slot] = 0;
				player.GetComponent<InventoryC>().itemQuantity[slot] = 0;
				player.GetComponent<InventoryC>().AutoSortItem();
			}
			//Add Cash
			player.GetComponent<InventoryC>().cash += price * quan;
			
		}else{
			//Sell Equipment
			player.GetComponent<InventoryC>().equipment[slot] = 0;
			player.GetComponent<InventoryC>().AutoSortEquipment();
			
			//Add Cash
			player.GetComponent<InventoryC>().cash += price * quan;
			
		}
		
	}
	
	void OnGUI() {
		if(!player){
			return;
		}
		cash = player.GetComponent<InventoryC>().cash;
		GUI.skin = skin1;
		
		if(enter && !menu){
			//GUI.DrawTexture(new Rect(Screen.width / 2 - 130, Screen.height - 120, 260, 80), button);
			if (GUI.Button (new Rect(Screen.width / 2 - 130, Screen.height - 180, 260, 80), button)) {
				shopMain = true;
				OnOffMenu();
			}
		}
		
		//Shop Main Menu
		if(menu && shopMain){
			GUI.Box (new Rect (Screen.width / 2 - 200,180,400,240), "");
			if (GUI.Button (new Rect (Screen.width / 2 - 160,270,150,80), "Buy")) {
				shopItem = true;
				shopMain = false;
			}
			if (GUI.Button (new Rect (Screen.width / 2 + 30,270,150,80), "Sell")) {
				itemInven = true;
				shopMain = false;
			}
			if (GUI.Button (new Rect (Screen.width / 2 + 105,190,60,60), "X")) {
				OnOffMenu();
			}
		}
		
		if(menu && itemInven && !sellwindow){
			windowRect = GUI.Window (2, windowRect, SellItemtWindow, "Shop");
			if (GUI.Button ( new Rect(windowRect.x -50, windowRect.y +105,50,100), "Item")) {
				//Switch to Item Tab
				select = 0;
				page = 0;
				itemInven = true;
				equipInven = false;
			}
			if (GUI.Button ( new Rect(windowRect.x -50, windowRect.y +225,50,100), "Equip")) {
				//Switch to Equipment Tab
				select = 0;
				page = 0;
				equipInven = true;
				itemInven = false;
			}
		}
		
		if(menu && equipInven && !sellwindow){
			windowRect = GUI.Window (2, windowRect, SellEquipmenttWindow, "Shop");
			if (GUI.Button ( new Rect(windowRect.x -50, windowRect.y +105,50,100), "Item")) {
				//Switch to Item Tab
				select = 0;
				page = 0;
				itemInven = true;
				equipInven = false;
			}
			if (GUI.Button ( new Rect(windowRect.x -50, windowRect.y +225,50,100), "Equip")) {
				//Switch to Equipment Tab
				select = 0;
				page = 0;
				equipInven = true;
				itemInven = false;
			}
		}

		int temp = 0;

		//---------------Sell Item Confirm Window------------------
		if(sellwindow){
			if(itemInven){
				if(itemSlot[select] == 0){
					sellwindow = false;
				}
				GUI.Box (new Rect (Screen.width / 2 - 140,230,280,120), "Price " + dataItem.usableItem[itemSlot[select]].price /2);
				
				//------------------Quantity--------------
				text = GUI.TextField(new Rect(Screen.width / 2 +5, 250, 50, 20), num.ToString() , 2);
				GUI.Label ( new Rect(Screen.width / 2 -65, 250, 60, 20), "Quantity");
				//text = GUI.TextField(new Rect(50, 50, 200, 50), text , 2);
				temp = 0;
				if (int.TryParse(text, out temp)){
					//num = Mathf.Clamp(0, out temp);
					num = temp;
				}else if (text == ""){
					num = 0;
				}
				//-----------------------------------
				
			}else{
				if(equipment[select] == 0){
					sellwindow = false;
				}
				GUI.Box (new Rect (Screen.width / 2 - 140,230,280,120), "Price " + dataItem.equipment[equipment[select]].price /2);
			}
			if (GUI.Button (new Rect (Screen.width / 2 - 100,285,80,30), "Sell")) {
				if(itemInven){
					//Sell Usable Item
					if(num > 0){
						//ShopBuy(itemShopSlot[select] , select , dataItem.usableItem[itemShopSlot[select]].price * num , num);
						//buywindow = false;
						ShopSell(itemSlot[select] , select , dataItem.usableItem[itemSlot[select]].price /2 , num);
						sellwindow = false;
					}
					
				}else{
					//Sell Equipment
					ShopSell(equipment[select] , select , dataItem.equipment[equipment[select]].price /2 , 1);
					sellwindow = false;
				}
				
			}
			if (GUI.Button (new Rect (Screen.width / 2 + 35,285,80,30), "Cancel")) {
				sellwindow = false;
			}
		}
		//---------------------------------------------------------------------------------------------
		//---------------------------------------------------------------------------------------------
		//-----------------------------------BUY----------------------------------------------------
		//---------------------------------------------------------------------------------------------
		//---------------------------------------------------------------------------------------------
		
		//-----------Buy Usable Item---------------------
		if(menu && shopItem && !buywindow && !buyerror){
			windowRect = GUI.Window (2, windowRect, BuyItemWindow, "Shop");
			if (GUI.Button ( new Rect(windowRect.x -50, windowRect.y +105,50,100), "Item")) {
				//Switch to Item Tab
				select = 0;
				page = 0;
				shopItem = true;
				shopEquip = false;
			}
			if (GUI.Button ( new Rect(windowRect.x -50, windowRect.y +225,50,100), "Equip")) {
				//Switch to Equipment Tab
				select = 0;
				page = 0;
				shopEquip = true;
				shopItem = false;
			}
		}
		
		//-----------Buy Equipment Item---------------------
		if(menu && shopEquip && !buywindow && !buyerror){
			windowRect = GUI.Window (2, windowRect, BuyEquipmentWindow, "Shop");
			if (GUI.Button ( new Rect(windowRect.x -50, windowRect.y +105,50,100), "Item")) {
				//Switch to Item Tab
				select = 0;
				page = 0;
				shopItem = true;
				shopEquip = false;
			}
			if (GUI.Button ( new Rect(windowRect.x -50, windowRect.y +225,50,100), "Equip")) {
				//Switch to Equipment Tab
				select = 0;
				page = 0;
				shopEquip = true;
				shopItem = false;
			}
		}
		
		//---------------Buy Item Confirm Window------------------
		if(buywindow){
			if(shopItem){
				if(itemShopSlot[select] == 0){
					buywindow = false;
				}
				GUI.Box (new Rect (Screen.width / 2 - 140,230,280,120), "Price " + dataItem.usableItem[itemShopSlot[select]].price);
				
				//------------------Quantity--------------
				text = GUI.TextField(new Rect(Screen.width / 2 +5, 250, 50, 20), num.ToString() , 2);
				GUI.Label ( new Rect(Screen.width / 2 -65, 250, 60, 20), "Quantity");
				//text = GUI.TextField(new Rect(50, 50, 200, 50), text , 2);
				temp = 0;
				if (int.TryParse(text , out temp)){
					//num = Mathf.Clamp(0, out temp);
					num = temp;
				}else if (text == ""){
					num = 0;
				}
				//-----------------------------------
				
			}else{
				if(equipmentShopSlot[select] == 0){
					buywindow = false;
				}
				GUI.Box (new Rect (Screen.width / 2 - 140,230,280,120), "Price " + dataItem.equipment[equipmentShopSlot[select]].price);
			}
			if (GUI.Button (new Rect (Screen.width / 2 - 100,285,80,30), "Buy")) {
				if(shopItem){
					//Sell Usable Item
					if(num > 0){
						ShopBuy(itemShopSlot[select] , select , dataItem.usableItem[itemShopSlot[select]].price * num , num);
						buywindow = false;
					}
				}else{
					//Sell Equipment
					ShopBuy(equipmentShopSlot[select] , select , dataItem.equipment[equipmentShopSlot[select]].price , 1);
					buywindow = false;
				}
				
			}
			if (GUI.Button (new Rect (Screen.width / 2 + 35,285,80,30), "Cancel")) {
				buywindow = false;
			}
		}
		//Error When Buying
		if(buyerror){
			GUI.Box (new Rect (Screen.width / 2 - 140,230,280,120), buyErrorLog);
			if (GUI.Button (new Rect (Screen.width / 2 - 40,285,80,30), "OK")) {
				buyerror = false;
			}
		}
		
	}
	
	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "Player") {
			InventoryC inven = other.GetComponent<InventoryC>();
			if(inven){
				player = other.gameObject;
				itemQuantity = player.GetComponent<InventoryC>().itemQuantity;
				cash = player.GetComponent<InventoryC>().cash;
				itemSlot = player.GetComponent<InventoryC>().itemSlot;
				equipment = player.GetComponent<InventoryC>().equipment;
				enter = true;
			}
			
		}
		
	}
	
	
	void OnTriggerExit(Collider other) {
		//if (other.gameObject.tag == "Player") {
		if (other.gameObject == player) {
			enter = false;
		}
	}
	
	public void OnOffMenu() {
		//Freeze Time Scale to 0 if Window is Showing
		page = 0;
		if(!menu && Time.timeScale != 0.0){
			menu = true;
			itemInven = false;
			shopItem = false;
			shopEquip = false;
			equipInven = false;
			sellwindow = false;
			buywindow = false;
			buyerror = false;
			//shopMain = false;
			Time.timeScale = 0.0f;
			ResetPosition();
			//Screen.lockCursor = false;
		}else if(menu){
			menu = false;
			Time.timeScale = 1.0f;
			//Screen.lockCursor = true;
		}
	}
	
	public void BuyItemWindow(int windowID) {
		//Close Window Button
		if (GUI.Button (new Rect (390,8,70,70), "X")) {
			OnOffMenu();
		}
		if (GUI.Button ( new Rect(30,30,75,75),dataItem.usableItem[itemShopSlot[0 + page]].icon)){
			select = 0 + page;
			buywindow = true;
		}
		GUI.Label (new Rect(125, 40, 320, 75), dataItem.usableItem[itemShopSlot[0 + page]].itemName.ToString() , itemNameText); //Item Name
		GUI.Label (new Rect(125, 65, 320, 75), dataItem.usableItem[itemShopSlot[0 + page]].description.ToString() , itemDescriptionText); //Item Description
		GUI.Label (new Rect(310, 55, 140, 40), "$ : " + dataItem.usableItem[itemShopSlot[0 + page]].price , itemDescriptionText); //Show Item's Price
		//----------------------------------------------
		if (GUI.Button ( new Rect(30,120,75,75),dataItem.usableItem[itemShopSlot[1 + page]].icon)){
			select = 1 + page;
			buywindow = true;
		}
		GUI.Label (new Rect(125, 130, 320, 75), dataItem.usableItem[itemShopSlot[1 + page]].itemName.ToString() , itemNameText); //Item Name
		GUI.Label (new Rect(125, 155, 320, 75), dataItem.usableItem[itemShopSlot[1 + page]].description.ToString() , itemDescriptionText); //Item Description
		GUI.Label (new Rect(310, 145, 140, 40), "$ : " + dataItem.usableItem[itemShopSlot[1 + page]].price , itemDescriptionText); //Show Item's Price
		//----------------------------------------------
		if (GUI.Button ( new Rect(30,210,75,75),dataItem.usableItem[itemShopSlot[2 + page]].icon)){
			select = 2 + page;
			buywindow = true;
		}
		GUI.Label (new Rect(125, 220, 320, 75), dataItem.usableItem[itemShopSlot[2 + page]].itemName.ToString() , itemNameText); //Item Name
		GUI.Label (new Rect(125, 245, 320, 75), dataItem.usableItem[itemShopSlot[2 + page]].description.ToString() , itemDescriptionText); //Item Description
		GUI.Label (new Rect(310, 235, 140, 40), "$ : " + dataItem.usableItem[itemShopSlot[2 + page]].price , itemDescriptionText); //Show Item's Price
		//----------------------------------------------
		if (GUI.Button ( new Rect(30,300,75,75),dataItem.usableItem[itemShopSlot[3 + page]].icon)){
			select = 3 + page;
			buywindow = true;
		}
		GUI.Label (new Rect(125, 310, 320, 75), dataItem.usableItem[itemShopSlot[3 + page]].itemName.ToString() , itemNameText); //Item Name
		GUI.Label (new Rect(125, 335, 320, 75), dataItem.usableItem[itemShopSlot[3 + page]].description.ToString() , itemDescriptionText); //Item Description
		GUI.Label (new Rect(310, 325, 140, 40), "$ : " + dataItem.usableItem[itemShopSlot[3 + page]].price , itemDescriptionText); //Show Item's Price
		//----------------------------------------------
		if (GUI.Button ( new Rect(30,390,75,75),dataItem.usableItem[itemShopSlot[4 + page]].icon)){
			select = 4 + page;
			buywindow = true;
		}
		GUI.Label (new Rect(125, 400, 320, 75), dataItem.usableItem[itemShopSlot[4 + page]].itemName.ToString() , itemNameText); //Item Name
		GUI.Label (new Rect(125, 425, 320, 75), dataItem.usableItem[itemShopSlot[4 + page]].description.ToString() , itemDescriptionText); //Item Description
		GUI.Label (new Rect(310, 415, 140, 40), "$ : " + dataItem.usableItem[itemShopSlot[4 + page]].price , itemDescriptionText); //Show Item's Price
		//----------------------------------------------
		if (GUI.Button (new Rect (220,485,50,52), "1")) {
			page = 0;
		}
		if (GUI.Button (new Rect (290,485,50,52), "2")) {
			page = pageMultiply;
		}	
		GUI.Label ( new Rect(20, 505, 150, 50), "$ " + cash.ToString() , itemDescriptionText);
		GUI.DragWindow (new Rect (0,0,10000,10000)); 
	}
	//---------------------------------------------------------------------------
	
	public void BuyEquipmentWindow(int windowID) {
		//Close Window Button
		if (GUI.Button (new Rect (390,8,70,70), "X")) {
			OnOffMenu();
		}
		if (GUI.Button ( new Rect(30,30,75,75),dataItem.equipment[equipmentShopSlot[0 + page]].icon)){
			select = 0 + page;
			buywindow = true;
		}
		GUI.Label (new Rect(125, 40, 320, 75), dataItem.equipment[equipmentShopSlot[0 + page]].itemName.ToString() , itemNameText); //Item Name
		GUI.Label (new Rect(125, 65, 320, 75), dataItem.equipment[equipmentShopSlot[0 + page]].description.ToString() , itemDescriptionText); //Item Description
		GUI.Label (new Rect(310, 55, 140, 40), "$ : " + dataItem.equipment[equipmentShopSlot[0 + page]].price , itemDescriptionText); //Show Item's Price
		//----------------------------------
		if (GUI.Button ( new Rect(30,120,75,75),dataItem.equipment[equipmentShopSlot[1 + page]].icon)){
			select = 1 + page;
			buywindow = true;
		}
		GUI.Label (new Rect(125, 130, 320, 75), dataItem.equipment[equipmentShopSlot[1 + page]].itemName.ToString() , itemNameText); //Item Name
		GUI.Label (new Rect(125, 155, 320, 75), dataItem.equipment[equipmentShopSlot[1 + page]].description.ToString() , itemDescriptionText); //Item Description
		GUI.Label (new Rect(310, 145, 140, 40), "$ : " + dataItem.equipment[equipmentShopSlot[1 + page]].price , itemDescriptionText); //Show Item's Price
		//----------------------------------
		if (GUI.Button ( new Rect(30,210,75,75),dataItem.equipment[equipmentShopSlot[2 + page]].icon)){
			select = 2 + page;
			buywindow = true;
		}
		GUI.Label (new Rect(125, 220, 320, 75), dataItem.equipment[equipmentShopSlot[2 + page]].itemName.ToString() , itemNameText); //Item Name
		GUI.Label (new Rect(125, 245, 320, 75), dataItem.equipment[equipmentShopSlot[2 + page]].description.ToString() , itemDescriptionText); //Item Description
		GUI.Label (new Rect(310, 235, 140, 40), "$ : " + dataItem.equipment[equipmentShopSlot[2 + page]].price , itemDescriptionText); //Show Item's Price
		//----------------------------------
		if (GUI.Button ( new Rect(30,300,75,75),dataItem.equipment[equipmentShopSlot[3 + page]].icon)){
			select = 3 + page;
			buywindow = true;
		}
		GUI.Label (new Rect(125, 310, 320, 75), dataItem.equipment[equipmentShopSlot[3 + page]].itemName.ToString() , itemNameText); //Item Name
		GUI.Label (new Rect(125, 335, 320, 75), dataItem.equipment[equipmentShopSlot[3 + page]].description.ToString() , itemDescriptionText); //Item Description
		GUI.Label (new Rect(310, 325, 140, 40), "$ : " + dataItem.equipment[equipmentShopSlot[3 + page]].price , itemDescriptionText); //Show Item's Price
		//----------------------------------
		if (GUI.Button ( new Rect(30,390,75,75),dataItem.equipment[equipmentShopSlot[4 + page]].icon)){
			select = 4 + page;
			buywindow = true;
		}
		GUI.Label (new Rect(125, 400, 320, 75), dataItem.equipment[equipmentShopSlot[4 + page]].itemName.ToString() , itemNameText); //Item Name
		GUI.Label (new Rect(125, 425, 320, 75), dataItem.equipment[equipmentShopSlot[4 + page]].description.ToString() , itemDescriptionText); //Item Description
		GUI.Label (new Rect(310, 415, 140, 40), "$ : " + dataItem.equipment[equipmentShopSlot[4 + page]].price , itemDescriptionText); //Show Item's Price
		//----------------------------------
		
		if (GUI.Button (new Rect (220,485,50,52), "1")) {
			page = 0;
		}
		if (GUI.Button (new Rect (290,485,50,52), "2")) {
			page = pageMultiply;
		}	
		GUI.Label ( new Rect(20, 505, 150, 50), "$ " + cash.ToString() , itemDescriptionText);
		//----------------------------------
		GUI.DragWindow (new Rect (0,0,10000,10000)); 
	}
	
	public void SellItemtWindow(int windowID) {
		//Close Window Button
		if (GUI.Button (new Rect (390,8,70,70), "X")) {
			OnOffMenu();
		}
		//Items Slot
		if (GUI.Button ( new Rect(30,30,75,75),dataItem.usableItem[itemSlot[0 + page]].icon)){
			select = 0 + page;
			sellwindow = true;
		}
		GUI.Label ( new Rect(125, 40, 320, 75), dataItem.usableItem[itemSlot[0 + page]].itemName.ToString() , itemNameText); //Item Name
		GUI.Label ( new Rect(125, 65, 320, 75), dataItem.usableItem[itemSlot[0 + page]].description.ToString() , itemDescriptionText); //Item Description
		if(itemQuantity[0 + page] > 0){
			GUI.Label ( new Rect(88, 88, 40, 30), itemQuantity[0 + page].ToString() , itemQuantityText); //Quantity
		}
		//------------------------------------------------------
		if (GUI.Button ( new Rect(30,120,75,75),dataItem.usableItem[itemSlot[1 + page]].icon)){
			select = 1 + page;
			sellwindow = true;
		}
		GUI.Label ( new Rect(125, 130, 320, 75), dataItem.usableItem[itemSlot[1 + page]].itemName.ToString() , itemNameText); //Item Name
		GUI.Label ( new Rect(125, 155, 320, 75), dataItem.usableItem[itemSlot[1 + page]].description.ToString() , itemDescriptionText); //Item Description
		if(itemQuantity[1 + page] > 0){
			GUI.Label ( new Rect(88, 178, 40, 30), itemQuantity[1 + page].ToString() , itemQuantityText); //Quantity
		}
		//------------------------------------------------------
		if (GUI.Button ( new Rect(30,210,75,75),dataItem.usableItem[itemSlot[2 + page]].icon)){
			select = 2 + page;
			sellwindow = true;
		}
		GUI.Label ( new Rect(125, 220, 320, 75), dataItem.usableItem[itemSlot[2 + page]].itemName.ToString() , itemNameText); //Item Name
		GUI.Label ( new Rect(125, 245, 320, 75), dataItem.usableItem[itemSlot[2 + page]].description.ToString() , itemDescriptionText); //Item Description
		if(itemQuantity[2 + page] > 0){
			GUI.Label ( new Rect(88, 268, 40, 30), itemQuantity[2 + page].ToString() , itemQuantityText); //Quantity
		}
		//------------------------------------------------------
		if (GUI.Button ( new Rect(30,300,75,75),dataItem.usableItem[itemSlot[3 + page]].icon)){
			select = 3 + page;
			sellwindow = true;
		}
		GUI.Label ( new Rect(125, 310, 320, 75), dataItem.usableItem[itemSlot[3 + page]].itemName.ToString() , itemNameText); //Item Name
		GUI.Label ( new Rect(125, 335, 320, 75), dataItem.usableItem[itemSlot[3 + page]].description.ToString() , itemDescriptionText); //Item Description
		if(itemQuantity[3 + page] > 0){
			GUI.Label ( new Rect(88, 358, 40, 30), itemQuantity[3 + page].ToString() , itemQuantityText); //Quantity
		}
		//------------------------------------------------------
		if (GUI.Button ( new Rect(30,390,75,75),dataItem.usableItem[itemSlot[4 + page]].icon)){
			select = 4 + page;
			sellwindow = true;
		}
		GUI.Label ( new Rect(125, 400, 320, 75), dataItem.usableItem[itemSlot[4 + page]].itemName.ToString() , itemNameText); //Item Name
		GUI.Label ( new Rect(125, 425, 320, 75), dataItem.usableItem[itemSlot[4 + page]].description.ToString() , itemDescriptionText); //Item Description
		if(itemQuantity[4 + page] > 0){
			GUI.Label ( new Rect(88, 448, 40, 30), itemQuantity[4 + page].ToString() , itemQuantityText); //Quantity
		}
		//------------------------------------------------------
		
		if (GUI.Button (new Rect (220,485,50,52), "1")) {
			page = 0;
		}
		if (GUI.Button (new Rect (290,485,50,52), "2")) {
			page = pageMultiply;
		}
		if (GUI.Button (new Rect (360,485,50,52), "3")) {
			page = pageMultiply *2;
		}
		
		GUI.Label ( new Rect(20, 505, 150, 50), "$ " + cash.ToString() , itemDescriptionText);
		
		//-----------------------------------------------------------
		GUI.DragWindow (new Rect (0,0,10000,10000)); 
	}
	
	public void SellEquipmenttWindow(int windowID){
		//Close Window Button
		if (GUI.Button (new Rect (390,8,70,70), "X")) {
			OnOffMenu();
		}
		if (GUI.Button ( new Rect(30,30,75,75),dataItem.equipment[equipment[0 + page]].icon)){
			select = 0 + page;
			sellwindow = true;
		}
		GUI.Label ( new Rect(125, 40, 320, 75), dataItem.equipment[equipment[0 + page]].itemName.ToString() , itemNameText); //Item Name
		GUI.Label ( new Rect(125, 65, 320, 75), dataItem.equipment[equipment[0 + page]].description.ToString() , itemDescriptionText); //Item Description
		//-----------------------------------------------------
		if (GUI.Button ( new Rect(30,120,75,75),dataItem.equipment[equipment[1 + page]].icon)){
			select = 1 + page;
			sellwindow = true;
		}
		GUI.Label ( new Rect(125, 130, 320, 75), dataItem.equipment[equipment[1 + page]].itemName.ToString() , itemNameText); //Item Name
		GUI.Label ( new Rect(125, 155, 320, 75), dataItem.equipment[equipment[1 + page]].description.ToString() , itemDescriptionText); //Item Description
		//-----------------------------------------------------
		if (GUI.Button ( new Rect(30,210,75,75),dataItem.equipment[equipment[2 + page]].icon)){
			select = 2 + page;
			sellwindow = true;
		}
		GUI.Label ( new Rect(125, 220, 320, 75), dataItem.equipment[equipment[2 + page]].itemName.ToString() , itemNameText); //Item Name
		GUI.Label ( new Rect(125, 245, 320, 75), dataItem.equipment[equipment[2 + page]].description.ToString() , itemDescriptionText); //Item Description
		//-----------------------------------------------------
		if (GUI.Button ( new Rect(30,300,75,75),dataItem.equipment[equipment[3 + page]].icon)){
			select = 3 + page;
			sellwindow = true;
		}
		GUI.Label ( new Rect(125, 310, 320, 75), dataItem.equipment[equipment[3 + page]].itemName.ToString() , itemNameText); //Item Name
		GUI.Label ( new Rect(125, 335, 320, 75), dataItem.equipment[equipment[3 + page]].description.ToString() , itemDescriptionText); //Item Description
		//-----------------------------------------------------
		
		if (GUI.Button (new Rect (220,485,50,52), "1")) {
			page = 0;
		}
		if (GUI.Button (new Rect (290,485,50,52), "2")) {
			page = 4;
		}
		if (GUI.Button (new Rect (360,485,50,52), "3")) {
			page = 8;
		}
		
		GUI.Label ( new Rect(20, 505, 150, 50), "$ " + cash.ToString() , itemDescriptionText);
		
		//-----------------------------------------------------------
		GUI.DragWindow (new Rect (0,0,10000,10000)); 
	}
	
	public void ResetPosition() {
		//Reset GUI Position when it out of Screen.
		if(windowRect.x >= Screen.width -30 || windowRect.y >= Screen.height -30 || windowRect.x <= -70 || windowRect.y <= -70 ){
			windowRect = originalRect;
		}
	}
}