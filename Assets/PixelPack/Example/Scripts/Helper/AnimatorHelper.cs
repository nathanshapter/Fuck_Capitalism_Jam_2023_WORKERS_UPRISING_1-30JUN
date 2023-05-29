/**
 *  Animator helper class.
 */

using System;
using System.Collections;
using UnityEngine;

public class AnimatorHelper : MonoBehaviour {

    /**
    * Wait for currently playing (non-looping) animation to end and run callback.
    * @param _animator Animator to check.
    * @param _animationName Currently playing animation's string name.
    * @param Oncomplete Callback when animation ends.
    * @param _delayBeforeAnimation additional delay before starting to check the animation.
    * @param _triggerCompleteAtNormalizedTime Wait till animation reaches normalized time. Default 1.
    * @param _animatorLayerIndex Animator's layer index. 
    */
    public static IEnumerator CheckAnimationCompleted(Animator _animator, string _animationName, Action Oncomplete, float _delayBeforeAnimation = 0f, float _triggerCompleteAtNormalizedTime = 1f, int _animatorLayerIndex = 0)
    {
        if (_delayBeforeAnimation > 0f)
            yield return new WaitForSeconds(_delayBeforeAnimation);

        //Now this a bit hackish but even if there is NO transition and correct animation has started, the name of the animation is different for god knows why...
        while (!_animator.GetCurrentAnimatorStateInfo(_animatorLayerIndex).IsName(_animationName) || _animator.GetCurrentAnimatorStateInfo(_animatorLayerIndex).normalizedTime < _triggerCompleteAtNormalizedTime)        
            yield return null;
        
        if (Oncomplete != null)
            Oncomplete();
    }
}
