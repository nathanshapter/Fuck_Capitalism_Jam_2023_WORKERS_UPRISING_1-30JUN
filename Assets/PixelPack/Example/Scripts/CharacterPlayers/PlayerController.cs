/**
 *  Example PlayerController that extends PlayerCharacter functionality.
 *  Your game could have several completely different playable characters
 *  with their own abilities (e.g. FlyingCharacterController would be quite different) 
 *  and they all should derive from PlayerCharacter,
 *  which provides some basic foundation to build on. 
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using UniRx;

public class PlayerController : PlayerCharacter
{
    [Header("Player Audio clips")]
    public AudioClip JumpAudioClip;
    public AudioClip WarpPipeAudioClip;

    [Header("Player properties")]
    public CameraFollow cam;
    public List<SuitSpriteSheet> SuitSpriteSheets = new List<SuitSpriteSheet>();

    [HideInInspector]
    public WarpZone OnWarpZone = null;

    [HideInInspector]
    public LadderTile OnLadder = null;

    [HideInInspector]
    public bool Warping = false;

    //Reactive property to tell what inventory item player is current using:
    public static ReactiveProperty<InventoryItemData> ItemCurrentlyUsed = new ReactiveProperty<InventoryItemData>();
    public Dictionary<string, Sprite> currentRenderSpriteSheet { get; private set; } //Current sprite sheet to render player's sprite (in LateUpdate)

    //Private properties that can be accessed only within this class    
    private int jumpCount = 0;
    private bool jumpStarted = false;
    private bool jumpButtonDown = false;
    private bool sliding = false;
    private bool ducking = false;
    private bool climbingLadder = false;
    private bool climbingOnCoolDown = false;

    private LadderTile previousLadder = null;
    private Sequence pipeWarpSequence;

    private const float inputDeadZoneTreshold = 0.1f;           //input dead zone
    private const float inputDuckOrClimbTreshold = 0.4f;        //additional treshold for ducking input (so that accidental ducking is harder)    

    private float climbingCoolDownTime = 0.2f;                  //time in seconds for player to be able to climb ladders/ropes again after release
    private float ladderBottomY;
    private float ladderTopY;
    private float frictionWalkSlowDownMultiplier = 3000f;       //How much player's walk speed slows down on slippery surfaces (they also change ForceMode).
    
    /**
    * Kill the character. This override extends the base functionality only by playing a sound effect.
    */
    public override void Die(TakeDamageType takeDamageType)
    {
        base.Die(takeDamageType);        
        AudioController.Instance.PlaySoundEffect(DieAudioClip);
    }

    /**
    * Update initPosition to new scene's spawnPoint for reset.
    */
    public void UpdateInitPosition(Vector2 newInitPosition)
    {
        initPosition = newInitPosition;
    }

    /**
    * Warp player and camera to PlayerSpawnPoint position
    * @param playerSpawnPoint PlayerSpawnPoint to warp player and camera to.
    */
    public void WarpToSpawnPoint(PlayerSpawnPoint playerSpawnPoint)
    {
        transform.position = playerSpawnPoint.transform.position;
        cam.WarpCameraPosition(playerSpawnPoint.transform.position);
    }

    /**
    * Extend the functionality by instantly settting the camera to Player's PlayerSpawnPointPosition
    */
    public override void Reset()
    {
        base.Reset();

        //Instantly set camera to Player's PlayerSpawnPointPosition
        cam.WarpCameraPosition(transform.position);
    }

    /**
    * Stop climbing ladder or rope. Eithered triggered by input, or when player drops from the ladder/rope.
    * @param jumpedOff If player jumped off the ladder (via input), wait till she's not touching the ladder anymore.
    */
    public void StopClimbing(bool jumpedOff = false)
    {
        climbingLadder = false;
        Rb2D.gravityScale = 1f;

        //Otherwise the re-grabbing is just too instant...
        if (!climbingOnCoolDown)
            StartCoroutine(ClimbingCoolDown(jumpedOff));
    }

    protected void Awake()
    {
        cam.Init(this);
        cam.transform.SetParent(null);
    }

    protected override void Start()
    {
        base.Start();
        LoadSpriteSheets();
        //Subscribe to inventory's InventoryItemUsed reactive property:
        Inventory.Instance.InventoryItemUsed.Subscribe(i => UseInventoryItem(i));
    }

    private void UseInventoryItem(InventoryItemData _item)
    {
        if (_item != null)
        {
            Debug.Log("item used: " + _item.ItemName);
            ItemCurrentlyUsed.Value = _item;
            currentRenderSpriteSheet = SuitSpriteSheets.Where(suit => suit.SuitItem == ItemCurrentlyUsed.Value).Select(s => s.SpriteDictionary).First();
        }
    }

    private void LoadSpriteSheets()
    {
        //Load sprite sheets to dictionaries:
        SuitSpriteSheets.ForEach(suit =>
        {
            suit.SpriteDictionary = Resources.
            LoadAll<Sprite>(suit.SpriteSheetName).
            ToDictionary<Sprite, string, Sprite>(key => key.name, value => value);
        });
    }

    private void LateUpdate()
    {
        //Render sprite from current suit's sprite sheet:
        if (currentRenderSpriteSheet != null)
            spriteRenderer.sprite = currentRenderSpriteSheet[spriteRenderer.sprite.name];        
    }

    protected override void HandleJumpInput(bool isDown)
    {
        if (isDown && (grounded != null || climbingLadder))
        {
            if (climbingLadder)
                StopClimbing(true);

            JumpStart();            
            AudioController.Instance.PlaySoundEffect(JumpAudioClip);

        }
        else
            JumpEnd();

        jumpButtonDown = isDown;
    }

    //Use fixed time update with rigidbody physics
    protected override void FixedUpdate()
    {
        if (!IsAlive.Value)
            return;

        //Groundcheck and physics limitations from CharacterBase
        base.FixedUpdate();

        anim.speed = 1f;

        if (InputEnabled)
        {
            if (!climbingLadder && jumpButtonDown && jumpStarted)
                JumpContinue();
        }

        //Handle invisible and teleport warps
        if (!Warping && OnWarpZone != null &&
            (
            OnWarpZone.WarpType == WarpZone.WarpingType.Invisible ||
            OnWarpZone.WarpType == WarpZone.WarpingType.Teleport ||
            OnWarpZone.WarpType == WarpZone.WarpingType.PipeLeft ||
            OnWarpZone.WarpType == WarpZone.WarpingType.PipeRight)
            )
            WarpSequence(OnWarpZone);

        //Duck and enter doors        
        VerticalMovement(AxisInput.y);

        if (climbingLadder)
            return;

        //Apply horizontal Force if no vertical                        
        if (!ducking)
            HorizontalMovement(AxisInput.x);

        //Flip sprite on horizontal movement
        if (!Warping)
            FlipSprite(AxisInput.x);

        //Slide in sloped terrain        
        sliding = Sliding(AxisInput.x, AxisInput.y);

        //Set correct animation
        if (!Warping)
            SetAnimation(Rb2D, AxisInput.x, AxisInput.y, sliding); //<-- if vertical down
    }

    private void SetAnimation(Rigidbody2D rb, float horizontal, float vertical, bool sliding)
    {
        ducking = false;

        if (grounded != null)
        {

            SlowDownFactorX = initSlowDownFactor;

            //inputDuckTreshold ensures that ducking doesn't trigger accidently. Especially with analog sticks...
            if (vertical < -(inputDeadZoneTreshold + inputDuckOrClimbTreshold))
            {
                if (sliding)
                    TriggerAnimation("Slide");
                else
                {
                    TriggerAnimation("Duck");
                    ducking = true;
                    if (FrictionDelta <= 0) //<-- Don't change SlowDownFactorX on slippery surfaces!
                        SlowDownFactorX = SlowDownFactorDuckingX;
                }
            }
            else if (Mathf.Abs(horizontal) > inputDeadZoneTreshold)
                TriggerAnimation("Walk");
            else
            {
                if (grounded.Angle == RayHitInfo.GroundAngle.Right30)
                    TriggerAnimation(spriteDirection == FacingDirection.Left ? "Slope30" : "Slope30Left");
                else if (grounded.Angle == RayHitInfo.GroundAngle.Right45)
                    TriggerAnimation(spriteDirection == FacingDirection.Left ? "Slope45" : "Slope45Left");
                else if (grounded.Angle == RayHitInfo.GroundAngle.Left30)
                    TriggerAnimation(spriteDirection == FacingDirection.Left ? "Slope30Left" : "Slope30");
                else if (grounded.Angle == RayHitInfo.GroundAngle.Left45)
                    TriggerAnimation(spriteDirection == FacingDirection.Left ? "Slope45Left" : "Slope45");
                else
                    TriggerAnimation("Idle");
            }
        }
        else
            TriggerAnimation("Jump");
    }

    private void StartClimbingLadder(LadderTile ladder)
    {
        previousLadder = ladder;
        ladderBottomY = ladder.transform.position.y + ladder.collider2d.offset.y;
        ladderTopY = ladder.transform.position.y + ladder.collider2d.bounds.extents.y * 2f - collider2d.size.y + ladder.collider2d.offset.y;

        //Zero out gravity while climbing
        Rb2D.gravityScale = 0;
        //Stop RigidBody
        SetRbVelocityZero();
        //Set character's pos to center of rope or ladder (level tile's pivot is on left, thus offset)
        Rb2D.position = new Vector2(OnLadder.transform.position.x + OnLadder.GetColliderBoundsSideOffset(), Rb2D.position.y > ladderTopY ? ladderTopY : Rb2D.position.y);

        TriggerAnimation("ClimbLadder");

        climbingLadder = true;
    }

    IEnumerator ClimbingCoolDown(bool jumped)
    {
        climbingOnCoolDown = true;

        //if player jumped off the ladder, wait till he's not touching the ladder anymore. Else wait for climbingCoolDownTime
        if (jumped)
            while (previousLadder.CheckPlayerWithinBounds(transform.position))
                yield return null;
        else
            yield return new WaitForSeconds(climbingCoolDownTime);

        climbingOnCoolDown = false;
        previousLadder = null;
    }

    private bool Sliding(float horizontal, float vertical)
    {
        if (grounded != null && grounded.Hit.normal.y < 1f)
        {
            SlowDownFactorX = 0.01f;

            if (vertical < -inputDeadZoneTreshold)
            {
                SlowDownFactorX = 1f;
                HorizontalMovement(grounded.Hit.normal.x);
            }
            else
            {
                if (Mathf.Sign(horizontal) == Mathf.Sign(grounded.Hit.normal.x)) //<-- Down hill
                {
                    if (Mathf.Abs(horizontal) > inputDeadZoneTreshold)
                        SlowDownFactorX = SlowDownFactorDownhillX; //<-- introduce variable for down hill walking?!                        
                    else
                        SlowDownFactorX = 0;
                }
                else //<-- up hill
                {
                    if (Mathf.Abs(horizontal) > inputDeadZoneTreshold)
                    {
                        //TODO: this doesn't work nicely with FrictionDelta and when approaching hill from the right
                        HorizontalMovement(-grounded.Hit.normal.x);
                        SlowDownFactorX = initSlowDownFactor;
                    }
                    else
                        SlowDownFactorX = 0;
                }
            }
            return true;
        }
        else
        {
            SlowDownFactorX = initSlowDownFactor;
            return false;
        }
    }

    private void HorizontalMovement(float inputH)
    {
        if (grounded != null)
        {
            //FrictionDelta is 0 on non-slippery surfaces. It changes on FrictionChangeTiles. 
            Rb2D.AddForce(new Vector2(inputH * (WalkForce + FrictionDelta * frictionWalkSlowDownMultiplier) * Time.deltaTime, 0), FrictionDelta > 0 ? ForceMode2D.Force : ForceMode2D.Impulse);
        }
        else
            Rb2D.AddForce(new Vector2(inputH * InAirForce * Time.deltaTime, 0), ForceMode2D.Impulse);
    }

    private void VerticalMovement(float inputV)
    {
        if (climbingLadder)
        {
            MoveOnLadder(inputV);
            return;
        }

        if (inputV < 0) //Down actions
        {
            //Go to pipe with down input
            if (OnWarpZone != null && OnWarpZone.WarpType == WarpZone.WarpingType.Pipe && Mathf.Abs(AxisInput.y) > (inputDeadZoneTreshold + inputDuckOrClimbTreshold))
                WarpSequence(OnWarpZone);
        }
        else if (inputV > 0) //Up actions
        {
            //Walk to door with up input
            if (OnWarpZone != null &&
                (OnWarpZone.WarpType == WarpZone.WarpingType.UpStairs ||
                 OnWarpZone.WarpType == WarpZone.WarpingType.DownStairs ||
                 OnWarpZone.WarpType == WarpZone.WarpingType.DownStairs ||
                 OnWarpZone.WarpType == WarpZone.WarpingType.HandleDoor ||
                 OnWarpZone.WarpType == WarpZone.WarpingType.ElevatorDoor) &&
                Mathf.Abs(AxisInput.y) > (inputDeadZoneTreshold + inputDuckOrClimbTreshold))
                WarpSequence(OnWarpZone);

            //Start climbing ladder/rope if on one.
            if (OnLadder != null)
            {
                if (!climbingLadder && !climbingOnCoolDown && AxisInput.y > (inputDeadZoneTreshold + inputDuckOrClimbTreshold) && previousLadder == null)
                    StartClimbingLadder(OnLadder);
            }
        }
    }

    private void MoveOnLadder(float inputV)
    {
        anim.speed = 0;

        if (transform.position.y <= ladderBottomY && inputV < 0) //Drop from the bottom of the ladder
            StopClimbing();
        else if (transform.position.y > ladderTopY && inputV > 0) //Do nothing at the top of the ladder
            return;
        else
        {
            //If user is pressing up or down, animate and move character on ladder
            if (inputV != 0)
            {
                anim.speed = 1f;
                Rb2D.MovePosition(Rb2D.position + new Vector2(0, inputV * 1f * Time.deltaTime));
            }
        }
    }

    private void JumpStart()
    {
        jumpStarted = true;
    }

    private void JumpEnd()
    {
        jumpStarted = false;
        jumpCount = 0;
    }

    private void JumpContinue()
    {
        if (jumpCount < MaxJumpCount)
            Rb2D.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);

        jumpCount++;
    }

    private void WarpSequence(WarpZone warpZone)
    {
        if (warpZone.CoolingDown || warpZone.WarpToZone.CoolingDown)
            return;

        warpZone.StartCoolDown();
        warpZone.WarpToZone.StartCoolDown();

        switch (warpZone.WarpType)
        {
            case WarpZone.WarpingType.Pipe:
                //Down to pipe
                StartWarp(warpZone, WarpPipeAudioClip, "Pipe");
                WarpPipeAnimation(warpZone);
                break;
            case WarpZone.WarpingType.PipeLeft:
            case WarpZone.WarpingType.PipeRight:
                //Left/right pipe
                StartWarp(warpZone, WarpPipeAudioClip, "Walk", warpZone.WarpType == WarpZone.WarpingType.PipeLeft ? -1f : 1f);
                WarpPipeAnimation(warpZone);
                break;
            case WarpZone.WarpingType.UpStairs:
                //Walk upstairs
                StartWarp(warpZone, WarpPipeAudioClip, "ToUpstairs", 1f, true, "Player_ToUpstairs");
                break;
            case WarpZone.WarpingType.DownStairs:
                //Walk downstairs
                StartWarp(warpZone, WarpPipeAudioClip, "ToDownstairs", 1f, true, "Player_ToDownstairs");
                break;
            case WarpZone.WarpingType.HandleDoor:
                //Door with a handle
                StartWarp(warpZone, WarpPipeAudioClip, "ToClosedDoor", null, true, "Player_Big_Up_Open_Door");
                break;
            case WarpZone.WarpingType.ElevatorDoor:
                //Elevator (or train) door
                StartWarp(warpZone, WarpPipeAudioClip, "ToElevator", null, true, "Player_ToElevator");
                ChangeSortingLayer(warpZone.spriteRenderer.sortingLayerName, warpZone.spriteRenderer.sortingOrder - 1, .3f, warpZone.WarpTimeDelay);
                break;
            case WarpZone.WarpingType.Invisible:
                //Invisible warp zone
                StartCoroutine(InvisibleWarp(warpZone));
                break;
            default:
                Debug.LogError("Invalid warp type.");
                break;
        }
    }

    private void StartWarp(WarpZone warpZone, AudioClip warpSound = null, string animationClip = null, float? forceSpriteFlip = null, bool playWarpZonesEnterAnimation = false, string waitForPlayerAnimationName = null)
    {
        Debug.Log("Warpzone obj: " + warpZone.gameObject.name);
        Warping = true;

        //Force sprite flip (optional)
        if (forceSpriteFlip.HasValue)
            FlipSprite(forceSpriteFlip.Value);

        //Player animation (optional)
        if (animationClip != null)
            TriggerAnimation(animationClip);

        //Warp start sound (optional)
        if (warpSound != null)
            AudioController.Instance.PlaySoundEffect(warpSound);
        
        //Disable input and collisions
        EnableInputCollisionAndPhysics(false);

        //Set position to middle of warp zone
        transform.position = Rb2D.position = new Vector2(warpZone.transform.position.x + warpZone.GetColliderSideOffset(), Rb2D.position.y + warpZone.YOffset);

        //Play WarpZone's enter animation:
        if (playWarpZonesEnterAnimation)
            warpZone.PlayEnterAnimation();

        //Wait for player's enter animation (name as string) to end:
        if (waitForPlayerAnimationName != null)
            HandleDoorWarpAnimation(waitForPlayerAnimationName, warpZone, warpZone.WarpTimeDelay);
    }

    //TODO: move this to helper class
    private void ChangeSortingLayer(string sortingLayerName, int sortingOrder, float delay, float duration)
    {
        StartCoroutine(ChangeSortingLayerRoutine(sortingLayerName, sortingOrder, delay, duration));
    }

    IEnumerator ChangeSortingLayerRoutine(string sortingLayerName, int sortingOrder, float delay, float duration)
    {
        //Get original sorting
        string origSortingLayerName = spriteRenderer.sortingLayerName;
        int origSortingOrder = spriteRenderer.sortingOrder;
        //Wait before change
        yield return new WaitForSeconds(delay);
        //Change sorting        
        spriteRenderer.sortingLayerName = sortingLayerName;
        spriteRenderer.sortingOrder = sortingOrder;
        //Hold change
        yield return new WaitForSeconds(duration);
        //Return to original sorting
        spriteRenderer.sortingLayerName = origSortingLayerName;
        spriteRenderer.sortingOrder = origSortingOrder;
    }

    IEnumerator InvisibleWarp(WarpZone warpZone)
    {
        Warping = true;

        //Disable input and collisions
        EnableInputCollisionAndPhysics(false);

        //Get WarpToPosition
        Vector2 warpToPosition = warpZone.WarpToZone.InOutPosition.position;

        //Disable renderer
        spriteRenderer.enabled = false;

        //Wait for a while
        yield return new WaitForSeconds(.4f);

        //Immeadiately set camera position
        if (warpZone.WarpCamera)
            cam.WarpCameraPosition(warpToPosition);

        Rb2D.position = warpToPosition;

        //Enable input and collisions
        EnableInputCollisionAndPhysics(true);

        warpZone.WarpToZone.CoolingDown = true;
        Warping = false;

        yield return new WaitForEndOfFrame();

        //Enable renderer
        spriteRenderer.enabled = true;
    }

    private void HandleDoorWarpAnimation(string animationName, WarpZone warpZone, float delayBeforeAnimation = 0f)
    {
        StartCoroutine(AnimatorHelper.CheckAnimationCompleted(anim, animationName, () => ExitDoorCallback(warpZone.WarpCamera, warpZone.WarpToZone.InOutPosition.position, warpZone.WarpToZone), delayBeforeAnimation));
    }

    private void ExitDoorCallback(bool warpCamera, Vector2 warpToPosition, WarpZone warpToZone)
    {
        //Immeadiately set camera position
        if (warpCamera)
            cam.WarpCameraPosition(warpToPosition);

        Rb2D.position = warpToPosition;
        warpToZone.PlayExitAnimation();
        EndWarp();
    }

    private void WarpPipeAnimation(WarpZone warpZone)
    {
        //Get WarpToPosition
        Vector2 warpToPosition = warpZone.WarpToZone.InOutPosition.position;

        //Animate with DoTween        
        pipeWarpSequence = DOTween.Sequence();
        pipeWarpSequence.Append(Rb2D.DOMove(Rb2D.position + warpZone.AnimatePlayerOffsetIn, 1f).From(Rb2D.position, false));
        pipeWarpSequence.AppendCallback(() => cam.WarpCameraPosition(warpToPosition));
        pipeWarpSequence.Append(Rb2D.DOMove(warpToPosition, 1f).From(warpToPosition + warpZone.AnimatePlayerOffsetOut, false));
        pipeWarpSequence.AppendCallback(() => EndWarp());
    }

    private void EndWarp()
    {
        Warping = false;
        EnableInputCollisionAndPhysics(true);
        TriggerAnimation("Idle");
    }

    private void EnableInputCollisionAndPhysics(bool enable)
    {
        //Enable / disable input
        if (enable)
            SubscribeToInput();
        else
            UnSubscribeInput();

        //Collisions
        IgnoreAllCollisions = !enable;
        //Stop RigidBody if disabling physics
        if (!enable)
            SetRbVelocityZero();
        //Set kinematic for animation
        Rb2D.isKinematic = !enable;
    }
}

/**
 * SuitSpriteSheet parameter object.
 */
[Serializable]
public class SuitSpriteSheet
{
    public InventoryItemData SuitItem;
    public string SpriteSheetName;
    [HideInInspector]
    public Dictionary<string, Sprite> SpriteDictionary;
}