using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Items
{
    public abstract class BaseItem : MonoBehaviour
    {
        //��Ʒ��һ��ö����
        public enum ItemType
        {
            Firearms,
            Attachment,
            Supply,
            Others
        }

        public ItemType currentItemType;

        public int itemId;
    }
}

