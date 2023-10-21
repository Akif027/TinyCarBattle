using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class CarSelection : MonoBehaviour
{
    public GameObject[] cars;  // Array to store your car objects
    private int currentCarIndex = 0;  // Index of the currently active car

    void Start()
    {
        // Ensure only the first car is initially active
        SetCarActive(currentCarIndex);
    }

    void Update()
    {
        // Check for input to switch between cars
        if (Input.GetKeyDown(KeyCode.N))
        {
            NextCar();
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            PreviousCar();
        }
        //Car Rotate
        StartCoroutine(RotateOverTime(10));
    }

   public void NextCar()
    {
        // Deactivate the current car
        SetCarActive(currentCarIndex, false);

        // Move to the next car
        currentCarIndex = (currentCarIndex + 1) % cars.Length;

        // Activate the new current car
        SetCarActive(currentCarIndex);
    }

    public void PreviousCar()
    {
        // Deactivate the current car
        SetCarActive(currentCarIndex, false);

        // Move to the previous car
        currentCarIndex = (currentCarIndex - 1 + cars.Length) % cars.Length;

        // Activate the new current car
        SetCarActive(currentCarIndex);
    }

    void SetCarActive(int index, bool isActive = true)
    {
        // Set the specified car's active state
        if (index >= 0 && index < cars.Length)
        {
            cars[index].SetActive(isActive);
        }
    
    }

    private void OnDisable()
    {
        PlayerPrefs.SetInt("CarIndex", currentCarIndex);
    }

    private void OnEnable()
    {
        PlayerPrefs.DeleteAll();
    }

    IEnumerator RotateOverTime(float duration)
    {
        float startRotation = transform.eulerAngles.y;
        float endRotation = startRotation + 360.0f;
        float t = 0.0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float yRotation = Mathf.Lerp(startRotation, endRotation, t / duration) % 360.0f;
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, yRotation,
            transform.eulerAngles.z);
            yield return null;
        }


    }
}
