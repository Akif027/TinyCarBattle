using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class Axle
{

  
    [Header("Axle settings")]


    public float width = 0.4f;


    public Vector2 offset = Vector2.zero;


    public float steerAngle = 0.0f;

    [Header("Wheel settings")]


    public float radius = 0.3f;

    [Range(0.0f, 1.0f)]

    public float laterialFriction = 0.1f;

    [Range(0.0f, 1.0f)]

    public float rollingFriction = 0.01f;

    [HideInInspector]

    public bool brakeLeft = false;

    [HideInInspector]

    public bool brakeRight = false;

    [HideInInspector]

    public bool handBrakeLeft = false;

    [HideInInspector]

    public bool handBrakeRight = false;


    public float brakeForceMag = 4.0f;

    [Header("Suspension settings")]


    public float stiffness = 8500.0f;


    public float damping = 3000.0f;

    public float restitution = 1.0f;


    public float lengthRelaxed = 0.55f;


    public float antiRollForce = 10000.0f;

    [HideInInspector]
    public WheelData wheelDataL = new WheelData();

    [HideInInspector]
    public WheelData wheelDataR = new WheelData();

    [Header("Visual settings")]

    public float visualScale = 0.03270531f;


    public GameObject wheelVisualLeft;


    public GameObject wheelVisualRight;


    public bool isPowered = false;



    public float afterFlightSlipperyK = 0.02f;

    public float brakeSlipperyK = 0.5f;


    public float handBrakeSlipperyK = 0.01f;

}

