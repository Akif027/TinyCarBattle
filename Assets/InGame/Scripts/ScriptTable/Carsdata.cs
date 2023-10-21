using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Carsdata", order = 1)]
public class Carsdata : ScriptableObject
{
    [SerializeField] int CarPrice;
    [SerializeField] string CarName;
    [SerializeField] GameObject CarType;
    [SerializeField] GameObject LobbyCars;

    public string _carName
    {
        get { return CarName; }
        set { CarName = value; }  
    }


    public GameObject TypeOfCar
    {
        get { return CarType; }
        set { CarType = value; }
    }



    public GameObject LobbyCar
    {
        get { return LobbyCars; }
        set { LobbyCar = value; }
    }


}
