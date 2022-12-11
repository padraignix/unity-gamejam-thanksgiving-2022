using UnityEngine;
using System.Collections;

public class QuestClientC : MonoBehaviour {

	public int questId = 1;
	public GameObject questData;
	//private int finishProgress = 0;
	public Texture2D button;
	public Texture2D textWindow;
	[HideInInspector]
	public bool enter = false;
	private bool showGui = false;
	private bool showError = false;
	[HideInInspector]
	public int s = 0;
	
	private GameObject player;
	
	public TextDialogue[] talkText = new TextDialogue[3];
	public TextDialogue[] ongoingQuestText = new TextDialogue[1];
	public TextDialogue[] finishQuestText = new TextDialogue[1];
	public TextDialogue[] alreadyFinishText = new TextDialogue[1];

	public TextDialogue[] beforeStartText = new TextDialogue[1];

	private string errorLog = "Quest Full...";
	
	public GUIStyle textStyle;
	private bool acceptQuest = false;
	public bool trigger = true;
	private int textLength = 0;
	public string showText = "";
	public string showText2 = "";
	public string showText3 = "";
	public string showText4 = "";
	private bool thisActive = false;
	private bool questFinish = false;
	private bool cannotTakeQuest = false;
	public GameObject spawnPrefabWhenAccept;
	public Transform spawnPoint; //The Spawn Point of spawnPrefabWhenAccept(In th scene)
	private bool alreadySpawn = false;

	public StartQuestCondition startConditionProgress;
	public StartQuestCondition startConditionEvent;
	
	void Update() {
		if(Input.GetKeyDown("e") && enter && thisActive && !showError){
			NextPage();
		}
	}

	void Activate(){
		if(enter && thisActive && !showError){
			NextPage();
		}
	}

	void CheckStartCondition(){
		if(startConditionProgress.enable){
			cannotTakeQuest = true;
			if(startConditionProgress.condition == Qcheck.Bigger && QuestStatC.questProgress[startConditionProgress.id] > startConditionProgress.useValue){
				cannotTakeQuest = false;
			}
			if(startConditionProgress.condition == Qcheck.Below && QuestStatC.questProgress[startConditionProgress.id] < startConditionProgress.useValue){
				cannotTakeQuest = false;
			}
			if(startConditionProgress.condition == Qcheck.Equal && QuestStatC.questProgress[startConditionProgress.id] == startConditionProgress.useValue){
				cannotTakeQuest = false;
			}
		}
		if(startConditionEvent.enable){
			cannotTakeQuest = true;
			if(startConditionEvent.condition == Qcheck.Bigger && QuestStatC.eventId[startConditionEvent.id] > startConditionEvent.useValue){
				cannotTakeQuest = false;
			}
			if(startConditionEvent.condition == Qcheck.Below && QuestStatC.eventId[startConditionEvent.id] < startConditionEvent.useValue){
				cannotTakeQuest = false;
			}
			if(startConditionEvent.condition == Qcheck.Equal && QuestStatC.eventId[startConditionEvent.id] == startConditionEvent.useValue){
				cannotTakeQuest = false;
			}
		}
	}
	
	public void NextPage(){
		CheckStartCondition();
		//Check if it already finish
		int ongoing = player.GetComponent<QuestStatC>().CheckQuestProgress(questId);
		int finish = questData.GetComponent<QuestDataC>().questData[questId].finishProgress;
		int qprogress = QuestStatC.questProgress[questId];
		if(qprogress >= finish + 9){
			textLength = alreadyFinishText.Length;
			if(s < textLength){
				showText = alreadyFinishText[s].textLine1;
				showText2 = alreadyFinishText[s].textLine2;
				showText3 = alreadyFinishText[s].textLine3;
				showText4 = alreadyFinishText[s].textLine4;
			}
			s++;
			TalkOnly();
			print("Already Clear");
			return;
		}
		
		if(acceptQuest){
			if(ongoing >= finish){ //Quest Complete
				textLength = finishQuestText.Length;
				if(s < textLength){
					showText = finishQuestText[s].textLine1;
					showText2 = finishQuestText[s].textLine2;
					showText3 = finishQuestText[s].textLine3;
					showText4 = finishQuestText[s].textLine4;
				}
				s++;
				FinishQuest();
			}else{ //Ongoing
				textLength = ongoingQuestText.Length;
				if(s < textLength){
					showText = ongoingQuestText[s].textLine1;
					showText2 = ongoingQuestText[s].textLine2;
					showText3 = ongoingQuestText[s].textLine3;
					showText4 = ongoingQuestText[s].textLine4;
				}
				s++;
				TalkOnly();
			}
		}else if(cannotTakeQuest){
			//When Cannot Start the Quest by Condition
			textLength = beforeStartText.Length;
			if(s < textLength){
				showText = beforeStartText[s].textLine1;
				showText2 = beforeStartText[s].textLine2;
				showText3 = beforeStartText[s].textLine3;
				showText4 = beforeStartText[s].textLine4;
			}
			s++;
			TalkOnly();
		}else{
			//Before Take the quest
			textLength = talkText.Length;
			if(s < textLength){
				showText = talkText[s].textLine1;
				showText2 = talkText[s].textLine2;
				showText3 = talkText[s].textLine3;
				showText4 = talkText[s].textLine4;
			}
			s++;
			TakeQuest();
		}
	}
	
