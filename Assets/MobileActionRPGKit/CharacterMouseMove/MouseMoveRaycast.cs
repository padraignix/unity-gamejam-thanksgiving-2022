using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MouseMoveRaycast : MonoBehaviour {
	GraphicRaycaster m_Raycaster;
	PointerEventData m_PointerEventData;
	EventSystem m_EventSystem;
	public CharacterMouseMoveC player;

	// Use this for initialization
	void Start(){
		m_Raycaster = GetComponent<GraphicRaycaster>();
		m_EventSystem = GetComponent<EventSystem>();
		if(!player){
			player = GameObject.FindWithTag("Player").GetComponent<CharacterMouseMoveC>();
		}
	}

	void Update () {
		//Ignore UI
		if(player){
			player.onUi = false;
			m_PointerEventData = new PointerEventData(m_EventSystem);
			m_PointerEventData.position = Input.mousePosition;
			List<RaycastResult> results = new List<RaycastResult>();
			
			m_Raycaster.Raycast(m_PointerEventData, results);
			
			foreach(RaycastResult result in results){
				//Debug.Log("Hit " + result.gameObject.name);
				if(result.gameObject.layer == 5){
					player.onUi = true;
				}
				/*if(Input.GetButton("Fire1") && result.gameObject.layer == 5){
					player.StopMoving();
				}*/
			}
		}
		//---------
	}
}
