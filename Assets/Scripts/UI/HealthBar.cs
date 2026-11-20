using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthSlider;
    public Slider healingSlider;
    public HitController player;
    public ConsumableController consumable;

    //variables
    //private int currentHealingBarValue = 0;

    // Update is called once per frame
    private void Awake() {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform.GetComponent<HitController>();
        }
        if (consumable == null)
        {
            consumable = GameObject.FindGameObjectWithTag("Player").transform.GetComponent<ConsumableController>();
        }
    }

    private void Update()
    {
        if(healthSlider.value != player.currentHealth){
            healthSlider.value = player.currentHealth;
        }

        if(healthSlider.maxValue != player.maxHealth){
            healthSlider.maxValue = player.maxHealth;
        }

        if(healingSlider.maxValue != player.maxHealth){
            healingSlider.maxValue = player.maxHealth;
        }

        if(healingSlider.value != player.currentHealing){
            healingSlider.value = player.currentHealing;
        }

        if(!consumable.isDrinking){
            healingSlider.value = 0;
        }
    }
}