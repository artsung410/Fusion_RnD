using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class PlayerHUD : MonoBehaviour
{
    public static PlayerHUD Instance;
    public TextMeshProUGUI chatText;

    private void Awake()
    {
        Instance = this;
    }
}
