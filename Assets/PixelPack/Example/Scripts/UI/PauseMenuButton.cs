/**
 *  Pause menu buttons.
 */

using UnityEngine.EventSystems; // Required when using Event data.
using UnityEngine.UI;
using UnityEngine;

public class PauseMenuButton : UIButton, ISelectHandler, IDeselectHandler, ISubmitHandler
{
    public enum ButtonType { Resume, Restart, WorldMap }
    public ButtonType Type;

    /**
    * Override OnSelect, change text color to black.
    */
    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        GetComponentInChildren<Text>().color = Color.black;
    }

    /**
    * Override OnDeselect, change text color to white.
    */
    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        GetComponentInChildren<Text>().color = Color.white;
    }
}