/**
 * WarpZone warps player to other target WarpZone.
 * It can be used as a door, teleport, pipe, stairs etc.
 */

using System.Collections;
using UnityEngine;

public class WarpZone : BaseTile
{
    public enum WarpingType { Pipe, HandleDoor, UpStairs, DownStairs, Teleport, Invisible, PipeLeft, PipeRight, ElevatorDoor };

    public Animator Anim;
    public string EnterAnimName = "Enter";
    public string ExitAnimName = "Exit";    
    public WarpingType WarpType = WarpingType.Pipe;
    public Vector2 AnimatePlayerOffsetIn = Vector2.zero;
    public Vector2 AnimatePlayerOffsetOut = Vector2.zero;
    public bool WarpCamera = true;
    public WarpZone WarpToZone;
    public Transform InOutPosition;
    public float YOffset = 0;
    public float WarpTimeDelay = 0;

    [HideInInspector]
    public bool CoolingDown = false;

    private float coolDownTime = 1f; //TODO: Hackish one second cooldown before re-entering
    
    protected PlayerController currentPlayer;

    /**
    * Extend reset by setting StartMovingWhenInViewport back to initial value.
    */
    public virtual void PlayEnterAnimation()
    {
        if (Anim == null)
            return;

        Anim.SetTrigger(EnterAnimName);
    }

    /**
    * Extend reset by setting StartMovingWhenInViewport back to initial value.
    */
    public virtual void PlayExitAnimation()
    {
        if (Anim == null)
            return;

        Anim.SetTrigger(ExitAnimName);
    }

    public void StartCoolDown()
    {
        if (!CoolingDown)
            StartCoroutine(CoolDownRoutine());
    }

    IEnumerator CoolDownRoutine()
    {
        CoolingDown = true;
        yield return new WaitForSeconds(coolDownTime);
        CoolingDown = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (CoolingDown)
            return;

        PlayerController player = collision.gameObject.GetComponent<PlayerController>();

        if (player != null)
        {
            player.OnWarpZone = this;
            currentPlayer = player;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();

        if (player != null)
        {
            player.OnWarpZone = null;
            //TODO: Bad fix to Unity's DontDestroyOnLoad parenting issue:
            //currentPlayer = null;
            CoolingDown = false;            
        }
    }
}
