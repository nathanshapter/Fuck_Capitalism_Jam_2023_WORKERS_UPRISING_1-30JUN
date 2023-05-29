/**
 * Ends level and starts level completion sequence when player collides with the tile.
 */

using UnityEngine;

public class EndTile : BaseTile
{
    public enum EndTileType { Crown, Baddie };
    public EndTileType Type;

    public Sprite CrownSprite;
    public Sprite BaddieSprite;

    private bool collected = false;

    private void OnValidate()
    {
        //End tile can be switched to use baddie sprite instead of crown sprite...
        SpriteRenderer renderer = GetComponentInChildren<SpriteRenderer>();
        if (Type == EndTileType.Crown)        
            renderer.sprite = CrownSprite;
        else
            renderer.sprite = BaddieSprite;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerCharacter player = collision.gameObject.GetComponent<PlayerCharacter>();
        if (player != null && !collected)
        {
            collected = true;

            if (Type == EndTileType.Crown)
                anim.SetTrigger("FromCrown");
            else
                anim.SetTrigger("FromBaddie");

            //Unsubscribe input, make the character walk off screen
            ApplicationController.Instance.StartLevelCompletionSequence(player);            
        }
    }
}
