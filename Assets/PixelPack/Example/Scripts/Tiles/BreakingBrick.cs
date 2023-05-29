/**
 * Tile that breaks to pieces when player collides with it from below.
 * BreakingBrick extends BounceTile and also bounces enemies.
 */

using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BreakingBrick : BounceTile
{    
    public bool AllowAutoBreak = true;
    public AudioClip BreakTileAudioClip;

    [HideInInspector]
    public static BreakingBrick SingleBrickToBreak;  //<-- this trick ensures that only single brick is hit at time

    private List<BrickPiece> brickPieces = new List<BrickPiece>();

    protected override void Start()
    {
        base.Start();

        brickPieces = GetComponentsInChildren<BrickPiece>().ToList();
        //Disable all pieces
        brickPieces.ForEach(b => b.gameObject.SetActive(false));        
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);

        if (playerCollidesFromBelow != null && AllowAutoBreak)
        {            
            SingleBrickToBreak = this;
            SingleBrickToBreak.Break();
        }
    }

    /**
    * Break the brick to BrickPiece pieces.
    */
    public void Break()
    {
        //Play brick breaking audio clip
        AudioController.Instance.PlaySoundEffect(BreakTileAudioClip);
        
        brickPieces.ForEach(b => {
            //Unparent from this trans
            b.transform.SetParent(null);
            b.gameObject.SetActive(true);
        });

        SetActiveAndEnabled(false);
    }

    /**
    * Extends Reset by parenting BrickPieces back to breaking brick.
    */
    public override void Reset()
    {
        base.Reset();

        //Parent back to this transform
        brickPieces.ForEach(b => {
            b.transform.SetParent(transform);
            b.Reset();
            b.gameObject.SetActive(false);
        });        
    }
}
