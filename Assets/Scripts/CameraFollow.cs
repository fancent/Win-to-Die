using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VehicleBehaviour;

public class CameraFollow : MonoBehaviour
{
    public WheelVehicle self;
    public Transform camLocation;
    public Transform lookLocation;
    public float Speed = 10.0f;
    public float currSpeed = 0f;
    public ParticleSystem speedlines;
 
    void FixedUpdate() {
        // camLocation.position -= new Vector3(-0.005f,-0.005f,0f);
        if (currSpeed < self._rb.velocity.magnitude)
        {
            camLocation.localPosition = new Vector3(0, camLocation.localPosition.y + self._rb.velocity.magnitude * 0.00005f, camLocation.localPosition.z - self._rb.velocity.magnitude * 0.0001f);
        }
        else 
        {
            camLocation.localPosition = new Vector3(0, Mathf.Max(3, camLocation.localPosition.y - self._rb.velocity.magnitude * 0.00005f), Mathf.Max(-5, camLocation.localPosition.z + self._rb.velocity.magnitude * 0.0001f));
        }
        
        transform.position = Vector3.Lerp(transform.position, camLocation.position, Speed * Time.deltaTime);
        transform.LookAt(lookLocation.position);
        currSpeed = self._rb.velocity.magnitude;

        var em = speedlines.emission;
        em.rateOverTime = transform.InverseTransformDirection(self._rb.velocity).magnitude * 0.075f;
    }
     
}
