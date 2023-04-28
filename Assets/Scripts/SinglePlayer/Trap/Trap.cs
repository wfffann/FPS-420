using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class Trap : MonoBehaviour
{
    public Collider coll;

    private void OnTriggerEnter(Collider other)
    {   
        
        Trapmechanism.Instance.isTrigger = true;
        //Debug.Log("Trapmechanism.Instance.isTrigger" + Trapmechanism.Instance.isTrigger);
    }

    private void OnTriggerExit(Collider other)
    {
        coll.enabled = false;
    }
}
