using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class GameSystem : MonoBehaviour
{
    GameObject p1, p2;
    GameObject p1wintext, p2wintext;
    bool end;
    AudioSource explode;

    public int playerId;
    private Player player;
    private CharacterController cc;
    bool isPressed;

    void Awake() {
        // Get the Rewired Player object for this player and keep it for the duration of the character's lifetime
        playerId = 0;
        player = ReInput.players.GetPlayer(playerId);
    }

    // Start is called before the first frame update
    void Start()
    {
        end = false;
        p1 = GameObject.FindWithTag("Player1");
        p2 = GameObject.FindWithTag("Player2");
        p1wintext = GameObject.Find("P1WINS");
        p2wintext = GameObject.Find("P2WINS");
        p1wintext.SetActive(false);
        p2wintext.SetActive(false);
        explode = gameObject.GetComponent<AudioSource>();
    }
    public void beginrace()
    {
        p1.GetComponent<Cart>().beginBoost();
        p2.GetComponent<Cart>().beginBoost();
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
        isPressed = player.GetButtonDown("Start");
        if(isPressed) {
            isPressed = true;
            beginrace();
        }
    }
}
