using UnityEngine;

public class BasicEnemy : EnemyBase
{
    public override void Awake()
    {
        hasArmor = false;
        base.Awake();
    }

    public override void OnHit(float amount, Vector2 sourcePosition)
    {
        Debug.Log($"{name} recibió {amount} daño. HP: {currentHP} / {maxHP}");
    }

    public override void OnDeath()
    {
        Debug.Log($"{name} eliminado.");
    }
}
