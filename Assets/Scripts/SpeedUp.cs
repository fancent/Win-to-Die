using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SpeedUp : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnTriggerEnter(Collider c)
    {
        GameSystem g_sys = GameObject.Find("GameSystem").GetComponent<GameSystem>();
        if (c.gameObject.tag == "Player1")
        {
            g_sys.speedUp("Player2");
        }
        else if(c.gameObject.tag == "Player2")
        {
            g_sys.speedUp("Player1");
        }
        Destroy(gameObject);
    }


    // Update is called once per frame
    void Update()
    {
    }
}
