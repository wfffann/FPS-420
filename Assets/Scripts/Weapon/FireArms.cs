using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Weapon
{
    public abstract class FireArms : MonoBehaviour
    {
        //���
        public GameObject bulletImpactPrefab;

        //��Ч
        public Transform muzzlePoint;
        public Transform casingPoint;

        public ParticleSystem muzzleParticle;
        public ParticleSystem casingParticle;

        public GameObject bulletPrefab;

        //�ӵ�
        public int ammoInMag = 30;
        public int maxAmmoCarried = 60;

        protected int currentAmmo;
        protected int currentMaxAmmoCarried;
        public int GetCurrentAmmoreturn => currentAmmo;
        public int GetCurrentMaxAmmoCarried => currentMaxAmmoCarried;

        public float spreadAngle;

        //��ǹ��Ƶ��
        public float fireRate;
        protected float lastFireTime;

        //������
        internal Animator gunAnimator;
        protected AnimatorStateInfo gunStateInfo;

        //��Ч
        public AudioSource firearmsShootingAudioSource;
        public AudioSource firearmsReloadAudioSource;
        public FireArmsAudioData fireArmsAudioData;
        public ImpactAudioData impactAudioData;

        //bool
        public bool isAiming;
        public bool isRealoding;
        public bool isFire;
        public bool isInspecting;
        public bool isRunning;

        protected bool isHoldingTrigger;
        public bool IsHoldingTrigger => isHoldingTrigger;

        //FOV
        public Camera eyeCamera;
        public Camera gunCamera;
        protected float originFOV;

        protected float eyeOriginFOV;
        protected float gunOriginFOV;

        protected Transform gunCameraTransform;
        protected Vector3 originalEyePosition;

        //Э��
        private IEnumerator doAimCoroutine;

        //Scope
        public List<ScopeInfo> scopeInfos;
        public ScopeInfo baseIronSight;
        protected ScopeInfo rigoutScopeInfo;

        protected virtual void Awake()
        {
            currentAmmo = ammoInMag;
            currentMaxAmmoCarried = maxAmmoCarried;


            gunAnimator = GetComponent<Animator>();

            doAimCoroutine = DoAim();

            eyeOriginFOV = eyeCamera.fieldOfView;
            originFOV = eyeCamera.fieldOfView;

            gunOriginFOV = gunCamera.fieldOfView;

            gunCameraTransform = gunCamera.transform;
            originalEyePosition = gunCameraTransform.localPosition;

            rigoutScopeInfo = baseIronSight;
        }

        public void DoAttack()
        {
            Shooting();
            
        }

        protected abstract void Shooting();

        protected abstract void Reload();

        //protected abstract void Aiming();

        //ִ����׼FOVЭ�̺���׼����
        internal void Aiming(bool _isAiming)
        {
            //��׼����Ч
            /*firearmsReloadAudioSource.clip = aimClip;
            firearmsReloadAudioSource.Play();*/

            isAiming = _isAiming;
            gunAnimator.SetBool("Aim", isAiming);
            if (doAimCoroutine == null)
            {
                doAimCoroutine = DoAim();
                StartCoroutine(doAimCoroutine);
            }
            else
            {
                StopCoroutine(doAimCoroutine);
                doAimCoroutine = null;
                doAimCoroutine = DoAim();
                StartCoroutine(doAimCoroutine);
            }
        }

        internal void HoldTrigger()
        {
            DoAttack();
            isHoldingTrigger = true;
        }

        internal void ReleaseTrigger()
        {
            isHoldingTrigger = false;
        }

        internal void ReloadAmmo()
        {
            Reload();
        }


        //��ǹ���
        protected bool IsAllowShooting()
        {
            // 715 1m
            // 715 / 60 = 11.7
            // 1 / 11.7
            return Time.time - lastFireTime > 1 / fireRate;
        }

        //�ӵ���ɢ��
        protected Vector3 CalculateSpreadOffset()
        {
            float tmp_SpreadPercent = spreadAngle / eyeCamera.fieldOfView;
            return tmp_SpreadPercent * Random.insideUnitCircle;
        }

        //��׼ʱFOV�仯��Э��
        protected IEnumerator DoAim()
        {
            while (true)
            {
                yield return null;

                //MainCamera
                float tmp_EyeCurrentFOV = 0;
                eyeCamera.fieldOfView = Mathf.SmoothDamp(eyeCamera.fieldOfView,
                    isAiming ? rigoutScopeInfo.eyeFOV : eyeOriginFOV,
                    ref tmp_EyeCurrentFOV, Time.deltaTime * 2);

                //GunCamera
                float tmp_GunCurrentFOV = 0;
                gunCamera.fieldOfView = Mathf.SmoothDamp(gunCamera.fieldOfView,
                    isAiming ? rigoutScopeInfo.gunFOV : gunOriginFOV,
                    ref tmp_GunCurrentFOV, Time.deltaTime * 2);

                //GunCamera.transform.localPosition
                Vector3 tmp_RefPosition = Vector3.zero;
                gunCameraTransform.localPosition = Vector3.SmoothDamp(gunCameraTransform.localPosition,
                    isAiming ? rigoutScopeInfo.gunCameraPosition : originalEyePosition,
                    ref tmp_RefPosition, Time.deltaTime * 2);
            }
        }

        //��⻻���Ƿ���ɵ�Э��
        protected IEnumerator CheckReloadAmmoAnimationEnd()
        {
            while (true)
            {
                //TODO:
                //isRealoding = true;

                yield return null;
                //��ȡ�����㶯������Ϣ
                gunStateInfo = gunAnimator.GetCurrentAnimatorStateInfo(2);
                //Ѱ�Ҷ�ӦTag��ȫ������
                if (gunStateInfo.IsTag("ReloadAmmo"))
                {
                    //���ö���ִ�е���ɶ�
                    if (gunStateInfo.normalizedTime > 0.9f)
                    {
                        //���������ӵ�����
                        int tmp_NeedAmmoCount = ammoInMag - currentAmmo;
                        //ʣ���ӵ�����
                        int tmp_RemainingAmmo = currentMaxAmmoCarried - tmp_NeedAmmoCount;

                        //���µ�ǰ�ӵ���Ϣ
                        if (tmp_RemainingAmmo <= 0) currentAmmo += currentMaxAmmoCarried;
                        else currentAmmo = ammoInMag;
                        currentMaxAmmoCarried = tmp_RemainingAmmo <= 0 ? 0 : tmp_RemainingAmmo;

                        //TODO:
                        //isRealoding = false;

                        yield break;
                    }
                }
            }
        }

        //װ����ǰ��ߵ���ֵ
        internal void SetUpCarriedScope(ScopeInfo _scopeInfo)
        {
            rigoutScopeInfo = _scopeInfo;
        }





        [System.Serializable]
        public class ScopeInfo
        {
            public string ScopeName;
            public GameObject ScopeGameObject;
            public float eyeFOV;
            public float gunFOV;
            public Vector3 gunCameraPosition;
        }



    }


}

