using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ItemBar : MonoBehaviour
{
    public ItemsController itemsController;
    public Image[] images;
    public Item[] itemList = new Item[3];
    public RuneIcon[] runeIcon = new RuneIcon[3];

    private void Awake()
    {
        if (itemsController == null)
        {
            itemsController = GameObject.FindGameObjectWithTag("Player").transform.GetComponent<ItemsController>();
        }
        //images = this.GetComponentsInChildren<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!itemList.SequenceEqual(itemsController.GetItemArray()))
        {
            //Debug.Log("no! updating...");
            itemsController.GetItemArray().CopyTo(itemList, 0);
            if (itemList.SequenceEqual(itemsController.GetItemArray()))
            {
                runeIcon[0].currentItem = itemList[0];
                runeIcon[1].currentItem = itemList[1];
                runeIcon[2].currentItem = itemList[2];
                UpdateImages();
            }
        }
    }

    private void UpdateImages()
    {
        // Get all the child slots in the GridLayoutGroup
        //Transform[] slots = this.GetComponentsInChildren<Transform>();

        for (int i = 0; i < itemList.Length; i++)
        {
            if (i < itemList.Length && itemList[i] != null)
            {
                // Update the slot with the item's image
                Image slotImage = images[i];
                if (slotImage != null)
                {
                    // Assuming itemImage is the name of the sprite in your Resources folder
                    Sprite itemSprite = itemList[i].ItemImage;
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

    private Image FindSlotImage(Transform slot)
    {
        foreach (Transform child in slot)
        {
            // Look for an Image component in the child objects
            Image childImage = child.GetComponent<Image>();
            if (childImage != null)
            {
                return childImage; // Return the first Image found in the child
            }
        }
        return null;
    }

    private bool AreArraysEqual(Item[] array1, Item[] array2)
    {
        if (array1.Length != array2.Length) return false;

        for (int i = 0; i < array1.Length; i++)
        {
            if (array1[i] != array2[i]) return false;
        }

        return true;
    }
}