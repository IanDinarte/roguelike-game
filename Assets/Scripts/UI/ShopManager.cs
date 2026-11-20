using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public GameObject shop;
    public GameObject toolTip;
    public TextMeshProUGUI toolTipNameText;
    public TextMeshProUGUI toolTipDescText;
    public GameObject player;
    public AttackController playerAttackController;
    public Transform itemDropLocation;

    public bool sellAttributes = true;

    public ShopAttribute[] attributesList;
    public Weapon[] weapons;
    public Item[] consumables;
    public Item[] runes;

    public bool shopActive = false;
    private float maxShopDistance = 2f;
    private bool columnLoaded;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerAttackController = player.GetComponent<AttackController>();
        shop = GameObject.Find("Shop");
        toolTip = GameObject.FindGameObjectWithTag("ToolTip");

        GameObject toolTipNameTextObject = GameObject.Find("ToolTipNameText");
        GameObject toolTipDescTextObject = GameObject.Find("ToolTipDescriptionText");

        if (toolTipNameTextObject != null && toolTipDescTextObject != null)
        {
            toolTipNameText = toolTipNameTextObject.GetComponent<TextMeshProUGUI>();
            toolTipDescText = toolTipDescTextObject.GetComponent<TextMeshProUGUI>();
        }
        else
        {
            Debug.LogError("item desc and name text object not found.");
        }
    }

    private void Start()
    {
        shop.SetActive(false);
    }

    private void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (shopActive && (Input.GetKeyDown(KeyCode.Escape) || distanceToPlayer > maxShopDistance))
        {
            HideShop();
        }
    }

    private void Initialize()
    {
        columnLoaded = false;

        if (sellAttributes)
        {
            SetAttributesColumn();
        }
        if (weapons.Length > 0)
        {
            StartCoroutine(SetColumn("Weapons", weapons));
        }
        if (consumables.Length > 0)
        {
            StartCoroutine(SetColumn("Consumables", consumables));
        }
        if (runes.Length > 0)
        {
            StartCoroutine(SetColumn("Runes", runes));
        }
    }

    private void SetAttributesColumn()
    {
        string address = "Assets/Prefabs/UI/Shop/AttributesColumn.prefab";

        Addressables.LoadAssetAsync<GameObject>(address).Completed += (AsyncOperationHandle<GameObject> handle) =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    GameObject column = Instantiate(handle.Result, shop.transform);
                    attributesList = column.GetComponentsInChildren<ShopAttribute>();

                    foreach (ShopAttribute attribute in attributesList)
                    {
                        attribute.Initialize(this);
                    }

                    columnLoaded = true;
                }
                else
                {
                    Debug.LogError($"Failed to load prefab with address: {address}");
                }
            };
    }


    private IEnumerator SetColumn(string type, Item[] itemList)
    {
        yield return new WaitUntil(() => columnLoaded == true);

        string address = "Assets/Prefabs/UI/Shop/ShopColumn.prefab";

        Addressables.LoadAssetAsync<GameObject>(address).Completed += (AsyncOperationHandle<GameObject> handle) =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    GameObject column = Instantiate(handle.Result, shop.transform);
                    column.GetComponentInChildren<TextMeshProUGUI>().text = type;

                    Transform content = column.transform.Find("Viewport/Content").transform;

                    PopulateColumn(content, itemList);
                }
                else
                {
                    Debug.LogError($"Failed to load prefab with address: {address}");
                }
            };
    }

    private void PopulateColumn(Transform location, Item[] itemList)
    {
        foreach (Item item in itemList)
        {
            string address = "Assets/Prefabs/UI/Shop/ShopItem.prefab";

            Addressables.LoadAssetAsync<GameObject>(address).Completed += (AsyncOperationHandle<GameObject> handle) =>
                {
                    if (handle.Status == AsyncOperationStatus.Succeeded)
                    {
                        GameObject shopItem = Instantiate(handle.Result, location);
                        ShopItem SIComp = shopItem.GetComponent<ShopItem>();
                        SIComp.SetItem(item);
                        SIComp.Initialize(this);
                    }
                    else
                    {
                        Debug.LogError($"Failed to load prefab with address: {address}");
                    }

                };
        }
    }

    public void UpdateAllAttributesCost()
    {
        foreach (ShopAttribute attribute in attributesList)
        {
            attribute.UpdateAttributeCostAndDesc();
        }
    }

    private void DestroyShop()
    {
        foreach (Transform child in shop.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    public void ShowShop()
    {
        playerAttackController.canAttack = false;
        shopActive = true;
        shop.SetActive(true);
        Initialize();
    }

    public void HideShop()
    {
        playerAttackController.canAttack = true;
        shopActive = false;
        toolTip.SetActive(false);
        DestroyShop();
        shop.SetActive(false);
    }

    //alguma cena pra iniciar a loja colocando os itens nas colunas da loja com o preço icone etc
}
