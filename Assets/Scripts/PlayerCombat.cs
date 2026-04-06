using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public float currentHeat;
    public float maxHeat = 100f;
    public float heatPerShot = 15f;
    public float heatCoolSpeed = 20f;
    public float overHeatCooldown = 2f;
    public float comboResetTime = 1f;
    public float attackRange = 0.5f;
    public int comboStep;
    public bool isOverheated;
    public BulletController shotFire;
    public Transform shotPoint;
    public Transform attackPoint;
    
    private bool _isAttacking;
    private float _overHeatTimer;
    private float _comboTimer;
    private PlayerController _playerCntrl;

    private void Start()
    {
        _playerCntrl = GetComponent<PlayerController>();
    }

    void Update()
    {
        if (_comboTimer > 0)
        {
            _comboTimer -= Time.deltaTime;
        }
        else
        {
            comboStep = 0;
        }

        HandleHeat();
    }

    public void ShotAttack()
    {
        Instantiate(shotFire, shotPoint.position, shotPoint.rotation)
                        .moveDir = new Vector2(_playerCntrl.graphics.localScale.x, 0f);
    }

    public void addHeat()
    {
        currentHeat += heatPerShot;

        if (currentHeat >= maxHeat)
        {
            currentHeat = maxHeat;
            isOverheated = true;
            _overHeatTimer = overHeatCooldown;
        }
    }

    void HandleHeat()
    {
        if (isOverheated)
        {
            _overHeatTimer -= Time.deltaTime;

            if (_overHeatTimer <= 0)
            {
                isOverheated = false;

                currentHeat = maxHeat * 0.7f;
            }
        }
        else
        {
            if (currentHeat > 0)
            {
                currentHeat -= heatCoolSpeed * Time.deltaTime;
                currentHeat = Mathf.Clamp(currentHeat, 0, maxHeat);
            }
        }
    }

    //>>>>>>>>>>>>>>>>>>>> Mantis Mode <<<<<<<<<<<<<<<<<<<<

    public void ComboAttack()
    {
        _isAttacking = true;

        comboStep++;

        if (comboStep > 3)
        {
            comboStep = 1;
        }

        _comboTimer = comboResetTime;

        //anim

        Invoke("DoMantisDamage", 0.1f);

        Invoke("EndAttack", 0.3f);
    }

      void DoMantisDamage()
      {
          Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
              attackPoint.position,
              attackRange//,
         //     enemyLayers
          );

          foreach (Collider2D enemy in hitEnemies)
          {
          //    enemy.GetComponent<EnemyHealthController>()?.DamageEnemy(mantisDamage);
          }
      }

    void EndAttack()
    {
        _isAttacking = false;
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}