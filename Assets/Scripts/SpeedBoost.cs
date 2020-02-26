using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VehicleBehaviour;

public class SpeedBoost : Effect
{
    float original_speed;
    public override void bgnAffect()
    {
        player.Cart_speedup();
        // original_speed = player.Speed;
        // player.Speed *= 1.25f;
    }
    public override void updAffect()
    {
        gameObject.transform.position = player.gameObject.transform.position;
    }
    public override void endAffect()
    {
        // player.Speed = original_speed;
    }
}
