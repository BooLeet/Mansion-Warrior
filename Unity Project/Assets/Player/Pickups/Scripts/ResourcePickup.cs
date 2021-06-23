using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ResourcePickup : MonoBehaviour
{
    public AudioClip pickupSound;
    public bool DestroyOnPickup { get; set; }
    public Color indicatorColor = Color.white;

    private void Start()
    {
        GameManager.Instance.RegisterPickup(this);
    }

    public void ResetPickup()
    {
        if (DestroyOnPickup)
        {
            GameManager.Instance.UnregisterPickup(this);
            Destroy(gameObject);
        }
        else
            gameObject.SetActive(true);
    }

    private void Update()
    {
        UpdateFunction();
        if (PlayerCharacter.Instance == null)
            return;

        float distanceToPlayer = Vector3.Distance(PlayerCharacter.Instance.transform.position, transform.position);
        if (distanceToPlayer < PlayerCharacter.Instance.stats.pickupDistance)
            PickUp(PlayerCharacter.Instance);
    }

    protected virtual void UpdateFunction() { }

    protected virtual void StartFunction() { }

    private void PickUp(PlayerCharacter player)
    {
        if (!CanAddResource(player))
            return;
        Audio.PlaySFX(pickupSound, transform.position, null, 0.9f);
        AddResource(player);
        player.PickupIndicator(indicatorColor);
        if (DestroyOnPickup)
        {
            GameManager.Instance.UnregisterPickup(this);
            Destroy(gameObject);
        }
        else
            gameObject.SetActive(false);
    }

    protected abstract void AddResource(PlayerCharacter player);

    protected abstract bool CanAddResource(PlayerCharacter player);
}
