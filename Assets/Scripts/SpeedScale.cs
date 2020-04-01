using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using VehicleBehaviour;
public class SpeedScale : MonoBehaviour
{
    public WheelVehicle v;
    public Image i;
    public int Side;//Left = -1 right = 1
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        i.transform.rotation = Quaternion.Euler(0, 0, (v.Speed / 270)*Side * 90); 
    }
}
