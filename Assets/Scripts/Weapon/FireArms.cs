using System.Collections;


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

        //��ǹ��Ƶ��
        public float fireRate;
        protected float lastFireTime;

        //������
        protected Animator gunAnimator;
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

        //FOV
        public Camera eyeCamera;
        //public Camera gunCamera;
        protected float originFOV;

        protected float eyeOriginFOV;
        protected float gunOriginFOV;

        protected Transform gunCameraTransform;
        protected Vector3 originalEyePosition;


        protected virtual void Start()
        {
            currentAmmo = ammoInMag;
            currentMaxAmmoCarried = maxAmmoCarried;
            gunAnimator = GetComponent<Animator>();
            eyeOriginFOV = eyeCamera.fieldOfView;
            originFOV = eyeCamera.fieldOfView;
        }

        public void DoAttack()
        {
            Shooting();
            
        }

        protected abstract void Shooting();

        protected abstract void Reload();

        protected abstract void Aiming();

        //��ǹ���
        protected bool IsAllowShooting()
        {
            // 715 1m
            // 715 / 60 = 11.7
            // 1 / 11.7
            return Time.time - lastFireTime > 1 / fireRate;
        }

        
    }
}

