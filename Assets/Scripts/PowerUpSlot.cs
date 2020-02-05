using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpSlot: MonoBehaviour
{
    PowerUp[] slot;
    Cart player;
    int size;

    public void Awake()
    {
        size = 1;
    }
    public void Start()
    {
        
    }
    public void Initialize(int siz, Cart pl)
    {
        size = siz;
        slot = new PowerUp[size];
        player = pl;
    }
    public void load(PowerUp item)
    {
        slot[0] = item;
    }
    public void use()
    {
        if (slot[0] == null)
        {
            return;
        
        }
        slot[0].activate(player);
        slot[0] = null;
    }
    public void Update()
    {
        
    }
}
