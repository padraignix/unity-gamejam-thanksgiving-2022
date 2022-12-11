using UnityEngine;
using System.Collections;

public class QuestProgressiveNPCC : MonoBehaviour {
	public int questId = 1;
	private GameObject player;
	
	public StartQuestCondition startConditionProgress;
	public StartQuestCondition startConditionEvent;
	
	public TextDialogue[] normalTalkText = new TextDialogue[1];
	public TextDialogue[] conditionPassText = new TextDialogue[1];
	
	private string showText = "";
	private string showText2 = "";
	private string showText3 = "";
	private string showText4 = "";
	
	private int textLength = 0;
	
	[HideInInspector]
	public bool enter = false;
	private bool showGui = false;
	
	public GUIStyle textStyle;
	public Texture2D button;
	public Texture2D textWindow;
	
	[HideInInspector]
	public int s = 0;
	
	private bool  cannotTakeQuest = false;

	public SetEventValues setEventValueWhenFinish;
	
	void Update(){
		if(Input.GetKeyDown("e") && enter){
			NextPage();
		}
	}
	
	void CheckStartCondition(){
		if(!player){
			player = GameObject.FindWithTag("Player");
		}
		bool haveQuest = player.GetComponent<QuestStatC>().CheckQuestSlot(questId);
		if(!haveQuest){
			cannotTakeQuest = true;
			return;
		}
		
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
	
	void NextPage(){
		CheckStartCondition();
		
		if(!cannotTakeQuest){
			//When pass Quest Condition
			textLength = conditionPassText.Length;
			if(s < textLength){
				showText = conditionPassText[s].textLine1;
				showText2 = conditionPassText[s].textLine2;
				showText3 = conditionPassText[s].textLine3;
				showText4 = conditionPassText[s].textLine4;
			}
			s++;
			Progress();
		}else{
			//When not Pass the Quest Condition
			textLength = normalTalkText.Length;
			if(s < textLength){
				showText = normalTalkText[s].textLine1;
				showText2 = normalTalkText[s].textLine2;
				showText3 = normalTalkText[s].textLine3;
				showText4 = normalTalkText[s].textLine4;
			}
			s++;
			TalkOnly();
		}
	}
	
	void OnGUI(){
		if(!player){
			return;
		}
		if(enter && !showGui){
			//GUI.DrawTexture( new Rect(Screen.width / 2 - 130, Screen.height - 120, 260, 80), button);
			if (GUI.Button ( new Rect(Screen.width / 2 - 130, Screen.height - 120, 260, 80), button)){
				NextPage();
			}
		}
		
		if(showGui && s <= textLength){
			GUI.DrawTexture( new Rect(Screen.width /2 - 308, Screen.height - 355, 615, 220), textWindow);
			GUI.Label ( new Rect(Screen.width /2 - 263, Screen.height - 320, 500, 200), showText , textStyle);
			GUI.Label ( new Rect(Screen.width /2 - 263, Screen.height - 290, 500, 200), showText2 , textStyle);
			GUI.Label ( new Rect(Screen.width /2 - 263, Screen.height - 260, 500, 200), showText3 , textStyle);
			GUI.Label ( new Rect(Screen.width /2 - 263, Screen.height - 230, 500, 200), showText4 , textStyle);
			if (GUI.Button ( new Rect(Screen.width /2 + 150,Screen.height - 200,140,60), "Next")) {
				NextPage();
			}
		}
		
	}
	
	void OnTriggerEnter(Collider other){
		if(other.tag == "Player"){
			s = 0;
			player = other.gameObject;
			enter = true;
		}
		
	}
	
	void OnTriggerExit(Collider other){
		if(other.tag == "Player"){
			s = 0;
			enter = false;
			CloseTalk();
		}
	}
	
	void TalkOnly(){
		if(s > textLength){
			showGui = false;
			CloseTalk();
		}else{
			Talking();
		}
	}
	
	void Progress(){
		if(s > textLength){
			showGui = false;
			if(player){
				QuestStatC qstat = player.GetComponent<QuestStatC>();
				if(qstat){
					player.GetComponent<QuestStatC>().Progress(questId);
				}
				if(setEventValueWhenFinish.enable){
					QuestStatC.SetEventValue(setEventValueWhenFinish.eventId , setEventValueWhenFinish.values , (int)setEventValueWhenFinish.setVal);
				}
			}
			CloseTalk();
		}else{
			Talking();
		}
		
	}
	
	void Talking(){
		if(!enter){
			return;
		}
		Time.timeScale = 0.0f;
		showGui = true;
		
	}
	
	void CloseTalk(){
		showGui = false;
		Time.timeScale = 1.0f;
		s = 0;
	}
	
}

[System.Serializable]
public class SetEventValues{
	public bool  enable = false;
	public int eventId = 0;
	public int values = 1;
	public MathCon setVal = MathCon.Equal;
}

public enum MathCon{
	Equal = 0,
	Plus = 1,
	Subtract = 2
}