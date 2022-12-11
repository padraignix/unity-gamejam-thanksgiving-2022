using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UiMasterC : MonoBehaviour {
	
	public GameObject uiPrefab;
	private GameObject st;
	private GameObject sk;
	private GameObject inv;
	private GameObject ev;
	private GameObject hp;
	private GameObject qu;

	void Start(){
		if(uiPrefab){
			ev = Instantiate(uiPrefab , uiPrefab.transform.position , uiPrefab.transform.rotation) as GameObject;
			DontDestroyOnLoad(ev.gameObject);
			
			st = ev.GetComponent<UiTabC>().statusWindow;
			st.GetComponent<StatusWindowCanvasC>().player = this.gameObject;
			
			hp = ev.GetComponent<UiTabC>().healthBar;
			hp.GetComponent<HealthBarCanvasC>().player = this.gameObject;
			
			inv = ev.GetComponent<UiTabC>().inventory;
			inv.GetComponent<InventoryUiCanvasC>().player = this.gameObject;
			
			sk = ev.GetComponent<UiTabC>().skillWindow;
			sk.GetComponent<SkillTreeCanvasC>().player = this.gameObject;

			qu = ev.GetComponent<UiTabC>().questWindow;
			qu.GetComponent<QuestUiCanvasC>().player = this.gameObject;
		}
	}
	
	void Update(){
		if(st && Input.GetKeyDown("c")){
			OnOffStatusMenu();
		}
		if(inv && Input.GetKeyDown("i")){
			OnOffInventoryMenu();
		}
		if(sk && Input.GetKeyDown("k")){
			OnOffSkillMenu();
		}
		if(sk && Input.GetKeyDown("q")){
			OnOffQuestMenu();
		}
	}
	
	void CloseAllMenu(){
		if(st)
			st.SetActive(false);
		if(inv)
			inv.SetActive(false);
		if(sk)
			sk.SetActive(false);
		if(qu)
			qu.SetActive(false);
	}
	
	public void OnOffStatusMenu(){
		//Freeze Time Scale to 0 if Status Window is Showing
		if(st.activeSelf == false){
			Time.timeScale = 0.0f;
			//Screen.lockCursor = false;
			CloseAllMenu();
			st.SetActive(true);
		}else{
			Time.timeScale = 1.0f;
			//Screen.lockCursor = true;
			CloseAllMenu();
		}
	}
	
	public void OnOffInventoryMenu(){
		//Freeze Time Scale to 0 if Status Window is Showing
		if(inv.activeSelf == false){
			Time.timeScale = 0.0f;
			//Screen.lockCursor = false;
			CloseAllMenu();
			inv.SetActive(true);
		}else{
			Time.timeScale = 1.0f;
			//Screen.lockCursor = true;
			CloseAllMenu();
		}
	}
	
	public void OnOffSkillMenu(){
		//Freeze Time Scale to 0 if Status Window is Showing
		if(sk.activeSelf == false){
			Time.timeScale = 0.0f;
			//Screen.lockCursor = false;
			CloseAllMenu();
			sk.SetActive(true);
			sk.GetComponent<SkillTreeCanvasC>().Start();
		}else{
			Time.timeScale = 1.0f;
			//Screen.lockCursor = true;
			CloseAllMenu();
		}
	}

	public void OnOffQuestMenu(){
		//Freeze Time Scale to 0 if Status Window is Showing
		if(qu.activeSelf == false){
			Time.timeScale = 0.0f;
			//Screen.lockCursor = false;
			CloseAllMenu();
			qu.SetActive(true);
			qu.GetComponent<QuestUiCanvasC>().UpdateQuestDetails();
		}else{
			Time.timeScale = 1.0f;
			//Screen.lockCursor = true;
			CloseAllMenu();
		}
	}
	
	public void DestroyAllUi(){
		if(ev)
			Destroy(ev);
	}
}