using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Rewired;

public class MainMenu : MonoBehaviour
{
    private Player player;
    void Awake()
    {
        player = ReInput.players.GetPlayer(0);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void beginGame()
    {
        SceneManager.LoadScene("TrackScene");
    }
    public void exitGame()
    {
        Application.Quit();
    }

    //Need code for controller to select buttons!


    // Update is called once per frame
    void Update()
    {
        if (player.GetButton("Accelerate"))
            beginGame();
    }
}
