using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VehicleBehaviour;

abstract public class PowerUp : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject imgAsset;
    public AimData aim = new AimData();
    void Awake()
    {
        
    }
    void Start()
    {
    }
    public abstract void activate(WheelVehicle user);
    // Update is called once per frame
    void Update()
    {
        
    }
}
