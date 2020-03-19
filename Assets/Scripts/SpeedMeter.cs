using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using VehicleBehaviour;
public class SpeedMeter : MonoBehaviour
{
    public WheelVehicle v;
    public Text t;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        t.text = $"{(int)(v.Speed/1.8)} km/h";
    }
}
