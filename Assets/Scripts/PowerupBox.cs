using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PowerupBox : MonoBehaviour
{
    // Start is called before the first frame update

    public PowerUp item;
    void Start()
    {
    }

    void OnTriggerEnter(Collider c)
    {
        Cart cart = c.gameObject.GetComponent<Cart>();
        PowerUpSlot slot = cart.GetComponent<PowerUpSlot>();
        slot.load(item);
        Destroy(gameObject);
    }


    // Update is called once per frame
    void Update()
    {
    }
}
