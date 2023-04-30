using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

//public enum EnemyState { GUARD, PATROL, CHASE, DEAD }
public class NewZombieMovement : MonoBehaviour
{
    //���
    private NavMeshAgent enemyAgent;
    private Animator animator;
    private CharacterStats characterStats;
    //public Collider coll;
    public Rigidbody zombieRigidbody;

    public Image playerHealthImage;
    public GameObject ownHealthImage;

    //ö����
    private EnemyState enemyState;

    //����
    [Header("Basic Settings")]
    private GameObject attackTarget;
    public float sightRadius;
    private Vector3 velocity;
    private float speed;
    private Vector3 originalPosition;
    public float lookAtTime;
    private float remainLookAtTime;
    private float remainLookAtTime2;
    private float lastAttackTime;



    [Header("Patrol State")]
    public float patrolRange;
    private Vector3 wayPoint;

    //bool
    private bool isGuard;
    private bool isDead;
    private bool alreadyDied;
    private int changeDir = 1;

    //���Player
    Vector3 Center;
    public float feelPlayerDistance;
    public GameObject centerPoint;

    public GameObject partolPointA;
    public GameObject partolPointB;

    private Vector3 originalPointA;
    private Vector3 originalPointB;

    private void Awake()
    {
        enemyAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();

        Collider collider = gameObject.GetComponent<Collider>();
        Center = collider.bounds.center;

        

        //coll.GetComponent<CapsuleCollider>();
        originalPosition = transform.position;

        //��¼�������趨�ٶ�
        speed = enemyAgent.speed;

        //��ʱ�ĵ����ٶ�
        velocity = enemyAgent.velocity;

        //��ʱ��
        remainLookAtTime = lookAtTime;

    }

    private void Start()
    {
        alreadyDied = false;

        //��ʼ����Ѳ�ߵ�״̬
        enemyState = EnemyState.PATROL;

        //������ȴʱ��
        lastAttackTime = characterStats.attackData.coolDown;

        //����Ѳ�ߵ�ԭ����λ��
        if(partolPointA != null && partolPointB != null)
        {
            originalPointA = partolPointA.transform.position;

            originalPointB = partolPointB.transform.position;
        }
        
        

        //enemyAgent.updateRotation = false;
    }


    private void FixedUpdate()
    {
        //FoundPlayer();
        /*// ����һ���������洢�������е���Ϣ
        RaycastHit hit;

        Vector3 localPosition = transform.InverseTransformPoint(transform.position);
        // ��������������������ϵת��Ϊ��������ϵ
        Vector3 localDirection = transform.InverseTransformDirection(Vector3.forward);*/

        /*// ����һ����������ǰ�����������ߣ�����Ϊ1����λ
        if (Physics.Raycast(localPosition, transform.forward, out hit, 5f))
        {
            // ���������ĳ����ײ���ཻ�����ӡ�ཻ���������ཻ���������
            
            Debug.Log("Hit point: " + hit.point);
            Debug.Log("Hit object name: " + hit.collider.gameObject.name);
        }*/


        //Vector3 localPosition = transform.InverseTransformPoint(transform.position);
        //Debug.DrawRay(localPosition, transform.forward, Color.blue);
        //Debug.DrawRay(centerPoint.transform.position , transform.forward * sightRadius, Color.red);
        /*if(attackTarget != null)
        {
            Debug.DrawRay(centerPoint.transform.position, attackTarget.transform.position - centerPoint.transform.position, Color.blue);
        }*/
        /*Debug.DrawLine(transform.position, new Vector3(transform.position.x + x, transform.position.y + y, transform.position.z + z), Color.red);
        Debug.DrawLine(transform.localPosition, new Vector3(transform.localPosition.x + x, transform.localPosition.y + y, transform.localPosition.z + z), Color.blue);*/

        //FoundPlayer();

        /*if(attackTarget!= null)
        {
            Debug.DrawLine(transform.position, attackTarget.transform.position - transform.position, Color.red);
        }*/

        //Debug.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y, transform.position.z + 5f),  Color.red);


        //�ж��Ƿ�����
        if (characterStats.CurrentHealth == 0)
            isDead = true;

        //״̬���л�
        SwitchStates();

        //�������л�
        SwitchAnimation();

        //���¼�ʱ��
        lastAttackTime -= Time.deltaTime;
    }

