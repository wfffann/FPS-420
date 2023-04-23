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
            if (currentAmmo <= 0) return;
            if (!IsAllowShooting()) return;

            //�ӵ�����
            currentAmmo -= 1;

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
            casingParticle.Play();

            //������
            mouseLook.FirngForTest();

            //���¼�ʱ��
            lastFireTime = Time.time;
        }

        //����
        protected override void Reload()
        {
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
            tmp_BulletScript.impactPrefab = bulletImpactPrefab;
            tmp_BulletScript.impactAudioData = impactAudioData;
            tmp_BulletScript.bulletSpeed = 100f;


            //�������
            if (tmp_Bullet != null)
            {
                Destroy(tmp_Bullet, 5);
            }
        }


        

        
    }
}

