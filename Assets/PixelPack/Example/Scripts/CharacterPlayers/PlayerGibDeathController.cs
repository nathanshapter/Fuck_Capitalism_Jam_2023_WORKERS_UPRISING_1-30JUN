/**
 *  Class to control player's upper and lower parts when gibbed.
 */

using UnityEngine;

public class PlayerGibDeathController : MonoBehaviour
{
    public float DestroyAfterSeconds = 3f;
    public GameObject BloodAnimation;
    public GameObject upperHalf;
    public GameObject lowerHalf;
    private void OnEnable()
    {
        //TODO: if player's sprite sheet changes, these sprite animations must change too!
        //Unparent halves from effect object:
        upperHalf.transform.SetParent(null);
        lowerHalf.transform.SetParent(null);

        //Add some random force to throw the pieces:
        upperHalf.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-2f, 2f), Random.Range(1f, 2f)), ForceMode2D.Impulse);
        lowerHalf.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-2f, 2f), Random.Range(1f, 2f)), ForceMode2D.Impulse);

        //Destroy instances after delay:
        Destroy(upperHalf, DestroyAfterSeconds);
        Destroy(lowerHalf, DestroyAfterSeconds);
        Destroy(gameObject, DestroyAfterSeconds);
    }
}
