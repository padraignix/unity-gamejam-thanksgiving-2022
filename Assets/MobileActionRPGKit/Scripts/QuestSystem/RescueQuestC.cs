using UnityEngine;
using System.Collections;

public class RescueQuestC : MonoBehaviour {

	public int questId = 1;
	public GameObject npcModel;
	public AnimationClip npcAnimation;
	public GameObject disappearEffect;
	public Texture2D button;
	private GameObject player;
	private bool enter = false;
	private bool clear = false;
	
	void Update(){
		if(Input.GetKeyDown("e") && enter){
			StartCoroutine(QuestEvent());
		}
	}

	void Activate(){
		if(enter){
			StartCoroutine(QuestEvent());
		}
	}
	
	void OnTriggerEnter(Collider other){
		if(other.tag == "Player"){
			player = other.gameObject;
			enter = true;
		}
		
	}
	
	void OnTriggerExit(Collider other){
		if(other.tag == "Player"){
			enter = false;
		}
	}
	
	public IEnumerator QuestEvent(){
		if(!clear){
			//Increase the progress of the Quest ID
			//The Function will automatic check If player have this quest(ID) in the Quest Slot or not.
			if(player){
				QuestStatC qstat = player.GetComponent<QuestStatC>();
				if(qstat){
					player.GetComponent<QuestStatC>().Progress(questId);
					if(npcAnimation){
						npcModel.GetComponent<Animation>().Play(npcAnimation.name);
					}
					clear = true;
					float wait = npcModel.GetComponent<Animation>()[npcAnimation.name].length;
					yield return new WaitForSeconds(wait + 0.3f);
					if(disappearEffect){
						Instantiate(disappearEffect , transform.position , transform.rotation);
					}
					Destroy(gameObject);
				}
			}
		}
	}
	
	void OnGUI() {
		if(!player){
			return;
		}
		/*if(enter && !clear){
		GUI.DrawTexture(Rect(Screen.width / 2 - 130, Screen.height - 120, 260, 80), button);
	}*/
		if(enter && !clear){
			if (GUI.Button (new Rect(Screen.width / 2 - 130, Screen.height - 180, 260, 80), button)){
				StartCoroutine(QuestEvent());
			}
		}
	}
}
