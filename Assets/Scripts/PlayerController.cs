using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float dashSpeed;
    public float dashTime;
    public float waitAfterDash;
    public float afterImgLifeTime;
    public float timeAfterImg;
    public bool canMove;

    public GameObject pistolMode;
    public GameObject mantisMode;
    public SpriteRenderer pistolSprtRdr;
    public SpriteRenderer pistolAfterImg;
    public SpriteRenderer mantisSprtRdr;
    public SpriteRenderer mantisAfterImg;
    public Animator animPistol;
    public Animator animMantis;
    public Transform groundCheck;
    public Transform graphics;
    public Rigidbody2D rgbd2D;

    public LayerMask whatIsGround;
    public Color pistolImgColor;
    public Color mantisImgColor;

    private float _dashCounter;
    private float _dashRecharge;
    private float _movSpeed = 5f;
    private float _jumpForce = 9f;
    private float _afterImgCounter;
    private bool _isOnGround;

    private PlayerAbility _abilities;
    private PlayerCombat _playerCmbt;

    void Start()
    {
        _abilities = GetComponent<PlayerAbility>();
        _playerCmbt = GetComponent<PlayerCombat>();
        canMove = true;
    }

    void Update()
    {
        PlayerMove();
        PlayerAttack();
        TransformMode();
    }

    void PlayerMove()
    {
        if (canMove)
        {
            if (_dashRecharge > 0)
            {
                _dashRecharge -= Time.deltaTime;
            }
            else
            {
                if (Input.GetButtonDown("Fire2"))
                {
                    if (pistolMode.activeSelf && _abilities.canDash)
                    {
                        _dashCounter = dashTime;
                        ShowAfterImage();
                    }
                    else if (mantisMode.activeSelf && _abilities.canDash)
                    {
                        _dashCounter = dashTime / 3f;
                        ShowAfterImage();
                    }
                }
            }

            if (_dashCounter > 0)
            {
                _dashCounter -= Time.deltaTime;
                rgbd2D.velocity = new Vector2(dashSpeed * graphics.transform.localScale.x, rgbd2D.velocity.y);
                _afterImgCounter -= Time.deltaTime;

                if (_afterImgCounter <= 0)
                {
                    ShowAfterImage();
                }

                _dashRecharge = waitAfterDash;
            }
            else
            {
                rgbd2D.velocity = new Vector2(Input.GetAxisRaw("Horizontal") * _movSpeed, rgbd2D.velocity.y);

                if (rgbd2D.velocity.x < 0)
                {
                    graphics.localScale = new Vector3(-1f, 1f, 1f);
                }
                else if (rgbd2D.velocity.x > 0)
                {
                    graphics.localScale = Vector3.one;
                }
            }

            _isOnGround = Physics2D.OverlapCircle(groundCheck.position, .2f, whatIsGround);

            //jumo
            if (Input.GetButtonDown("Jump") && _isOnGround)
            {
                rgbd2D.velocity = new Vector2(rgbd2D.velocity.x, _jumpForce);
            }
        }
    }

    public void TransformMode()
    {
        if (!mantisMode.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                mantisMode.SetActive(true);
                pistolMode.SetActive(false);
            }
        }
        else if (!pistolMode.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                pistolMode.SetActive(true);
                mantisMode.SetActive(false);
            }
        }
    }

    void PlayerAttack()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (pistolMode.activeSelf)
            {
                if (!_playerCmbt.isOverheated)
                {
                    _playerCmbt.ShotAttack();
                    _playerCmbt.addHeat();
                }
            }
            else if (mantisMode.activeSelf)
            {
                _playerCmbt.ComboAttack();
            }
        }
    }

    public void ShowAfterImage()
    {
        if (pistolMode.activeSelf)
        {
            SpriteRenderer image = Instantiate(pistolAfterImg, transform.position, graphics.transform.rotation);
            image.sprite = pistolSprtRdr.sprite;
            image.transform.localScale = graphics.transform.localScale;
            image.color = pistolImgColor;

            Destroy(image.gameObject, afterImgLifeTime);
        }
        else if (mantisMode.activeSelf)
        {
            SpriteRenderer image = Instantiate(mantisAfterImg, graphics.transform.position, graphics.transform.rotation);
            image.sprite = mantisSprtRdr.sprite;
            image.transform.localScale = graphics.transform.localScale;
            image.color = mantisImgColor;

            Destroy(image.gameObject, afterImgLifeTime);
        }
        _afterImgCounter = timeAfterImg;
    }
}