using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PixelCrushers.DialogueSystem; 

public class ResourceManager : MonoBehaviour
{
    public Text foodText;
    public int foodAmount;
    public int foodRate = -1;
    public Text woodText;
    public int woodAmount;
    public int wr1;
    public int wr2;
    public Text metalText;
    public int metalAmount;
    public int mr1;
    public int mr2;
    public Text mechanicalText;
    public int mechanicalAmount;
    public int mer1;
    public int mer2;
    
    public int DelayAmount = 1;
    protected float Timer;

    public AnimationClip death;
    private CharacterMotorC cmc;
    private TopDownControllerC tdc;
    private PlayerAnimationC pac;
    private Selector sel;
    private ResourceManager rm;

    public GameObject exitB;
    public GameObject failText;
    public GameObject successText;

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
            foodAmount = DialogueLua.GetVariable("Health").AsInt;
            woodAmount = DialogueLua.GetVariable("Wood").AsInt;
            metalAmount = DialogueLua.GetVariable("Metal").AsInt;
            mechanicalAmount = DialogueLua.GetVariable("Gear").AsInt;

            if (foodAmount <= 0)
            {
                DeathFailure();
            }
            else if (DialogueLua.GetQuestField("Fix the AI Drone", "State").AsString == "success")
            {
                SuccessWin();    
            }
            else
            {
                wr1 = DialogueLua.GetVariable("WoodRate-1").AsInt;
                mr1 = DialogueLua.GetVariable("MetalRate-1").AsInt;
                mer1 = DialogueLua.GetVariable("GearRate-1").AsInt;
                
                wr2 = DialogueLua.GetVariable("WoodRate-2").AsInt;
                mr2 = DialogueLua.GetVariable("MetalRate-2").AsInt;
                mer2 = DialogueLua.GetVariable("GearRate-2").AsInt;

                foodAmount += foodRate;
                foodText.text = foodAmount.ToString();

                woodAmount += wr1 + wr2;
                woodText.text = woodAmount.ToString();

                metalAmount += mr1 + mr2;
                metalText.text = metalAmount.ToString();

                mechanicalAmount += mer1 + mer2;
                mechanicalText.text = mechanicalAmount.ToString();

                DialogueLua.SetVariable("Health", foodAmount);
                DialogueLua.SetVariable("Wood", woodAmount);
                DialogueLua.SetVariable("Metal", metalAmount);
                DialogueLua.SetVariable("Gear", mechanicalAmount);
            }
         }        
    }

    public void DeathFailure()
    {
        DialogueLua.SetVariable("Health", 0);
        

        cmc = this.GetComponent<CharacterMotorC>();
        cmc.enabled = false;
        pac = this.GetComponent<PlayerAnimationC>();
        pac.DoDeath();
        pac.enabled = false;
        sel = this.GetComponent<Selector>();
        sel.enabled = false;
        rm = this.GetComponent<ResourceManager>();
        rm.enabled = false;
        tdc = this.GetComponent<TopDownControllerC>(); 
        tdc.enabled = false;

        exitB.SetActive(true);
        failText.SetActive(true);
    }

    public void SuccessWin()
    {
        DialogueLua.SetVariable("Health", 0);
        

        cmc = this.GetComponent<CharacterMotorC>();
        cmc.enabled = false;
        pac = this.GetComponent<PlayerAnimationC>();
        pac.enabled = false;
        sel = this.GetComponent<Selector>();
        sel.enabled = false;
        rm = this.GetComponent<ResourceManager>();
        rm.enabled = false;
        tdc = this.GetComponent<TopDownControllerC>(); 
        tdc.enabled = false;

        exitB.SetActive(true);
        successText.SetActive(true);
    }
}
