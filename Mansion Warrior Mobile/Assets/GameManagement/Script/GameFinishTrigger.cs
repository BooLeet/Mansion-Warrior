using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFinishTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerCharacter player = other.GetComponent<PlayerCharacter>();
        if (player)
            GameMode.Instance.SuccessEnd();
    }

}
