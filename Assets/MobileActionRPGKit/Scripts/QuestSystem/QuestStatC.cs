using UnityEngine;
using System.Collections;

public class QuestStatC : MonoBehaviour {

	public GameObject questDataBase;
	
	public static int[] questProgress = new int[50];
	public static int[] eventId = new int[50];
	public int[] questSlot = new int[5];
	
	public GUIStyle questNameText;
	public GUIStyle questDescriptionText;
	
	public GUISkin skin1;
	public Rect windowRect = new Rect (360 ,140 ,480 ,490);
	private Rect originalRect;
	
	private bool menu = false;
	public bool useLegacyUi = false;
	
	void Start(){
		QuestDataC quest = questDataBase.GetComponent<QuestDataC>();
		// If Array Length of questProgress Variable < QuestData.Length
		if(questProgress.Length < quest.questData.Length){
			questProgress = new int[quest.questData.Length];
		}
		originalRect = windowRect;
	}
	
	void Update(){
		if(Input.GetKeyDown("q") && useLegacyUi){
			OnOffMenu();
		}
	}
	
	public bool AddQuest(int id){
		bool full = false;
		bool geta = false;
		
		int pt = 0;
		while(pt < questSlot.Length && !geta){
			if(questSlot[pt] == id){
				print("You Already Accept this Quest");
				geta = true;
				
			}else if(questSlot[pt] == 0){
				questSlot[pt] = id;
				geta = true;
			}else{
				pt++;
				if(pt >= questSlot.Length){
					full = true;
					print("Full");
				}
			}
			
		}
		return full;
	}
	
