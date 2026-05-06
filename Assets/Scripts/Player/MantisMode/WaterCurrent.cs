using UnityEngine;
using System.Collections.Generic;

public class WaterCurrent : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifetime = 1.5f;
    [SerializeField] private float damagePerHit = 20f;
    [SerializeField] private float hitCooldown = 0.3f;

    [Header("VFX")]
    [SerializeField] private GameObject splashEffect;

    private Vector2 direction;
    private LayerMask enemyLayer;
    private Rigidbody2D rgbd;

    private Dictionary<Collider2D, float> hitTimers 
              = new Dictionary<Collider2D, float>();

    private void OnTriggerEnter2D(Collider2D other) => TryDamage(other);
    private void OnTriggerStay2D(Collider2D other) => TryDamage(other);

    public void Initialize(Vector2 _direction, LayerMask _layer)
    {
        direction = _direction.normalized;
        enemyLayer = _layer;
    }

    private void Awake()
    {
        rgbd = GetComponent<Rigidbody2D>();
        rgbd.gravityScale = 0f;
    }

    private void Start()
    {
        rgbd.linearVelocity = direction * speed;
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        var toRemove = new List<Collider2D>();
        foreach (var kv in hitTimers)
        {
            if (kv.Value <= Time.time)
                toRemove.Add(kv.Key);
        }
        foreach (var key in toRemove)
            hitTimers.Remove(key);
    }

    private void TryDamage(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & enemyLayer) == 0) return;

        if (hitTimers.TryGetValue(other, out float nexHit) && Time.time < nexHit) return;

        hitTimers[other] = Time.time + hitCooldown;

        IDamageable target = other.GetComponent<IDamageable>();
        target?.TakeDamage(damagePerHit, transform.position, "watercurrent");
    }

    private void OnDestroy()
    {
        if (splashEffect != null)
            Instantiate(splashEffect, transform.position, Quaternion.identity);
    }
}
