﻿using UnityEngine;
using System.Collections;

public class BulletStandardDestroy : MonoBehaviour {

    public float maxDamage;
    public float damage;
    public float bulletSpeed;
    public float rateOfFire;
    public float bulletOffsetMultiplier;

    public bool destroy = true;

    public CubeMovement owner;

    void Start()
    {
        Destroy(gameObject, 10.0f);

    }

    void OnCollisionEnter(Collision collision)
    {

        var hit = collision.gameObject;
        var health = hit.GetComponent<PlayerHealth>();

        if (health != null)
        {
            if (health.isLocalPlayer)
            {
                //var animation = GameObject.Find("UI").GetComponent<FloatingPoints>();
               // if (damage > 0.0f)
                   // animation.startDamageAnimation(damage);

            }
            //owner.score += (int)damage;
            health.TakeDamage(damage,owner);
        }
        if(destroy)
            Destroy(gameObject);
    }
}
