using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform camLocation;
    public Transform lookLocation;
    public float Speed = 10.0f;
 
    void FixedUpdate() {
        transform.position = Vector3.Lerp(transform.position, camLocation.position, Speed * Time.deltaTime);
        transform.LookAt(lookLocation.position);
    }
     
}
