/**
 * Adds constant force to character when on this surface. 
 * For conveyor belts etc.
 */

using UnityEngine;

public class AddForceTile : MonoBehaviour
{
    public Vector2 Force;                   //Force is added constantly, so use small values between -10/+10.    
    CharacterBase character;

    private void OnCollisionEnter2D(Collision2D other)
    {
        character = other.transform.GetComponent<CharacterBase>();        
    }

    private void FixedUpdate()
    {
        if (character != null) 
        {
            if (character.grounded == null)
            {
                character.ExternalForce = Vector2.zero;
                character = null;
            }
            else
                character.ExternalForce = Force;
        }
    }    
}
