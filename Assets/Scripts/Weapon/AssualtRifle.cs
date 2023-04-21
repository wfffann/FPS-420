using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Scripts.Weapon
{
    public class AssualtRifle : FireArms
    {
        private IEnumerator reloadAmmoCheckerCoroutine;
        //Э��
        private IEnumerator doAimCoroutine;


        protected override void Start()
        {
            base.Start();
            reloadAmmoCheckerCoroutine = CheckReloadAmmoAnimationEnd();
            doAimCoroutine = DoAim();

        }

        private void Update()
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

        }

        //ִ����׼FOVЭ�̺���׼����
        protected override void Aiming()
        {
            //��׼����Ч
            /*firearmsReloadAudioSource.clip = aimClip;
            firearmsReloadAudioSource.Play();*/

        
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

        //��׼ʱFOV�仯��Э��
        protected IEnumerator DoAim()
        {
            while (true)
            {
                yield return null;

                //MainCamera
                float tmp_EyeCurrentFOV = 0;
                eyeCamera.fieldOfView = Mathf.SmoothDamp(eyeCamera.fieldOfView,
                    isAiming ? 26 : eyeOriginFOV,
                    ref tmp_EyeCurrentFOV, Time.deltaTime * 2);

               /* //GunCamera
                float tmp_GunCurrentFOV = 0;
                gunCamera.fieldOfView = Mathf.SmoothDamp(gunCamera.fieldOfView,
                    isAiming ? rigoutScopeInfo.gunFOV : gunOriginFOV,
                    ref tmp_GunCurrentFOV, Time.deltaTime * 2);

                //GunCamera.transform.localPosition
                Vector3 tmp_RefPosition = Vector3.zero;
                gunCameraTransform.localPosition = Vector3.SmoothDamp(gunCameraTransform.localPosition,
                    isAiming ? rigoutScopeInfo.gunCameraPosition : originalEyePosition,
                    ref tmp_RefPosition, Time.deltaTime * 2);*/
            }
        }

        protected override void Shooting()
        {
            Debug.Log("Shoting!");
            if (currentAmmo <= 0) return;
            if (!IsAllowShooting()) return;
            currentAmmo -= 1;
            gunAnimator.Play("Fire", isAiming ? 1 : 0, 0);

            //��Ч
            firearmsShootingAudioSource.clip = fireArmsAudioData.shootingAudioClip;
            firearmsShootingAudioSource.Play();

            //ǹ����Ч
            muzzleParticle.Play();
            CreateBullet();
            casingParticle.Play();
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
            
            var tmp_BulletScript = tmp_Bullet.AddComponent<Bullet>();
            //tmp_BulletRigidbody.velocity = tmp_Bullet.transform.forward * 100f;
            tmp_BulletScript.impactPrefab = bulletImpactPrefab;
            tmp_BulletScript.impactAudioData = impactAudioData;
            tmp_BulletScript.bulletSpeed = 100f;


            //�������
            if (tmp_Bullet != null)
            {
                Destroy(tmp_Bullet, 5);
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

        
    }
}

