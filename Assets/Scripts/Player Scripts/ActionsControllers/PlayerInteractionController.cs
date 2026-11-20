using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerInteractionController : MonoBehaviour
{
    private AttackController attackController;
    private ConsumableController consumableController;
    private DefendController defendController;
    private EquipmentController equipmentController;
    private HitController hitController;
    private GameObject closestItem;  // The currently selected item
    private GameObject lastHighlightedItem; // To keep track of the previously highlighted item
    private ItemsController itemsController;
    public LayerMask interactionLayer;       // Layer mask for interaction

    //interaction box
    private GameObject interactionBox;
    private TextMeshProUGUI itemNameText;
    private TextMeshProUGUI itemDescText;

    //variables
    public float detectionRadius = 5f; // Radius for detecting items

    private float goldMultiplier = 1f;

    private int[] smallGoldRange = { 10, 20 };
    private int[] mediumGoldRange = { 25, 50 };
    private int[] largeGoldRange = { 50, 100 };

    public float GoldMultiplier => goldMultiplier;

    public void SetGoldMultiplier(float newGoldMultiplier) => goldMultiplier = newGoldMultiplier;

    private void Awake()
    {
        attackController = this.GetComponent<AttackController>();
        consumableController = this.GetComponent<ConsumableController>();
        defendController = this.GetComponent<DefendController>();
        equipmentController = this.GetComponent<EquipmentController>();
        hitController = GetComponent<HitController>();
        itemsController = this.GetComponent<ItemsController>();

        interactionBox = GameObject.Find("InteractionBox");

        GameObject itemNameTextObject = GameObject.Find("ItemNameText");
        GameObject itemDescTextObject = GameObject.Find("ItemDescriptionText");
        if (itemNameTextObject != null && itemDescTextObject != null)
        {
            itemNameText = itemNameTextObject.GetComponent<TextMeshProUGUI>();
            itemDescText = itemDescTextObject.GetComponent<TextMeshProUGUI>();
        }
        else
        {
            Debug.LogError("item desc and name text object not found.");
        }

        interactionBox.SetActive(false);
    }

    private void Update()
    {
        FindClosestItem();
        HandleInteraction();
    }

    private void FindClosestItem()
    {
        // Find all items within the detection radius
        //Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius, itemLayer);
        Vector3 pos2 = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);

        Collider[] hitColliders = Physics.OverlapCapsule(transform.position, pos2, detectionRadius, interactionLayer);
        float closestDistance = Mathf.Infinity;
        GameObject nearestItem = null;

        foreach (Collider hitCollider in hitColliders)
        {
            float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                nearestItem = hitCollider.gameObject;
                ChestController chestController = nearestItem.GetComponent<ChestController>();
                if (chestController != null && chestController.isOpen)
                {
                    nearestItem = null;
                }
            }
        }

        // Update the closest item
        closestItem = nearestItem;

        // Highlight the closest item and remove the outline from others
        if (closestItem != lastHighlightedItem)
        {
            // Remove the outline from the previously highlighted item
            if (lastHighlightedItem != null)
            {
                interactionBox.SetActive(false);
                RemoveOutline(lastHighlightedItem);
            }

            // Add the outline to the new closest item
            if (closestItem != null)
            {
                ItemPickup itemPickup = closestItem.GetComponent<ItemPickup>();

                if (itemPickup != null)
                {
                    if (itemPickup.item != null)
                    {
                        itemNameText.text = itemPickup.item.ItemName;
                        itemDescText.text = itemPickup.item.ItemStats;
                    }
                    else
                    {
                        itemNameText.text = closestItem.name ?? " ";
                        itemDescText.text = " ";
                    }
                }
                else
                {
                    itemNameText.text = closestItem.name ?? " ";
                    itemDescText.text = " ";
                }

                interactionBox.SetActive(true);
                AddOutline(closestItem);
            }

            lastHighlightedItem = closestItem;
        }
    }

    private void HandleInteraction()
    {
        if (Input.GetKeyDown(KeyCode.E) && closestItem != null && !attackController.isAttacking && !defendController.isDefending && Time.timeScale > 0)
        {
            // Perform the pickup action
            //Debug.Log($"Picked up: {closestItem.name}");

            interactionBox.SetActive(false);

            //string[] itemName = closestItem.name.Split('_');

            ItemPickup itemPickup = closestItem.GetComponent<ItemPickup>();

            if (itemPickup != null)
            {
                if (itemPickup.item.Type == "Rune")
                {
                    if (itemsController.AddItem(itemPickup.item))
                    {
                        Destroy(closestItem);
                    }
                    else
                    {
                        interactionBox.SetActive(true);
                    }
                }
                else if (itemPickup.item.Type == "Weapon" && itemPickup.item is Weapon weapon)
                {
                    equipmentController.ChangeWeapon(weapon);
                    Destroy(closestItem);
                }
                else if (itemPickup.item.Type == "Consumable")
                {
                    AddConsumable(itemPickup.item);
                    Destroy(closestItem);
                }
            }
            else
            {
                ChestController chestController = closestItem.GetComponent<ChestController>();
                chestController?.Open();

                ShopManager shopManager = closestItem.GetComponent<ShopManager>();
                if (shopManager != null)
                {
                    if (shopManager.shopActive)
                    {
                        shopManager.HideShop();
                    }
                    else
                    {
                        hitController.SetSpawnPoint(transform.position);
                        hitController.SetLightPosition(hitController.LightTransform.position);
                        shopManager.ShowShop();
                    }
                }

                PassageController passageController = closestItem.GetComponent<PassageController>();
                passageController?.LoadLevel();
            }

            // if (itemName[0] == "Chest")
            // {
            //     ChestController chestController = closestItem.GetComponent<ChestController>();
            //     if (chestController != null)
            //     {
            //         chestController.Open();
            //     }
            // }
            // else if (itemName[0] == "Item")
            // {
            //     ItemPickup itemPickup = closestItem.GetComponent<ItemPickup>();
            //     if (itemPickup != null)
            //     {
            //         if (itemsController.AddItem(itemPickup.item))
            //         {
            //             Destroy(closestItem);
            //         }

            //     }
            // }
            // else
            // {
            //     if (itemName[0] == "Potion")
            //     {
            //         consumableController.SetPotions(consumableController.GetPotions() + 1);
            //     }
            //     else if (itemName[0] == "Arrow")
            //     {
            //         equipmentController.SetArrows(equipmentController.GetArrows() + 5);
            //     }
            //     else
            //     {
            //         ItemPickup itemPickup = closestItem.GetComponent<ItemPickup>();
            //         if (itemPickup != null)
            //         {
            //             equipmentController.ChangeWeapon(itemPickup.weapon);
            //         }
            //         else
            //         {
            //             Debug.LogError("Item pickup is null");
            //         }

            //     }
            //     Destroy(closestItem);
            // }
        }
    }

    private void AddOutline(GameObject item)
    {
        Outline outline = item.GetComponent<Outline>();
        if (outline != null)
        {
            outline.enabled = true;
        }
    }

    private void RemoveOutline(GameObject item)
    {
        Outline outline = item.GetComponent<Outline>();
        if (outline != null)
        {
            outline.enabled = false;
        }
    }

    //this should belong to consumable controller
    void AddConsumable(Item consumable)
    {
        switch (consumable.ItemId)
        {
            case "01":
                consumableController.SetPotions(consumableController.Potions + 1);
                break;
            case "02":
                equipmentController.SetArrows(equipmentController.Arrows + 5);
                break;
            case "03":
                equipmentController.SetArrows(equipmentController.Arrows + 10);
                break;
            case "04": //small gold
                equipmentController.SetGold(equipmentController.Gold + (int)(Random.Range(smallGoldRange[0], smallGoldRange[1] + 1) * goldMultiplier));
                break;
            case "05": //medium gold
                equipmentController.SetGold(equipmentController.Gold + (int)(Random.Range(mediumGoldRange[0], mediumGoldRange[1] + 1) * goldMultiplier));
                break;
            case "06": //large gold
                equipmentController.SetGold(equipmentController.Gold + (int)(Random.Range(largeGoldRange[0], largeGoldRange[1] + 1) * goldMultiplier));
                break;
        }
    }
}
