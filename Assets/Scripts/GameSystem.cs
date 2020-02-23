using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Rewired;

public class GameSystem : MonoBehaviour
{
    GameObject p1, p2;
    GameObject p1wintext, p2wintext;
    GameObject startButton;
    bool start;
    bool paused;
    bool end;
    AudioSource explode;


    public GameObject pauseUI;
    private Player player;
    public int playerId = 0;

    void Awake()
    {
        player = ReInput.players.GetPlayer(playerId);
    }

    void init()
    {
        start = false;
        Time.timeScale = 0f;
        pauseUI.SetActive(false);
        paused = false;

    }
    // Start is called before the first frame update
    void Start()
    {
        end = false;
        p1 = GameObject.Find("LeftKart");
        p2 = GameObject.Find("RightKart");
        startButton = GameObject.Find("StartButton");
        p1wintext = GameObject.Find("P1WINS");
        p2wintext = GameObject.Find("P2WINS");
        p1wintext.SetActive(false);
        p2wintext.SetActive(false);
        explode = gameObject.GetComponent<AudioSource>();
        init();
    }
    public void beginrace()
    {
        start = true;
        p1.GetComponent<Cart>().beginBoost();
        p2.GetComponent<Cart>().beginBoost();
        Time.timeScale = 1f;
        startButton.SetActive(false);
    }
    public void resume()
    {
        
        Time.timeScale = 1f;
        pauseUI.SetActive(false);
        paused = false;
    }
    public void pause()
    {
        
        Time.timeScale = 0f;
        pauseUI.SetActive(true);
        paused = true;
    }

    public void win(string name)
    {
        if (end == true)
            return;
        end = true;

        explode.Play();
        if (name == "Player1")
            p1wintext.SetActive(true);
        else if (name == "Player2")
            p2wintext.SetActive(true);
        else
            Debug.LogError("Wrong Player id");

        p1.GetComponent<Cart>().End();
        p2.GetComponent<Cart>().End();
    }

    public void restart()
    {
        SceneManager.LoadScene("MainScene");
        init();
    }
    public void returnMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void speedUp(string name, float speed=1.2f)
    {
        if (name == "Player1")
            p1.GetComponent<Cart>().Cart_speedup(speed);
        else if (name == "Player2")
            p2.GetComponent<Cart>().Cart_speedup(speed);
        else
             Debug.LogError("Wrong Player id");
    }
    // Update is called once per frame
    void Update()
    {
        if (player.GetButtonDown("Start Game") || Input.GetKeyDown(KeyCode.P))
        {
            if (start)
            {
                if (paused)
                    resume();
                else
                    pause();
            }
            else
                beginrace();
        }
        
        if (player.GetButtonDown("Restart Game") || Input.GetKeyDown(KeyCode.Escape))
            restart();
    }
}
