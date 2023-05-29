/**
 *  All objects in scene are derived from this class: collectables, tiles, enemies and players.
 *  Extends MonoBehaviour with Reset functionality.
 */

using UnityEngine;

public class PlatformerObject : MonoBehaviour
{
    protected Vector2 initPosition;

    public SpriteRenderer spriteRenderer { get; private set; }

    protected virtual void Start()
    {
        //get SpriteRenderer component TODO: this is dangerous, since there are multiple SpriteRenderers...
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        //Get init values
        initPosition = transform.localPosition;
    }

    /**
    * Stop all coroutines, reset position to initPosition and set GameObject active to true.
    */
    public virtual void Reset()
    {
        //Stop all running coroutines
        StopAllCoroutines();

        //Restore init values
        transform.localPosition = initPosition;

        //Activate GO
        gameObject.SetActive(true);
    }
}
