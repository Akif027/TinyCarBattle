using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBound : MonoBehaviour
{
    public Transform arenaCenter;
    public float arenaRadius = 10f;

    private void Start()
    {
        arenaCenter = UImanager.instance.arenaCenter;
    }
    private void Update()
    {
        // Calculate the direction from the center of the circle to the car
        Vector3 directionToCenter = transform.position - arenaCenter.position;

        // Clamp the magnitude (radius) to stay within the circular bounds
        Vector3 clampedDirection = Vector3.ClampMagnitude(directionToCenter, arenaRadius);

        // Calculate the new position relative to the center
        Vector3 newPosition = arenaCenter.position + clampedDirection;

        // Update the car's position
        transform.position = newPosition;
    }

}