/**
 *  Class for input controlled player characters. 
 *  Extend this class to create different kinds of playable characters to your game.
 */

using UniRx;
using UnityEngine;

public class PlayerCharacter : CharacterBase {

    //Coins and score as reactive properties for UI
    public static ReactiveProperty<int> Coins { get; private set; } = new ReactiveProperty<int>(0);    /**< Coins ReactiveProperty for the UI.*/
    public static ReactiveProperty<int> Score { get; private set; } = new ReactiveProperty<int>(0);    /**< Score ReactiveProperty for the UI.*/
    
    public BooleanNotifier DeathCompleted = new BooleanNotifier(false);

    [Header("Reload map when completed (debug)")]
    public bool ReloadLoadedMap = false; //<-- set to true, to reload current scene in the end of the level...

    public bool IgnoreAllCollisions                                                             /**< Change player's layer to ignore all collisions. */
    {
        get { return ignoreAllCollisions; }
        set {            
            ignoreAllCollisions = value;
            //Set PlayerCharacter to use IgnoreAllCollisions or Player layer
            gameObject.layer = ignoreAllCollisions ? GameHelper.IgnoreAllLayer : GameHelper.PlayerLayer;
        }
    }

    [Header("Basic Audio clips")]
    public AudioClip DieAudioClip;                          /**< Dying sound effect. */    
    public AudioClip DealStompDamageAudioClip;              /**< Stomping sound effect. */
    public AudioClip CollectCoinClip;                       /**< Collect coin sound effect. */

    protected Vector2 AxisInput;
    protected bool InputEnabled = true;
    protected bool ignoreAllCollisions = false;
    
    private InputController inputController;
    private CompositeDisposable disposables = new CompositeDisposable();

    /**
    * Enable or disable PlayerCharacter (don't use SetActive() directly).
    * @param isEnabled Enable or disable character.
    */
    public void SetEnabled(bool isEnabled)
    {
        //NOTE: this is a bit obfuscating. SetEnabled is called every time scene is loaded and thus works. Also it's fucked to trust in some string animation name...
        //Prevent "Walk" animation locking to what is actually "Idle" in animator
        currentAnimationName = "Idle";

        InputEnabled = isEnabled;
        gameObject.SetActive(isEnabled);
    }

    /**
    * Change Coins and Score ReactiveProperty values.
    * @param deltaCoinsAmount Positive or negative change to amount of coins.
    * @param deltaScoreAmount Positive or negative change to score.
    */
    public void ChangeCoinsAndScore(int deltaCoinsAmount, int deltaScoreAmount)
    {
        if (deltaCoinsAmount > 0)
            AudioController.Instance.PlaySoundEffect(CollectCoinClip);

        Coins.Value += deltaCoinsAmount;
        Score.Value += deltaScoreAmount;
    }

    protected override void Start()
    {
        base.Start();

        inputController = InputController.Instance;
        
        //Get input subscriptions
        SubscribeToInput();

        //Set PlayerCharacters to use Player layer
        gameObject.layer = GameHelper.PlayerLayer;
    }

    /**
    * Subscribe to InputController's ReactivePropertys. Enable input.   
    */
    public void SubscribeToInput()
    {
        //TODO: Prevent re-subscribing to input in first place! Check the logic.
        if (disposables != null)
            disposables.Dispose();

        disposables = new CompositeDisposable();

        //Get input from input controller        
        inputController.Horizontal.Subscribe(horizontal => AxisInput.x = horizontal).AddTo(disposables);
        inputController.Vertical.Subscribe(vertical => AxisInput.y = vertical).AddTo(disposables);           
        inputController.SelectAndJump.Subscribe(jump => HandleJumpInput(jump)).AddTo(disposables);

        InputEnabled = true;
    }

    /**
    * Set character's input externally (for cut scenes etc.)
    */
    public void OverrideAxisInput(Vector2 fakeInput)
    {
        AxisInput = fakeInput;
    }

    /**
    * Unsubscribe from InputController's ReactivePropertys (dispose subscriptions). Disable input.   
    */
    public void UnSubscribeInput()
    {        
        disposables.Dispose();
        InputEnabled = false;
    }

    /**
    * Reset PlayerCharacter. 
    * Set RigidBody2D's isKinematic to false.
    * Subscribe to input.
    */
    public override void Reset()
    {
        base.Reset();
        Rb2D.isKinematic = false;
        SubscribeToInput();
    }

    /**
    * Kill PlayerCharacter. 
    * Start basic dying sequence, show proper animations and wait for them to finish.    
    */
    public override void Die(TakeDamageType takeDamageType)
    {
        base.Die(takeDamageType);
        
        CharacterAnimationSequence animSeq = ScriptableObject.CreateInstance<CharacterAnimationSequence>();

        switch (takeDamageType)
        {
            case TakeDamageType.Undefined:
                Debug.Log("Generic death anim.");                
                StartCoroutine(animSeq.BounceDeathSequence(this, () => DeathCompleted.SwitchValue(),"Die"));
                break;            
            case TakeDamageType.Burned:
                Debug.Log("Burned death anim.");
                StartCoroutine(animSeq.BounceDeathSequence(this, () => DeathCompleted.SwitchValue(), "FireDeath"));
                break;
            case TakeDamageType.Drowned:
                StartCoroutine(animSeq.DrownDeathSequence(this, () => DeathCompleted.SwitchValue()));
                break;
            case TakeDamageType.Bloody:
            case TakeDamageType.Stomped:
            case TakeDamageType.Gibbed:
                Debug.Log("Gibbed death anim.");
                StartCoroutine(animSeq.GibDeathSequence(this, () => DeathCompleted.SwitchValue()));
                break;
            default:
                Debug.LogError("Unknown death type! Showing generic death.");        
                StartCoroutine(animSeq.BounceDeathSequence(this, () => DeathCompleted.SwitchValue(), "Die"));
                break;
        }
    }

    protected virtual void HandleShooting(bool isDown) { }
    protected virtual void HandleJumpInput(bool isDown) { }

    private void OnDestroy() {
        UnSubscribeInput();
    }
}
