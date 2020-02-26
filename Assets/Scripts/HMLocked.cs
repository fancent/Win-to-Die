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
    }
    public override void updAffect()
    {
        gameObject.transform.position = player.gameObject.transform.position;
    }
    public override void endAffect()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
