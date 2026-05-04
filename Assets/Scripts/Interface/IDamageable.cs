using UnityEngine;

public interface IDamageable
{
    void TakeDamage(float amount, Vector2 sourcePosition, string sourceType = "generic");
}
