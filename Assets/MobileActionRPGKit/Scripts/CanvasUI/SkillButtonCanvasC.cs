using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SkillButtonCanvasC : MonoBehaviour {
	
	public int buttonId = 0;
	public Image iconImageObj;
	public Sprite icon;
	public Sprite iconLocked;
	
	public SkillTreeCanvasC skillTree;

	void Start(){
		if(!skillTree){
			skillTree = transform.root.GetComponent<SkillTreeCanvasC>();
		}
	}
	
	
	public void UpdateIcon(){
		iconImageObj.color = Color.white;
		if(skillTree.skillSlots[buttonId].locked){
			iconImageObj.sprite = iconLocked;
			return;
		}else{
			iconImageObj.sprite = icon;
		}
		
		if(!skillTree.skillSlots[buttonId].learned){
			iconImageObj.color = Color.gray;
		}
	}
	
	
}