using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WeaponIcon : MonoBehaviour
{
    public Weapon currentWeapon;
    public GameObject toolTip;
    private TextMeshProUGUI toolTipNameText;
    private TextMeshProUGUI toolTipDescText;
    public bool isHovering = false;

    private void Awake()
    {
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
        toolTip.SetActive(false);
    }

    public void ShowTooltip()
    {
        isHovering = true;

        StartCoroutine(ToolTip());
    }

    private IEnumerator ToolTip()
    {
        toolTip.SetActive(true);
        toolTipNameText.text = currentWeapon.ItemName;

        while (isHovering == true)
        {
            if (Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftShift))
            {

                toolTipDescText.text = currentWeapon.WeaponType + "\n" + currentWeapon.ItemStats + "\n\n" + currentWeapon.ItemDescription;
            }
            else
            {
                toolTipDescText.text = currentWeapon.ItemStats + "\n<color=yellow>Shift for details</color>";
            }

            yield return new WaitForSeconds(0f);
        }
    }

    public void HideTooltip()
    {
        isHovering = false;
        toolTip.SetActive(false);
    }
}
