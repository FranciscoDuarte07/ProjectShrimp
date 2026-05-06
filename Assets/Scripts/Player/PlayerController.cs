using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float jumpForce = 14f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.15f;

    [Header("Gráficos de modo  (hijos de Graphics)")]
    [SerializeField] private Transform graphicsTransform;
    [SerializeField] private GameObject pistolModeGraphics;  
    [SerializeField] private GameObject mantisModeGraphics; 

    /*-------------------------------------------------------
                        Pistol Mode    
      -------------------------------------------------------*/
    
    [Header("Disparo (Modo Pistola)")]
    [SerializeField] public GameObject BubblePrefab;
    [SerializeField] public Transform ShootPoint;     
    [SerializeField] public float BulletSpeed = 14f;

    [Header("Special Attack Base (Modo Pistola)")]
    [SerializeField] public GameObject BubblePrisonPrefab;
    [SerializeField] public float BubblePrisonCooldown = 8f;

    /*-------------------------------------------------------
                        Mantis Mode    
      -------------------------------------------------------*/
    
    [Header("Combate (Modo Mantis)")]
    [SerializeField] public Transform ComboPoint;     
    [SerializeField] public LayerMask EnemyLayer;

    [Header("Special Attack Base (Modo Mantis)")]
    [SerializeField] public GameObject WaterCurrentPrefab;
    [SerializeField] public float MegaPunchCooldown = 6f;

    /*-------------------------------------------------------
                        References   
      -------------------------------------------------------*/

    [Header("UI  (Canvas hijo de Player)")]
    [SerializeField] public CannonHeatUI CannonHeatUI;
    [SerializeField] public GameObject CannonOverchargeUI;

    [Header("Input Actions")]
    [SerializeField] private InputActionAsset inputActions;

    public bool IsRight { get; private set; } = true;
    public bool IsGrounded { get; private set; }

    public Action OnCannonOverheated;
    public Action OnCannonCooled;

    private Rigidbody2D rb;
    private CombatMode currentMode;
    private PistolMode modoPistola;
    private MantisMode modoMantis;

    private InputAction actionMove;
    private InputAction actionJump;
    private InputAction actionAttack;
    private InputAction actionSpecialAttack;
    private InputAction actionTransform;

    private Vector2 moveInput;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        modoPistola = new PistolMode(this);
        modoMantis = new MantisMode(this);
        BindInputActions();
    }

    private void OnEnable()
    {
        inputActions?.Enable();
        actionJump.performed += OnJumpPerformed;
        actionAttack.performed += OnAttackPerformed;
        actionSpecialAttack.performed += OnSpecialAttackPerformed;
        actionTransform.performed += OnTransformPerformed;
    }

    private void OnDisable()
    {
        actionJump.performed -= OnJumpPerformed;
        actionAttack.performed -= OnAttackPerformed;
        actionSpecialAttack.performed -= OnSpecialAttackPerformed;
        actionTransform.performed -= OnTransformPerformed;
        inputActions?.Disable();
    }

    private void Start()
    {
        SetMode(modoPistola);
    }

    private void Update()
    {
        moveInput = actionMove.ReadValue<Vector2>();
        IsGrounded = CheckGrounded();
        currentMode?.OnUpdate();
        FlipSprite(moveInput.x);
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
    }

    private void BindInputActions()
    {
        if (inputActions == null) { Debug.LogError("[Shrimpy] Falta InputActionAsset."); return; }
        actionMove = inputActions.FindAction("Move", true);
        actionJump = inputActions.FindAction("Jump", true);
        actionAttack = inputActions.FindAction("Attack", true);
        actionSpecialAttack = inputActions.FindAction("SpecialAttack", true);
        actionTransform = inputActions.FindAction("Transform", true);
    }

    private void OnJumpPerformed(InputAction.CallbackContext ctx)
    {
        if (IsGrounded)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    private void OnAttackPerformed(InputAction.CallbackContext ctx)
        => currentMode?.PrimaryAction();

    private void OnSpecialAttackPerformed(InputAction.CallbackContext ctx)
        => currentMode?.SpecialAction();

    private void OnTransformPerformed(InputAction.CallbackContext ctx)
        => TransformMode();

    public void TransformMode()
    {
        SetMode(currentMode == modoPistola ? (CombatMode)modoMantis : modoPistola);
    }

    private void SetMode(CombatMode newMode)
    {
        currentMode?.OnExit();
        currentMode = newMode;
        currentMode.OnEnter();

        bool isPistol = currentMode == modoPistola;

        if (CannonOverchargeUI != null) CannonOverchargeUI.SetActive(isPistol);
        if (pistolModeGraphics != null) pistolModeGraphics.SetActive(isPistol);
        if (mantisModeGraphics != null) mantisModeGraphics.SetActive(!isPistol);
    }

    private bool CheckGrounded()
        => groundCheck != null &&
           Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

    private void FlipSprite(float h)
    {
        if (h > 0.01f && !IsRight) Flip();
        if (h < -0.01f && IsRight) Flip();
    }

    private void Flip()
    {
        IsRight = !IsRight;

        if (graphicsTransform != null)
        {
            Vector3 s = graphicsTransform.localScale;
            s.x *= -1;
            graphicsTransform.localScale = s;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
        modoMantis?.DrawGizmos(ComboPoint != null ? ComboPoint : transform, IsRight);
    }
}
