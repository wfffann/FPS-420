using System.Collections;
using System.Collections.Generic;
using Scripts.Weapon;
using UnityEngine;

public class CrossHair : MonoBehaviour
{
    public RectTransform reticle;
    public CharacterController characterController;

    public float originalSize;
    public float targetSize;

    public float currentSize;

    //public GameObject weaponManager;

    private void Update()
    {
        if (transform.gameObject.activeInHierarchy)
        {
            bool tmp_IsFire = WeaponManager.Instance.isHoldingTrigger;

            //判断是否移动
            bool tmp_IsMoving = characterController.velocity.magnitude > 0f;
            if (tmp_IsMoving || tmp_IsFire)
            {
                currentSize = Mathf.Lerp(currentSize, targetSize, Time.deltaTime * 5);
            }
            else
            {
                currentSize = Mathf.Lerp(currentSize, originalSize, Time.deltaTime * 5);
            }
            //设置新锚点
            reticle.sizeDelta = new Vector2(currentSize, currentSize);
        }
    }
}
