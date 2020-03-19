using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using VehicleBehaviour;

public class SpeedLineAdjust : MonoBehaviour
{
    // Start is called before the first frame update
    public ParticleSystem p;
    public WheelVehicle v;
    ParticleSystem.EmissionModule e;
    void Start()
    {
        e = p.emission;
    }

    // Update is called once per frame
    void Update()
    {
        e.rateOverTime = Mathf.Max(0f, v.Speed-50)/5f;
    }
}
