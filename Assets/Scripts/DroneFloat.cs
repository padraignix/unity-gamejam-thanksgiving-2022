using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneFloat : MonoBehaviour
{
    public float speed = 0.2f;
    public float neg;
    public int mult;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float y = Mathf.PingPong(Time.time * speed, 1) * mult - neg;
        this.transform.position = new Vector3(this.transform.position.x, y, this.transform.position.z);
    }
}
