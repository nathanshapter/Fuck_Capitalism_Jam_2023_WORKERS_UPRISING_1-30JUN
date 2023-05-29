/**
 *  Base class for different types of interactive tiles (breakable, coin tiles, etc.).
 */

using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BaseTile : PlatformerObject
{
    [HideInInspector]
    public Rigidbody2D Rb2D { get; private set; }

    [HideInInspector]
    public Collider2D collider2d { get; private set; }

    protected Animator anim;

    protected override void Start()
    {
        base.Start();

        //Get RigidBody2D component
        Rb2D = GetComponent<Rigidbody2D>();
        collider2d = GetComponentInChildren<Collider2D>();
        anim = GetComponentInChildren<Animator>();
    }
    /**
    * Add impulse force to move the tile (e.g. flying BrickPieces)
    * @param direction Force direction.
    * @param addForce  Impulse force to add.
    */
    public virtual void AddImpulseForce(Vector2 direction, float addForce)
    {
        if (Rb2D != null)
            Rb2D.AddForce(direction * addForce, ForceMode2D.Impulse);
    }

    /**
    * Return Collider2D bounds extents x. Side offset for top edge checking.
    */
    public float GetColliderBoundsSideOffset()
    {
        return collider2d.bounds.extents.x;
    }

    /**
    * Return Collider2D offset x.
    */
    public float GetColliderSideOffset()
    {
        return collider2d.offset.x;
    }

    protected virtual void SetActiveAndEnabled(bool active)
    {
        gameObject.SetActive(active);
    }

    protected List<T> GetOverlappingObjectColliders<T>()
    {
        Collider2D[] overlappingObjects = new Collider2D[10]; //<-- get max 10 objects (seems overkill, but really quite possible...)
        Rb2D.OverlapCollider(new ContactFilter2D(), overlappingObjects); //<-- check overlaps and collect them to overlappingObjects

        return overlappingObjects.ToList().Where(o => o != null && o.gameObject.GetComponent<T>() != null).Select(o => o.GetComponent<T>()).ToList(); 
    }
}
