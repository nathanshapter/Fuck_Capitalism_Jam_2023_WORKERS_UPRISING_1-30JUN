using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CanvasScript : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI switches, deathText, ammoText;
   
    DeathCounter dc;


    int baseSwitch;
    int switchesClicked;
    Shooting playerGun;
    private void Start()
    {
        playerGun = FindObjectOfType<Shooting>();
        dc = FindObjectOfType<DeathCounter>();
       
        
     //   switches.text = $"{switchesClicked}/{baseSwitch}";
    //    deathText.text = $"  X  {dc.amountOfDeaths}";
        UpdateAmmoText();
    }

    public void UpdateLeverText()
    {
        switchesClicked++;
        switches.text = $"{switchesClicked}/{baseSwitch}";
    }
    public void UpdateDeathText()
    {
        deathText.text = $"  X  {dc.amountOfDeaths}";
    }
    public void UpdateAmmoText()
    {
        ammoText.text = $"{playerGun.ammo} / {playerGun.ammoMax}";
    }
}