	public void SortQuest(){
		int pt = 0;
		int nextp = 0;
		bool clearr = false;
		while(pt < questSlot.Length){
			if(questSlot[pt] == 0){
				nextp = pt + 1;
				while(nextp < questSlot.Length && !clearr){
					if(questSlot[nextp] > 0){
						//Fine Next Slot and Set
						questSlot[pt] = questSlot[nextp];
						questSlot[nextp] = 0;
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
	
	void OnGUI() {
		if(menu){
			windowRect = GUI.Window (5, windowRect, QuestWindow, "Quest Log");
			//------------------------
		}
	}
	
	public void QuestWindow(int windowID) {
		GUI.skin = skin1;
		QuestDataC data = questDataBase.GetComponent<QuestDataC>();
		//Close Window Button
		if (GUI.Button (new Rect (390,8,70,70), "X")) {
			OnOffMenu();
		}
		//GUI.Box (new Rect (260,140,280,385), "Quest Log");
		
		if(questSlot[0] > 0){
			//Quest Name
			GUI.Label (new Rect (15, 45, 280, 40), data.questData[questSlot[0]].questName , questNameText);
			//Quest Info + Progress
			GUI.Label (new Rect (15, 75, 280, 40), data.questData[questSlot[0]].description + " (" + questProgress[questSlot[0]].ToString() + " / " + data.questData[questSlot[0]].finishProgress + ")" , questDescriptionText);
			//Cancel Quest
			if (GUI.Button (new Rect (290,45,84,50), "Cancel")) {
				questProgress[questSlot[0]] = 0;
				questSlot[0] = 0;
				SortQuest();
			}
		}
		//-----------------------------------------
		if(questSlot[1] > 0){
			//Quest Name
			GUI.Label (new Rect (15, 125, 280, 40), data.questData[questSlot[1]].questName , questNameText);
			//Quest Info + Progress
			GUI.Label (new Rect (15, 155, 280, 40), data.questData[questSlot[1]].description + " (" + questProgress[questSlot[1]].ToString() + " / " + data.questData[questSlot[1]].finishProgress + ")" , questDescriptionText);
			//Cancel Quest
			if (GUI.Button (new Rect (290,125,84,50), "Cancel")) {
				questProgress[questSlot[1]] = 0;
				questSlot[1] = 0;
				SortQuest();
			}
		}
		//-----------------------------------------
		if(questSlot[2] > 0){
			//Quest Name
			GUI.Label (new Rect (15, 205, 280, 40), data.questData[questSlot[2]].questName , questNameText);
			//Quest Info + Progress
			GUI.Label (new Rect (15, 235, 280, 40), data.questData[questSlot[2]].description + " (" + questProgress[questSlot[2]].ToString() + " / " + data.questData[questSlot[2]].finishProgress + ")" , questDescriptionText);
			//Cancel Quest
			if (GUI.Button (new Rect (290,205,84,50), "Cancel")) {
				questProgress[questSlot[2]] = 0;
				questSlot[2] = 0;
				SortQuest();
			}
		}
		//-----------------------------------------
		if(questSlot[3] > 0){
			//Quest Name
			GUI.Label (new Rect (15, 285, 280, 40), data.questData[questSlot[3]].questName , questNameText);
			//Quest Info + Progress
			GUI.Label (new Rect (15, 315, 280, 40), data.questData[questSlot[3]].description + " (" + questProgress[questSlot[3]].ToString() + " / " + data.questData[questSlot[3]].finishProgress + ")" , questDescriptionText);
			//Cancel Quest
			if (GUI.Button (new Rect (290,285,84,50), "Cancel")) {
				questProgress[questSlot[3]] = 0;
				questSlot[3] = 0;
				SortQuest();
			}
		}
		//-----------------------------------------
		if(questSlot[4] > 0){
			//Quest Name
			GUI.Label (new Rect (15, 365, 280, 40), data.questData[questSlot[4]].questName , questNameText);
			//Quest Info + Progress
			GUI.Label (new Rect (15, 395, 280, 40), data.questData[questSlot[4]].description + " (" + questProgress[questSlot[4]].ToString() + " / " + data.questData[questSlot[4]].finishProgress + ")" , questDescriptionText);
			//Cancel Quest
			if (GUI.Button (new Rect (290,365,84,50), "Cancel")) {
				questProgress[questSlot[4]] = 0;
				questSlot[4] = 0;
				SortQuest();
			}
		}
		
		//-----------------------------------------
		GUI.DragWindow (new Rect (0,0,10000,10000)); 
	}
	
	public void OnOffMenu() {
		//Freeze Time Scale to 0 if Window is Showing
		if(!menu && Time.timeScale != 0.0){
			menu = true;
			ResetPosition();
			Time.timeScale = 0.0f;
		}else if(menu){
			menu = false;
			Time.timeScale = 1.0f;
		}
		
	}
	
	public void Progress(int id) {
		//Check for You have a quest ID match to one of Quest Slot
		for(int n = 0; n < questSlot.Length ; n++){
			if(questSlot[n] == id && id != 0){
				QuestDataC data = questDataBase.GetComponent<QuestDataC>();
				// Check If The Progress of this quest < Finish Progress then increase 1 of Quest Progress Variable
				if(questProgress[id] < data.questData[questSlot[n]].finishProgress){
					questProgress[id] += 1;
				}
				print("Quest Slot =" + n);
			}
		}
	}
	//-----------------------------------------------
	
	public bool CheckQuestSlot(int id) {
		//Check for You have a quest ID match to one of Quest Slot
		bool exist = false;
		for(int n = 0; n < questSlot.Length ; n++){
			if(questSlot[n] == id && id != 0){
				//You Have this quest in the slot
				exist = true;
			}
			
		}
		return exist;
		
	}
	
	public int CheckQuestProgress(int id) {
		//Check for You have a quest ID match to one of Quest Slot
		int qProgress = 0;
		for(int n = 0; n < questSlot.Length ; n++){
			if(questSlot[n] == id && id != 0){
				//You Have this quest in the slot
				qProgress = questProgress[id];
			}
		}
		return qProgress;
	}
	
	//---------------------------------------
	
	public void Clear(int id) {
		//Check for You have a quest ID match to one of Quest Slot
		for(int n = 0; n < questSlot.Length ; n++){
			if(questSlot[n] == id && id != 0){
				//QuestDataC data = questDataBase.GetComponent<QuestDataC>();
				questProgress[id] += 10;
				questSlot[n] = 0;
				SortQuest();
				print("Quest Slot =" + n);
			}
		}
	}
	
	public void ResetPosition() {
		//Reset GUI Position when it out of Screen.
		if(windowRect.x >= Screen.width -30 || windowRect.y >= Screen.height -30 || windowRect.x <= -70 || windowRect.y <= -70 ){
			windowRect = originalRect;
		}
	}

	public static void SetEventValue(int id , int val , int con){
		if(con == 0){
			//Set Equal
			eventId[id] = val;
		}else if(con == 1){
			//Plus
			eventId[id] += val;
		}else if(con == 2){
			//Subtract
			eventId[id] -= val;
		}
	}

}