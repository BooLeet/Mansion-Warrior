using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Poloskun : Interactable
{
    public NavMeshAgent navMeshAgent;
    public Transform graphicTransform;
    GameModeSurvival gameMode;
    public int pointsToGadza = 3000;
    private int currentPoints;
    private int pointTracker;

    public AudioClip gadzaSound;
    public AudioClip explosionSound;
    public AudioClip[] talkSound;
    PlayerCharacter player;
    public float maxScale = 5;

    public GameObject gadzaEffect;

    void Start()
    {
        gameMode = GameMode.Instance as GameModeSurvival;
        player = PlayerCharacter.Instance;
    }

    private bool CanGadza()
    {
        return currentPoints >= pointsToGadza;
    }

    public override bool CanInteract()
    {
        return true;
    }

    public override string GetPrompt(Character interactingCharacter)
    {
        if (CanGadza())
            return "ГАДЗА";
        else
            return currentPoints.ToString() + " / " + pointsToGadza.ToString();
    }

    protected override void _Interact(Character interactingCharacter)
    {
        if (CanGadza())
        {
            Audio.PlaySFX(gadzaSound, transform.position, transform, 0.8f);
            Audio.PlaySFX(explosionSound, transform.position, transform, 0.8f);
            currentPoints -= pointsToGadza;
            Instantiate(gadzaEffect, transform.position + Vector3.up, transform.rotation);
            Damage.ExplosiveDamage(transform.position, 1000, 100, player, int.MaxValue, false);
        }
        else
        {
            Audio.PlaySFX(talkSound[Random.Range(0, talkSound.Length)], transform.position, transform, 0.5f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        int delta = gameMode.PTS - pointTracker;
        currentPoints += delta;
        pointTracker = gameMode.PTS;

        if(player == null)
        {
            player = PlayerCharacter.Instance;
            return;
        }

        navMeshAgent.SetDestination(player.Position);
        graphicTransform.localScale = Vector3.one * (1 + Mathf.Clamp(currentPoints / pointsToGadza, 0, 1) * maxScale);
    }
}
