using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Rewired;

public class Cart : MonoBehaviour
{
    public PowerUpSlot slot;
    public StatusList stat;
    public GameObject slotAnchor;

    public int inputMethod;
    // 0 for wasd, 1 for Dir, otherwise(use 2) for Controller
    public Rigidbody m_rigidbody;
    public float speed;
    public int brake;
    public bool beginned, ended;

    public int playerId = 0;
    private Player player;

    void Awake()
    {
        player = ReInput.players.GetPlayer(playerId);
    }
    // Start is called before the first frame update
    void Start()
    {
        stat = gameObject.GetComponent<StatusList>();
        slot = gameObject.GetComponent<PowerUpSlot>();
        slot.Initialize(1, this);
        beginned = false;
        ended = false;
        m_rigidbody = gameObject.GetComponent<Rigidbody>();
        m_rigidbody.velocity = Vector3.zero;
    }

    public void beginBoost()
    {
        beginned = true;
        m_rigidbody.velocity = Vector3.forward;
    }

    void _moveWASD() {

        float speedBoost = speed * 10;
        if (Input.GetKey(KeyCode.W))
            speedBoost *= 1.2f;

        m_rigidbody.AddForce(speedBoost * m_rigidbody.velocity.normalized);
        m_rigidbody.AddForce(Vector3.forward * 2);
        float turnDegree = 0;
        if (Input.GetKey(KeyCode.A))
            turnDegree -= 1;
        if (Input.GetKey(KeyCode.D))
            turnDegree += 1;
        Vector3 tmp = new Vector3(0, 25* turnDegree, 0);
        m_rigidbody.velocity = Quaternion.Euler(tmp * Time.deltaTime) * m_rigidbody.velocity;
        //Debug.Log(m_rigidbody.velocity);
        tmp = m_rigidbody.velocity;
        tmp = Quaternion.Euler(0, 90, 0)* tmp;
        transform.rotation = Quaternion.LookRotation(tmp);
        //Debug.Log(transform.rotation);
    }

    void _moveDir()
    {
        float speedBoost = speed * 10;
        if (Input.GetKey(KeyCode.UpArrow))
            speedBoost *= 1.2f;

        m_rigidbody.AddForce(speedBoost * m_rigidbody.velocity.normalized);
        m_rigidbody.AddForce(Vector3.forward * 2);
        float turnDegree = 0;
        if (Input.GetKey(KeyCode.LeftArrow))
            turnDegree -= 1;
        if (Input.GetKey(KeyCode.RightArrow))
            turnDegree += 1;
        Vector3 tmp = new Vector3(0, 25 * turnDegree, 0);
        m_rigidbody.velocity = Quaternion.Euler(tmp * Time.deltaTime) * m_rigidbody.velocity;
        //Debug.Log(m_rigidbody.velocity);
        tmp = m_rigidbody.velocity;
        tmp = Quaternion.Euler(0, 90, 0) * tmp;
        transform.rotation = Quaternion.LookRotation(tmp);
        //Debug.Log(transform.rotation);
    }

    void _moveController()
    {
        float speedBoost = speed * 10;
        if (player.GetButton("Accelerate"))
            speedBoost *= 1.2f;

        m_rigidbody.AddForce(speedBoost * m_rigidbody.velocity.normalized);
        m_rigidbody.AddForce(Vector3.forward * 2);
        float turnDegree = 0;
        turnDegree += player.GetAxis("Move Horizontal");
        Vector3 tmp = new Vector3(0, 25 * turnDegree, 0);
        m_rigidbody.velocity = Quaternion.Euler(tmp * Time.deltaTime) * m_rigidbody.velocity;
        //Debug.Log(m_rigidbody.velocity);
        tmp = m_rigidbody.velocity;
        tmp = Quaternion.Euler(0, 90, 0) * tmp;
        transform.rotation = Quaternion.LookRotation(tmp);
        //Debug.Log(transform.rotation);
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
    public void End()
    {
        m_rigidbody.velocity *= 0f;
        ended = true;
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

    void _useItem()
    {
        if (player.GetButton("Use Item"))
            slot.use();
        // if (inputMethod == 0 && Input.GetKey(KeyCode.E))
        //     slot.use();
        // else if (inputMethod == 1 && Input.GetKey(KeyCode.RightControl))
        //     slot.use();
    }

    // Update is called once per frame
    void Update()
    {
        if(!beginned || ended)
        {
            Vector3 tmp = m_rigidbody.velocity;
            tmp.x = tmp.y = 0f;
            m_rigidbody.velocity = tmp;
            return;
        }
        /*
        Vector3 vec;
        if (inputMethod == 0)
        {
            vec = _genMoveVecWASD();
        }
        else if (inputMethod == 1)
        {
            vec = _genMoveVecDir();
        }
        else {
            vec = _genMoveVecController();
        }
        _addForce(vec);
        */
        _useItem();
        _moveController();
        // if (inputMethod == 0)
        //     _moveWASD();
        // else if (inputMethod == 1)
        //     _moveDir();
    }
}
