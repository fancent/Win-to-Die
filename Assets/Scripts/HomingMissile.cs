using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMissile : MonoBehaviour
{
    // Start is called before the first frame update
    public Rigidbody m_rigidbody;
    public bool ignited;
    public Cart owner, target;
    public SpeedBoost sbPrefab;//drag
    public Vector3 constspeed;
    public HMLocked hmlPrefeb;//drag
    public float boost_Duration;//Modify outside
    public float hit_delay;//Modify outside
    float flytimer=0f;
    private void Awake()
    {
        ignited = false;
    }
    void Start()
    {


    }
    void findTarget(Vector3 r)
    {
        Cart[] carts = FindObjectsOfType<Cart>();
        Cart tgt = null;
        float tmp, bd = 10000000f;
        foreach (Cart c in carts)
        {
            if (c == owner)
                continue;
            tmp = owner.transform.position.y - c.transform.position.y;
            if (90f < r.y && r.y <= 270f && tmp > 0)
                continue;
            if (!(90f < r.y && r.y <= 270f) && tmp < 0)
                continue;
            tmp = Mathf.Abs(tmp);
            if (tmp < bd)
            {
                bd = tmp;
                tgt = c;
            }  
        }
        if (tgt != null)
            target = tgt;
        else
        {
            r.y = 180 - r.y;
            findTarget(r);
        }
           
    }
    public void ignite(Cart user, Vector3 rotation)
    {
        Debug.Log("HM ignited~~~~");
        m_rigidbody = gameObject.GetComponent<Rigidbody>();
        owner = user;
        m_rigidbody.velocity = constspeed = Quaternion.Euler(new Vector3(-80, 15, 0)) //TODO: change this to "x" where z is normal
             * user.m_rigidbody.velocity*0.5f + user.m_rigidbody.velocity;
        findTarget(rotation);
        ignited = true;
        flytimer = 0;
        Debug.Log("ign");
        Debug.Log(ignited);
    }
    void attach(Cart target)
    {
        Debug.Log("Attached");
        SpeedBoost clone = Instantiate(sbPrefab);
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
        if (hit)
        {
            if (hit == owner)
                return;
            else
            {
                attach(hit);
            }
        }
        Destroy(gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        if (ignited)
        {
            flytimer += Time.deltaTime;
            if (flytimer <= 1f)
                m_rigidbody.velocity = constspeed;
            else
            {
                float remtime = hit_delay - flytimer;
                m_rigidbody.velocity = target.m_rigidbody.velocity + (target.transform.position - m_rigidbody.transform.position) / remtime;
            }
            Debug.Log(flytimer);
        }

    }
}
