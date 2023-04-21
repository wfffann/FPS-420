using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Scripts.Weapon
{
    public class AssualtRifle : FireArms
    {
        private IEnumerator reloadAmmoCheckerCoroutine;
        //协程
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

        //执行瞄准FOV协程和瞄准动画
        protected override void Aiming()
        {
            //瞄准的音效
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

        //瞄准时FOV变化的协程
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

            //音效
            firearmsShootingAudioSource.clip = fireArmsAudioData.shootingAudioClip;
            firearmsShootingAudioSource.Play();

            //枪焰特效
            muzzleParticle.Play();
            CreateBullet();
            casingParticle.Play();
            lastFireTime = Time.time;
        }

        //换弹
        protected override void Reload()
        {
            //更新动画层的权重
            gunAnimator.SetLayerWeight(2, 1);
            //根据当前的子弹数量判断执行的动画
            gunAnimator.SetTrigger(currentAmmo > 0 ? "ReloadLeft" : "ReloadOutOf");

            //音效
            firearmsReloadAudioSource.clip = currentAmmo > 0 ? fireArmsAudioData.reloadLeft : fireArmsAudioData.reloadOutOf;
            firearmsReloadAudioSource.Play();

            //防止协程卡死
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

        //实例化子弹
        protected void CreateBullet()
        {
            GameObject tmp_Bullet = Instantiate(bulletPrefab, muzzlePoint.position, muzzlePoint.rotation);
            
            var tmp_BulletScript = tmp_Bullet.AddComponent<Bullet>();
            //tmp_BulletRigidbody.velocity = tmp_Bullet.transform.forward * 100f;
            tmp_BulletScript.impactPrefab = bulletImpactPrefab;
            tmp_BulletScript.impactAudioData = impactAudioData;
            tmp_BulletScript.bulletSpeed = 100f;


            //销魂对象
            if (tmp_Bullet != null)
            {
                Destroy(tmp_Bullet, 5);
            }
        }


        //检测换弹是否完成的协程
        protected IEnumerator CheckReloadAmmoAnimationEnd()
        {
            while (true)
            {   
                //TODO:
                //isRealoding = true;

                yield return null;
                //获取第三层动画机信息
                gunStateInfo = gunAnimator.GetCurrentAnimatorStateInfo(2);
                //寻找对应Tag的全部动画
                if (gunStateInfo.IsTag("ReloadAmmo"))
                {
                    //检测该动画执行的完成度
                    if (gunStateInfo.normalizedTime > 0.9f)
                    {
                        //换弹需求子弹数量
                        int tmp_NeedAmmoCount = ammoInMag - currentAmmo;
                        //剩余子弹数量
                        int tmp_RemainingAmmo = currentMaxAmmoCarried - tmp_NeedAmmoCount;

                        //更新当前子弹信息
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

