using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

namespace Scripts.Weapon
{
    public abstract class FireArms : MonoBehaviour
    {
        public Transform muzzlePoint;
        public Transform casingPoint;

        public ParticleSystem muzzleParticle;
        public ParticleSystem casingParticle;

        //子弹
        public int ammoInMag = 30;
        public int maxAmmoCarried = 60;

        protected int currentAmmo;
        protected int currentMaxAmmoCarried;

        //开枪的频率
        public float fireRate;
        protected float lastFireTime;

        protected Animator gunAnimator;

        protected virtual void Start()
        {
            currentAmmo = ammoInMag;
            currentMaxAmmoCarried = maxAmmoCarried;
        }

        public void DoAttack()
        {
            if (currentAmmo <= 0) return;
            currentAmmo -= 1;
            Shooting();
            lastFireTime = Time.time;
        }

        protected abstract void Shooting();

        protected abstract void Reload();

        //开枪间隔
        protected bool IsAllowShooting()
        {
            // 715 1m
            // 715 / 60 = 11.7
            // 1 / 11.7
            return Time.time - lastFireTime > 1 / fireRate;
        }
    }
}

