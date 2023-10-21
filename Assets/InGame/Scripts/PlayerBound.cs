using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBound : MonoBehaviour
{
    public float minX, maxX, minZ, maxZ; // Define the boundaries of the area

    void Update()
    {
        // Get the current position of the player
        Vector3 currentPosition = transform.position;

        // Clamp the X and Z coordinates to stay within the defined boundaries
        float clampedX = Mathf.Clamp(currentPosition.x, minX, maxX);
        float clampedZ = Mathf.Clamp(currentPosition.z, minZ, maxZ);

        // Update the position of the player with the clamped values
        transform.position = new Vector3(clampedX, currentPosition.y, clampedZ);
    }
}