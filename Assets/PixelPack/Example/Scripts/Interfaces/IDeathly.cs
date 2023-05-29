using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDeathly
{
    void KillCharacter(CharacterBase character, CharacterBase.TakeDamageType deathType);    
}
