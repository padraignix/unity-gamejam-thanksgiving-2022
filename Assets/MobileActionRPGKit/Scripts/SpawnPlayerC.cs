using UnityEngine;
using System.Collections;

public class SpawnPlayerC : MonoBehaviour {
	
	public GameObject player;
	//public GameObject mainCamPrefab;
	private Transform mainCam;
	public static bool onLoadGame = false;
	
	void Start(){
		//Check for Current Player in the scene
		GameObject currentPlayer = GameObject.FindWithTag("Player");
		if(currentPlayer){
			// If there are the player in the scene already. Check for the Spawn Point Name
			// If it match then Move Player to the SpawnpointPosition
			string spawnPointName = currentPlayer.GetComponent<StatusC>().spawnPointName;
			GameObject spawnPoint = GameObject.Find(spawnPointName);
			if(spawnPoint && !onLoadGame){
				if(currentPlayer.GetComponent<UnityEngine.AI.NavMeshAgent>()){
					currentPlayer.GetComponent<UnityEngine.AI.NavMeshAgent>().ResetPath();
					currentPlayer.GetComponent<UnityEngine.AI.NavMeshAgent>().SetDestination(transform.position);
					currentPlayer.SetActive(false);
				}
				currentPlayer.transform.position = spawnPoint.transform.position;
				currentPlayer.transform.rotation = spawnPoint.transform.rotation;
				currentPlayer.SetActive(true);
			}
			onLoadGame = false;
			GameObject oldCam = currentPlayer.GetComponent<AttackTriggerC>().Maincam.gameObject;
			if(!oldCam){
				return;
			}
			GameObject[] cam = GameObject.FindGameObjectsWithTag("MainCamera"); 
			foreach (GameObject cam2 in cam) { 
				if(cam2 != oldCam){
					Destroy(cam2.gameObject);
				}
			}
			
			if(currentPlayer.GetComponent<SpawnPartnerC>()){
				currentPlayer.GetComponent<SpawnPartnerC>().MoveToMaster();
			}
			// If there are the player in the scene already. We will not spawn the new player.
			return;
		}
		//Spawn Player
		Instantiate(player, transform.position , transform.rotation);
	}
}