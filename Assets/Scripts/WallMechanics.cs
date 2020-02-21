using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallMechanics : MonoBehaviour
{
    public float pushForce = 10;
    public Rigidbody rb;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter(Collision other)
    {
        GameSystem g_sys = GameObject.Find("GameSystem").GetComponent<GameSystem>();
        g_sys.speedUp(other.gameObject.tag, 1.05f);
        if (this.gameObject.tag == "Right Wall")
        {
            Debug.Log("pushLeft");
            other.gameObject.GetComponent<Rigidbody>().AddForce((Vector3.left) * pushForce, ForceMode.Impulse);
        }
        else if (this.gameObject.tag == "Left Wall")
        {
            Debug.Log("pushRight");
            other.gameObject.GetComponent<Rigidbody>().AddForce((Vector3.right) * pushForce, ForceMode.Impulse);
        }
    }
}
