using UnityEngine;

public class PistolMode : CombatMode
{
    public float heatPerShot = 12f;
    public float cooldownRate = 12f;
    public float cooldownDelay = 1.5f;
    public float overheatPenaltyTime = 2f;

    private float heatLevel = 0f;
    private bool isOverheated = false;
    private float timeSinceShot = 0f;
    private float shootTimer = 0f;
    private float penaltyTimer = 0f;

    private float specialCooldownTimer = 0f;

    public float HeatLevel => heatLevel;
    public bool IsOverheated => isOverheated;
    public override string ModeName => "Pistola";

    public PistolMode(PlayerController owner) : base(owner) { }

    public override void OnEnter()
    {
        RefreshSliderVisibility();
    }

    public override void OnExit() 
    {
        if (player.CannonOverchargeUI != null)
            player.CannonOverchargeUI.SetActive(false);
    }

    public override void OnUpdate()
    {
        shootTimer -= Time.deltaTime;
        timeSinceShot += Time.deltaTime;

        if (specialCooldownTimer > 0f)
            specialCooldownTimer -= Time.deltaTime;

        if (isOverheated)
            HandleOverheatPenalty();
        else
            HandleCooldown();

        player.CannonHeatUI?.UpdateHeat(heatLevel, isOverheated);
        RefreshSliderVisibility();
    }

    public override void PrimaryAction()
    {
        if (isOverheated || shootTimer > 0f) return;
        Shoot();
    }

    private void Shoot()
    {
        if (player.BubblePrefab != null && player.ShootPoint != null)
        {
            GameObject bubble = Object.Instantiate(
                player.BubblePrefab,
                player.ShootPoint.position,
                player.ShootPoint.rotation);

            Rigidbody2D rgbd2d = bubble.GetComponent<Rigidbody2D>();
            if (rgbd2d != null)
            {
                float dir = player.IsRight ? 1f : -1f;
                rgbd2d.linearVelocity = new Vector2(dir * player.BulletSpeed, 0f);
            }  
        }

        heatLevel = Mathf.Clamp(heatLevel + heatPerShot, 0f, 100f);
        timeSinceShot = 0f;

        if (heatLevel >= 100f)
            TriggerOverheat();
    }

    public override void SpecialAction()
    {
        if (specialCooldownTimer > 0f) return;
        if (player.BubblePrisonPrefab == null) return;

        specialCooldownTimer = player.BubblePrisonCooldown;

        Object.Instantiate(
            player.BubblePrisonPrefab,
            player.transform.position,
            Quaternion.identity);
    }

    private void HandleOverheatPenalty()
    {
        penaltyTimer -= Time.deltaTime;
        if (penaltyTimer <= 0f)
            ResolveOverheat();
    }

    private void HandleCooldown()
    {
        if (timeSinceShot >= cooldownDelay && heatLevel > 0f)
            heatLevel = Mathf.Max(0f, heatLevel - cooldownRate * Time.deltaTime);
        
    }

    private void TriggerOverheat()
    {
        isOverheated = true;
        penaltyTimer = overheatPenaltyTime;
        player.OnCannonOverheated?.Invoke();
    }

    private void ResolveOverheat()
    {
        isOverheated = false;
        heatLevel = 70f;
        player.OnCannonCooled?.Invoke();
    }

    private void RefreshSliderVisibility()
    {
        if (player.CannonOverchargeUI == null) return;
        player.CannonOverchargeUI?.SetActive(heatLevel > 0f || isOverheated);
    }
}