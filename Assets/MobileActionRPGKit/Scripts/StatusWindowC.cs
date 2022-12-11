using UnityEngine;
using System.Collections;

[RequireComponent (typeof(StatusC))]

public class StatusWindowC : MonoBehaviour {

	private bool show = false;
	public GUIStyle textStyle;
	public GUIStyle textStyle2;
	
	//Icon for Buffs
	public Texture2D braveIcon;
	public Texture2D barrierIcon;
	public Texture2D faithIcon;
	public Texture2D magicBarrierIcon;
	
	public Rect windowRect = new Rect (250, 220, 550, 450);
	private Rect originalRect;
	
	void Start(){
		originalRect = windowRect;
	}
	
	void Update(){
		if(Input.GetKeyDown("c")){
			OnOffMenu();
		}
	}
	
	void OnGUI(){
		StatusC stat = GetComponent<StatusC>();
		if(show){
			windowRect = GUI.Window (0, windowRect, StatWindow, "Status");
			
		}
		
		//Show Buffs Icon
		if(stat.brave){
			GUI.DrawTexture( new Rect(360,50,40,40), braveIcon);
		}
		if(stat.barrier){
			GUI.DrawTexture( new Rect(400,50,40,40), barrierIcon);
		}
		if(stat.faith){
			GUI.DrawTexture( new Rect(440,50,40,40), faithIcon);
		}
		if(stat.mbarrier){
			GUI.DrawTexture( new Rect(480,50,40,40), magicBarrierIcon);
		}
	}
	
	public void StatWindow(int windowID) {
		StatusC stat = GetComponent<StatusC>();
		//GUI.Box ( new Rect(180,170,240,380), "Status");
		GUI.Label ( new Rect(20, 40, 100, 50), "Level" , textStyle);
		GUI.Label ( new Rect(20, 100, 100, 50), "STR" , textStyle);
		GUI.Label ( new Rect(270, 100, 100, 50), "DEF" , textStyle);
		GUI.Label ( new Rect(20, 190, 100, 50), "MATK" , textStyle);
		GUI.Label ( new Rect(270, 190, 100, 50), "MDEF" , textStyle);
		
		GUI.Label ( new Rect(20, 300, 100, 50), "EXP" , textStyle);
		GUI.Label ( new Rect(20, 350, 150, 50), "Next LV" , textStyle);
		GUI.Label ( new Rect(20, 400, 200, 50), "Status Point" , textStyle);
		//Close Window Button
		if (GUI.Button ( new Rect(470,5,70,70), "X")) {
			OnOffMenu();
		}
		//Status
		int lv = stat.level;
		int atk = stat.atk;
		int def = stat.def;
		int matk = stat.matk;
		int mdef = stat.mdef;
		int exp = stat.exp;
		int next = stat.maxExp - exp;
		int stPoint = stat.statusPoint;
		
		GUI.Label ( new Rect(50, 40, 100, 50), lv.ToString() , textStyle2);
		GUI.Label ( new Rect(80, 100, 100, 50), atk.ToString() , textStyle2);
		GUI.Label ( new Rect(310, 100, 100, 50), def.ToString() , textStyle2);
		GUI.Label ( new Rect(80, 190, 100, 50), matk.ToString() , textStyle2);
		GUI.Label ( new Rect(310, 190, 100, 50), mdef.ToString() , textStyle2);
		
		GUI.Label ( new Rect(320, 300, 100, 50), exp.ToString() , textStyle2);
		GUI.Label ( new Rect(320, 350, 100, 50), next.ToString() , textStyle2);
		GUI.Label ( new Rect(320, 400, 100, 50), stPoint.ToString() , textStyle2);
		
		if (GUI.Button ( new Rect(210,90,50,50), "+") && stPoint > 0) {
			GetComponent<StatusC>().atk += 1;
			GetComponent<StatusC>().statusPoint -= 1;
			GetComponent<StatusC>().CalculateStatus();
		}
		if (GUI.Button ( new Rect(430,90,50,50), "+") && stPoint > 0) {
			GetComponent<StatusC>().def += 1;
			GetComponent<StatusC>().maxHealth += 5;
			GetComponent<StatusC>().statusPoint -= 1;
			GetComponent<StatusC>().CalculateStatus();
		}
		if (GUI.Button ( new Rect(210,180,50,50), "+") && stPoint > 0) {
			GetComponent<StatusC>().matk += 1;
			GetComponent<StatusC>().maxMana += 3;
			GetComponent<StatusC>().statusPoint -= 1;
			GetComponent<StatusC>().CalculateStatus();
		}
		if (GUI.Button ( new Rect(430,180,50,50), "+") && stPoint > 0) {
			GetComponent<StatusC>().mdef += 1;
			GetComponent<StatusC>().statusPoint -= 1;
			GetComponent<StatusC>().CalculateStatus();
		}
		GUI.DragWindow (new Rect (0,0,10000,10000)); 
	}
	
	public void OnOffMenu() {
		//Freeze Time Scale to 0 if Status Window is Showing
		if(!show && Time.timeScale != 0.0){
			show = true;
			Time.timeScale = 0.0f;
			ResetPosition();
			//Screen.lockCursor = false;
		}else if(show){
			show = false;
			Time.timeScale = 1.0f;
			//Screen.lockCursor = true;
		}
	}
	
	public void ResetPosition() {
		//Reset GUI Position when it out of Screen.
		if(windowRect.x >= Screen.width -30 || windowRect.y >= Screen.height -30 || windowRect.x <= -70 || windowRect.y <= -70 ){
			windowRect = originalRect;
		}
		
	}

}