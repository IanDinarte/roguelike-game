using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TooltipController : MonoBehaviour
{
    private float xOffset = -125f;
    private float yOffset = 0f;

    void Update()
    {
        Vector3 pos = Input.mousePosition;
        transform.position = new Vector3(pos.x + xOffset, pos.y + yOffset, pos.z);
    }
}
