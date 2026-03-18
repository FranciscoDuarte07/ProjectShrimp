using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float velMov;
    [SerializeField] float smoothMov;
    private float horizontalMov = 0f;
    private Vector3 vel = Vector3.zero;
    private bool lookRight = true;

    [SerializeField] bool shooterMode = false;
    [SerializeField] bool bodyMode = false;
    [SerializeField] Animator anim;


    private void Start()
    {
        anim = GetComponent<Animator>();
        shooterMode = true;    
    }

    // Update is called once per frame
    void Update()
    {
        horizontalMov = Input.GetAxisRaw("Horizontal") * velMov;


        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (shooterMode)
            {
                shooterMode = false;
                anim.SetTrigger("Change_Style");
                anim.SetBool("ShooterMode", false);
                anim.SetBool("BodyMode", true);
                bodyMode = true;
            }
            else if (bodyMode)
            {
                bodyMode = false;
                anim.SetTrigger("Change_Style");
                anim.SetBool("BodyMode", false);
                anim.SetBool("ShooterMode", true);
                shooterMode = true;
            }
            else
            {
                shooterMode = true;
            }
        }
    }
}
