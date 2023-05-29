/**
 *  Scripted animation sequences for CharacterBase type characters.
 */

using System;
using System.Collections;
using UnityEngine;
using System.Linq;

public class CharacterAnimationSequence : ScriptableObject
{
    public IEnumerator BounceDeathSequence(CharacterBase _character, Action _animationCompletedCallback, string _animationName, float _delay = .3f, float _bounceUpSpeed = 4f, float _fallAddition = -20f, float _fallTime = 2f)
    {
        //Bounce up a bit and then fall.       

        Vector2 position = _character.Rb2D.position;

        _character.TriggerAnimation(_animationName); //<-- trigger death sprite animation
        yield return new WaitForSeconds(_delay);

        float startTime = Time.time;
        while (Time.time - startTime < _fallTime)
        {
            position.y += _bounceUpSpeed * Time.fixedDeltaTime;
            _bounceUpSpeed += _fallAddition * Time.fixedDeltaTime;

            _character.Rb2D.position = position;
                                    
            yield return null;
        }

        //Callback after the animation:
        _animationCompletedCallback();
    }

    public IEnumerator GibDeathSequence(CharacterBase _character, Action _animationCompletedCallback, float _waitAfterDeath = 2f)
    {
        //Hide original sprite
        _character.spriteRenderer.enabled = false;
        
        //TODO: if player's sprite sheet changes, these sprite animations must change too!
        DeathEffectObject deathEffectObj = _character.DeathEffects.FirstOrDefault(de => de.DeathType == CharacterBase.TakeDamageType.Gibbed);
        GameObject objInstance = Instantiate(_character.DeathEffects.FirstOrDefault(de => de.DeathType == CharacterBase.TakeDamageType.Gibbed).ObjectToInstantiate, _character.Rb2D.position,Quaternion.identity);

        //Get SimpleSpriteAnimator component and switch it's sprite sheet to player's current suit sheet:
        if (_character is PlayerController player)
        {
            objInstance.GetComponent<PlayerGibDeathController>().upperHalf.GetComponent<SimpleSpriteAnimator>().SwitchRuntimeSpriteSheet(player.currentRenderSpriteSheet);
            objInstance.GetComponent<PlayerGibDeathController>().lowerHalf.GetComponent<SimpleSpriteAnimator>().SwitchRuntimeSpriteSheet(player.currentRenderSpriteSheet);
        }

        objInstance.SetActive(true);
        
        yield return new WaitForSeconds(_waitAfterDeath);

        //Show original sprite again...
        _character.spriteRenderer.enabled = true;

        //Callback after the animation:
        _animationCompletedCallback();
    }

    public IEnumerator DrownDeathSequence(CharacterBase _character, Action _animationCompletedCallback, float _waitAfterDeath = 2f)
    {
        //Hide original sprite
        _character.spriteRenderer.enabled = false;

        //TODO: if player's sprite sheet changes, these sprite animations must change too!
        DeathEffectObject deathEffectObj = _character.DeathEffects.FirstOrDefault(de => de.DeathType == CharacterBase.TakeDamageType.Drowned);
        GameObject objInstance = Instantiate(_character.DeathEffects.FirstOrDefault(de => de.DeathType == CharacterBase.TakeDamageType.Drowned).ObjectToInstantiate, _character.Rb2D.position, Quaternion.identity);

        //Get SimpleSpriteAnimator component and switch it's sprite sheet to player's current suit sheet:
        if (_character is PlayerController player)
            objInstance.GetComponent<SimpleSpriteAnimator>().SwitchRuntimeSpriteSheet(player.currentRenderSpriteSheet);

        objInstance.SetActive(true);

        yield return new WaitForSeconds(_waitAfterDeath);

        //Show original sprite again...
        _character.spriteRenderer.enabled = true;

        //Callback after the animation:
        _animationCompletedCallback();
    }
}
