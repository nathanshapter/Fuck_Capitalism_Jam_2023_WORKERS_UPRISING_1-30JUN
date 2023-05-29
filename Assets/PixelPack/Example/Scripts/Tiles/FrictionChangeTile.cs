/**
 * Add this script to surfaces that are slippery or sticky.
 * Friction delta values must be small numbers, so that overall SlowDownFactor + FrictionDelta doesn't exceed 1.
 */

using UnityEngine;

public class FrictionChangeTile : MonoBehaviour
{
    public float FrictionDelta = .06f;

    private void OnCollisionStay2D(Collision2D other)
    {        
        CharacterBase character = other.transform.GetComponent<CharacterBase>();
        if (character!= null)
        {
            character.FrictionDelta = FrictionDelta;
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        CharacterBase character = other.transform.GetComponent<CharacterBase>();
        if (character != null)
        {
            character.FrictionDelta = 0;
        }
    }
}
