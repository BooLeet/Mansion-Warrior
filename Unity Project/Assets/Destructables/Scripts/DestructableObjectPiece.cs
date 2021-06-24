using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableObjectPiece : MonoBehaviour {
    private BoxCollider boxCollider;
    private Rigidbody rb;
    private bool destructionEnabled = false;
    private float timeCounter = 0;
    private Vector3 startingScale;
    private float effectTime;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        boxCollider.enabled = false;
    }

    public void Destruct(float explosionForce,Vector3 explosionPosition,float explosionRadius,float collisionActiveTime, int layer)
    {
        gameObject.layer = layer;
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (!meshFilter)
            return;

        transform.parent = null;
        DestructableObjectPieceRegistry.GetInstance().Register(this);
        boxCollider.enabled = true;

        rb =  gameObject.AddComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.AddExplosionForce(explosionForce, explosionPosition, explosionRadius);

        timeCounter = effectTime = collisionActiveTime;
        destructionEnabled = true;
        startingScale = transform.localScale;
    }

    public void SetupCollider()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    private void Update()
    {
        if (!destructionEnabled)
            return;

        transform.localScale = Vector3.Lerp(Vector3.zero, startingScale, Mathf.Clamp(timeCounter / effectTime, 0.001f, 1));
        timeCounter -= Time.deltaTime;
        if (timeCounter <= 0)
            Destroy(gameObject);
    }
}
