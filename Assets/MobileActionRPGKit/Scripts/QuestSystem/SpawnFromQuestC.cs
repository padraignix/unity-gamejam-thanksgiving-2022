using UnityEngine;
using System.Collections;

public class SpawnFromQuestC : MonoBehaviour {

	public int questId = 1;
	public GameObject spawnPrefab;
	public int progressAbove = 0;	//Will spawn your spawnPrefab if your progression of the quest greater than this.
	public int progressBelow = 9999;	//Will spawn your spawnPrefab if your progression of the quest lower than this.
	private GameObject player;
	
	void Start() {
		Spawn();
	}
	
	public void Spawn() {
		player = GameObject.FindWithTag("Player");
		//The Function will automatic check If player have this quest(ID) in the Quest Slot or not.
		if(player){
			QuestStatC qstat = player.GetComponent<QuestStatC>();
			if(qstat){
				bool letSpawn = player.GetComponent<QuestStatC>().CheckQuestSlot(questId);
				int checkProgress = player.GetComponent<QuestStatC>().CheckQuestProgress(questId);
				
				if(letSpawn && checkProgress >= progressAbove && checkProgress < progressBelow){
					//Spawn your spawnPrefab if player have this quest in the quest slot.
					GameObject m = (GameObject)Instantiate(spawnPrefab , transform.position , transform.rotation);
					m.name = spawnPrefab.name;
				}
			}
		}
	}

}
