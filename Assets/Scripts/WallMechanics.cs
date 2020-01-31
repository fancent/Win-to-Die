using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallMechanics : MonoBehaviour
{
    public float pushForce = 10;
    public Rigidbody rb;
 
    // Use this for initialization
    void Start() {
        rb = GetComponent<Rigidbody>();
    }
 
    // Update is called once per frame
    void Update() {
 
    }
 
    void OnCollisionEnter(Collision other) {
        if(other.gameObject.tag == "Right Wall") {
            Debug.Log("pushLeft");
            rb.AddForce(Vector3.left * pushForce, ForceMode.Impulse);
        }
        else if(other.gameObject.tag == "Left Wall") {
            Debug.Log("pushRight");
            rb.AddForce(Vector3.right * pushForce, ForceMode.Impulse);
        }
    }
}
