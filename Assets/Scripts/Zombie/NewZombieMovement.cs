using System;
using System.Collections;
using System.Collections.Generic;
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

    private void Awake()
    {
        enemyAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();
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
    }


    private void FixedUpdate()
    {
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
        if (alreadyDied) return;

        //ֻ���ڷ�����״̬
        if (isDead)
            enemyState = EnemyState.DEAD;
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

                //Ѳ���ٶ�
                enemyAgent.speed = speed * 0.5f;

                //�ж��Ƿ������Ѳ�ߵ�
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
                    /*Debug.Log("�����ߵ�Ѳ�ߵ���");
                    Debug.Log("����wayPoint��ľ���Ϊ��" + Convert.ToString(Vector3.Distance(wayPoint, transform.position) <= enemyAgent.stoppingDistance));
                    Debug.Log("����wayPoint��ľ���Ϊ��" + Convert.ToString(Vector3.Distance(wayPoint, transform.position)));*/

                    isGuard = false;
                    enemyAgent.destination = wayPoint;
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

                //��������ѣ���ͣ��ԭ��
                if (!FoundPlayer())
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
                }

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

                animator.SetBool("Dead", true);
                Debug.Log("����������");
                alreadyDied = true;
                //coll.enabled = false;

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

    //�ڷ�Χ��Ѱ�����
    bool FoundPlayer()
    {
        //Բ���ڼ����ײ��
        var colliders = Physics.OverlapSphere(transform.position, sightRadius);
        foreach (var target in colliders)
        {
            if (target.CompareTag("Player"))
            {
                //�ҵ�����Ŀ��
                attackTarget = target.gameObject;
                return true;
            }
        }

        //û���ҵ�����Ŀ��
        attackTarget = null;
        return false;
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

    void Hit()
    {
        if (attackTarget != null)
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();
            //����˺�
            targetStats.TakeDamage(characterStats, targetStats);
            //�������Ѫ��
            playerHealthImage.fillAmount = (float)targetStats.CurrentHealth / targetStats.MaxHealth;
        }
    }
}
