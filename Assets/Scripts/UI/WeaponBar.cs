using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class WeaponBar : MonoBehaviour
{
    public EquipmentController equipmentController;
    public Image[] images;
    public Weapon[] weaponList = new Weapon[2];
    public WeaponIcon[] weaponIcon = new WeaponIcon[2];

    private void Awake()
    {
        if (equipmentController == null)
        {
            equipmentController = GameObject.FindGameObjectWithTag("Player").transform.GetComponent<EquipmentController>();
        }
    }

    private void Update()
    {
        if (!weaponList.SequenceEqual(equipmentController.EquippedWeapons))
        {
            equipmentController.EquippedWeapons.CopyTo(weaponList, 0);
            if (weaponList.SequenceEqual(equipmentController.EquippedWeapons))
            {
                weaponIcon[0].currentWeapon = weaponList[0];
                weaponIcon[1].currentWeapon = weaponList[1];
                UpdateImages();
            }
        }
    }

    private void UpdateImages()
    {
        for (int i = 0; i < weaponList.Length; i++)
        {
            if (i < weaponList.Length && weaponList[i] != null)
            {
                // Update the slot with the item's image
                Image slotImage = images[i];
                if (slotImage != null)
                {
                    // Assuming itemImage is the name of the sprite in your Resources folder
                    Sprite itemSprite = weaponList[i].ItemImage;
                    if (itemSprite != null)
                    {
                        slotImage.sprite = itemSprite;
                        slotImage.enabled = true;
                    }
                }
            }
            else
            {
                // Clear the slot if no item exists
                Image slotImage = images[i];
                if (slotImage != null)
                {
                    slotImage.sprite = null;
                    slotImage.enabled = false;
                }
            }
        }
    }
}