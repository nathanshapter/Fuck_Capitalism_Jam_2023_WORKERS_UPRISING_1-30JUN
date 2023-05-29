/**
 * Simple Box or CircleCollider2D that kills CharacterBase characters.
 * This can be added to several types of visible or invisible obstacles to kill characters.
 * Implements IDeathly interface to kill the CharacterBase type of characters.
 */

using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class KillZone : MonoBehaviour, IDeathly
{
    public bool DrawGizmo = true;
    public Color GizmoColor = new Color(1, 0, 0, 0.5f);
    public CharacterBase.TakeDamageType DeathType = CharacterBase.TakeDamageType.Undefined;

    /**
    * Implements IDeathly interface to kill characters with different visuals
    * @param character Character to kill.
    * @param deathType Death type of the character to kill.
    */
    public void KillCharacter(CharacterBase character, CharacterBase.TakeDamageType deathType)
    {
        character.Die(deathType);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        CharacterBase character = collision.gameObject.GetComponent<CharacterBase>();
        if (character != null && character.DieInKillZone)
            KillCharacter(character, DeathType);
    }
    private void OnDrawGizmos()
    {
        if (!DrawGizmo)
            return;

        if (transform.localScale == Vector3.one)
        {
            Collider2D coll2D = GetComponent<Collider2D>();
            Gizmos.color = GizmoColor;

            //Draw box or sphere gizmo depending on collider type:
            if (coll2D is BoxCollider2D boxCollider2D)                            
                Gizmos.DrawCube(boxCollider2D.transform.position + new Vector3(boxCollider2D.offset.x, boxCollider2D.offset.y, 0), new Vector3(boxCollider2D.size.x, boxCollider2D.size.y, .1f));            
            else if (coll2D is CircleCollider2D circleCollider2D)            
                Gizmos.DrawSphere(circleCollider2D.transform.position, circleCollider2D.radius);                        
        }
        else
            Debug.LogError("Scale the collider, not object!");
    }
}