using UnityEngine;

public class BubbleProjectile : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private float damage = 10f;
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private LayerMask hitLayers;

    [Header("VFX")]
    [SerializeField] private GameObject popEffect;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & hitLayers) == 0) return;

        IDamageable target = other.GetComponent<IDamageable>();
        target?.TakeDamage(damage, transform.position);

        Pop();
    }

    private void Pop()
    {
        if (popEffect != null)
            Instantiate(popEffect, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
