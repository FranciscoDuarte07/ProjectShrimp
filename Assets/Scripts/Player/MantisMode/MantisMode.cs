using UnityEngine;

public class MantisMode : CombatMode
{
    public float attackDuration = 0.3f;  
    public float attackCooldown = 0.45f; 
    public float attackDamage = 15f;
    public float attackRange = 1.2f;

    private float attackTimer = 0f;
    private float cooldownTimer = 0f;
    private bool isAttacking = false;
    private int comboCount = 0;
    private float comboResetTimer = 0f;
    private const float ComboWindow = 1.2f;

    public float megaPunchDamage = 60f;
    public float megaPunchRange = 1.8f;
    private float specialCooldownTimer = 0f;

    public override string ModeName => "Mantis";

    public MantisMode(PlayerController owner) : base(owner) { }

    public override void OnEnter()
    {
        isAttacking = false;
        cooldownTimer = 0f;
    }

    public override void OnExit()
    {
        isAttacking = false;
    }

    public override void OnUpdate()
    {
        if (attackTimer > 0f) attackTimer -= Time.deltaTime;
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;
        if (specialCooldownTimer > 0f) specialCooldownTimer -= Time.deltaTime;

        if (comboResetTimer > 0f)
        {
            comboResetTimer -= Time.deltaTime;
            if (comboResetTimer <= 0f) comboCount = 0;
        }

        if (isAttacking && attackTimer <= 0f)
            EndAttack();
    }

    public override void PrimaryAction()
    {
        if (cooldownTimer > 0f) return;

        StartAttack();
    }

    private void StartAttack()
    {
        isAttacking = true;
        attackTimer = attackDuration;
        cooldownTimer = attackCooldown;
        comboCount = (comboCount + 1) % 3; 
        comboResetTimer = ComboWindow;

        string animTrigger = comboCount == 0 ? "AttackFinisher" : $"Attack{comboCount}";
        //owner.Animator?.SetTrigger(animTrigger);

        PerformHitDetection(attackRange, attackDamage, false);
    }

    private void EndAttack()
    {
        isAttacking = false;
    }

    public override void SpecialAction()
    {
        if (specialCooldownTimer > 0f) return;
        specialCooldownTimer = player.MegaPunchCooldown;

        PerformHitDetection(megaPunchRange, megaPunchDamage, armorOnly: true);
        SpawnWaterCurrent();
    }

    private void SpawnWaterCurrent()
    {
        if (player.WaterCurrentPrefab == null) return;

        Vector2 direction = player.IsRight ? Vector2.right : Vector2.left;
        Vector3 spawnPosition = player.transform.position + (Vector3)(direction * 0.6f);
        Quaternion rotation = Quaternion.Euler(0f, 0f, player.IsRight ? 0f : 180f);

        GameObject current = Object.Instantiate(player.WaterCurrentPrefab, spawnPosition, rotation);
        WaterCurrent wc = current.GetComponent<WaterCurrent>();
        if (wc != null)
            wc.Initialize(direction, player.EnemyLayer);
    }

    private void PerformHitDetection(float range, float damage, bool armorOnly)
    {
        Vector2 attackDirection = player.IsRight ? Vector2.right : Vector2.left;
        Vector2 attackOrigin = (Vector2)player.transform.position + attackDirection * 0.5f;

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            attackOrigin,
            range,
            player.EnemyLayer
        );

        string sourceType = armorOnly ? "megapunch" : "melee";

        foreach (Collider2D hit in hits)
        {
            IDamageable damageable = hit.GetComponent<IDamageable>();
            damageable?.TakeDamage(attackDamage, player.transform.position, sourceType);
        }
    }

    public void DrawGizmos(Transform origin, bool isFacingRight)
    {
        Vector2 dir = isFacingRight ? Vector2.right : Vector2.left;

        Gizmos.color = isAttacking ? Color.red : Color.yellow;
        Gizmos.DrawWireSphere((Vector2)origin.position + dir * 0.5f, attackRange);

        Gizmos.color = new Color(0f, 0.6f, 1f, 0.5f);
        Gizmos.DrawWireSphere((Vector2)origin.position + dir * 0.5f, megaPunchRange);
    }
}
