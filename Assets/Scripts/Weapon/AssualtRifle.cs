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

        //执行瞄准FOV协程和瞄准动画
        /*protected override void Aiming()
        {
            //瞄准的音效
            *//*firearmsReloadAudioSource.clip = aimClip;
            firearmsReloadAudioSource.Play();*//*

        
            
        }*/

        

        protected override void Shooting()
        {
            //Debug.Log("Shoting!");
            if (currentAmmo <= 0) return;
            if (!IsAllowShooting()) return;

            //子弹数量
            currentAmmo -= 1;

            //动画层的选择
            gunAnimator.Play("Fire", isAiming ? 1 : 0, 0);

            //音效
            firearmsShootingAudioSource.clip = fireArmsAudioData.shootingAudioClip;
            firearmsShootingAudioSource.Play();

            //枪焰特效
            muzzleParticle.Play();

            //子弹
            CreateBullet();

            //弹壳
            casingParticle.Play();

            //后坐力
            mouseLook.FirngForTest();

            //更新计时器
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

            //子弹的散射（圆周内
            tmp_Bullet.transform.eulerAngles += CalculateSpreadOffset();

            var tmp_BulletScript = tmp_Bullet.AddComponent<Bullet>();
            //tmp_BulletRigidbody.velocity = tmp_Bullet.transform.forward * 100f;

            //在这个脚本获取有利于替换和管理
            tmp_BulletScript.impactPrefab = bulletImpactPrefab;
            tmp_BulletScript.impactAudioData = impactAudioData;
            tmp_BulletScript.bulletSpeed = 100f;


            //销魂对象
            if (tmp_Bullet != null)
            {
                Destroy(tmp_Bullet, 5);
            }
        }


        

        
    }
}

