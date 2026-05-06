using UnityEngine;

public abstract class EnemyBase : MonoBehaviour, IDamageable
{
    [Header("Stats base")]
    public float maxHP = 30f;
    public float currentHP;

    [Header("Armadura")]
    public bool hasArmor = false;
    public float maxArmorHP = 20f;
    public float currentArmorHP;
    private bool armorBroken = false;

    [Header("Armadura - fuentes que la rompen")]
    [SerializeField] private string[] armorBrokenSource 
        = { "melee", 
            "megapunch", 
            "watercurrent", 
            "bubbleexplosion" };

    public virtual void Awake()
    {
        currentHP = maxHP;
        currentArmorHP = hasArmor ? maxArmorHP : 0f;
    }

    public virtual void TakeDamage(float amount, Vector2 sourcePosition, string sourceType = "generic")
    {
        if (hasArmor && !armorBroken)
        {
            if (CanDamageArmor(sourceType))
                ApplyArmorDamage(amount, sourcePosition);
            else
                OnArmorBlocked(sourceType);
            return;
        }
        ApplyDamage(amount, sourcePosition);
    }

    private bool CanDamageArmor(string sourceType)
    {
        foreach (string s in armorBrokenSource)
            if (s == sourceType) return true;
        return false;
    }

    private void ApplyArmorDamage(float amount, Vector2 sourcePosition)
    {
        currentArmorHP -= amount;
        OnArmorHit(amount, sourcePosition);

        if (currentArmorHP <= 0f)
        {
            currentArmorHP = 0f;
            armorBroken = true;
            OnArmorBroken();
        }
    }

    public virtual void RestoreArmor(float amount = -1f)
    {
        if (!hasArmor) return;

        currentArmorHP = amount < 0 ? maxArmorHP : Mathf.Min(currentArmorHP + amount, maxArmorHP);
        armorBroken = currentArmorHP <= 0f;
        OnArmorRestored();
    }

    private void ApplyDamage(float amount, Vector2 sourcePosition)
    {
        currentHP -= amount;
        OnHit(amount, sourcePosition);

        if (currentHP <= 0f)
        {
            currentHP = 0f;
            Die();
        }
    }

    public virtual void Die()
    {
        OnDeath();
        Destroy(gameObject);
    }  

    public virtual void OnHit(float amount, Vector2 sourcePosition) { }
    public virtual void OnArmorHit(float amount, Vector2 sourcePosition) { }
    public virtual void OnArmorBroken() { Debug.Log($"{name}: ¡Armadura rota!"); }
    public virtual void OnArmorRestored() { Debug.Log($"{name}: Armadura restaurada."); }
    public virtual void OnArmorBlocked(string sourceType) { Debug.Log($"{name}: {sourceType} bloqueado por armadura."); }
    public virtual void OnDeath() { Debug.Log($"{name}: muerto."); }

}
