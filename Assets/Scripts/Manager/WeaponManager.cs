using System.Collections;
using System.Collections.Generic;
using Scripts.Items;
using Scripts.Weapon;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using static Scripts.Weapon.FireArms;

public class WeaponManager : Singleton<WeaponManager>
{
    //枪械
    public FireArms mainWeapon;
    public FireArms secondaryWeapon;

    public FireArms carriedWeapon;
    [SerializeField] private FPCharacterControllerMovement fPCharacterControllerMovement;

    private AnimatorStateInfo animatorStateInfo;
    private IEnumerator waitingForHoisterEndCoroutine;

    //private IEnumerator waitForHolsterEndCoroutine;


    //Item
    public Transform worldCameraTransform;
    public float raycastMaxDistance = 2;
    public LayerMask checkItemLayerMask;
    public List<FireArms> arms = new List<FireArms>(); //手臂的链表

    //bool
    public bool isHoldingTrigger;
    //public bool isInspecting;

    //UI
    public GameObject crosshair;
    public Text ammoCountText;
    public List<Sprite> weaponSprites = new List<Sprite>();
    public Image image;

    //音效
    public AudioSource audioSource;
    public AudioClip takeOutClip;



    private void Start()
    {
        //fPCharacterControllerMovement = FindObjectOfType<FPCharacterControllerMovement>();
        //carriedWeapon = mainWeapon;
        /*//匹配枪对应的Animator
        fPCharacterControllerMovement.SetUpAnimator(carriedWeapon.gunAnimator);*/
        
        //fPCharacterControllerMovement.SetUpAnimator(carriedWeapon.gunAnimator);

        //如果此时有主武器
        if (mainWeapon)
        {
            carriedWeapon = mainWeapon;
            //匹配枪对应的Animator
            fPCharacterControllerMovement.SetUpAnimator(carriedWeapon.gunAnimator);
        }

        //清空子弹信息
        ammoCountText.text = "";

        //清空图像
        image.sprite = null;
    }

    private void Update()
    {
        

        //TODO：应该是有按键再发射出射线？
        //射线检测Items
        CheckItem();

        //检测是否拥有枪
        if (!carriedWeapon) return;

        //主副武器的切换
        SwapWeapon();

        //射击
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

        //换弹
        if (Input.GetKeyDown(KeyCode.R))
        {
            carriedWeapon.ReloadAmmo();
        }

        //瞄准
        if (Input.GetMouseButtonDown(1))
        {
            crosshair.SetActive(false);

            carriedWeapon.Aiming(true);

        }
        if (Input.GetMouseButtonUp(1))
        {
            crosshair.SetActive(true);
            carriedWeapon.Aiming(false);
        }

        //检视武器
        if (Input.GetKeyDown(KeyCode.T))
        {
            //isInspecting = true;
            carriedWeapon.gunAnimator.SetTrigger("Show");
            //StartCoroutine(CheckInspectingAnimationEnd());

        }

        //更新子弹数据
        UpdateAmmoText(carriedWeapon.GetCurrentAmmoreturn, carriedWeapon.GetCurrentMaxAmmoCarried);
    }

