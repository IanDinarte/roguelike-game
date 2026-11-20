using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldController : MonoBehaviour
{
    private BoxCollider hitBox;
    private EquipmentController equipmentController;
    [SerializeField] private GameObject sparkGameObject;
    [SerializeField] private AudioSource defendSound;

    private void Awake()
    {
        hitBox = GetComponent<BoxCollider>();
        equipmentController = GetComponentInParent<EquipmentController>();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("EnemyWeapon"))
        {
            var collisionPoint = other.ClosestPoint(transform.position);
            sparkGameObject.transform.position = collisionPoint;

            ParticleSystem sparkParticle = sparkGameObject.GetComponent<ParticleSystem>();
            if (sparkParticle != null)
            {
                defendSound.Play();
                sparkParticle.Play();
            }
        }
    }

    public void ActivateHitBox()
    {
        hitBox.enabled = true;
    }
    
    public void DeactivateHitBox(){
        hitBox.enabled = false;
    }
}
