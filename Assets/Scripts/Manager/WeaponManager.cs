using System.Collections;
using System.Collections.Generic;
using Scripts.Items;
using Scripts.Weapon;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static Scripts.Weapon.FireArms;

public class WeaponManager : MonoBehaviour
{
    public FireArms mainWeapon;
    public FireArms secondaryWeapon;

    private FireArms carriedWeapon;
    [SerializeField] private FPCharacterControllerMovement fPCharacterControllerMovement;

    private AnimatorStateInfo animatorStateInfo;
    private IEnumerator waitingForHoisterEndCoroutine;


    //Item
    public Transform worldCameraTransform;
    public float raycastMaxDistance = 2;
    public LayerMask checkItemLayerMask;
    public List<FireArms> arms = new List<FireArms>(); //�ֱ۵�����

    //bool
    public bool isHoldingTrigger;
    public bool isInspecting;

    private void Start()
    {
        //fPCharacterControllerMovement = FindObjectOfType<FPCharacterControllerMovement>();
        //carriedWeapon = mainWeapon;
        /*//ƥ��ǹ��Ӧ��Animator
        fPCharacterControllerMovement.SetUpAnimator(carriedWeapon.gunAnimator);*/
        
        //fPCharacterControllerMovement.SetUpAnimator(carriedWeapon.gunAnimator);

        if (mainWeapon)
        {
            carriedWeapon = mainWeapon;
            //ƥ��ǹ��Ӧ��Animator
            fPCharacterControllerMovement.SetUpAnimator(carriedWeapon.gunAnimator);
        }
    }

    private void Update()
    {
        CheckItem();

        //����Ƿ�ǹ
        if (!carriedWeapon) return;

        SwapWeapon();

        

        //���
        if (Input.GetMouseButton(0))
        {
            isHoldingTrigger = true;

            carriedWeapon.HoldTrigger();
        }
        if (Input.GetMouseButtonUp(0))
        {
            isHoldingTrigger = false;
            carriedWeapon.ReleaseTrigger();
        }

        //����
        if (Input.GetKeyDown(KeyCode.R))
        {
            carriedWeapon.ReloadAmmo();
        }

        //��׼
        if (Input.GetMouseButtonDown(1))
        {
            //crosshair.SetActive(false);

            carriedWeapon.Aiming(true);

        }
        if (Input.GetMouseButtonUp(1))
        {
            //crosshair.SetActive(true);
            carriedWeapon.Aiming(false);
        }

        //��������
        if (Input.GetKeyDown(KeyCode.T))
        {
            isInspecting = true;
            carriedWeapon.gunAnimator.SetTrigger("Show");
            //StartCoroutine(CheckInspectingAnimationEnd());

        }
    }

