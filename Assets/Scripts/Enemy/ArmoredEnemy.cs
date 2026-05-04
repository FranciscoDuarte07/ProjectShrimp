using UnityEngine;

public class ArmoredEnemy : EnemyBase
{
    public override void Awake()
    {
        hasArmor = true;
        base.Awake();
    }

    public override void OnArmorHit(float amount, Vector2 sourcePosition)
    {
        Debug.Log($"{name}: Armadura golpeada. Armadura: {currentArmorHP}/{maxArmorHP}");
    }

    public override void OnArmorBroken()
    {
        base.OnArmorBroken();
    }

    public override void OnArmorRestored()
    {
        base.OnArmorRestored();
    }

    public override void OnArmorBlocked(string sourceType)
    {
        base.OnArmorBlocked(sourceType);
    }

    public override void OnHit(float amount, Vector2 sourcePosition)
    {
        Debug.Log($"{name} recibió {amount} daño (armadura rota). HP: {currentHP}/{maxHP}");
    }

    public override void OnDeath()
    {
        base.OnDeath();
    }
}
