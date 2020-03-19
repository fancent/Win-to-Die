using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VehicleBehaviour;

public class HomingMissile : MonoBehaviour
{
    public Rigidbody _rb;
    public bool ignited;
    public WheelVehicle owner, target;
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
        WheelVehicle[] carts = FindObjectsOfType<WheelVehicle>();
        WheelVehicle tgt = null;
        float tmp, bd = 10000000f;
        foreach (WheelVehicle c in carts)
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

    public void ignite(WheelVehicle user, Vector3 rotation)
    {
        Debug.Log("HM ignited~~~~");
        _rb = gameObject.GetComponent<Rigidbody>();
        owner = user;
        _rb.velocity = constspeed =  //TODO: change this to "x" where z is normal
             (user._rb.transform.up *7f -user._rb.transform.right) * 2f + user._rb.velocity*0.5f + user._rb.velocity;
        findTarget(rotation);
        HMLocked clone = Instantiate(hmlPrefeb);
        clone.Initialize(hit_delay, target);
        target.stat.LoadEffect(clone);
        ignited = true;
        flytimer = 0;
        Debug.Log("ign");
        Debug.Log(ignited);
    }

    void attach(WheelVehicle target)
    {
        Debug.Log("Attached");
        SpeedBoost clone = Instantiate(sbPrefab);
        clone.Initialize(boost_Duration, target);
        target.stat.LoadEffect(clone);
    }

    void OnTriggerEnter(Collider c)
    {
        Debug.Log("Trig");
        Debug.Log(ignited);
        Debug.Log(c.gameObject);
        if (!ignited)
            return;
        WheelVehicle hit = c.gameObject.GetComponent<WheelVehicle>();
        if (hit)
        {
            if (hit == owner)
                return;
            else
            {
                attach(hit);
            }
        }
        else
        {
            return;
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
                _rb.velocity = constspeed;
            else
            {
                float remtime = hit_delay - flytimer;
                _rb.velocity = target._rb.velocity + (target.transform.position + Vector3.up - _rb.transform.position) / remtime;
            }
            gameObject.transform.rotation = Quaternion.LookRotation(_rb.velocity);
            Debug.Log(flytimer);
        }

    }
}
