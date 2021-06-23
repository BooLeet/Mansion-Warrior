using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Character attacker;
    float damage;
    Rigidbody rb;
    float speed = 10;
    GameObject hitEffect;

    public static void StartProjectile(Character attacker, Vector3 source, Vector3 direction, float damage, float speed, float radius, GameObject graphic, GameObject hitEffect,int layer)
    {
        GameObject obj = new GameObject("Projectile");
        obj.transform.position = source;
        obj.transform.forward = direction;
        Projectile projectile = obj.AddComponent<Projectile>();
        projectile.rb = obj.AddComponent<Rigidbody>();
        projectile.rb.useGravity = false;
        projectile.rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        projectile.rb.freezeRotation = true;

        projectile.attacker = attacker;
        projectile.damage = damage;
        projectile.speed = speed;
        projectile.hitEffect = hitEffect;
        obj.AddComponent<SphereCollider>().radius = Mathf.Abs(radius);
        obj.layer = layer;
        Instantiate(graphic, obj.transform);
    }

    private void FixedUpdate()
    {
        rb.MovePosition(transform.position + transform.forward * Time.fixedDeltaTime * speed);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Entity entityToDamage = collision.collider.GetComponent<Entity>();
        if (entityToDamage == attacker)
            return;
        if(entityToDamage)
            DamageFunction.SendDamageFeedback(attacker, entityToDamage, entityToDamage.TakeDamage(damage, attacker, attacker? attacker.Position : transform.position));

        Instantiate(hitEffect, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
