using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PoiseBar : MonoBehaviour 
{
    public Slider poiseSlider;
    public HitController player;

    // Update is called once per frame
    private void Awake() {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform.GetComponent<HitController>();
        }
    }

    private void Update()
    {
        if(poiseSlider.value != player.currentPoise){
            poiseSlider.value = player.currentPoise;
        }

        if(poiseSlider.maxValue != player.maxPoise){
            poiseSlider.maxValue = player.maxPoise;
        }
    }
}
