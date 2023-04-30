using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Items
{
    public class Supply : BaseItem
    {
        public enum SupplyType
        {
            HealthSupply,
            AmmoSupply,
            Other
        }


        public SupplyType currentSupplyType;


        public string supplyName;
    }
}

