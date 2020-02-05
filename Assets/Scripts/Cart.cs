using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Rewired;

public class Cart : MonoBehaviour
{
    public int playerId; 
    public int inputMethod;
    // 0 for wasd, 1 for Dir, otherwise(use 2) for Controller
    public Rigidbody m_rigidbody;
    public float speed;
    public float rotation;
    public int brake;
    public bool beginned, ended;

    public float moveSpeed = 3.0f;
    private Player player;
    private Vector3 moveVector;
    // private CharacterController cc;
    // Start is called before the first frame update

    void Awake() {
        // Get the Rewired Player object for this player and keep it for the duration of the character's lifetime
        player = ReInput.players.GetPlayer(playerId);
        
        // Get the character controller
        // cc = GetComponent<CharacterController>();
    }
    
    void Start()
    {
        beginned = false;
        ended = false;
        m_rigidbody = GetComponent<Rigidbody>();
        m_rigidbody.velocity = Vector3.zero;
    }

    public void End()
    {
        m_rigidbody.velocity *= 0f;
        ended = true;
    }

    private void ProcessInput() {
        // Process movement

        float turnYaw = player.GetAxis("Move Horizontal");
        // Debug.Log(turnYaw);
        bool accelerating = player.GetButtonDown("Accelerate");
        
        float moveVertical = speed;
        if (accelerating){
            Debug.Log("pressed accelerating");
            moveVertical += speed * 0.5f;
        } else {
            moveVertical = speed;
        }
        Vector3 movement = new Vector3(0.0f, 0.0f, 1.0f);
        Vector3 turn = new Vector3(0.0f, turnYaw, 0.0f);
        m_rigidbody.AddRelativeForce(movement * moveVertical);
        // transform.Translate(-moveVertical, 0, 0);
        transform.Rotate(turn * rotation);
    }

    public void beginBoost()
    {
        beginned = true;
        m_rigidbody.velocity = Vector3.forward;
    }

    void _moveController() {
        float speedBoost = speed * 10;
        if (player.GetButton("Accelerate"))
            speedBoost *= 1.1f;

        m_rigidbody.AddForce(speedBoost * m_rigidbody.velocity.normalized);
        m_rigidbody.AddForce(Vector3.forward * 2);
        float turnDegree = 0;
        if (player.GetAxis("Move Horizontal") < 0)
            turnDegree -= 1;
        if (player.GetAxis("Move Horizontal") > 0)
            turnDegree += 1;
        Vector3 tmp = new Vector3(0, 25* turnDegree, 0);
        m_rigidbody.velocity = Quaternion.Euler(tmp * Time.deltaTime) * m_rigidbody.velocity;
        // Debug.Log(m_rigidbody.velocity);
        tmp = m_rigidbody.velocity;
        tmp = Quaternion.Euler(0, 90, 0)* tmp;
        transform.rotation = Quaternion.LookRotation(tmp);
        // Debug.Log(transform.rotation);
    }

    void _moveWASD() {
        float speedBoost = speed * 10;
        if (Input.GetKey(KeyCode.W))
            speedBoost *= .7f;

        m_rigidbody.AddForce(speedBoost * m_rigidbody.velocity.normalized);
        m_rigidbody.AddForce(Vector3.forward * 2);
        float turnDegree = 0;
        if (Input.GetKey(KeyCode.A))
            turnDegree -= 1;
        if (Input.GetKey(KeyCode.D))
            turnDegree += 1;
        Vector3 tmp = new Vector3(0, 25* turnDegree, 0);
        m_rigidbody.velocity = Quaternion.Euler(tmp * Time.deltaTime) * m_rigidbody.velocity;
        // Debug.Log(m_rigidbody.velocity);
        tmp = m_rigidbody.velocity;
        tmp = Quaternion.Euler(0, 90, 0)* tmp;
        transform.rotation = Quaternion.LookRotation(tmp);
        // Debug.Log(transform.rotation);
    }

    void _moveDir()
    {
        float speedBoost = speed * 10;
        if (Input.GetKey(KeyCode.UpArrow))
            speedBoost *= .7f;

        m_rigidbody.AddForce(speedBoost * m_rigidbody.velocity.normalized);
        m_rigidbody.AddForce(Vector3.forward * 2);
        float turnDegree = 0;
        if (Input.GetKey(KeyCode.LeftArrow))
            turnDegree -= 1;
        if (Input.GetKey(KeyCode.RightArrow))
            turnDegree += 1;
        Vector3 tmp = new Vector3(0, 25 * turnDegree, 0);
        m_rigidbody.velocity = Quaternion.Euler(tmp * Time.deltaTime) * m_rigidbody.velocity;
        // Debug.Log(m_rigidbody.velocity);
        tmp = m_rigidbody.velocity;
        tmp = Quaternion.Euler(0, 90, 0) * tmp;
        transform.rotation = Quaternion.LookRotation(tmp);
        // Debug.Log(transform.rotation);
    }

    Vector3 _genMoveVecWASD()
    {
        float horizontal= 0, vertical = 0;
        //WD positive, AS negative 
        if (Input.GetKey(KeyCode.W))
            horizontal += .5f*speed;
        if (Input.GetKey(KeyCode.A))
            vertical -= speed;
        if (Input.GetKey(KeyCode.D))
            vertical += speed;
        horizontal += speed;
        return (Vector3.forward * horizontal) + (Vector3.right * vertical);
    }
    
    Vector3 _genMoveVecDir()
    {
        float horizontal = 0, vertical = 0;
        //WD positive, AS negative 
        if (Input.GetKey(KeyCode.UpArrow))
            horizontal += .5f*speed;
        if (Input.GetKey(KeyCode.LeftArrow))
            vertical -= speed;
        if (Input.GetKey(KeyCode.RightArrow))
            vertical += speed;
        horizontal += speed;
        return (Vector3.forward * horizontal) + (Vector3.right * vertical);
    }
    

    public void Cart_speedup(float speed=1.2f)
    {
        m_rigidbody.velocity *= speed;
    }

    Vector3 _genMoveVecController()
    {
        //TODO 
        throw new NotImplementedException();
    }

    void _addForce(Vector3 vec)
    {
         m_rigidbody.AddForce(10*vec);
    }



    // Update is called once per frame
    void Update()
    {
        // GetInput();
        // ProcessInput();
        if(!beginned || ended)
        {
            Vector3 tmp = m_rigidbody.velocity;
            tmp.x = tmp.y = 0f;
            m_rigidbody.velocity = tmp;
            return;
        }
        _moveController();
        // if (inputMethod == 0)
        //     _moveWASD();
        // else if (inputMethod == 1)
        //     _moveDir();
    }
}
