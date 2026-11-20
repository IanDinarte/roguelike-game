using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using TMPro;

public class EnemyLootController : MonoBehaviour
{
    public Transform dropLocation;
    public List<Item> lootList = new List<Item>();

    private Item GetLoot()
    {
        int randomNumber = Random.Range(1, 101);
        List<Item> possibleItems = new List<Item>();
        foreach (Item item in lootList)
        {
            if (randomNumber <= item.DropChance)
            {
                possibleItems.Add(item);
            }
        }
        if (possibleItems.Count > 0)
        {
            //Debug.Log("Random Number:" + randomNumber);
            return possibleItems.OrderBy(item => item.DropChance).First();
        }
        return null;
    }

    public void DropLoot()
    {
        Item item = GetLoot();

        string address = "Assets/Prefabs/Pickables/Items/" + item.Type + "/" + item.GetFileName() + ".prefab";

            Addressables.LoadAssetAsync<GameObject>(address).Completed += (AsyncOperationHandle<GameObject> handle) =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    Instantiate(handle.Result, dropLocation.position, handle.Result.transform.rotation);
                }
                else
                {
                    Debug.LogError($"Failed to load prefab with address: {address}");
                }
            };
    }
}
