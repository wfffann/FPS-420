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
    //组件
    private NavMeshAgent enemyAgent;
    private Animator animator;
    private CharacterStats characterStats;
    //public Collider coll;
    public Rigidbody zombieRigidbody;

    public Image playerHealthImage;
    public GameObject ownHealthImage;

    //枚举类
    private EnemyState enemyState;

    //数据
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

    //检测Player
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

        //记录导航的设定速度
        speed = enemyAgent.speed;

        //此时的导航速度
        velocity = enemyAgent.velocity;

        //计时器
        remainLookAtTime = lookAtTime;

    }

    private void Start()
    {
        alreadyDied = false;

        //初始赋予巡逻的状态
        enemyState = EnemyState.PATROL;

        //攻击冷却时间
        lastAttackTime = characterStats.attackData.coolDown;

        //存贮巡逻点原来的位置
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
        /*// 定义一个变量来存储射线命中的信息
        RaycastHit hit;

        Vector3 localPosition = transform.InverseTransformPoint(transform.position);
        // 将方向向量从世界坐标系转换为本地坐标系
        Vector3 localDirection = transform.InverseTransformDirection(Vector3.forward);*/

        /*// 发射一条从人物正前方发出的射线，长度为1个单位
        if (Physics.Raycast(localPosition, transform.forward, out hit, 5f))
        {
            // 如果射线与某个碰撞体相交，则打印相交点的坐标和相交物体的名称
            
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


        //判断是否死亡
        if (characterStats.CurrentHealth == 0)
            isDead = true;

        //状态的切换
        SwitchStates();

        //动画的切换
        SwitchAnimation();

        //更新计时器
        lastAttackTime -= Time.deltaTime;
    }

    //敌人状态的切换
    void SwitchStates()
    {   
        //确认死亡（状态不能改变
        if (alreadyDied) return;

        //只有在非死亡状态
        if (isDead)
            enemyState = EnemyState.DEAD;

        //发现玩家
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

                //巡逻速度
                /*enemyAgent.speed = speed * 0.5f;*/

                /*//判断是否到了随机巡逻点
                if (Vector3.Distance(wayPoint, transform.position) <= enemyAgent.stoppingDistance)
                {
                    //观察状态 -- 计时器的时间
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
                    //没有走到随机巡逻的点
                    *//*Debug.Log("正在走到巡逻点中");
                    Debug.Log("我与wayPoint点的距离为：" + Convert.ToString(Vector3.Distance(wayPoint, transform.position) <= enemyAgent.stoppingDistance));
                    Debug.Log("我与wayPoint点的距离为：" + Convert.ToString(Vector3.Distance(wayPoint, transform.position)));*//*

                    isGuard = false;
                    enemyAgent.destination = wayPoint;
                }*/


                //两点之间进行巡逻(有些Zombie为站桩
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
                //更新追击速度
                enemyAgent.speed = speed;

                //更新追击位置
                if (attackTarget != null)
                {
                    isGuard = false;
                    enemyAgent.destination = attackTarget.transform.position;
                }

                //TODO：能否被拉脱的机制？
                //如果被拉脱，则停在原地
                /*if (!FoundPlayer())
                {
                    //观察状态 -- 计时器的时间
                    if (remainLookAtTime2 > 0)
                    {
                        //留在原地
                        enemyAgent.destination = transform.position;
                        isGuard = true;
                        remainLookAtTime2 -= Time.deltaTime;
                        //Debug.Log("remainLookAtTime2:" + remainLookAtTime2);

                        //enemyState = EnemyState.PATROL;
                        //enemyAgent.isStopped = false;
                    }
                    else
                    {
                        //拉脱后回归巡逻的状态
                        isGuard = false;
                        enemyState = EnemyState.PATROL;
                        //更新计时器
                        remainLookAtTime2 = UnityEngine.Random.Range(0, lookAtTime);
                    }
                }*/

                //如果玩家在攻击范围内
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
                //直接关闭导航
                enemyAgent.enabled = false;

                //播放死亡动画
                animator.SetBool("Dead", true);
                //Debug.Log("死亡动画！");
                alreadyDied = true;
                //coll.enabled = false;

                //强制关闭Rigidbody
                zombieRigidbody.Sleep();

                //关闭血条
                ownHealthImage.SetActive(false);

                Destroy(gameObject, 3f);
                break;
        }
    }

    /*//切换动画
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

    //动画的切换（基于Velocity -1D
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

    //攻击
    void ZombieAttack()
    {
        //再次判断是否在攻击范围内
        if (TargetInAttackRange())
        {
            //Debug.Log("3");
            //Attack动作里会有一个Hit（）的一个函数，造成伤害
            animator.SetTrigger("Attack");
        }
    }

    //在范围内寻找玩家(sightRadius
    bool FoundPlayer()
    {
        //圆形内检测碰撞体
        var colliders = Physics.OverlapSphere(transform.position, feelPlayerDistance);
        foreach (var target in colliders)
        {
            if (target.CompareTag("Player"))
            {
                //找到攻击目标
                attackTarget = target.gameObject;
                //Debug.Log("发现Player！");
                //return true;

                //Debug.Log("正在计算Zombie与Playerd 的点乘");
                //发现Player玩家 后 再计算 点乘
                float dot = Vector3.Dot(centerPoint.transform.forward.normalized, (attackTarget.transform.position - centerPoint.transform.position).normalized);
                return dot >= dotThreshol;

            }
        }

        //没有找到攻击目标
        attackTarget = null;
        return false;
    }

    //扇形视野内寻找玩家
    private float dotThreshol = 0.5f;
    private bool IsFindPlayerInSector()
    {
        if(attackTarget != null)
        {
            Debug.Log("正在计算Zombie与Playerd 的点乘");
            float dot = Vector3.Dot(transform.forward, attackTarget.transform.position - transform.position);
            return dot >= dotThreshol;
        }
        else
        {
            return false;
        }
    }


    //随机巡逻点
    void GetNewWayPoint()
    {
        float randomX = UnityEngine.Random.Range(-patrolRange, patrolRange);
        float randomZ = UnityEngine.Random.Range(-patrolRange, patrolRange);
        Vector3 randomPoint = new Vector3(randomX + originalPosition.x, transform.position.y, randomZ + originalPosition.z);

        //排除不能导航到的点（返回的是bool
        NavMeshHit hit;
        wayPoint = NavMesh.SamplePosition(randomPoint, out hit, patrolRange, 1) ? hit.position : transform.position;

        //刷新计时器
        remainLookAtTime = UnityEngine.Random.Range(0, lookAtTime);
    }

    //绘画巡逻范围
    /*private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sightRadius);
    }*/

    //判断攻击目标是否在攻击范围
    bool TargetInAttackRange()
    {
        //Debug.Log("玩家在攻击范围内！");
        if (attackTarget != null)
            return Vector3.Distance(transform.position, attackTarget.transform.position) <= characterStats.attackData.attackRange;
        else
            return false;
    }

    //判断攻击目标是否在扇形的攻击范围
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
            //造成伤害
            targetStats.TakeDamage(characterStats, targetStats);
            //更新玩家血量
            playerHealthImage.fillAmount = (float)targetStats.CurrentHealth / targetStats.MaxHealth;

            WeaponManager.Instance.StartFlashCourtinue();
        }
    }



}
