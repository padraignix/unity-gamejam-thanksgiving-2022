using UnityEngine;
using System.Collections;

public class MoveForwardC : MonoBehaviour {

	public float Speed = 20f;
	public Vector3 relativeDirection = Vector3.forward;
	public float duration = 1.0f;
	
	void Start() {
		Destroy(gameObject, duration);
	}
	
	void Update() {
		Vector3 absoluteDirection = transform.rotation * relativeDirection;
		transform.position += absoluteDirection *Speed* Time.deltaTime;
		
	}

}
