using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopAttribute : MonoBehaviour
{
    public string attribute;
    private LevelController levelController;
    private EquipmentController equipmentController;
    private ShopManager shopManager;
    public TextMeshProUGUI buttonText;
    public TextMeshProUGUI text;
    public string attributeDescription;

    // private void Awake()
    // {
    // }

    // private void Start()
    // {

    // }

    public void Initialize(ShopManager newShopManager)
    {
        text.text = attribute;
        shopManager = newShopManager;
        levelController = shopManager.player.GetComponent<LevelController>();
        equipmentController = shopManager.player.GetComponent<EquipmentController>();

        UpdateAttributeCostAndDesc();
    }

    public void UpdateAttributeCostAndDesc()
    {
        buttonText.text = levelController.AttributesCost + " Gold";

        switch (attribute)
        {
            case "Strength":
                attributeDescription = levelController.StrDesc;
                break;
            case "Constitution":
                attributeDescription = levelController.ConDesc;
                break;
            case "Vigor":
                attributeDescription = levelController.VigDesc;
                break;
            case "Agility":
                attributeDescription = levelController.AgiDesc;
                break;
            case "Luck":
                attributeDescription = levelController.LukDesc;
                break;
        }
    }

    public void ShowInfo()
    {
        shopManager.toolTip.SetActive(true);
        shopManager.toolTipNameText.text = attribute;
        shopManager.toolTipDescText.text = attributeDescription;
    }

    public void HideInfo()
    {
        shopManager.toolTip.SetActive(false);
    }

    public void BuyItem()
    {
        if (equipmentController.Gold >= levelController.AttributesCost)
        {
            equipmentController.SetGold(equipmentController.Gold - levelController.AttributesCost);
            
            switch (attribute)
            {
                case "Strength":
                    levelController.IncreaseStrength(1);
                    break;
                case "Constitution":
                    levelController.IncreaseConstitution(1);
                    break;
                case "Vigor":
                    levelController.IncreaseVigor(1);
                    break;
                case "Agility":
                    levelController.IncreaseAgility(1);
                    break;
                case "Luck":
                    levelController.IncreaseLuck(1);
                    break;
            }

            shopManager.UpdateAllAttributesCost();
        }
    }
}
