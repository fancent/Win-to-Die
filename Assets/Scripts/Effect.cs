﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VehicleBehaviour;

public class Effect : MonoBehaviour
{
    // Start is called before the first frame update
    public float duration;
    protected WheelVehicle player;
    public float lifespan;//0: not initialized, 1: not beginned, 2: working, 3: ended, -1: errored
    //UI stuff missing for now

    public void Awake()
    {
        lifespan = 0;
    }
    void Start()
    {
        
    }
    public virtual void bgnAffect() { }
    public virtual void updAffect() { }
    public virtual void endAffect() { }
    public virtual void fxupdAffect() { }

    public void Initialize(float dur, WheelVehicle pl)
    {
        duration = dur;
        player = pl;
        lifespan = 1;
    }

    public bool Is_End()
    {
        return lifespan == 3;
    }
    // Update is called once per frame

    public void Begin()
    {
        if (lifespan != 1)
        {
            Debug.LogError("Initializing an initialized Effect. This Effect Object will be set to [errored].");
            lifespan = -1;
            return;
        }
        bgnAffect();
        lifespan = 2;
    }
    public void Endlife(bool endeff)
    {
        if(endeff)
            endAffect();
        lifespan = 3;
        Debug.Log("End of my life!");
    }

    public void Update()
    {
        if (lifespan == 2) {
            duration -= Time.deltaTime;
            updAffect();
            if (duration < 0)
                Endlife(true);
        }
    }

    private void FixedUpdate()
    {
        if (lifespan == 2)
        {
            fxupdAffect();
        }
    }

}
