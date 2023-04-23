using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class FPCharacterControllerMovement : Singleton<FPCharacterControllerMovement>
{
    //���
    private CharacterController characterController;
    private Transform characterTransform;
    private Animator characterAnimator;

    //��������
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
        //�ж��Ƿ���
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
            //��ȡ�ƶ�����
            var tmp_Horizontal = Input.GetAxis("Horizontal");
            var tmp_Vertical = Input.GetAxis("Vertical");

            //��ȡ����
            movementDirection =
                characterTransform.TransformDirection(new Vector3(tmp_Horizontal, 0, tmp_Vertical));

            //��Ծ
            if (Input.GetButtonDown("Jump"))
            {
                movementDirection.y = jumpHeight;  //ָ���������ķ���ʹ�С
            }
        }

        //�¶�
        if (Input.GetKeyDown(KeyCode.C))
        {
            var tmp_CurrentHeight = isCrouch ? originHeight : crouchHeight;
            StartCoroutine(DoCrouch(tmp_CurrentHeight));
            isCrouch = !isCrouch;
        }

        //����(���Ƿ����Ӱ��
        if (isCrouch)
        {
            tmp_CurrentSpeed = Input.GetKey(KeyCode.LeftShift) ? crouchSpringSpeed : crouchWalkSpeed;
        }
        else
        {
            tmp_CurrentSpeed = Input.GetKey(KeyCode.LeftShift) ? springSpeed : walkSpeed;
        }

        //��ɫ�ٶ��붯�����ӵı���
        var tmp_Velocity = characterController.velocity;
        //��ֹ�¶������������ߵĶ���
        tmp_Velocity.y = 0;
        velocity = new Vector3(tmp_Velocity.x, 0, tmp_Velocity.z).magnitude;


        if (characterAnimator != null)
        {
            characterAnimator.SetFloat("velocity", velocity, 0.2f, Time.deltaTime);  // 0.2Ϊ����ʱ��
        }

        //����Ч��
        movementDirection.y -= gravity * Time.deltaTime;
        //û���������ƶ�
        characterController.Move(movementDirection * tmp_CurrentSpeed * Time.deltaTime);
        //���������ƶ�
        //characterController.SimpleMove(tmp_MovementDirection * Time.deltaTime * movementSpeed);
    }

    //�¶�Э��
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

    //�л�Animator
    internal void SetUpAnimator(Animator _animator)
    {
        characterAnimator = _animator;
    }
}

