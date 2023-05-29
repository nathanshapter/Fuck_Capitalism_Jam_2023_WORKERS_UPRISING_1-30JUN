﻿/**
 * Singleton class to handle Unity's input as ReactiveProperties.
 */

using UnityEngine;
using UnityEngine.EventSystems;
using UniRx;

public class InputController : SingletonMono<InputController>
{   
    public ReactiveProperty<float> Horizontal = new ReactiveProperty<float>();
    public ReactiveProperty<float> Vertical = new ReactiveProperty<float>();
    public ReactiveProperty<int> InventoryLeft = new ReactiveProperty<int>();
    public ReactiveProperty<int> InventoryRight = new ReactiveProperty<int>();
    public ReactiveProperty<float> HorizontalRaw = new ReactiveProperty<float>();
    public ReactiveProperty<float> VerticalRaw = new ReactiveProperty<float>();
    public ReactiveProperty<MouseMovement> MouseMove = new ReactiveProperty<MouseMovement>();
    public ReactiveProperty<float> MouseHorizontal = new ReactiveProperty<float>();
    public ReactiveProperty<float> MouseVertical = new ReactiveProperty<float>();
    public ReactiveProperty<bool> SelectAndJump = new ReactiveProperty<bool>();
    public ReactiveProperty<bool> InventoryUse = new ReactiveProperty<bool>();
    public ReactiveProperty<bool> Cancel = new ReactiveProperty<bool>();
    public ReactiveProperty<bool> Anykey = new ReactiveProperty<bool>();
        
    public bool InputEnabled
    {
        get { return inputEnabled; }
        set {
            inputEnabled = value;
            //Reset all inputs if input is disabled
            if (!inputEnabled)
            {
                Horizontal.Value = Vertical.Value = HorizontalRaw.Value = VerticalRaw.Value = 0;
                InventoryLeft.Value = InventoryRight.Value = 0;
                InventoryUse.Value = SelectAndJump.Value = Cancel.Value = Anykey.Value = false;                     
            }
        }
    }

    private bool inputEnabled = true;

    private GameObject lastselectedGO;

    public void Init()
    {
        UpdateProperties();    
    }

    private void Update()
    {
        if (inputEnabled)
            UpdateProperties();

        //This a bit dirty hack, but unfortunately required since there's no alternative.
        //Prevents mouse clicks taking focus from UI elements.
        PreventMouseFromTakingFocus();
    }

    private void PreventMouseFromTakingFocus()
    {
        if (EventSystem.current.currentSelectedGameObject == null)        
            EventSystem.current.SetSelectedGameObject(lastselectedGO);        
        else        
            lastselectedGO = EventSystem.current.currentSelectedGameObject;        
    }

    private void UpdateProperties()
    {
        

        //Input axes from left analog stick
        Horizontal.Value = Input.GetAxis("Horizontal");
        Vertical.Value = Input.GetAxis("Vertical");

        //Input axes from xbone triggers for inventory browsing:
        InventoryLeft.Value = (int)Input.GetAxis("InventoryLeft");
        InventoryRight.Value = (int)Input.GetAxis("InventoryRight");

        HorizontalRaw.Value = Input.GetAxisRaw("Horizontal");
        VerticalRaw.Value = Input.GetAxisRaw("Vertical");

        MouseHorizontal.Value = Input.GetAxis("Mouse X");
        MouseVertical.Value = Input.GetAxis("Mouse Y");

        MouseMove.Value = new MouseMovement(Input.mousePosition, new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")));

        //Buttons

        //Fire1 is SELECT and JUMP (xbox A)
        if (Input.GetButtonDown("Fire1"))
            SelectAndJump.Value = true;

        if (Input.GetButtonUp("Fire1"))
            SelectAndJump.Value = false;

        //Fire3 is ACTIVATE INVENTORY ITEM USE (xbox Y)
        if (Input.GetButtonDown("InventoryUse"))
            InventoryUse.Value = true;

        if (Input.GetButtonUp("InventoryUse"))
            InventoryUse.Value = false;

        //Cancel is BACK or CANCEL (xbox B)
        if (Input.GetButtonDown("Cancel"))
            Cancel.Value = true;
        
        if (Input.GetButtonUp("Cancel"))
            Cancel.Value = false;

        //Keys
        Anykey.Value = Input.anyKey;
    }
}

/**
 * Combine mouse movement's position and delta.
 */
public class MouseMovement
{
    public Vector2 Position;
    public Vector2 Delta;

    public MouseMovement() { }

    public MouseMovement(Vector2 _pos, Vector2 _delta)
    {
        Position = _pos;
        Delta = _delta;
    }
}
