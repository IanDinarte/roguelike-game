using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestController : MonoBehaviour
{
    private Animator animator;
    public GameObject itemPrefab;
    public Transform dropLocation;
    public bool isOpen = false;
    
    private void Awake() {
        animator = this.GetComponent<Animator>();    
    }

    public void Open(){
        animator.SetBool("IsOpen", true);
        isOpen = true;
    }

    public void DropItem(){
        if(itemPrefab != null){
            Instantiate(itemPrefab, dropLocation.position, itemPrefab.transform.rotation);
        }
    }
}
