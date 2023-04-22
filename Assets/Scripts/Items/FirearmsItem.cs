using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Items
{
    public class FirearmsItem : BaseItem
    {
        //武器类别的一个枚举类
        public enum FirearmsType
        {
            AssultRefile,
            HandGun,
        }

        public FirearmsType currentFirearmsType;
        public string armsName;
    }
}

