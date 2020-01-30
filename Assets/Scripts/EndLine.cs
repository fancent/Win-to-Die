using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndLine : MonoBehaviour
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
            g_sys.win("Player2");
        }
        else if (c.gameObject.tag == "Player2")
        {
            g_sys.win("Player1");
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
