using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceObjectManager : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject wood;
    public GameObject metal;
    public GameObject gear;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setWood()
    {
        wood.SetActive(true);
        metal.SetActive(false);
        gear.SetActive(false);
    }
    public void setMetal()
    {
        wood.SetActive(false);
        metal.SetActive(true);
        gear.SetActive(false); 
    }
    public void setGear()
    {
        wood.SetActive(false);
        metal.SetActive(false);
        gear.SetActive(true);         
    }
    public void setClear()
    {
        wood.SetActive(false);
        metal.SetActive(false);
        gear.SetActive(false);          
    }
}
