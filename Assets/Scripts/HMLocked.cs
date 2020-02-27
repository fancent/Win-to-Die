using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HMLocked : Effect
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public override void bgnAffect()
    {
        Debug.Log("HMLed");
        return;
    }
    public override void updAffect()
    {
        // Debug.Log(player.gameObject.transform.position);
        gameObject.transform.position = player.gameObject.transform.position + Vector3.up;
    }
    public override void endAffect()
    {
        return;
    }
}
