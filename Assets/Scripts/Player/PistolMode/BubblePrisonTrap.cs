using UnityEngine;
using System.Collections;

public class BubblePrisonTrap : MonoBehaviour
{
    [Header("Trap Settings")]
    [SerializeField] private float trapLifetime = 5f;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Prison Setting")]
    [SerializeField] private float imprisonDPS = 8f;
    [SerializeField] private float imprisonDuration = 3f;
    [SerializeField] private float imprisonTickRate = 0.3f;

    [Header("Explosion Settings")]
    [SerializeField] private float explosionRadius = 3.5f;
    [SerializeField] private float exposionDamage = 25f;
    [SerializeField] private float explosionArmorDmg = 10f;

    [Header("VFX")]
    [SerializeField] private GameObject captureEffect;
    [SerializeField] private GameObject explosionEffect;
    [SerializeField] private GameObject popEffect;

    private bool isOccupied = false;
    private GameObject prisoner = null;
    private EnemyBase prisonerEnemy = null;
    private Coroutine imprisonRoutine;

    private void Start()
    {
        Destroy(gameObject, trapLifetime);
    }

    private void OnDestroy()
    {
        if (!isOccupied && popEffect != null)
            Instantiate(popEffect, transform.position, Quaternion.identity);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isOccupied) return;
        if (((1 << other.gameObject.layer) & enemyLayer) == 0) return;

        EnemyBase enemy = other.GetComponent<EnemyBase>();
        if (enemy == null) return;

        if (enemy.hasArmor && enemy.currentArmorHP > 0f) return;

        Capture(other.gameObject, enemy);
    }

    private void Capture(GameObject target, EnemyBase enemy)
    {
        isOccupied = true;
        prisoner = target;
        prisonerEnemy = enemy;

        if (captureEffect != null)
            Instantiate(captureEffect, prisoner.transform.position, Quaternion.identity);

        IPrisonable prisionable = prisoner.GetComponent<IPrisonable>();
        prisionable?.OnImprisoned();

        imprisonRoutine = StartCoroutine(ImprisonRoutine());
    }

    private IEnumerator ImprisonRoutine()
    {
        float elapsed = 0f;

        while (elapsed < imprisonDuration)
        {
            yield return new WaitForSeconds(imprisonTickRate);
            elapsed += imprisonTickRate;

            if (prisoner == null || prisonerEnemy == null)
            {
                TriggerExplosion(true);
                yield break;
            }

            float tickDamage = imprisonDPS * imprisonTickRate;
            prisonerEnemy.TakeDamage(tickDamage, transform.position, "bubble");

            if (prisonerEnemy == null || prisonerEnemy.currentHP <= 0f)
            {
                TriggerExplosion(true);
                yield break;
            }
        }

        Release();
    }

    private void TriggerExplosion(bool killedPrisoner)
    {
        if (explosionEffect != null)
            Instantiate(explosionEffect, transform.position, Quaternion.identity);

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius, enemyLayer);

        foreach(Collider2D hit in hits)
        {
            if (hit == null) continue;

            EnemyBase enemy = hit.GetComponent<EnemyBase>();
            IDamageable damageable = hit.GetComponent<IDamageable>();
            if (damageable == null) continue;

            bool hasIntactArmor = enemy != null && enemy.hasArmor && enemy.currentArmorHP > 0f;

            if (hasIntactArmor)
            {
                damageable.TakeDamage(explosionArmorDmg, transform.position, "bubbleexplosion");
            }
            else
            {
                damageable.TakeDamage(exposionDamage, transform.position, "bubble");
            }
        }

        if (imprisonRoutine != null)
            StopCoroutine(imprisonRoutine);

        Destroy(gameObject);
    }

    private void Release()
    {
        if (prisoner != null)
        {
            IPrisonable prisonable = prisoner.GetComponent<IPrisonable>();
            prisonable?.OnReleased();
        }

        isOccupied = false;
        prisoner = null;
        prisonerEnemy = null;

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.2f, 0.6f, 1f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
