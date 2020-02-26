using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VehicleBehaviour;

public class WallMechanics : MonoBehaviour
{
    public float pushForce = 10;
    public bool wallside;//false = left, true = right
    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter(Collision other)
    {
        // GameSystem g_sys = GameObject.Find("GameSystem").GetComponent<GameSystem>();
        // g_sys.speedUp(other.gameObject.tag, 1.05f);
        // if (this.gameObject.tag == "Right Wall")
        // {
        //     Debug.Log(other);

        //     other.gameObject.GetComponent<Rigidbody>().AddForce((Vector3.left) * pushForce, ForceMode.Impulse);
        // }s
        // else if (this.gameObject.tag == "Left Wall")
        // {
        //     other.gameObject.GetComponent<Rigidbody>().AddForce((Vector3.right) * pushForce, ForceMode.Impulse);
        // }
        // Debug.Log("Hit wall");
        // Debug.Log(wallside);
        
        WheelVehicle w = other.gameObject.GetComponent<WheelVehicle>();
        // Debug.Log(w.gameObject);
        Vector3 norm = -other.GetContact(0).normal.normalized;
        Vector3 indir =   (other.GetContact(0).point) - (w.transform.position);
        Vector3 dir =(indir - 2*Vector3.Dot(indir, norm)*norm);
        dir = (new Vector3(dir.x, 0, dir.z)).normalized;
        Debug.Log(norm);
        Debug.Log(other.GetContact(0).point);
        Debug.Log(w.transform.position);
        Debug.Log(indir);
        Debug.Log(dir);
        if(w == null)
            return;
        w.Cart_speedup(dir, 2f);
    }
}
