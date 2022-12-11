using UnityEngine;
using System.Collections;

public class MinimapCameraC : MonoBehaviour {
	
	public Transform target;
	
	IEnumerator Start() {
		if(!target){
			yield return new WaitForSeconds(0.1f);
			target = GameObject.FindWithTag ("Player").transform;
		}
		
	}
	
	void Update() {
		if(!target){
			return;
		}
		if(Input.GetKeyDown(KeyCode.KeypadPlus) && GetComponent<Camera>().orthographicSize >= 20){
			GetComponent<Camera>().orthographic = true;
			GetComponent<Camera>().orthographicSize -= 10;
		}
		if(Input.GetKeyDown(KeyCode.KeypadMinus) && GetComponent<Camera>().orthographicSize <= 70){
			GetComponent<Camera>().orthographic = true;
			GetComponent<Camera>().orthographicSize += 10;
		}
		transform.position = new Vector3(target.position.x ,transform.position.y ,target.position.z);
	}
	
	public void FindTarget() {
		if(target){
			return;
		}
		Transform newTarget = GameObject.FindWithTag ("Player").transform;
		if(newTarget){
			target = newTarget;
		}
	}
}
