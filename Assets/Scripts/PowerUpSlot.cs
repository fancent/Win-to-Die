using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VehicleBehaviour;

public class PowerUpSlot: MonoBehaviour
{
    PowerUp[] slot;
    WheelVehicle player;
    int size;
    Transform[] rt;
    GameObject[] imgs;
    public GameObject canvas;
    public AudioSource source;
    public AudioClip fire;

    public void Awake()
    {
        size = 1;
        
    }
    public void Start()
    {
        
    }
    public void Initialize(int siz, WheelVehicle pl)
    {
        canvas = GameObject.Find("MainUICanvas");
        size = siz;
        rt = new RectTransform[size];
        slot = new PowerUp[size];
        imgs = new GameObject[size];
        player = pl;
        rt[0] = pl.slotAnchor.transform;
    }
    public bool load(PowerUp item)
    {
        if (slot[0] != null)
            return false;
        slot[0] = item;
        imgs[0] = Instantiate(item.imgAsset, rt[0].position, rt[0].rotation);
        imgs[0].transform.parent = canvas.transform;
        return true;
    }
    public int getAimMethod()
    {
        if (slot[0] == null)
            return -1;
        return slot[0].aim.aimMethod;
    }

    public void setAim_rot(Vector3 rot)
    {
        if (slot[0] != null)
        {
            Debug.Log(slot[0].aim);
            slot[0].aim.rot = rot;
        }
    }
    public void use()
    {
        if (slot[0] == null)
        {
            return;
        
        }
        source.PlayOneShot(fire);
        slot[0].activate(player);
        slot[0] = null;
        Debug.Log(imgs[0]);
        Destroy(imgs[0]);

        imgs[0] = null;
    }
    public void Update()
    {
        
    }
}
