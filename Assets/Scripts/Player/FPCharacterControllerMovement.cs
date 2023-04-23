using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class FPCharacterControllerMovement : Singleton<FPCharacterControllerMovement>
{
    //组件
    private CharacterController characterController;
    private Transform characterTransform;
    private Animator characterAnimator;

    //人物数据
    private Vector3 movementDirection;

    private static float gravity = 9.8f;

    public float jumpHeight;
    private float crouchHeight = 1f;
    public float originHeight;

    public float springSpeed;
    public float walkSpeed;

    public float crouchSpringSpeed;
    public float crouchWalkSpeed;

    public float velocity;

    //Bool
    public bool isCrouch;
    public bool isRunning;



    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        //characterAnimator = GetComponentInChildren<Animator>();
        characterTransform = transform;
        originHeight = characterController.height;

    }

    private void Update()
    {
        //判断是否奔跑
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            isRunning = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isRunning = false;
        }

        float tmp_CurrentSpeed = walkSpeed;

        if (characterController.isGrounded)
        {
            //获取移动输入
            var tmp_Horizontal = Input.GetAxis("Horizontal");
            var tmp_Vertical = Input.GetAxis("Vertical");

            //获取朝向
            movementDirection =
                characterTransform.TransformDirection(new Vector3(tmp_Horizontal, 0, tmp_Vertical));

            //跳跃
            if (Input.GetButtonDown("Jump"))
            {
                movementDirection.y = jumpHeight;  //指明了向量的方向和大小
            }
        }

        //下蹲
        if (Input.GetKeyDown(KeyCode.C))
        {
            var tmp_CurrentHeight = isCrouch ? originHeight : crouchHeight;
            StartCoroutine(DoCrouch(tmp_CurrentHeight));
            isCrouch = !isCrouch;
        }

        //奔跑(受是否蹲下影响
        if (isCrouch)
        {
            tmp_CurrentSpeed = Input.GetKey(KeyCode.LeftShift) ? crouchSpringSpeed : crouchWalkSpeed;
        }
        else
        {
            tmp_CurrentSpeed = Input.GetKey(KeyCode.LeftShift) ? springSpeed : walkSpeed;
        }

        //角色速度与动画连接的变量
        var tmp_Velocity = characterController.velocity;
        //防止下蹲起立出现行走的动画
        tmp_Velocity.y = 0;
        velocity = new Vector3(tmp_Velocity.x, 0, tmp_Velocity.z).magnitude;


        if (characterAnimator != null)
        {
            characterAnimator.SetFloat("velocity", velocity, 0.2f, Time.deltaTime);  // 0.2为过渡时间
        }

        //重力效果
        movementDirection.y -= gravity * Time.deltaTime;
        //没有重力的移动
        characterController.Move(movementDirection * tmp_CurrentSpeed * Time.deltaTime);
        //有重力的移动
        //characterController.SimpleMove(tmp_MovementDirection * Time.deltaTime * movementSpeed);
    }

    //下蹲协程
    private IEnumerator DoCrouch(float _target)
    {
        float tmp_CurrentHeight = 0;
        while (Mathf.Abs(characterController.height - _target) > 0.1f)
        {
            yield return null;
            characterController.height =
                Mathf.SmoothDamp(characterController.height, _target, ref tmp_CurrentHeight, Time.deltaTime * 5);
        }
    }

    //切换Animator
    internal void SetUpAnimator(Animator _animator)
    {
        characterAnimator = _animator;
    }
}

