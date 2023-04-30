using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class DoorOpen : MonoBehaviour
{
    public Animator doorAnimator;

    

    public Image doorOpenImage;

    private bool isOpen = false;
    private bool isReady = false;   




    private void Update()
    {
        
        if(isReady)
        {
            if(Input.GetKeyDown(KeyCode.E))
            {
                doorAnimator.SetBool("Open", true);
                doorOpenImage.transform.gameObject.SetActive(false);
                isOpen = true;
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("¼ì²âµ½Íæ¼Ò£¡");
        if (!isOpen)
        {
            isReady = true;
            doorOpenImage.transform.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(!isOpen)
        {
            isReady = false;
            doorOpenImage.transform.gameObject.SetActive(false);
        }
    }
}
