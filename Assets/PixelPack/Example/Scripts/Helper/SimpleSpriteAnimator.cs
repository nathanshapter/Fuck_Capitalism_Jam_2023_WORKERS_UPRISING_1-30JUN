/**
 *  Since AnimatorController/Animation workflow is tedious and extremely error prone, it's
 *  easier to handle simple sprite animations with a simple animation class.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SpriteRenderer))]
public class SimpleSpriteAnimator : MonoBehaviour
{
    [Tooltip("List of sprites (animation keyframes) to use in the animation")]
    public List<Sprite> Sprites = new List<Sprite>();
    [Tooltip("Delay per frame")]
    public float AnimationFrameDelay = .1f;
    [Tooltip("Random delay before animation starts")]
    public float RandomStartDelay = 0f;
    [Tooltip("Random delay after every loop")]
    public float RandomLoopDelay = 0f;
    [Tooltip("Play frames in random order")]
    public bool PlayRandomFrames = false;
    [Tooltip("Play animation reversed")]
    public bool Reversed = false;
    [Tooltip("Disable GameObject after the last loop")]
    public bool DisableAfterLoops = false;
    [Tooltip("Destroy GameObject after the last loop")]
    public bool DestroyAfterLoops = false;
    [Tooltip("Stop animation to last frame after the last loop")]
    public bool StopToLastFrame = false;

    [Tooltip("Amount of loops to play. Zero = infinite")]
    public int Loops = 0;
    

    private int currentFrame = 0;
    private int totalFrames;
    private SpriteRenderer spriteRenderer;
    private Image image;
    private int loopsPlayed;
    
    private Dictionary<string, Sprite> currentRenderSpriteSheet;        //Current sprite sheet to render animation with

    private void Start()
    {
        //if GameObject has UI Image component, update image sprite:
        image = GetComponent<Image>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        totalFrames = Sprites.Count;        

        if (Reversed)
            Sprites.Reverse(); //<-- reverse list, reverse animation
    }
    
    private void OnEnable()
    {
        //Start coroutine in OnEnable, so that coroutine resets for PlatformerObjects that are disabled (e.g. Coin)
        StartCoroutine(Animation());
        loopsPlayed = 0;
    }

    private void LateUpdate()
    {
        //Render sprite from current sprite sheet:
        if (currentRenderSpriteSheet != null) {
            if (image == null)
                spriteRenderer.sprite = currentRenderSpriteSheet[spriteRenderer.sprite.name];
            else
                image.sprite = currentRenderSpriteSheet[spriteRenderer.sprite.name];
        }
    }

    /**
    * Switch sprite sheet in runtime. Continue rendering the animation with changed sprite sheet.
    * @param _newSpriteSheet Character to kill.
    */

    public void SwitchRuntimeSpriteSheet(Dictionary<string, Sprite> _newSpriteSheet)
    {
        currentRenderSpriteSheet = _newSpriteSheet;
    }

    IEnumerator Animation()
    {
        //Random delay in start
        yield return new WaitForSeconds(Random.Range(0, RandomStartDelay));

        while(Loops == 0 || loopsPlayed < Loops)
        {
            if (PlayRandomFrames)
            {
                currentFrame = Random.Range(0, totalFrames);
            }
            else
            {
                if (currentFrame < totalFrames - 1)
                {
                    currentFrame++;
                }
                else
                {                    
                    loopsPlayed++;
                    if (loopsPlayed < Loops || (!StopToLastFrame && !DestroyAfterLoops && !DisableAfterLoops))
                        currentFrame = 0;                    
                }
            }

            if (image == null)
                spriteRenderer.sprite = Sprites[currentFrame];
            else
                image.sprite = Sprites[currentFrame];

            //Random delay after every loop
            if (currentFrame == 0)
                yield return new WaitForSeconds(Random.Range(0, RandomLoopDelay));

            if (loopsPlayed == Loops && (StopToLastFrame || DestroyAfterLoops || DisableAfterLoops))
                yield return null;
            else 
                yield return new WaitForSeconds(AnimationFrameDelay);

        }

        if (DisableAfterLoops)
            gameObject.SetActive(false);
        else if (DestroyAfterLoops)
            Destroy(gameObject);
    }
}
