using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float bulletSpeed;
    public int amountDamage = 1;

    public Rigidbody2D rgbd2D;
    public Vector2 moveDir;
    public GameObject impactBullet;

    // Update is called once per frame
    void Update()
    {
        rgbd2D.velocity = moveDir * bulletSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy")
        {
   //         other.GetComponent<EnemyHealthController>().DamageEnemy(amountDamage);
        }

        if (other.tag == "Boss")
        {
   //        BossHealthController.instance.TakeDamage(amountDamage);
        }

        if (impactBullet != null)
        {
            Instantiate(impactBullet, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
