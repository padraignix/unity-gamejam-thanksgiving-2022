using UnityEngine;
using System.Collections;

public class Unparent : MonoBehaviour {
	public GameObject player;
	public bool dontDestroyOnLoad = false;
	// Use this for initialization
	void Start () {
		//transform.parent = null;
		transform.SetParent(null , true);
		if(!player){
			player = GameObject.FindWithTag("Player");
		}
		if(dontDestroyOnLoad){
			DontDestroyOnLoad(transform.gameObject);
		}
	}

	void Update() {
		if(!player){
			Destroy(gameObject);
		}
	}
}
