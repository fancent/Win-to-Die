using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicRocket : MonoBehaviour
{
    // Start is called before the first frame update
    public Rigidbody m_rigidbody;
    public Vector3 constspeed;
    public bool ignited;
    public Cart owner;
    public SpeedBoost sbPrefab;//drag
    public float boost_Duration;//Modify outside

    private void Awake()
    {
        ignited = false;
        Debug.Log("Called");
    }
    void Start()
    {
        
        
    }
    public void ignite(Cart user, Vector3 rotation)
    {
        m_rigidbody = gameObject.GetComponent<Rigidbody>();
        owner = user;
        m_rigidbody.velocity = constspeed = Quaternion.Euler(rotation) * user.m_rigidbody.velocity * 1.2f + user.m_rigidbody.velocity;
        ignited = true;
        Debug.Log("ign");
        Debug.Log(ignited);
    }
    void attach(Cart target) {
        Debug.Log("Attached");
        SpeedBoost clone= Instantiate(sbPrefab);
        clone.Initialize(boost_Duration, target);
        clone.Begin();
    }
    void OnTriggerEnter(Collider c)
    {
        Debug.Log("Trig");
        Debug.Log(ignited);
        if (!ignited)
            return;
        Cart hit = c.gameObject.GetComponent<Cart>();
        if(hit) {
            if (hit == owner)
                return;
            else {
                attach(hit);    
            }
        }
        Destroy(gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        Debug.Log(m_rigidbody.velocity);
        m_rigidbody.velocity = constspeed;
    }
}
