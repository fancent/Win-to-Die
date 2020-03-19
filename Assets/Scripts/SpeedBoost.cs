using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VehicleBehaviour;

public class SpeedBoost : Effect
{
    Vector3 original_speed;
    public float inst_boost=1.15f, speed_increase=1.25f;
    public override void bgnAffect()
    {
        player.Cart_Boost(inst_boost);        
    }
    public override void updAffect()
    {
        player.Cart_speedup(speed_increase);
        gameObject.transform.position = player.gameObject.transform.position;
    }
    public override void endAffect()
    {
    }
}
