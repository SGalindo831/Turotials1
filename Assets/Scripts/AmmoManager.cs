using UnityEditor;
using UnityEngine;
using TMPro;
using System.Collections;

public class AmmoManager : MonoBehaviour
{
    public static AmmoManager Instance { get; set; }
      //UI
    public TextMeshProUGUI ammoDisplay;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }   
    }
}