using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem; 

public class NPCBark : MonoBehaviour
{
        
    public int DelayAmount;
    protected float Timer;
    public GameObject speaker;
    public string bark;
    private Animation anim;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Timer += Time.deltaTime;
 
         if (Timer >= DelayAmount)
         {
            Timer = 0f;
            anim = this.gameObject.GetComponent<Animation>();
            anim.Play("Talk");
            DialogueManager.BarkString(bark,speaker.transform);
            anim.PlayQueued("Idle");
         }
    }
}
