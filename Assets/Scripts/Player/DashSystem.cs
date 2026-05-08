using UnityEngine;

public class DashSystem 
{
    private readonly PlayerController player;
    private readonly Rigidbody2D rgbd;

    [Header ("Settings")]
    private readonly GameObject afterimagePrefab;
    private readonly float afterimageInterval;
    private readonly float afterimageFadeTime;

    [Header ("States")]
    private bool isDashing = false;
    private float dashTimer = 0f;
    private float cooldownTimer = 0f;
    private float afterimageTimer = 0f;

    [Header ("Parameters")]
    private float activeDashSpeed = 0f;
    private float activeDir = 1f;
    private float originalGravity = 1f;

    public bool IsDashing => isDashing;
    public bool IsReady => cooldownTimer <= 0f && !isDashing;
    public float CooldownRatio => cooldownTimer;
   
    private readonly Transform graphicsTransform;
    public DashSystem(PlayerController player, Rigidbody2D rgbd, Transform graphicsTransform,
                      GameObject afterimagePrefab,
                      float afterimageInterval = 0.05f,
                      float afterimageFadeTime = 0.2f)
    {
        this.player = player;
        this.rgbd = rgbd;
        this.graphicsTransform = graphicsTransform;
        this.afterimagePrefab = afterimagePrefab;
        this.afterimageInterval = afterimageInterval;
        this.afterimageFadeTime = afterimageFadeTime;
    }

    public void OnUpdate()
    {
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;

        if (!isDashing) return;

        rgbd.linearVelocity = new Vector2(activeDir * activeDashSpeed, 0f);

        afterimageTimer -= Time.deltaTime;
        if (afterimageTimer <= 0f)
        {
            SpawnAfterimage();
            afterimageTimer = afterimageInterval;
        }

        dashTimer -= Time.deltaTime;
        if (dashTimer <= 0f)
            EndDash();
    }

    public void TryDash(bool isRight, float distance, float duration, float cooldown)
    {
        if (!IsReady) return;

        isDashing = true;
        dashTimer = duration;
        cooldownTimer = cooldown;
        afterimageTimer = 0f; 

        activeDir = isRight ? 1f : -1f;
        activeDashSpeed = distance / duration;

        originalGravity = rgbd.gravityScale;
        rgbd.gravityScale = 0f;
    }

    private void EndDash()
    {
        isDashing = false;
        rgbd.gravityScale = originalGravity;
        rgbd.linearVelocity = new Vector2(0f, rgbd.linearVelocity.y);
    }

    private void SpawnAfterimage()
    {
        if (afterimagePrefab == null) return;

        GameObject image = Object.Instantiate(
            afterimagePrefab,
            player.transform.position,
            player.transform.rotation);

        image.transform.localScale = graphicsTransform != null
            ? graphicsTransform.lossyScale
            : player.transform.localScale;

        AfterimageEffect effect = image.GetComponent<AfterimageEffect>();
        if (effect != null)
            effect.Initialize(afterimageFadeTime);
        else
            Object.Destroy(image, afterimageFadeTime);
    }
}
