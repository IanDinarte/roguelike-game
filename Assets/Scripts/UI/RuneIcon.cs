using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RuneIcon : MonoBehaviour
{
    public Item currentItem;
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
        toolTip.SetActive(false);//
    }

    public void ShowTooltip()
    {
        if (currentItem != null)
        {
            isHovering = true;

            StartCoroutine(ToolTip());
        }
    }

    private IEnumerator ToolTip()
    {
        toolTip.SetActive(true);
        toolTipNameText.text = currentItem.ItemName;

        while (isHovering == true)
        {
            if (Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftShift))
            {
                toolTipDescText.text = currentItem.ItemStats + "\n\n" + currentItem.ItemDescription;
            }
            else
            {
                toolTipDescText.text = "\n<color=yellow>Shift for details</color>";
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
