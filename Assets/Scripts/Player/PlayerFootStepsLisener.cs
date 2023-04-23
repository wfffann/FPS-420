using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFootStepsLisener : MonoBehaviour
{   
    //组件
    private CharacterController characterController;
    private Transform footStepTransform;

    //音效
    public FootStepAudioData footstepAudioData;
    public AudioSource footAudioSource;

    //数据
    private float nextPlayTime;

    public LayerMask layerMask;

    //人物的状态枚举
    public enum State
    {
        idle,
        walk,
        sprinting,
        crouching,
        others
    }

    public State characterState;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        footStepTransform = transform;
    }

    private void FixedUpdate()
    {
        if (characterController.isGrounded)
        {
            //在运动时
            if (characterController.velocity.normalized.magnitude >= 0.1f)
            {

                nextPlayTime += Time.deltaTime;

                //判断各种状态
                if (characterController.velocity.magnitude > 4.5f)
                {
                    characterState = State.sprinting;
                }
                else if (characterController.velocity.magnitude <= 4f)
                {
                    characterState = State.walk;
                }
                else if (characterController.velocity.magnitude < 4f && GetComponent<FPCharacterControllerMovement>().isCrouch)
                {
                    characterState = State.crouching;
                }
                else if (characterController.velocity.magnitude < 0.2f)
                {
                    characterState = State.idle;
                }

                //播放移动声音
                bool tmp_IsHit = Physics.Linecast(footStepTransform.position,
                    //注意需要跟新他的终点位置，和skinWidth,否则无法接触地面
                    footStepTransform.position + Vector3.down * (characterController.height / 2 + characterController.skinWidth - characterController.center.y),
                    out RaycastHit tmp_HitInfo, layerMask);

                //向下检测碰撞的Tag，以影响延时
                if (tmp_IsHit)
                {
                    foreach (var tmp_AudioElement in footstepAudioData.footStepAudios)
                    {
                        if (tmp_HitInfo.collider.CompareTag(tmp_AudioElement.Tag))
                        {
                            float tmp_Delay = 0;

                            switch (characterState)
                            {
                                case State.idle:
                                    tmp_Delay = float.MaxValue;
                                    break;
                                case State.walk:
                                    tmp_Delay = tmp_AudioElement.Delay;
                                    break;
                                case State.sprinting:
                                    tmp_Delay = tmp_AudioElement.sprintingDelay;
                                    break;
                                case State.crouching:
                                    tmp_Delay = tmp_AudioElement.crouchingDealy;
                                    break;
                                case State.others:
                                    break;
                            }

                            //如果超过延时
                            if (nextPlayTime >= tmp_Delay)
                            {
                                int tmp_AudioCount = tmp_AudioElement.AudioClips.Count;
                                int tmp_AudioIndex = Random.Range(0, tmp_AudioCount);
                                footAudioSource.clip = tmp_AudioElement.AudioClips[tmp_AudioIndex];
                                footAudioSource.Play();

                                //清空计时
                                nextPlayTime = 0;
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}
