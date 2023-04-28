using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Trapmechanism : Singleton<Trapmechanism>
{
    //需要传入攻击的数据
    public CharacterStats trapCharacterStats;
    public bool isTrigger;

    public GameObject targetPoint;

    private Vector3 direction;
    private float distance;

    /*private void Update()
    {
        if(isTrigger)
        {   
            //Vector3 dirction =  targetPoint.transform.position - transform.position;
            transform.Translate( 
               Vector3.forward * Time.deltaTime);
        }
    }*/

    


    private void Update()
    {
        if (isTrigger && transform.position.z > targetPoint.transform.position.z)
        {
            transform.Translate(0, 0, 2f * Time.deltaTime);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("碰撞到了玩家！");
        if (collision.transform.gameObject.CompareTag("Player"))
        {
            var targetStats = collision.transform.gameObject.GetComponent<CharacterStats>();
            targetStats.TakeDamage(trapCharacterStats, targetStats);
        }
    }
}
