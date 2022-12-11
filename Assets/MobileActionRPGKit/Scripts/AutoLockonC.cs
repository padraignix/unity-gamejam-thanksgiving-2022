using UnityEngine;
using System.Collections;

public class AutoLockonC : MonoBehaviour {

	public float radius = 20.0f;  //this is how far it checks for other objects
	public float lockOnRange = 5.0f;
	private Transform lockTarget;
	private bool lockon = false;
	private GameObject target;
	
	public GameObject test;
	
	void Update() {

		Vector3 lookOn;

		if (Input.GetButton("Fire2")) {
			//lockTarget = FindClosestEnemy().transform;
			FindClosestEnemy();
			if (lockTarget) {
				lookOn = lockTarget.position;
				lookOn.y = transform.position.y;
				transform.LookAt(lookOn);
				lockon = true;
			}
		}
		
		if (Input.GetButtonUp ("Fire2")) {
			lockon = false;
		}
		
		if (Input.GetKeyDown ("j")) {
			FindClosestEnemy();
			if (lockTarget) {
				lookOn = lockTarget.position;
				lookOn.y = transform.position.y;
				transform.LookAt(lookOn);
			}
		}
		
	}
	
	// Find the closest enemy 
	void FindClosestEnemy() { 
		Vector3 checkPos = transform.position + transform.forward * lockOnRange;
		GameObject closest; 
		
		Instantiate(test , checkPos , transform.rotation);
		
		if (lockon) {
			closest = target;
			if (!closest) {
				lockon = false;
			}
			//return closest;
			lockTarget = closest.transform;
		}
		
		float distance = Mathf.Infinity; 
		Vector3 position = transform.position; 
		//var objectsAroundMe : Collider[] = Physics.OverlapSphere(transform.position, radius);
		lockTarget = null; // Reset Lock On Target
		Collider[] objectsAroundMe = Physics.OverlapSphere(checkPos , radius);
		foreach (Collider obj in objectsAroundMe){
			if (obj.CompareTag("Enemy")) {
				Vector3 diff = (obj.transform.position - position); 
				float curDistance = diff.sqrMagnitude; 
				if (curDistance < distance) { 
					//------------
					closest = obj.gameObject; 
					distance = curDistance;
					target = closest;
					lockTarget = closest.transform;
				} 
			}
		}
		//return closest; 
		
	}

}
