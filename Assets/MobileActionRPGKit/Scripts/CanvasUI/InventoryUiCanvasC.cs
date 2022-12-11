using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InventoryUiCanvasC : MonoBehaviour {

	public GameObject player;
	public Text moneyText;
	
	public Image[] itemIcons = new Image[16];
	public Text[] itemNameText = new Text[16];
	public Text[] descriptionText = new Text[16];
	
	public Image[] equipmentIcons = new Image[16];
	public Text[] equipmentNameText = new Text[16];
	public Text[] equipmentdescriptionText = new Text[16];
	
	public Image weaponIcons;
	public Image armorIcons;
	
	public GameObject database;
	private ItemDataC db; 
	
	void Start(){
		db = database.GetComponent<ItemDataC>();
	}
	
	void Update(){
		if(!player){
			return;
		}
		//itemIcons[0].GetComponent<Image>().sprite = db.usableItem[player.GetComponent<Inventory>().itemSlot[0]].iconSprite;
		
		for(int a = 0; a < itemIcons.Length; a++){
			itemIcons[a].GetComponent<Image>().sprite = db.usableItem[player.GetComponent<InventoryC>().itemSlot[a]].iconSprite;
			itemIcons[a].GetComponent<Image>().color = db.usableItem[player.GetComponent<InventoryC>().itemSlot[a]].spriteColor;
		}
		
		for(int q = 0; q < itemNameText.Length; q++){
			string qty = player.GetComponent<InventoryC>().itemQuantity[q].ToString();
			if(qty == "0"){
				qty = "";
			}
			itemNameText[q].GetComponent<Text>().text = db.usableItem[player.GetComponent<InventoryC>().itemSlot[q]].itemName + "  : " + qty;
			descriptionText[q].GetComponent<Text>().text = db.usableItem[player.GetComponent<InventoryC>().itemSlot[q]].description;
		}
		
		for(int b = 0; b < equipmentIcons.Length; b++){
			equipmentIcons[b].GetComponent<Image>().sprite = db.equipment[player.GetComponent<InventoryC>().equipment[b]].iconSprite;
			equipmentIcons[b].GetComponent<Image>().color = db.equipment[player.GetComponent<InventoryC>().equipment[b]].spriteColor;
			equipmentNameText[b].GetComponent<Text>().text = db.equipment[player.GetComponent<InventoryC>().equipment[b]].itemName;
			equipmentdescriptionText[b].GetComponent<Text>().text = db.equipment[player.GetComponent<InventoryC>().equipment[b]].description;
		}
		
		if(weaponIcons){
			weaponIcons.GetComponent<Image>().sprite = db.equipment[player.GetComponent<InventoryC>().weaponEquip].iconSprite;
			weaponIcons.GetComponent<Image>().color = db.equipment[player.GetComponent<InventoryC>().weaponEquip].spriteColor;
		}
		if(armorIcons){
			armorIcons.GetComponent<Image>().sprite = db.equipment[player.GetComponent<InventoryC>().armorEquip].iconSprite;
			armorIcons.GetComponent<Image>().color = db.equipment[player.GetComponent<InventoryC>().armorEquip].spriteColor;
		}
		if(moneyText){
			moneyText.GetComponent<Text>().text = player.GetComponent<InventoryC>().cash.ToString();
		}
	}
	
	
	public void UseItem(int itemSlot){
		if(!player){
			return;
		}
		player.GetComponent<InventoryC>().UseItem(itemSlot);
		
	}
	
	public void EquipItem(int itemSlot){
		if(!player){
			return;
		}
		player.GetComponent<InventoryC>().EquipItem(player.GetComponent<InventoryC>().equipment[itemSlot] , itemSlot);
	}
	
	public void UnEquip(int type){
		//0 = Weapon, 1 = Armor
		if(!player){
			return;
		}
		int id = 0;
		if(type == 0){
			id = player.GetComponent<InventoryC>().weaponEquip;
		}
		if(type == 1){
			id = player.GetComponent<InventoryC>().armorEquip;
		}
		player.GetComponent<InventoryC>().UnEquip(id);
	}
	
	public void CloseMenu(){
		Time.timeScale = 1.0f;
		//Screen.lockCursor = true;
		gameObject.SetActive(false);
	}
}