    //����״̬���л�
    void SwitchStates()
    {   
        //ȷ��������״̬���ܸı�
        if (alreadyDied) return;

        //ֻ���ڷ�����״̬
        if (isDead)
            enemyState = EnemyState.DEAD;

        //�������
        else if (FoundPlayer())
        {
            enemyAgent.isStopped = false;
            enemyState = EnemyState.CHASE;
        }
        
        //Debug.Log(enemyState);



        switch (enemyState)
        {
            case EnemyState.GUARD:
                break;
            case EnemyState.PATROL:
                //isGuard = true;

                //Ѳ���ٶ�
                /*enemyAgent.speed = speed * 0.5f;*/

                /*//�ж��Ƿ������Ѳ�ߵ�
                if (Vector3.Distance(wayPoint, transform.position) <= enemyAgent.stoppingDistance)
                {
                    //�۲�״̬ -- ��ʱ����ʱ��
                    if (remainLookAtTime > 0)
                    {
                        isGuard = true;
                        remainLookAtTime -= Time.deltaTime;
                        //Debug.Log("remainLookAtTime1:" + remainLookAtTime);
                    }
                    else
                    {
                        GetNewWayPoint();
                        isGuard = false;
                    }
                }
                else
                {
                    //û���ߵ����Ѳ�ߵĵ�
                    *//*Debug.Log("�����ߵ�Ѳ�ߵ���");
                    Debug.Log("����wayPoint��ľ���Ϊ��" + Convert.ToString(Vector3.Distance(wayPoint, transform.position) <= enemyAgent.stoppingDistance));
                    Debug.Log("����wayPoint��ľ���Ϊ��" + Convert.ToString(Vector3.Distance(wayPoint, transform.position)));*//*

                    isGuard = false;
                    enemyAgent.destination = wayPoint;
                }*/


                //����֮�����Ѳ��(��ЩZombieΪվ׮
                enemyAgent.speed = speed * 0.5f;

                if (partolPointA != null && partolPointB != null)
                {
                    if (changeDir == 1)
                    {
                        if (Vector3.Distance(transform.position, originalPointA) <= enemyAgent.stoppingDistance)
                        {
                            changeDir = 2;
                        }
                        else
                        {
                            enemyAgent.destination = originalPointA;

                        }
                    }
                    else if (changeDir == 2)
                    {
                        if (Vector3.Distance(transform.position, originalPointB) <= enemyAgent.stoppingDistance)
                        {
                            changeDir = 1;
                        }
                        else
                        {
                            enemyAgent.destination = originalPointB;

                        }
                    }
                }
                else
                {
                    isGuard = true;
                }

                break;

            case EnemyState.CHASE:
                //����׷���ٶ�
                enemyAgent.speed = speed;

                //����׷��λ��
                if (attackTarget != null)
                {
                    isGuard = false;
                    enemyAgent.destination = attackTarget.transform.position;
                }

                //TODO���ܷ����ѵĻ��ƣ�
                //��������ѣ���ͣ��ԭ��
                /*if (!FoundPlayer())
                {
                    //�۲�״̬ -- ��ʱ����ʱ��
                    if (remainLookAtTime2 > 0)
                    {
                        //����ԭ��
                        enemyAgent.destination = transform.position;
                        isGuard = true;
                        remainLookAtTime2 -= Time.deltaTime;
                        //Debug.Log("remainLookAtTime2:" + remainLookAtTime2);

                        //enemyState = EnemyState.PATROL;
                        //enemyAgent.isStopped = false;
                    }
                    else
                    {
                        //���Ѻ�ع�Ѳ�ߵ�״̬
                        isGuard = false;
                        enemyState = EnemyState.PATROL;
                        //���¼�ʱ��
                        remainLookAtTime2 = UnityEngine.Random.Range(0, lookAtTime);
                    }
                }*/

                //�������ڹ�����Χ��
                if (TargetInAttackRange())
                {
                    //Debug.Log("1");
                    isGuard = true;
                    //enemyAgent.isStopped= true;
                    if (lastAttackTime < 0)
                    {
                        lastAttackTime = characterStats.attackData.coolDown;
                        //Debug.Log("2");
                        ZombieAttack();

                        //Debug.Log("lastAttackTime:" + lastAttackTime);
                    }
                }

                break;
            case EnemyState.DEAD:
                //ֱ�ӹرյ���
                enemyAgent.enabled = false;

                //������������
                animator.SetBool("Dead", true);
                //Debug.Log("����������");
                alreadyDied = true;
                //coll.enabled = false;

                //ǿ�ƹر�Rigidbody
                zombieRigidbody.Sleep();

                //�ر�Ѫ��
                ownHealthImage.SetActive(false);

                Destroy(gameObject, 3f);
                break;
        }
    }

