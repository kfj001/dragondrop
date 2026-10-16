using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletLogic : MonoBehaviour
{
    private int lifeSpan = 260;
    public new Rigidbody2D rigidbody;
    [Range(.1f, 20f)] public float velocity;
    public GameObject impactEffect;

    void Start()
    {
        rigidbody.velocity = transform.right * velocity;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Collide(collision);
    }

    // Update is called once per frame
    void Update()
    {
        lifeSpan -= 1;

        if (lifeSpan <= 0)
        {
            Fizzle();
        }
    }

    void Collide(Collision2D collision)
    {
        Fizzle();
        // impact against parameterized gameObject
        Instantiate(impactEffect, gameObject.transform.position, gameObject.transform.rotation);
        // Tell this gameObject's enemy it's been hit
        collision.gameObject.GetComponent<EnemyScript>()?.Struck();
    }

    void Fizzle()
    {
        Destroy(this.gameObject);
    }
}
