using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Scripts.Weapon
{
    public class AssualtRifle : FireArms
    {
        private IEnumerator reloadAmmoCheckerCoroutine;
        private FPMouseLook mouseLook;
        public GameObject smallCasingPrefab;
        public GameObject bigCasingPrefab;


        protected override void Awake()
        {
            base.Awake();
            reloadAmmoCheckerCoroutine = CheckReloadAmmoAnimationEnd();

            mouseLook = FindObjectOfType<FPMouseLook>();
        }
        

        /*private void Update()
        {
            if(Input.GetMouseButton(0))
            {
                DoAttack();
            }
            if(Input.GetKeyDown(KeyCode.R))
            {
                Reload();
            }
            if(Input.GetMouseButtonDown(1))
            {   
                isAiming = true;
                Aiming();
            }
            if (Input.GetMouseButtonUp(1))
            {
                isAiming = false;
                Aiming();
            }

        }*/

        //ִ����׼FOVЭ�̺���׼����
        /*protected override void Aiming()
        {
            //��׼����Ч
            *//*firearmsReloadAudioSource.clip = aimClip;
            firearmsReloadAudioSource.Play();*//*

        
            
        }*/

        

        protected override void Shooting()
        {
            //Debug.Log("Shoting!");
            //���������
            if (currentAmmo <= 0) return;
            if (!IsAllowShooting() || isRealoding || FPCharacterControllerMovement.Instance.isRunning) return;

            //�ӵ�����
            currentAmmo -= 1;

            //��ϻû���ӵ��Ķ���
            if(currentAmmo <= 0)
            {
                WeaponManager.Instance.carriedWeapon.gunAnimator.SetBool("OutOfAmmo", true);
            }

            //�������ѡ��
            gunAnimator.Play("Fire", isAiming ? 1 : 0, 0);

            //��Ч
            firearmsShootingAudioSource.clip = fireArmsAudioData.shootingAudioClip;
            firearmsShootingAudioSource.Play();

            //ǹ����Ч
            muzzleParticle.Play();

            //�ӵ�
            CreateBullet();

            //����
            smokePuff.Play();

            //��Ч
            StartCoroutine(Light());

            //�������ǵ�ʵ���Լ�Э����Ч
            CreatCasing();
            
            //������
            mouseLook.FirngForTest();

            //���¼�ʱ��
            lastFireTime = Time.time;
        }

        //ʵ��������(���������Ч��Э��
        protected void CreatCasing()
        {
            if(WeaponManager.Instance.carriedWeapon == WeaponManager.Instance.mainWeapon)
            {
                Instantiate(bigCasingPrefab, casingPoint.position, casingPoint.rotation);
            }
            else
            {
                Instantiate(smallCasingPrefab, casingPoint.position, casingPoint.rotation);
            }
        }

        //����
        protected override void Reload()
        {   
            //�滻����
            WeaponManager.Instance.carriedWeapon.gunAnimator.SetBool("OutOfAmmo", false);

            //���¶������Ȩ��
            gunAnimator.SetLayerWeight(2, 1);

            //���ݵ�ǰ���ӵ������ж�ִ�еĶ���
            gunAnimator.SetTrigger(currentAmmo > 0 ? "ReloadLeft" : "ReloadOutOf");

            //��Ч
            firearmsReloadAudioSource.clip = currentAmmo > 0 ? fireArmsAudioData.reloadLeft : fireArmsAudioData.reloadOutOf;
            firearmsReloadAudioSource.Play();

            //��ֹЭ�̿���
            if (reloadAmmoCheckerCoroutine == null)
            {
                reloadAmmoCheckerCoroutine = CheckReloadAmmoAnimationEnd();
                StartCoroutine(reloadAmmoCheckerCoroutine);
            }
            else
            {
                StartCoroutine(reloadAmmoCheckerCoroutine);
                reloadAmmoCheckerCoroutine = null;
                reloadAmmoCheckerCoroutine = CheckReloadAmmoAnimationEnd();
                StartCoroutine(reloadAmmoCheckerCoroutine);
            }
        }

        //ʵ�����ӵ�
        protected void CreateBullet()
        {
            GameObject tmp_Bullet = Instantiate(bulletPrefab, muzzlePoint.position, muzzlePoint.rotation);

            //�ӵ���ɢ�䣨Բ����
            tmp_Bullet.transform.eulerAngles += CalculateSpreadOffset();

            var tmp_BulletScript = tmp_Bullet.AddComponent<Bullet>();
            //tmp_BulletRigidbody.velocity = tmp_Bullet.transform.forward * 100f;

            //������ű���ȡ�������滻�͹���
            tmp_BulletScript.impactPrefabs = bulletImpactPrefabs;

            tmp_BulletScript.impactAudioData = impactAudioData;
            tmp_BulletScript.bulletSpeed = 100f;


            //�������
            if (tmp_Bullet != null)
            {
                Destroy(tmp_Bullet, 5);
            }
        }

        //Э����ʱ�ƹ�
        private IEnumerator Light()
        {   
            light.transform.gameObject.SetActive(true);

            yield return new WaitForSeconds(Random.Range(0.05f, 0.1f));

            light.transform.gameObject.SetActive(false);
        }

        
    }
}

