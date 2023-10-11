using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelData 
{
    // is wheel touched ground or not ?
    [HideInInspector]
    public bool isOnGround = false;

    // wheel ground touch point
    [HideInInspector]
    public RaycastHit touchPoint = new RaycastHit();

    // real yaw, after Ackermann steering correction
    [HideInInspector]
    public float yawRad = 0.0f;

    // visual rotation
    [HideInInspector]
    public float visualRotationRad = 0.0f;

    // suspension compression
    [HideInInspector]
    public float compression = 0.0f;

    // suspension compression on previous update
    [HideInInspector]
    public float compressionPrev = 0.0f;

    [HideInInspector]
    public string debugText = "-";
}
