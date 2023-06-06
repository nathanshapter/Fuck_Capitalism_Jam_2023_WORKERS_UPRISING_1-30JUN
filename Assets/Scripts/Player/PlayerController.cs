using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float horizontal;
    Rigidbody2D rb;
    [SerializeField] float speedActuel = 10;
    public bool isFacingRight;
    [SerializeField] float coyoteTime;
    float coyoteTimeCounter;
    [SerializeField] GameObject groundCheck;
    public LayerMask groundLayer, boxLayer;
    [SerializeField] float jumpingPower = 50;

    Animator animator;


    [SerializeField] CinemachineVirtualCamera virtualCamera;

    [SerializeField] private AudioClip jumpClip, landClip, runningClip, deathClip;

    RespawnManager rm;
    PlayerInput playerInput;
    public float vertical;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        
        rm = GetComponent<RespawnManager>();
        playerInput = GetComponent<PlayerInput>();

    }
    private void Update()
    {
       
        FlipPlayer();
        if (IsGrounded()) 
        {
            coyoteTimeCounter = coyoteTime;
            animator.SetBool("isGrounded", true);
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
            animator.SetBool("isGrounded", false);

        }       
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Ground"))
        {
            transform.SetParent(collision.transform);
            AudioManager.instance.PlaySound(landClip, 1.5f);
        }
    }

   
    public void Move(InputAction.CallbackContext context)
    {
        
        if (IsGrounded())
        {
            AudioManager.instance.PlaySound(runningClip,1);
        }
        horizontal = context.ReadValue<Vector2>().x;


          animator.SetBool("isRunning", true);
        if (context.canceled)
        {
            animator.SetBool("isRunning", false);
            AudioManager.instance.StopSound(runningClip);
        }


    }
   
    public void Suicide(InputAction.CallbackContext context)
    {
        GetComponent<RespawnManager>().ProcessDeath();
    }
    public void PanCameraUp(InputAction.CallbackContext context)
    {
       // after holding for 1.5 seconds should pan the camera up/down
    }
    public void PanCameraDown(InputAction.CallbackContext context)
    {
       
    }
  


    public void Jump(InputAction.CallbackContext context)
    {
        transform.SetParent(null);        

        if (context.performed && coyoteTimeCounter > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);

            animator.SetTrigger("Jump");
            AudioManager.instance.PlaySound(jumpClip, 0.5f);
        }
        if (context.canceled && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            coyoteTimeCounter = 0f;
            AudioManager.instance.StopSound(jumpClip);
        }
    }

    public IEnumerator DisableControls()
    {
        playerInput.enabled = false;
        yield return new WaitForSeconds(rm.waitBeforeRespawn);
        playerInput.enabled = true;
    }
    public bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.transform.position, 1, groundLayer);

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.transform.position, 1);
    }

    public bool OnBox() // useful for later for other layers
    {
        return Physics2D.OverlapCircle(groundCheck.transform.position, 0.5f, boxLayer);
    }


    private void FlipPlayer()
    {
        rb.velocity = new Vector2(horizontal * speedActuel, rb.velocity.y);


        if (isFacingRight && horizontal > 0f)
        {
            Flip();
        }
        else if (!isFacingRight && horizontal < 0f)
        {
            Flip();

        }
    }
    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x /= -1f;
        transform.localScale = localScale;

    }
 


}
