﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void beginGame()
    {
        SceneManager.LoadScene("MainScene");
    }
    public void exitGame()
    {
        Application.Quit();
    }

    //Need code for controller to select buttons!


    // Update is called once per frame
    void Update()
    {
        
    }
}