    /*//�л�����
    private void SwitchAnimation()
    {
        if (isGuard)
        {
            animator.SetFloat("MoveSpeed", 0, 0.2f, Time.deltaTime);
        }
        else
        {
            velocity = enemyAgent.velocity;
            var tmp_velocity = new Vector3(velocity.x, 0, velocity.z).magnitude;
            if (tmp_velocity >= 0f) tmp_velocity = 1f;
            else tmp_velocity = 0f;
            animator.SetFloat("MoveSpeed", tmp_velocity, 0.2f, Time.deltaTime);
            //Debug.Log(tmp_velocity);
        }
    }*/

    //�������л�������Velocity -1D
    private void SwitchAnimation()
    {
        if (isGuard)
        {
            animator.SetFloat("Velocity", 0, 0.2f, Time.deltaTime);
        }
        else
        {
            velocity = enemyAgent.velocity;
            var tmp_velocity = new Vector3(velocity.x, 0, velocity.z).magnitude;
            if (tmp_velocity >= 0f) tmp_velocity = 2f;
            else tmp_velocity = 0f;

            animator.SetFloat("Velocity", tmp_velocity, 0.2f, Time.deltaTime);
            //Debug.Log(tmp_velocity);
        }
    }

    //����
    void ZombieAttack()
    {
        //�ٴ��ж��Ƿ��ڹ�����Χ��
        if (TargetInAttackRange())
        {
            //Debug.Log("3");
            //Attack���������һ��Hit������һ������������˺�
            animator.SetTrigger("Attack");
        }
    }

    //�ڷ�Χ��Ѱ�����(sightRadius
    bool FoundPlayer()
    {
        //Բ���ڼ����ײ��
        var colliders = Physics.OverlapSphere(transform.position, feelPlayerDistance);
        foreach (var target in colliders)
        {
            if (target.CompareTag("Player"))
            {
                //�ҵ�����Ŀ��
                attackTarget = target.gameObject;
                //Debug.Log("����Player��");
                //return true;

                //Debug.Log("���ڼ���Zombie��Playerd �ĵ��");
                //����Player��� �� �ټ��� ���
                float dot = Vector3.Dot(centerPoint.transform.forward.normalized, (attackTarget.transform.position - centerPoint.transform.position).normalized);
                return dot >= dotThreshol;

            }
        }

        //û���ҵ�����Ŀ��
        attackTarget = null;
        return false;
    }

    //������Ұ��Ѱ�����
    private float dotThreshol = 0.5f;
    private bool IsFindPlayerInSector()
    {
        if(attackTarget != null)
        {
            Debug.Log("���ڼ���Zombie��Playerd �ĵ��");
            float dot = Vector3.Dot(transform.forward, attackTarget.transform.position - transform.position);
            return dot >= dotThreshol;
        }
        else
        {
            return false;
        }
    }


    //���Ѳ�ߵ�
    void GetNewWayPoint()
    {
        float randomX = UnityEngine.Random.Range(-patrolRange, patrolRange);
        float randomZ = UnityEngine.Random.Range(-patrolRange, patrolRange);
        Vector3 randomPoint = new Vector3(randomX + originalPosition.x, transform.position.y, randomZ + originalPosition.z);

        //�ų����ܵ������ĵ㣨���ص���bool
        NavMeshHit hit;
        wayPoint = NavMesh.SamplePosition(randomPoint, out hit, patrolRange, 1) ? hit.position : transform.position;

        //ˢ�¼�ʱ��
        remainLookAtTime = UnityEngine.Random.Range(0, lookAtTime);
    }

    //�滭Ѳ�߷�Χ
    /*private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sightRadius);
    }*/

    //�жϹ���Ŀ���Ƿ��ڹ�����Χ
    bool TargetInAttackRange()
    {
        //Debug.Log("����ڹ�����Χ�ڣ�");
        if (attackTarget != null)
            return Vector3.Distance(transform.position, attackTarget.transform.position) <= characterStats.attackData.attackRange;
        else
            return false;
    }

    //�жϹ���Ŀ���Ƿ������εĹ�����Χ
    bool TargetInAttackRangeInSecor()
    {
        float dot = Vector3.Dot(centerPoint.transform.forward.normalized, (attackTarget.transform.position - centerPoint.transform.position).normalized);
        return dot >= dotThreshol;
    }

    void Hit()
    {
        if (attackTarget != null && TargetInAttackRangeInSecor())
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();
            //����˺�
            targetStats.TakeDamage(characterStats, targetStats);
            //�������Ѫ��
            playerHealthImage.fillAmount = (float)targetStats.CurrentHealth / targetStats.MaxHealth;

            WeaponManager.Instance.StartFlashCourtinue();
        }
    }



}
