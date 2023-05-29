/**
 *  Extend from Enemy class to create enemies.
 */

using UnityEngine;

public class Enemy : CharacterBase
{
    [Header("Generic Enemy Attributes")]
    public bool ChangeDirectionOnSideCollision = true;  /**< Change direction if enemy's side collides with something.*/
    public bool ChangeDirectionOnAboutToFall = false;   /**< Change direction if enemy's about to fall from a ledge.*/
    public bool TakeDamageOnStomp = true;               /**< Take damage if player stomps the enemy.*/
    public bool BounceFromBricks = true;                /**< Bounce enemy if player bounces tile below.*/
    public bool WakeUpOnCamViewport = true;             /**< Wake up enemy when player enters camera viewport.*/

    protected override void Start()
    {
        //If enemy is always active
        if (!WakeUpOnCamViewport)
            IsActive.Value = true;

        base.Start();

        //Set Enemies to use Enemy layer
        gameObject.layer = GameHelper.EnemyLayer;
    }

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsAlive.Value)
            return;

        PlayerCharacter player = collision.gameObject.GetComponent<PlayerCharacter>();
        if (player != null)
        {
            ContactPoint2D point = collision.GetContact(0);

            //Stompable enemy 
            if (TakeDamageOnStomp)
            {                
                //Enemy takes hit (player deals damage to enemy)
                if (point.normal.x < .5f && player.transform.position.y > (transform.position.y + collider2d.size.y)) //<-- 45 degrees or more above enemy and on top of enemy collider
                {
                    player.Rb2D.velocity = new Vector2(player.Rb2D.velocity.x, 3f);                    
                    TakeDamage(player, TakeDamageType.Stomped);                    
                }
                else //Player takes hit (enemy deals damage to player)
                    player.TakeDamage(this);
            }
            else
                player.TakeDamage(this);
        }
    }
    
    protected override void FixedUpdate()
    {   
        //Activate enemy if transform.position point is within camera viewport
        if (!IsActive.Value && CameraHelper.IsPointWithinViewport(Camera.main, Camera.main.transform.position, transform.position))
            IsActive.Value = true;

        if (!IsAlive.Value || !IsActive.Value)
            return;

        //Ground/side checks from CharacterBase
        base.FixedUpdate();

        //Change walk direction on collision if ChangeDirectionOnSideCollision == true
        if (ChangeDirectionOnSideCollision && sideHit != null)                    
            ChangeDirOnSideCollision(sideHit);
        
        //Flip direction, if groundcheck fails on bottom left/right but is still grounded from center
        if (ChangeDirectionOnAboutToFall && grounded != null)
            ChangeDirOnFall(grounded);
    }

    //Flip enemy's walking direction if she's about to fall from the ledge.
    protected virtual void ChangeDirOnFall(RayHitInfo groundHit)
    {
        if (!groundHit.BottomLeftGrounded && groundHit.BottomRightGrounded)
        {
            SetRbVelocityZero();
            facingDirection = FacingDirection.Right;
        }
        else if (groundHit.BottomLeftGrounded && !groundHit.BottomRightGrounded)
        {
            SetRbVelocityZero();
            facingDirection = FacingDirection.Left;
        }
    }

    //Flip enemy's walking direction, if side hits something.
    protected virtual void ChangeDirOnSideCollision(RayHitInfo sideHit)
    {        
        if (sideHit != null)
            facingDirection = sideHit.Type == RayHitInfo.HitType.Left ? FacingDirection.Right : FacingDirection.Left;
    }
}