using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class TeleporterC : MonoBehaviour {
	
	public string teleportToMap = "BaseCamp";
	public string spawnPointName = "PlayerSpawnPointC"; //Use for Move Player to the SpawnPoint Position
	//Vector3 spawnPosition;
	
	void OnTriggerEnter(Collider other){
		if(other.tag == "Player"){
			if(other.GetComponent<CharacterMouseMoveC>()){
				other.GetComponent<CharacterMouseMoveC>().StopMoving();
			}
			other.GetComponent<StatusC>().spawnPointName = spawnPointName;
			StartCoroutine(ChangeMap(other.gameObject));
		}
	}
	
	IEnumerator ChangeMap(GameObject player){
		player.GetComponent<StatusC>().freeze = true;
		yield return new WaitForSeconds(0.1f);
		player.GetComponent<StatusC>().freeze = false;
		//Application.LoadLevel(teleportToMap);
		SceneManager.LoadScene(teleportToMap, LoadSceneMode.Single);
	}
}