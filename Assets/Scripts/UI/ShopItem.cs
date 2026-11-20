using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using TMPro;

public class ShopItem : MonoBehaviour
{
    public Image itemImage;
    public Item item;
    public ShopManager shopManager;
    public TextMeshProUGUI text;
    public TextMeshProUGUI buttonText;

    public Item SItem => item;
    public void SetItem(Item newItem) => item = newItem;

    private void Awake()
    {
        shopManager = this.GetComponentInParent<ShopManager>();
        itemImage = this.GetComponentInChildren<Image>();
        text = this.GetComponentInChildren<TextMeshProUGUI>();

        GameObject button = transform.Find("Info/Button").gameObject;

        buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Start()
    {
        if (item != null)
        {
            text.text = item.ItemName;
            buttonText.text = item.Price + " Gold";
            itemImage.sprite = item.ItemImage;
        }
    }

    public void Initialize(ShopManager newShopManager)
    {
        shopManager = newShopManager;

        if (item != null)
        {
            text.text = item.ItemName + " cost: " + item.Price;
            itemImage.sprite = item.ItemImage;
        }
    }

    public void ShowInfo()
    {
        shopManager.toolTip.SetActive(true);
        shopManager.toolTipNameText.text = item.ItemName;
        if (item is Weapon weapon)
        {
            shopManager.toolTipDescText.text = weapon.WeaponType + "\n" + weapon.ItemStats + "\n\n" + weapon.ItemDescription;
        }
        else
        {
            shopManager.toolTipDescText.text = item.ItemStats + "\n\n" + item.ItemDescription;
        }
    }

    public void HideInfo()
    {
        shopManager.toolTip.SetActive(false);
    }

    public void BuyItem()
    {
        EquipmentController equipmentController = shopManager.player.GetComponent<EquipmentController>();
        if (equipmentController.Gold >= item.Price)
        {
            string address = "Assets/Prefabs/Pickables/Items/" + item.Type + "/" + item.GetFileName() + ".prefab";

            Addressables.LoadAssetAsync<GameObject>(address).Completed += (AsyncOperationHandle<GameObject> handle) =>
                {
                    if (handle.Status == AsyncOperationStatus.Succeeded)
                    {
                        Instantiate(handle.Result, shopManager.itemDropLocation.position, handle.Result.transform.rotation);

                        equipmentController.SetGold(equipmentController.Gold - item.Price);
                    }
                    else
                    {
                        Debug.LogError($"Failed to load prefab with address: {address}");
                    }
                };
        }
    }
}
