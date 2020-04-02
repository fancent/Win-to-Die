using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Rewired;
using VehicleBehaviour;

public class GameSystem : MonoBehaviour
{
    GameObject p1, p2;
    GameObject p1wintext, p2wintext;
    public GameObject startButton;
    public Text countdown;
    bool start;
    bool paused;
    bool end;
    bool beginning;
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
        countdown.gameObject.SetActive(false);
        beginning = false;
        start = false;
        Time.timeScale = 0f;
        pauseUI.SetActive(false);
        paused = false;
        beginrace();
    }
    // Start is called before the first frame update
    void Start()
    {
        countdown.gameObject.SetActive(false);
        end = false;
        p1 = GameObject.Find("LeftFisherPrice");
        p2 = GameObject.Find("RightFisherPrice");
        p1wintext = GameObject.Find("P1WINS");
        p2wintext = GameObject.Find("P2WINS");
        p1wintext.SetActive(false);
        p2wintext.SetActive(false);
        explode = gameObject.GetComponent<AudioSource>();
        init();
    }

    IEnumerator cd()
    {
        beginning = true;
        yield return new WaitForSecondsRealtime(1f);
        countdown.gameObject.SetActive(true);
        countdown.color = new Color(255/255f, 205 / 255f, 74 / 255f, 255 / 255f);
        countdown.fontSize = 130;
        countdown.text = "3";
        yield return new WaitForSecondsRealtime(1);
        countdown.fontSize = 140;
        countdown.text = "2";
        yield return new WaitForSecondsRealtime(1);
        countdown.fontSize = 150;
        countdown.text = "1";
        yield return new WaitForSecondsRealtime(1);
        Time.timeScale = 1f;
        countdown.text = "GO!";
        countdown.color = new Color(67 / 255f, 231 / 255f, 181 / 255f, 255 / 255f);
        countdown.fontSize = 160;
        beginning = false;
        yield return new WaitForSecondsRealtime(1.5f);
        countdown.gameObject.SetActive(false);
    }
    public void beginrace()
    {
        
        p1.GetComponent<WheelVehicle>().beginBoost();
        p2.GetComponent<WheelVehicle>().beginBoost();
        start = true;
        StartCoroutine(cd());
        startButton.SetActive(false);
    }
    public void resume()
    {
        StartCoroutine(cd());
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

        p1.GetComponent<WheelVehicle>().End(name == "Player1");
        p2.GetComponent<WheelVehicle>().End(name == "Player2");
    }

    public void restart()
    {
        countdown.gameObject.SetActive(false);
        SceneManager.LoadScene("TrackScene");
        init();
    }
    public void returnMenu()
    {

        SceneManager.LoadScene("Menu");
    }

    public void speedUp(string name, float speed=1.2f)
    {
        if (name == "Player1")
            p1.GetComponent<WheelVehicle>().Cart_speedup(speed);
        else if (name == "Player2")
            p2.GetComponent<WheelVehicle>().Cart_speedup(speed);
        else
             Debug.LogError("Wrong Player id");
    }
    // Update is called once per frame
    void Update()
    {
        if (player.GetButtonDown("Start Game") || Input.GetKeyDown(KeyCode.P))
        {
            if (beginning)
                return;
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
