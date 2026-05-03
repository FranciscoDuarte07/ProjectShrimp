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

    public override string ModeName => "Mantis";

    public MantisMode(PlayerController owner) : base(owner) { }

    public override void OnEnter()
    {
        Debug.Log("[Shrimpy] Activando Modo Mantis");
        isAttacking = false;
        cooldownTimer = 0f;
    }

    public override void OnExit()
    {
        Debug.Log("[Shrimpy] Saliendo de Modo Mantis");
        isAttacking = false;
    }

    public override void OnUpdate()
    {
        if (attackTimer > 0f) attackTimer -= Time.deltaTime;
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;
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

        PerformHitDetection();

        Debug.Log($"[Shrimpy] ¡Golpe! Combo: {comboCount}");
    }

    private void EndAttack()
    {
        isAttacking = false;
    }

    private void PerformHitDetection()
    {
        Vector2 attackDirection = player.IsFacingRight ? Vector2.right : Vector2.left;
        Vector2 attackOrigin = (Vector2)player.transform.position + attackDirection * 0.5f;

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            attackOrigin,
            attackRange,
            player.EnemyLayer
        );

        foreach (Collider2D hit in hits)
        {
            IDamageable damageable = hit.GetComponent<IDamageable>();
            damageable?.TakeDamage(attackDamage, player.transform.position);
            Debug.Log($"[Shrimpy] Golpeó a {hit.name} por {attackDamage} daño.");
        }
    }

    public void DrawGizmos(Transform origin, bool isFacingRight)
    {
        Vector2 dir = isFacingRight ? Vector2.right : Vector2.left;
        Gizmos.color = isAttacking ? Color.red : Color.yellow;
        Gizmos.DrawWireSphere((Vector2)origin.position + dir * 0.5f, attackRange);
    }
}
