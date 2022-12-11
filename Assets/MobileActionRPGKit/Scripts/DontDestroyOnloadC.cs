using UnityEngine;
using System.Collections;

public class DontDestroyOnloadC : MonoBehaviour {
	
	void Awake(){
		//this.transform.parent = null;
		transform.SetParent(null , true);
		DontDestroyOnLoad(transform.gameObject);
	}
}