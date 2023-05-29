/**
 * Collectable Coin extends CollectableBase.
 * Collect (disable) the coin and call collector's ChangeCoinsAndScore method to give her a coin and score.
 */
public class Coin : CollectableBase
{
    protected override void Collect(PlayerCharacter collector)
    {
        base.Collect(collector);
        collector.ChangeCoinsAndScore(1, 100); //And one coin and 100 score
    }
}
