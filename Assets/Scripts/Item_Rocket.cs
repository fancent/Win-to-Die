using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Rocket : PowerUp
{
    // Start is called before the first frame update
    public Rocket prefab;//Drag
    void Start()
    {
        
        
    }
    override public void activate(Cart user)
    {
        Rocket clone = Instantiate(prefab, user.gameObject.transform.position + Vector3.up, user.gameObject.transform.rotation);
        clone.ignite(user);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
