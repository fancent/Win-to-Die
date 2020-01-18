using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Cart : MonoBehaviour
{

    public int inputMethod;
    // 0 for wasd, 1 for Dir, otherwise(use 2) for Controller
    public Rigidbody rigidbody;
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = gameObject.GetComponent<Rigidbody>();
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
    
    Vector3 _genMoveVecController()
    {
        //TODO 
        throw new NotImplementedException();
    }

    void _addForce(Vector3 vec)
    {
         rigidbody.AddForce(10*vec);
    }



    // Update is called once per frame
    void Update()
    {
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
    }
}
