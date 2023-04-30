using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Box : MonoBehaviour
{
    public Animator boxAnimator;



    public Image boxOpenImage;

    private bool isOpen = false;
    private bool isReady = false;
    //private bool 

    private AnimatorStateInfo boxAnimatorInfo;
    public Collider boxColl;

    public GameObject ammoSupply;
    public Collider ammoSupplyColl;

    


    private void Update()
    {

        if (isReady)
        {
            if (!isOpen)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    boxAnimator.SetBool("Open", true);
                    boxOpenImage.transform.gameObject.SetActive(false);

                    StartCoroutine(checkBoxAnimation());
                    isOpen = true;
                }
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("¼ì²âµ½Íæ¼Ò£¡");
        if (!isOpen)
        {
            isReady = true;
            boxOpenImage.transform.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isOpen)
        {
            isReady = false;
            boxOpenImage.transform.gameObject.SetActive(false);
        }
    }

    private IEnumerator checkBoxAnimation()
    {
        boxAnimatorInfo = boxAnimator.GetCurrentAnimatorStateInfo(0);

        /*if (boxAnimatorInfo.IsTag("Open"))
        {
            
        }*/

        if (boxAnimatorInfo.normalizedTime >= 0.95f)
        {
            boxColl.enabled = false;
            ammoSupplyColl.enabled = true;
            yield return new WaitForSeconds(1f);
            ammoSupply.SetActive(true);
            ammoSupply.layer = LayerMask.NameToLayer("Item");
        }

        yield return null;  
    }
}
