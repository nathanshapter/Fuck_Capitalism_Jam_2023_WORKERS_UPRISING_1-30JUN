using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CanvasScript : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI leverText, deathText;
   
    DeathCounter dc;


    int baseSwitch;
    int switchesClicked;
    private void Start()
    {
        dc = FindObjectOfType<DeathCounter>();
       
        
        leverText.text = $"{switchesClicked}/{baseSwitch}";
        deathText.text = $"  X  {dc.amountOfDeaths}";
    }

    public void UpdateLeverText()
    {
        switchesClicked++;
        leverText.text = $"{switchesClicked}/{baseSwitch}";
    }
    public void UpdateDeathText()
    {
        deathText.text = $"  X  {dc.amountOfDeaths}";
    }
}
