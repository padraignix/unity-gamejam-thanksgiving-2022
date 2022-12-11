using UnityEngine;
using System.Collections;

[System.Serializable]
public class TextDialogue {
	public string textLine1 = "";
	public string textLine2 = "";
	public string textLine3 = "";
	public string textLine4 = "";
}

public class DialogueC : MonoBehaviour {
	
	public TextDialogue[] message = new TextDialogue[1];
	
	public Texture2D button;
	public Texture2D textWindow;

	[HideInInspector]
	public bool enter = false;

	private bool showGui = false;

	[HideInInspector]
	public int s = 0;

	private GameObject player;
	
	[HideInInspector]
	public bool talkFinish = false;
	public string sendMsgWhenFinish = "";
	public GameObject sendMsgTarget;
	
	public GUIStyle textStyle;
	
	void Update(){
		if(Input.GetKeyDown("e") && enter){
			if(s <= 0 && Time.timeScale == 0.0f){
				return;
			}
			NextPage();
		}		
	}

	void Activate(){
		if(enter){
			NextPage();
		}
	}
	
	void OnTriggerEnter(Collider other){
		if (other.tag == "Player") {
			s = 0;
			talkFinish = false;
			player = other.gameObject;
			enter = true;
		}		
	}
	
	void OnTriggerExit(Collider other){
		if(other.tag == "Player") {
			s = 0;
			enter = false;
			CloseTalk();
		}		
	}
	
	public void CloseTalk(){
		showGui = false;
		//Screen.lockCursor = true;
		s = 0;
	}
	
	public void NextPage(){
		if (!enter) {
			return;
		}
		s++;
		if(s > message.Length){
			showGui = false;
			talkFinish = true;
			Time.timeScale = 1.0f;
			CloseTalk();
			if(sendMsgWhenFinish != ""){
				if(!sendMsgTarget){
					sendMsgTarget = this.gameObject;
				}
				sendMsgTarget.SendMessage(sendMsgWhenFinish , SendMessageOptions.DontRequireReceiver);
			}
		}else{
			Time.timeScale = 0.0f;
			talkFinish = false;
			//Screen.lockCursor = false;
			showGui = true;
		}
	}
	
	void OnGUI(){
		if(!player){
			return;
		}
		if(s <= 0 && Time.timeScale == 0.0f){
			return;
		}
		if(enter && !showGui){
			//GUI.DrawTexture(Rect(Screen.width / 2 - 130, Screen.height - 120, 260, 80), button);
			if(GUI.Button (new Rect(Screen.width / 2 - 130, Screen.height - 180, 260, 80), button)){
				NextPage();
			}
		}
		if(showGui && s <= message.Length){ 
			GUI.DrawTexture(new Rect(Screen.width /2 - 308, Screen.height - 255, 615, 220), textWindow);
			GUI.Label (new Rect (Screen.width /2 - 263, Screen.height - 220, 500, 200), message[s-1].textLine1 , textStyle);
			GUI.Label (new Rect (Screen.width /2 - 263, Screen.height - 190, 500, 200), message[s-1].textLine2 , textStyle);
			GUI.Label (new Rect (Screen.width /2 - 263, Screen.height - 160, 500, 200), message[s-1].textLine3 , textStyle);
			GUI.Label (new Rect (Screen.width /2 - 263, Screen.height - 130, 500, 200), message[s-1].textLine4 , textStyle);
			if(GUI.Button (new Rect (Screen.width /2 + 150,Screen.height - 100,140,60), "Next")) {
				NextPage();
			}
		}
	}
}