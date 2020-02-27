using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VehicleBehaviour;

public class Item_HomingMissile : PowerUp
{
    // Start is called before the first frame update
    public HomingMissile prefab;//Drag
    Item_HomingMissile()
    {
        aim.aimMethod = 1;
        Debug.Log("Called init");
    }
    void Start()
    {

    }
    override public void activate(WheelVehicle user)
    {
        HomingMissile clone = Instantiate(prefab, user.gameObject.transform.position + Vector3.up, user.gameObject.transform.rotation);
        clone.ignite(user, aim.rot);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