	public void TakeQuest(){
		if(s > textLength){
			showGui = false;
			StartCoroutine(AcceptQuest());
			CloseTalk();
		}else{
			Talking();
		}
		
	}
	
	public void TalkOnly() {
		if(s > textLength){
			showGui = false;
			CloseTalk();
		}else{
			Talking();
		}
	}
	
	public void FinishQuest() {
		if(s > textLength){
			showGui = false;
			questData.GetComponent<QuestDataC>().QuestClear(questId , player);
			player.GetComponent<QuestStatC>().Clear(questId);
			print("Clear");
			questFinish = true;
			CloseTalk();
		}else{
			Talking();
		}
	}
	
	public IEnumerator AcceptQuest() {
		bool full = player.GetComponent<QuestStatC>().AddQuest(questId);
		if(full){
			//Quest Full
			showError = true; //Show Quest Full Window
			yield return new WaitForSeconds(1);
			showError = false;
			
		}else{
			acceptQuest = player.GetComponent<QuestStatC>().CheckQuestSlot(questId);
			if(spawnPrefabWhenAccept && !alreadySpawn){
				if(!spawnPoint){
					spawnPoint = this.transform;
				}
				GameObject m = (GameObject)Instantiate(spawnPrefabWhenAccept , spawnPoint.position , spawnPoint.rotation);
				m.name = spawnPrefabWhenAccept.name;
				print("Spawned");
				alreadySpawn = true;
			}
		}
		
	}
	
	public void CheckQuestCondition() {
		QuestDataC quest = questData.GetComponent<QuestDataC>();
		int progress = player.GetComponent<QuestStatC>().CheckQuestProgress(questId);
		
		if(progress >= quest.questData[questId].finishProgress){
			//Quest Clear
			quest.QuestClear(questId , player);
			
		}
		
	}
	
	void OnGUI() {
		if(!player){
			return;
		}
		if(enter && !showGui && !showError && trigger){
			//GUI.DrawTexture(new Rect(Screen.width / 2 - 130, Screen.height - 120, 260, 80), button);
			if (GUI.Button (new Rect(Screen.width / 2 - 130, Screen.height - 120, 260, 80), button)){
				NextPage();
			}
		}
		
		if(showError){
			GUI.DrawTexture(new Rect(Screen.width /2 - 308, Screen.height - 355, 615, 220), textWindow);
			GUI.Label (new Rect (Screen.width /2 - 263, Screen.height - 320, 500, 200), errorLog , textStyle);
			if (GUI.Button (new Rect (Screen.width /2 + 150,Screen.height - 200,140,60), "OK")) {
				showError = false;
			}
		}
		if(showGui && !showError && s <= textLength){
			GUI.DrawTexture(new Rect(Screen.width /2 - 308, Screen.height - 355, 615, 220), textWindow);
			//GUI.Label (new Rect (Screen.width /2 - 263, Screen.height - 320, 500, 200), showText , textStyle);

			GUI.Label (new Rect (Screen.width /2 - 263, Screen.height - 320, 500, 200), showText , textStyle);
			GUI.Label (new Rect (Screen.width /2 - 263, Screen.height - 290, 500, 200), showText2 , textStyle);
			GUI.Label (new Rect (Screen.width /2 - 263, Screen.height - 260, 500, 200), showText3 , textStyle);
			GUI.Label (new Rect (Screen.width /2 - 263, Screen.height - 230, 500, 200), showText4 , textStyle);

			if (GUI.Button (new Rect (Screen.width /2 + 150,Screen.height - 200,140,60), "Next")) {
				NextPage();
			}
		}
		
	}
	
	
	void OnTriggerEnter(Collider other) {
		if(!trigger){
			return;
		}
		if(other.tag == "Player"){
			s = 0;
			player = other.gameObject;
			acceptQuest = player.GetComponent<QuestStatC>().CheckQuestSlot(questId);
			enter = true;
			thisActive = true;
		}
		
	}
	
	void OnTriggerExit(Collider other) {
		if(!trigger){
			return;
		}
		if(other.tag == "Player"){
			s = 0;
			enter = false;
			CloseTalk();
		}
		thisActive = false;
		showError = false;
		
	}
	
	public void Talking() {
		if(!enter){
			return;
		}
		Time.timeScale = 0.0f;
		showGui = true;
		
	}
	
	public void CloseTalk() {
		showGui = false;
		Time.timeScale = 1.0f;
		s = 0;
		
	}
	
	public bool ActivateQuest(GameObject p) {
		player = p;
		acceptQuest = player.GetComponent<QuestStatC>().CheckQuestSlot(questId);
		thisActive = false;
		trigger = false;
		NextPage();
		return questFinish;
		
	}

}

[System.Serializable]
public class StartQuestCondition{
	public bool  enable = false;
	public int id = 1;
	public int useValue = 5;
	public Qcheck condition = Qcheck.Bigger;
}
	
public enum Qcheck{
	Bigger = 0,
	Below = 1,
	Equal = 2
}
	
