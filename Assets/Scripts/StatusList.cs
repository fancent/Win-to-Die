using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VehicleBehaviour;

public class StatusList : MonoBehaviour
{
    // Start is called before the first frame update
    Effect[] cur_status;
    WheelVehicle player;
    void Start() 
    {
        cur_status = new Effect[10];
        for (int i = 0; i < 10; i++)
            cur_status[i] = null;
    }

    public void LoadEffect(Effect e)
    {
        //e should been initialized already.
        for (int i = 0; i < 10; i++)
            if (cur_status[i] == null) {
                cur_status[i] = e;
                Debug.Log(e);
                e.Begin();
                return;
            }
                
    }
    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < 10; i++)
            if (cur_status[i] != null)
                if (cur_status[i].Is_End()) {
                    Object.Destroy(cur_status[i]);
                    cur_status[i] = null;
                }
        int j = 0;
        for (int i = 0; i < 10; i++)
            if(cur_status[i]!= null)
            {
                if (j != i)
                {
                    cur_status[j] = cur_status[i];
                    cur_status[i] = null;
                }
                j++;
            }
    }
}
