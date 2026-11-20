using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossHealthBar : MonoBehaviour
{
    public Slider healthSlider;
    public BossHitController boss;
    public TextMeshProUGUI bossName;

    //variables
    //private int currentHealingBarValue = 0;

    // Update is called once per frame
    private void Awake()
    {
        bossName = this.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Start() {
        this.gameObject.SetActive(false);
    }

    public void Initialize()
    {
        boss = GameObject.FindGameObjectWithTag("Boss").transform.GetComponent<BossHitController>();
        bossName.text = boss.BossName;
    }

    private void Update()
    {
        if (healthSlider.value != boss.CurrentHealth)
        {
            healthSlider.value = boss.CurrentHealth;
        }

        if (healthSlider.maxValue != boss.MaxHealth)
        {
            healthSlider.maxValue = boss.MaxHealth;
        }
    }
}