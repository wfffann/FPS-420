using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Items
{
    public class AttachmentItem : BaseItem
    {
        public enum AttachmentType
        {
            Scope,
            Other
        }


        public AttachmentType currentAttachmentType;


        public string itemName;
    }
}


