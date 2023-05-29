/**
 *  Adds and updates rectangular EdgeCollider2D for GameObject. It makes easier to setup EdgeColliders
 *  for tiles with varying sizes. You can't use adjacent BoxCollider2Ds, because RigidBody2D's
 *  stick to their vertical edges, even if they are perfectly aligned.
 */

using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(SpriteRenderer))]
public class AutoEdgeCollider2D : MonoBehaviour
{
    public float ExtendSizeX = 0f;
    public float ExtendSizeY = 0f;
    private void LateUpdate()
    {
        //Poll sprite renderer size and adjust EdgeCollider2D points
        if (Application.isEditor && !Application.isPlaying) {
            if (GetComponent<EdgeCollider2D>() == null)
                gameObject.AddComponent<EdgeCollider2D>();

            Vector2 size = GetComponent<SpriteRenderer>().size;            
            EdgeCollider2D ec = GetComponent<EdgeCollider2D>();
            ec.offset = new Vector2(-ExtendSizeX, -ExtendSizeY);
            ec.points = new Vector2[5] { 
                new Vector2(0,0), new Vector2(size.x + ExtendSizeX * 2f, 0), 
                new Vector2(size.x + ExtendSizeX * 2f, size.y + ExtendSizeY * 2f), new Vector2(0, size.y + ExtendSizeY * 2f), new Vector2(0,0) };
        }
    }
}