    //�������Item��
    private void CheckItem()
    {
        //�����Z�ᷢ�����Ϊ2�����߼����ײ
        bool tmp_IsItem = Physics.Raycast(worldCameraTransform.position, worldCameraTransform.forward,
            out RaycastHit tmp_RaycastHit, raycastMaxDistance, checkItemLayerMask);



        if (tmp_IsItem)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                //Ѱ��BaseItem��Ķ���
                bool tmp_HasItem = tmp_RaycastHit.collider.TryGetComponent(out BaseItem tmp_BaseItem);
                if (tmp_HasItem)
                {
                    //ʰȡ
                    PickUpWeapon(tmp_BaseItem);
                    PickUpAttachment(tmp_BaseItem);

                    //�����Ѿ���ʰ��Item
                    DestroyTargetItem(tmp_RaycastHit.transform.gameObject);
                }
            }
        }
    }

    //ʰȡ����
    private void PickUpWeapon(BaseItem _baseItem)
    {
        //�������������FirearmsItem�Ķ��󣨲�������
        if (!(_baseItem is FirearmsItem tmp_FirearmsItem)) return;

        //�����������ֱ�
        foreach (FireArms tmp_Arm in arms)
        {
            if (tmp_FirearmsItem.armsName.CompareTo(tmp_Arm.name) != 0) continue;

            switch (tmp_FirearmsItem.currentFirearmsType)
            {
                case FirearmsItem.FirearmsType.AssultRefile:
                    mainWeapon = tmp_Arm;
                    break;

                case FirearmsItem.FirearmsType.HandGun:
                    secondaryWeapon = tmp_Arm;
                    break;
            }

            //װ������
            SetUpCarriedWeapon(tmp_Arm);

        }
    }

    //ʰȡ���
    private void PickUpAttachment(BaseItem _baseItem)
    {
        //�������Ʒ������ߵ������
        if (!(_baseItem is AttachmentItem tmp_AttachmentItem)) return;


        switch (tmp_AttachmentItem.currentAttachmentType)
        {
            case AttachmentItem.AttachmentType.Scope:
                //�������
                foreach (ScopeInfo tmp_ScopeInfo in carriedWeapon.scopeInfos)
                {
                    //�滻���ʱ����Ҫ�����ز���Ŀ������
                    if (tmp_ScopeInfo.ScopeName.CompareTo(tmp_AttachmentItem.itemName) != 0)
                    {
                        tmp_ScopeInfo.ScopeGameObject.SetActive(false);
                        continue;
                    }

                    //�滻ΪĿ�����
                    tmp_ScopeInfo.ScopeGameObject.SetActive(true);
                    //����ԭ�е���ߣ���ΪBaseIronû�зŽ���һ��ѭ��
                    carriedWeapon.baseIronSight.ScopeGameObject.SetActive(false);
                    carriedWeapon.SetUpCarriedScope(tmp_ScopeInfo);
                }
                break;

            case AttachmentItem.AttachmentType.Other:

                break;
        }
    }

    //Destroy TargetItem ʰȡ�����Ʒ�ᱻ����
    private void DestroyTargetItem(GameObject _baseItem)
    {
        Destroy(_baseItem.gameObject);
    }

    //�л�����
    private void SwapWeapon()
    {


        if (Input.GetKeyDown(KeyCode.Alpha1))
        {


            if (mainWeapon == null) return;
            if (carriedWeapon == mainWeapon) return;

            if (carriedWeapon.gameObject.activeInHierarchy)
            {
                StartWaitingForHoisterCoroutine();
                carriedWeapon.gunAnimator.SetTrigger("Hoister");
            }
            else
            {
                SetUpCarriedWeapon(mainWeapon);
            }

            //PlayTakeOutAudio(audioSource);

            fPCharacterControllerMovement.SetUpAnimator(carriedWeapon.gunAnimator);
        }
        //����������
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            

            if (secondaryWeapon == null) return;
            if (carriedWeapon == secondaryWeapon) return;

            if (carriedWeapon.gameObject.activeInHierarchy)
            {
                StartWaitingForHoisterCoroutine();
                carriedWeapon.gunAnimator.SetTrigger("Hoister");
            }
            else
            {
                SetUpCarriedWeapon(secondaryWeapon);
            }

            //PlayTakeOutAudio(audioSource);

            
        }

    }

    //��ʼ��ǹ��Э��
    private void StartWaitingForHoisterCoroutine()
    {
        if (waitingForHoisterEndCoroutine == null)
            waitingForHoisterEndCoroutine = WaitingForHoisterEnd();
        StartCoroutine(waitingForHoisterEndCoroutine);
    }

    //�ȴ�����ǹ�Ķ���������Э��
    private IEnumerator WaitingForHoisterEnd()
    {
        while (true)
        {
            AnimatorStateInfo tmp_AnimatorStateInfo = carriedWeapon.gunAnimator.GetCurrentAnimatorStateInfo(0);
            if (tmp_AnimatorStateInfo.IsTag("Hoister"))
            {
                if (tmp_AnimatorStateInfo.normalizedTime >= 0.9f)
                {
                    var tmp_TargetWeapon = carriedWeapon == mainWeapon ? secondaryWeapon : mainWeapon;
                    SetUpCarriedWeapon(tmp_TargetWeapon);
                    waitingForHoisterEndCoroutine = null;
                    yield break;
                }
            }
            yield return null;
        }
    }

    //װ��Ŀ������
    private void SetUpCarriedWeapon(FireArms _targetWeapon)
    {
        //�Ƿ����ص�ǰ����
        if (carriedWeapon != null)
        {
            carriedWeapon.gameObject.SetActive(false);
        }


        carriedWeapon = _targetWeapon;

        /*//����������Icon
        UpdateWeaponSprite();

        PlayTakeOutAudio(audioSource);*/

        //����Ŀ�������
        carriedWeapon.gameObject.SetActive(true);
        fPCharacterControllerMovement.SetUpAnimator(carriedWeapon.gunAnimator);
    }

}