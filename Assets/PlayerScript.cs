using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public float speed;
    public float rotation;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>(); 
    }

    void FixedUpdate()
    {
        float moveVertical = Input.GetAxis("Vertical");
        float turnYaw = Input.GetAxis("Horizontal");

        Vector3 movement = new Vector3(0.0f, 0.0f, moveVertical);
        Vector3 turn = new Vector3(0.0f, turnYaw, 0.0f);
        rb.AddRelativeForce(movement * speed);
        transform.Rotate(turn * rotation);
    }
}
