/**
 *  Base class for all UI buttons.
 */

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // Required when using Event data.
using UniRx;

public class UIButton : MonoBehaviour, ISelectHandler, IDeselectHandler, ISubmitHandler
{    
    public bool InitiallySelect = false;
    
    [HideInInspector]
    public Button ButtonComponent;

    [HideInInspector]
    public ReactiveProperty<bool> InternalSelected = new ReactiveProperty<bool>(false);
    public BooleanNotifier Submitted = new BooleanNotifier(false);

    protected Animator anim;

    protected virtual void Awake()
    {
        ButtonComponent = GetComponent<Button>();        
        anim = GetComponent<Animator>();
        if (InitiallySelect)
            Select();
    }

    /**
    * Call button component's Select method
    */
    public virtual void Select()
    {
        //TODO: terrible hack, but otherwise the piece of shit Button component won't show selected color...
        ButtonComponent.interactable = false;
        ButtonComponent.interactable = true;

        ButtonComponent.Select();        
        
    }

    /**
    * Set ReactiveProperty IntervalSelected's value to true.
    */
    public virtual void OnSelect(BaseEventData eventData)
    {        
        InternalSelected.Value = true;        
    }

    /**
    * Set ReactiveProperty IntervalSelected's value to false.
    */
    public virtual void OnDeselect(BaseEventData eventData)
    {        
        InternalSelected.Value = false;        
    }

    /**
    * Switch Submitted BooleanNotifier's value
    */
    public virtual void OnSubmit(BaseEventData eventData)
    {
        //TODO: terrible hack, but otherwise the piece of shit Button component won't show correct color...
        ButtonComponent.interactable = false;
        ButtonComponent.interactable = true;

        Submitted.SwitchValue();
    }
}