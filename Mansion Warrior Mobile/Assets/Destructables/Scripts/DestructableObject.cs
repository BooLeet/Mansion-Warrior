using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableObject : MonoBehaviour {
    public DestructableObjectPiece[] pieces;
    public float explosionForce = 1000;
    public float explosionRadius = 10;
    public float collisionActiveTime = 10;
    public int destructableObjectLayer = 16;
    public bool useGravity = false;
    public Transform explosionPositionOverride;

    public void Destruct(Vector3 explosionPosition)
    {
        if (explosionPositionOverride)
            explosionPosition = explosionPositionOverride.position;

        foreach (DestructableObjectPiece piece in pieces)
        {
            piece.Destruct(explosionForce, explosionPosition, explosionRadius, collisionActiveTime, destructableObjectLayer, useGravity);
        }
            
    }


    public void SetUpChildDestructables()
    {
        pieces = GetComponentsInChildren<DestructableObjectPiece>();
        foreach (DestructableObjectPiece piece in pieces)
        {
            piece.SetupCollider();
        }
    }

    public void ClearChildDestructables()
    {
        pieces = null;
    }
}
