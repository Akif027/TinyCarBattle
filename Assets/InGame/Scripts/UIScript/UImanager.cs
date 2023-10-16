using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UImanager : MonoBehaviour
{
   public Text countdownText;
    public static UImanager instance;
    private void Awake()
    {
        instance = this;
    }
}
