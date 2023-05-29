/**
 *  Main menu buttons.
 */

using UnityEngine.EventSystems; // Required when using Event data.

public class MenuButton : UIButton, ISelectHandler, IDeselectHandler, ISubmitHandler
{
    public enum ButtonType { OnePlayerNewGame, TwoPlayerNewGame, Settings }
    public ButtonType Type;

    /**
    * Override base select, trigger animation "Selected"
    */
    public override void Select()
    {
        base.Select();
        anim.SetTrigger("Selected");
    }
}