using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSystem : MonoBehaviour
{
    GameObject p1, p2;
    // Start is called before the first frame update
    void Start()
    {
        p1 = GameObject.FindWithTag("Player1");
        p2 = GameObject.FindWithTag("Player2");
    }

    public void speedUp(string name)
    {
        if (name == "Player1")
            p1.GetComponent<Cart>().Cart_speedup();
        else if (name == "Player2")
            p2.GetComponent<Cart>().Cart_speedup();
        else
             Debug.LogError("Wrong Player id");
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
