using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SkillWindowC : MonoBehaviour {

	public GameObject player;
	public GameObject database;

	public Image[] skillButton = new Image[3];
	public int[] skill = new int[3];
	public int[] skillListSlot = new int[15];

	[System.Serializable]
	public class LearnSkillLV{
		public int level = 1;
		public int skillId = 1;
	}
	public LearnSkillLV[] learnSkill = new LearnSkillLV[2];
	
	private bool menu = false;
	private bool shortcutPage = true;
	private bool skillListPage = false;
	private int skillSelect = 0;
	
	public GUISkin skin1;
	public Rect windowRect = new Rect (360 ,80 ,360 ,185);
	private Rect originalRect;
	
	public GUIStyle skillNameText;
	public GUIStyle skillDescriptionText;
	public GUIStyle showLearnSkillText;
	
	private bool showSkillLearned = false;
	private string showSkillName = "";

	public int pageMultiply = 5;
	private int page = 0;
	public bool useLegacyUi = false;
	
	void Start(){
		if(!player){
			player = this.gameObject;
		}
		originalRect = windowRect;
		//skillIcon = new Image[skill.Length];
		AssignAllSkill();
	}
	
	void Update(){
		if(Input.GetKeyDown("k") && useLegacyUi) {
			OnOffMenu();
		}		
	}
	
	public void OnOffMenu() {
		//Freeze Time Scale to 0 if Window is Showing
		if(!menu && Time.timeScale != 0.0){
			menu = true;
			skillListPage = false;
			shortcutPage = true;
			Time.timeScale = 0.0f;
			ResetPosition();
			//Screen.lockCursor = false;
		}else if(menu){
			menu = false;
			Time.timeScale = 1.0f;
			//Screen.lockCursor = true;
		}
	}
	
	void OnGUI() {
		//SkillDataC dataItem = database.GetComponent<SkillDataC>();
		if(showSkillLearned){
			GUI.Label (new Rect (Screen.width /2 -50, 85, 400, 50), "You Learned  " + showSkillName , showLearnSkillText);
		}
		if(menu && shortcutPage){
			windowRect = GUI.Window (3, windowRect, SkillShortcut, "Skill");
		}
		//---------------Skill List----------------------------
		if(menu && skillListPage){
			windowRect = GUI.Window (3, windowRect, AllSkill, "Skill");
		}
		
	}
	
	public void AssignSkill(int id, int sk) {
		SkillDataC dataSkill = database.GetComponent<SkillDataC>();
		player.GetComponent<AttackTriggerC>().skill[id].manaCost = dataSkill.skill[skillListSlot[sk]].manaCost;
		player.GetComponent<AttackTriggerC>().skill[id].skillPrefab = dataSkill.skill[skillListSlot[sk]].skillPrefab;
		player.GetComponent<AttackTriggerC>().skill[id].skillAnimation = dataSkill.skill[skillListSlot[sk]].skillAnimation;
		player.GetComponent<AttackTriggerC>().skill[id].icon = dataSkill.skill[skillListSlot[sk]].icon;
		player.GetComponent<AttackTriggerC>().skill[id].cantStack = dataSkill.skill[skillListSlot[sk]].cantStack;
		skill[id] = skillListSlot[sk];
		//-------Assign Icon to Skill Button-----------
		if(skillButton[id]){
			skillButton[id].sprite = dataSkill.skill[skillListSlot[sk]].iconSprite;
		}
		print(sk);
		
	}
	
	public void AssignAllSkill() {
		/*AssignSkill(0 , skill[0]);
		AssignSkill(1 , skill[1]);
		AssignSkill(2 , skill[2]);*/
		if(!player){
			player = this.gameObject;
		}
		int n = 0;
		SkillDataC dataSkill = database.GetComponent<SkillDataC>();
		while(n <= 2){
			player.GetComponent<AttackTriggerC>().skill[n].manaCost = dataSkill.skill[skill[n]].manaCost;
			player.GetComponent<AttackTriggerC>().skill[n].skillPrefab = dataSkill.skill[skill[n]].skillPrefab;
			player.GetComponent<AttackTriggerC>().skill[n].skillAnimation = dataSkill.skill[skill[n]].skillAnimation;
			player.GetComponent<AttackTriggerC>().skill[n].icon = dataSkill.skill[skill[n]].icon;
			player.GetComponent<AttackTriggerC>().skill[n].cantStack = dataSkill.skill[skill[n]].cantStack;
			//-------Assign Icon to Skill Button-----------
			if(skillButton[n]){
				skillButton[n].sprite = dataSkill.skill[skill[n]].iconSprite;
			}
			n++;
		}
		
	}
	
	public void SkillShortcut(int windowID) {
		SkillDataC dataSkill = database.GetComponent<SkillDataC>();
		windowRect.width = 490;
		windowRect.height = 275;
		//Close Window Button
		if (GUI.Button (new Rect (400,8,65,65), "X")) {
			OnOffMenu();
		}
		
		//Skill Shortcut
		if (GUI.Button (new Rect (40,85,120,120), dataSkill.skill[skill[0]].icon)) {
			skillSelect = 0;
			skillListPage = true;
			shortcutPage = false;
		}
		GUI.Label (new Rect (90, 210, 20, 20), "1");
		if (GUI.Button (new Rect (190,85,120,120), dataSkill.skill[skill[1]].icon)) {
			skillSelect = 1;
			skillListPage = true;
			shortcutPage = false;
		}
		GUI.Label (new Rect (245, 210, 20, 20), "2");
		if (GUI.Button (new Rect (340,85,120,120), dataSkill.skill[skill[2]].icon)) {
			skillSelect = 2;
			skillListPage = true;
			shortcutPage = false;
		}
		GUI.Label (new Rect (400, 210, 20, 20), "3");
		
		GUI.DragWindow (new Rect (0,0,10000,10000));
		
	}
	
	public void AllSkill(int windowID) {
		SkillDataC dataSkill = database.GetComponent<SkillDataC>();
		windowRect.width = 400;
		windowRect.height = 575;
		//Close Window Button
		if (GUI.Button (new Rect (310,8,70,70), "X")) {
			OnOffMenu();
		}
		if (GUI.Button (new Rect (20,60,75,75), dataSkill.skill[skillListSlot[0 + page]].icon)) {
			AssignSkill(skillSelect , 0 + page);
			shortcutPage = true;
			skillListPage = false;
			
		}
		GUI.Label (new Rect (110, 70, 140, 40), dataSkill.skill[skillListSlot[0 + page]].skillName , skillNameText); //Show Skill's Name
		GUI.Label (new Rect (110, 95, 140, 40), dataSkill.skill[skillListSlot[0 + page]].description , skillDescriptionText); //Show Skill's Description
		GUI.Label (new Rect (310, 90, 140, 40), "MP : " + dataSkill.skill[skillListSlot[0 + page]].manaCost , skillDescriptionText); //Show Skill's MP Cost
		//-----------------------------
		if (GUI.Button (new Rect (20,150,75,75), dataSkill.skill[skillListSlot[1 + page]].icon)) {
			AssignSkill(skillSelect , 1 + page);
			shortcutPage = true;
			skillListPage = false;
			
		}
		GUI.Label (new Rect (110, 160, 140, 40), dataSkill.skill[skillListSlot[1 + page]].skillName , skillNameText); //Show Skill's Name
		GUI.Label (new Rect (110, 185, 140, 40), dataSkill.skill[skillListSlot[1 + page]].description , skillDescriptionText); //Show Skill's Description
		GUI.Label (new Rect (310, 180, 140, 40), "MP : " + dataSkill.skill[skillListSlot[1 + page]].manaCost , skillDescriptionText); //Show Skill's MP Cost
		//-----------------------------
		if (GUI.Button (new Rect (20,240,75,75), dataSkill.skill[skillListSlot[2 + page]].icon)) {
			AssignSkill(skillSelect , 2 + page);
			shortcutPage = true;
			skillListPage = false;
			
		}
		GUI.Label (new Rect (110, 250, 140, 40), dataSkill.skill[skillListSlot[2 + page]].skillName , skillNameText); //Show Skill's Name
		GUI.Label (new Rect (110, 275, 140, 40), dataSkill.skill[skillListSlot[2 + page]].description , skillDescriptionText); //Show Skill's Description
		GUI.Label (new Rect (310, 270, 140, 40), "MP : " + dataSkill.skill[skillListSlot[2 + page]].manaCost , skillDescriptionText); //Show Skill's MP Cost
		//-----------------------------
		if (GUI.Button (new Rect (20,330,75,75), dataSkill.skill[skillListSlot[3 + page]].icon)) {
			AssignSkill(skillSelect , 3 + page);
			shortcutPage = true;
			skillListPage = false;
			
		}
		GUI.Label (new Rect (110, 340, 140, 40), dataSkill.skill[skillListSlot[3 + page]].skillName , skillNameText); //Show Skill's Name
		GUI.Label (new Rect (110, 365, 140, 40), dataSkill.skill[skillListSlot[3 + page]].description , skillDescriptionText); //Show Skill's Description
		GUI.Label (new Rect (310, 360, 140, 40), "MP : " + dataSkill.skill[skillListSlot[3 + page]].manaCost , skillDescriptionText); //Show Skill's MP Cost
		//-----------------------------
		if (GUI.Button (new Rect (20,420,75,75), dataSkill.skill[skillListSlot[4 + page]].icon)) {
			AssignSkill(skillSelect , 4 + page);
			shortcutPage = true;
			skillListPage = false;
			
		}
		GUI.Label (new Rect (110, 430, 140, 40), dataSkill.skill[skillListSlot[4 + page]].skillName , skillNameText); //Show Skill's Name
		GUI.Label (new Rect (110, 455, 140, 40), dataSkill.skill[skillListSlot[4 + page]].description , skillDescriptionText); //Show Skill's Description
		GUI.Label (new Rect (310, 450, 140, 40), "MP : " + dataSkill.skill[skillListSlot[4 + page]].manaCost , skillDescriptionText); //Show Skill's MP Cost
		//-----------------------------
		
		
		if (GUI.Button (new Rect (150,515,50,52), "1")) {
			page = 0;
		}
		if (GUI.Button (new Rect (220,515,50,52), "2")) {
			page = pageMultiply;
		}
		if (GUI.Button (new Rect (290,515,50,52), "3")) {
			page = pageMultiply *2;
		}
		
		GUI.DragWindow (new Rect (0,0,10000,10000));
	}
	
	public void AddSkill(int id) {
		bool geta = false;
		int pt = 0;
		while (pt < skillListSlot.Length && !geta) {
			if(skillListSlot[pt] == id){
				// Check if you already have this skill.
				geta = true;
			}else if(skillListSlot[pt] == 0){
				// Add Skill to empty slot.
				skillListSlot[pt] = id;
				StartCoroutine(ShowLearnedSkill(id));
				geta = true;
			}else{
				pt++;
			}
			
		}
		
	}
	
	public IEnumerator ShowLearnedSkill(int id) {
		SkillDataC dataSkill = database.GetComponent<SkillDataC>();
		showSkillLearned = true;
		showSkillName = dataSkill.skill[id].skillName;
		yield return new WaitForSeconds(10.5f);
		showSkillLearned = false;
		
	}
	
	public void ResetPosition() {
		//Reset GUI Position when it out of Screen.
		if(windowRect.x >= Screen.width -30 || windowRect.y >= Screen.height -30 || windowRect.x <= -70 || windowRect.y <= -70 ){
			windowRect = originalRect;
		}
	}
	
	public void LearnSkillByLevel(int lv) {
		int c = 0;
		while(c < learnSkill.Length){
			if(lv >= learnSkill[c].level){
				AddSkill(learnSkill[c].skillId);
			}
			c++;
		}
		
	}

	public bool HaveSkill(int id){
		bool have = false;
		for(int a = 0; a < skillListSlot.Length; a++){
			if(skillListSlot[a] == id){
				have = true;
			}
		}
		return have;
	}
	
	public void AssignSkillByID(int slot , int skillId){
		SkillDataC dataSkill = database.GetComponent<SkillDataC>();
		AttackTriggerC atk = GetComponent<AttackTriggerC>();
		atk.skill[slot].manaCost = dataSkill.skill[skillId].manaCost;
		atk.skill[slot].skillPrefab = dataSkill.skill[skillId].skillPrefab;
		atk.skill[slot].skillAnimation = dataSkill.skill[skillId].skillAnimation;
		atk.skill[slot].icon = dataSkill.skill[skillId].icon;
		atk.skill[slot].cantStack = dataSkill.skill[skillId].cantStack;
		skill[slot] = skillId;
		//-------Assign Icon to Skill Button-----------
		if(skillButton[slot]){
			skillButton[slot].sprite = dataSkill.skill[skillId].iconSprite;
		}
		//atk.skill[slot].delay = dataSkill.skill[skillId].delay;
	}
}