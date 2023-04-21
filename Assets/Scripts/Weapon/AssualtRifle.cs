using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Weapon
{
    public class AssualtRifle : FireArms
    {
        protected override void Shooting()
        {
            Debug.Log("Shoting!");
        }
        protected override void Reload()
        {
            throw new System.NotImplementedException();
        }

        
    }
}

