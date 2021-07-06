using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAnimator : MonoBehaviour
{
    [Header("References")]
    public Animator animator;
    public AICharacter character;
    public DestructableObject destructableObject;
    public Transform attackSource;
    [System.Serializable]
    public struct AttackAnimation
    {
        public string[] triggers;
        public float duration;
    }
    [Header("Triggers")]
    public string idleTrigger;
    public string walkTrigger;
    public AttackAnimation attackAnimation;
    private int attackTriggerIndex = 0;

    [Header("Audio")]
    public AudioClip attackSound;
    public AudioClip walkSound;
    public AudioClip[] miscSounds;

    [Space]
    public GameObject damageEffectPrefab;

    public void Idle()
    {
        SetTrigger(idleTrigger);
    }

    public void Walk()
    {
        SetTrigger(walkTrigger);
    }

    public void Attack()
    {
        SetTrigger(attackAnimation.triggers[attackTriggerIndex++]);
        attackTriggerIndex %= attackAnimation.triggers.Length;
    }

    public void Alarm()
    {

    }

    public void Death(Vector3 explosionSource)
    {
        destructableObject.Destruct(explosionSource);
    }

    public void AttackDamageCall()
    {
        character.AttackDamageCall();
    }

    protected void SetTrigger(string trigger)
    {
        if (trigger.Length == 0)
            return;

        animator.SetTrigger(trigger);
    }

    public Vector3 GetAttackSource()
    {
        return attackSource.position;
    }

    public void DamageEffect()
    {
        if (damageEffectPrefab)
            Instantiate(damageEffectPrefab, character.Position, Quaternion.identity);
    }

    #region Audio

    public void PlayAttackSound()
    {
        Audio.PlaySFX(attackSound, transform.position, transform, 0.8f);
    }

    public void PlayWalkSound()
    {
        Audio.PlaySFX(walkSound, transform.position, transform, 0.9f);
    }

    public void PlayMiscSound(int soundIndex)
    {
        if (soundIndex < 0 || soundIndex == miscSounds.Length)
            return;

        Audio.PlaySFX(miscSounds[soundIndex], transform.position + transform.forward, transform, 1f);
    }

    #endregion
}
