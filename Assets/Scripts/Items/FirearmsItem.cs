using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Items
{
    public class FirearmsItem : BaseItem
    {
        //��������һ��ö����
        public enum FirearmsType
        {
            AssultRefile,
            HandGun,
        }

        public FirearmsType currentFirearmsType;
        public string armsName;
    }
}

