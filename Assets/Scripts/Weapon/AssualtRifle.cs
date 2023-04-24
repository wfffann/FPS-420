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
            //射击的条件
            if (currentAmmo <= 0) return;
            if (!IsAllowShooting() || isRealoding || FPCharacterControllerMovement.Instance.isRunning) return;

            //子弹数量
            currentAmmo -= 1;

            //弹匣没有子弹的动画
            if(currentAmmo <= 0)
            {
                WeaponManager.Instance.carriedWeapon.gunAnimator.SetBool("OutOfAmmo", true);
            }

            //动画层的选择
            gunAnimator.Play("Fire", isAiming ? 1 : 0, 0);

            //音效
            firearmsShootingAudioSource.clip = fireArmsAudioData.shootingAudioClip;
            firearmsShootingAudioSource.Play();

            //枪焰特效
            muzzleParticle.Play();

            //子弹
            CreateBullet();

            //烟雾
            smokePuff.Play();

            //光效
            StartCoroutine(Light());

            //创建弹壳的实例以及协程音效
            CreatCasing();
            
            //后坐力
            mouseLook.FirngForTest();

            //更新计时器
            lastFireTime = Time.time;
        }

        //实例化弹壳(自身带有音效的协程
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

        //换弹
        protected override void Reload()
        {   
            //替换动画
            WeaponManager.Instance.carriedWeapon.gunAnimator.SetBool("OutOfAmmo", false);

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
            tmp_BulletScript.impactPrefabs = bulletImpactPrefabs;

            tmp_BulletScript.impactAudioData = impactAudioData;
            tmp_BulletScript.bulletSpeed = 100f;


            //销魂对象
            if (tmp_Bullet != null)
            {
                Destroy(tmp_Bullet, 5);
            }
        }

        //协程延时灯光
        private IEnumerator Light()
        {   
            light.transform.gameObject.SetActive(true);

            yield return new WaitForSeconds(Random.Range(0.05f, 0.1f));

            light.transform.gameObject.SetActive(false);
        }

        
    }
}

