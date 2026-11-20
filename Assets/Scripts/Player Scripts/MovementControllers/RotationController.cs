using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationController : MonoBehaviour
{
    //variables
    float rotationFactorPerFrame = 15.0f;
    private HitController hitController;

    void Awake()
    {
        hitController = this.GetComponent<HitController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!hitController.HasCC()) {
            handleRotation();
        }
    }

    void handleRotation()
    {
        // Get the mouse position in screen coordinates
        Vector3 mouseScreenPosition = Input.mousePosition;

        // Convert the screen position to a point in the world
        Ray ray = Camera.main.ScreenPointToRay(mouseScreenPosition);
        Plane plane = new Plane(Vector3.up, Vector3.zero); // Plane with normal pointing up (y-axis), assuming your ground plane is at y = 0
        float distance;
        if (plane.Raycast(ray, out distance))
        {
            Vector3 positionToLookAt = ray.GetPoint(distance);

            // Keep the y-coordinate the same as the character's y-coordinate
            positionToLookAt.y = transform.position.y;

            // Calculate the target rotation
            Quaternion currentRotation = transform.rotation;
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt - transform.position);

            // Smoothly rotate towards the target rotation
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame * Time.deltaTime);
        }
    }
}
