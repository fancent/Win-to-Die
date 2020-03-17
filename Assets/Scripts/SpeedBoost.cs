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
        player.Cart_speedup(inst_boost);
        original_speed = player._rb.velocity;
        player._rb.velocity *= 1.25f;
    }
    public override void updAffect()
    {
        gameObject.transform.position = player.gameObject.transform.position;
    }
    public override void endAffect()
    {
        player._rb.velocity = original_speed;
    }
}
