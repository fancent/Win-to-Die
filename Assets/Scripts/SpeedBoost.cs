using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBoost : Effect
{
    float original_speed;
    public override void bgnAffect()
    {
        player.Cart_speedup(1.15f);
        original_speed = player.speed;
        player.speed *= 1.25f;
    }
    public override void updAffect()
    {
        gameObject.transform.position = player.gameObject.transform.position;
    }
    public override void endAffect()
    {
        player.speed = original_speed;
    }
}
