using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Rocket : PowerUp
{
    // Start is called before the first frame update
    public Rocket prefab;//Drag
    Item_Rocket()
    {
        aim.aimMethod = 1;
        Debug.Log("Called init");
    }
    void Start()
    {
        
    }
    override public void activate(Cart user)
    {
        Rocket clone = Instantiate(prefab, user.gameObject.transform.position + Vector3.up, user.gameObject.transform.rotation);
        clone.ignite(user, aim.rot);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
