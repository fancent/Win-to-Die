using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VehicleBehaviour;

public class HeightBar : MonoBehaviour
{
    public WheelVehicle cart;
    public Slider slider;
    public Image fill;
    public Color start;
    public Color end;
    private float minHeight=147f;
    private float maxHeight=988f;

    // Start is called before the first frame update
    void Start()
    {
        slider.value = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        
        float currentHeight = cart.transform.position.y;
        float fillAmount = (currentHeight-minHeight)/(maxHeight-minHeight);
        //Debug.Log(fillAmount);
        slider.value = fillAmount;
        fill.color = Color.Lerp(end, start, fillAmount);
    }
}