    //检测物体Item类
    private void CheckItem()
    {
        //相机的Z轴发射距离为2的射线检测碰撞
        bool tmp_IsItem = Physics.Raycast(worldCameraTransform.position, worldCameraTransform.forward,
            out RaycastHit tmp_RaycastHit, raycastMaxDistance, checkItemLayerMask);

        if (tmp_IsItem)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                //寻找BaseItem类的对象
                bool tmp_HasItem = tmp_RaycastHit.collider.TryGetComponent(out BaseItem tmp_BaseItem);
                if (tmp_HasItem)
                {
                    //拾取
                    PickUpWeapon(tmp_BaseItem);
                    PickUpAttachment(tmp_BaseItem);

                    //TODO：如果此时有武器了，交换武器，扔出被交换的武器，摧毁交换的武器
                    //销毁已经捡拾的Item
                    DestroyTargetItem(tmp_RaycastHit.transform.gameObject);
                }
            }
        }
    }

    //拾取武器
    private void PickUpWeapon(BaseItem _baseItem)
    {
        //如果此武器不是FirearmsItem的对象（不是武器
        if (!(_baseItem is FirearmsItem tmp_FirearmsItem)) return;

        //遍历武器的手臂
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

            //装配武器
            SetUpCarriedWeapon(tmp_Arm);

        }
    }

    //拾取瞄具
    private void PickUpAttachment(BaseItem _baseItem)
    {
        //如果此物品不是瞄具的类对象
        if (!(_baseItem is AttachmentItem tmp_AttachmentItem)) return;


        switch (tmp_AttachmentItem.currentAttachmentType)
        {
            case AttachmentItem.AttachmentType.Scope:
                //遍历瞄具
                foreach (ScopeInfo tmp_ScopeInfo in carriedWeapon.scopeInfos)
                {
                    //替换瞄具时，需要先隐藏不是目标的瞄具
                    if (tmp_ScopeInfo.ScopeName.CompareTo(tmp_AttachmentItem.itemName) != 0)
                    {
                        tmp_ScopeInfo.ScopeGameObject.SetActive(false);
                        continue;
                    }

                    //替换为目标瞄具
                    tmp_ScopeInfo.ScopeGameObject.SetActive(true);
                    //隐藏原有的瞄具（因为BaseIron没有放进上一波循环
                    carriedWeapon.baseIronSight.ScopeGameObject.SetActive(false);
                    carriedWeapon.SetUpCarriedScope(tmp_ScopeInfo);
                }
                break;

            case AttachmentItem.AttachmentType.Other:

                break;
        }
    }

    //Destroy TargetItem 拾取后的物品会被销毁
    private void DestroyTargetItem(GameObject _baseItem)
    {
        Destroy(_baseItem.gameObject);
    }

    //切换武器
    private void SwapWeapon()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (mainWeapon == null) return;
            if (carriedWeapon == mainWeapon) return;
            
            //替换为副武器
            if (carriedWeapon.gameObject.activeInHierarchy)
            {   
                StartWaitingForHoisterCoroutine();
                carriedWeapon.gunAnimator.SetTrigger("Hoister");
            }
            else
            {
                SetUpCarriedWeapon(mainWeapon);
                //StartWaitForHolsterCoroutine();
            }

            //PlayTakeOutAudio(audioSource);

            fPCharacterControllerMovement.SetUpAnimator(carriedWeapon.gunAnimator);
        }
        //更换副武器
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (secondaryWeapon == null) return;
            if (carriedWeapon == secondaryWeapon) return;

            //替换为主武器
            if (carriedWeapon.gameObject.activeInHierarchy)
            {
                StartWaitingForHoisterCoroutine();
                carriedWeapon.gunAnimator.SetTrigger("Hoister");
            }
            else
            {
                SetUpCarriedWeapon(secondaryWeapon);
                //StartWaitForHolsterCoroutine();
            }

            //PlayTakeOutAudio(audioSource);
        }
    }

    //开始换枪的协程
    private void StartWaitingForHoisterCoroutine()
    {
        if (waitingForHoisterEndCoroutine == null)
            waitingForHoisterEndCoroutine = WaitingForHoisterEnd();
        StartCoroutine(waitingForHoisterEndCoroutine);
    }

    /*private void StartWaitForHolsterCoroutine()
    {
        if (waitForHolsterEndCoroutine == null)
            waitForHolsterEndCoroutine = WaitForHolsterEnd();
        StartCoroutine(waitForHolsterEndCoroutine);
    }
*/

    //等待放下枪的动画结束的协程(包含了换枪的逻辑
    private IEnumerator WaitingForHoisterEnd()
    {
        while (true)
        {
            AnimatorStateInfo tmp_AnimatorStateInfo = carriedWeapon.gunAnimator.GetCurrentAnimatorStateInfo(0);
            if (tmp_AnimatorStateInfo.IsTag("Hoister"))
            {
                //Debug.Log("检测动画完成度中");
                //检测动画完成度
                if (tmp_AnimatorStateInfo.normalizedTime >= 0.95f)
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

    /*private IEnumerator WaitForHolsterEnd()
    {
        while (true)
        {
            AnimatorStateInfo tmp_AnimatorStateInfo = carriedWeapon.gunAnimator.GetCurrentAnimatorStateInfo(0);
            if (tmp_AnimatorStateInfo.IsTag("Hoister"))
            {
                Debug.Log("检测动画完成度中");
                //检测动画完成度
                if (tmp_AnimatorStateInfo.normalizedTime >= 0.90f)
                {
                    
                    yield break;
                }
            }
            yield return null;
        }
    }*/


    //装配目标武器
    private void SetUpCarriedWeapon(FireArms _targetWeapon)
    {
        //是否隐藏当前武器
        if (carriedWeapon != null)
        {
            carriedWeapon.gameObject.SetActive(false);
        }


        carriedWeapon = _targetWeapon;

        //更新武器的Icon
        UpdateWeaponSprite();

        //拿出枪械的音效
        PlayTakeOutAudio(audioSource);

        //激活目标的武器
        carriedWeapon.gameObject.SetActive(true);
        fPCharacterControllerMovement.SetUpAnimator(carriedWeapon.gunAnimator);
    }

    //更新子弹的数据
    private void UpdateAmmoText(int _currentAmmo, int _currentMaxAmmo)
    {
        ammoCountText.text = _currentAmmo + "/" + _currentMaxAmmo;
    }

    //更新武器的Icon
    private void UpdateWeaponSprite()
    {
        foreach (var tmp_Sprite in weaponSprites)
        {
            if (carriedWeapon.transform.name.CompareTo(tmp_Sprite.name) == 0)
            {
                if (!image.gameObject.activeInHierarchy) image.gameObject.SetActive(true);

                image.sprite = tmp_Sprite;
            }
        }
    }

    //拿出枪械的音效
    private void PlayTakeOutAudio(AudioSource audioSource)
    {
        audioSource.clip = takeOutClip;
        audioSource.Play();
    }

    


}
