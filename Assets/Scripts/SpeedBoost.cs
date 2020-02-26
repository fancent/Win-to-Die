﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBoost : Effect
{
    float original_speed;
    public float inst_boost=1.15f, speed_increase=1.25f;
    public override void bgnAffect()
    {
        player.Cart_speedup(inst_boost);
        original_speed = player.speed;
        player.speed *= speed_increase;
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
