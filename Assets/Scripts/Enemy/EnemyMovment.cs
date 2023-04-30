using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Scripts.Weapon;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;


public class EnemyMovment : MonoBehaviour
{
    //组件
    private NavMeshAgent enemyAgent;
    private Animator animator;
    private CharacterStats characterStats;
    private Collider coll;
    private Rigidbody enemyRigdbody;

    //public Image playerHealthImage;
    public GameObject ownHealthImage;

    /*public GameObject lookAtPoint;*/


    //枪械
    public ParticleSystem muzzleParticle;
    public ParticleSystem casingParticle;
    public Transform muzzlePoint;
    public Transform casingPoint;

    public GameObject bulletPrefab;

    //音效
    public AudioSource firearmsShootingAudioSource;
    public AudioSource firearmsReloadAudioSource;
    public FireArmsAudioData fireArmsAudioData;
    public ImpactAudioData impactAudioData;

    //ImpactPrefab
    public List<GameObject> bulletImpactPrefabs;



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
    private bool isDead = false;
    private bool isExposure;
    private bool isFindingPoint;
    private bool isAlreadyDied;

    //WallPoint
    public float findWallPointRange;
    public List<GameObject> wallPoints;
    private GameObject targetPoint;

    //某一面墙的点
    List<Transform> points = new List<Transform>();

    //敌人生成时刻存贮的前排墙的点
    List<Transform> prePoints = new List<Transform>();

    //敌人生成时刻存贮的后排墙的点
    List<Transform> afterPoints = new List<Transform>();

    //存贮point 和 距离的dic
    Dictionary<float, Transform> pointDit = new Dictionary<float, Transform>();

    //距离的列表
    List<float> distances = new List<float>();

    //暴露的射线层
    public LayerMask playerLayerMask;
    public GameObject rawPoint;
    
    
    

    private void Awake()
    {
        enemyRigdbody = GetComponent<Rigidbody>();
        coll = GetComponent<Collider>();
        enemyAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();
        originalPosition = transform.position;
        //记录导航的初始速度
        speed = enemyAgent.speed;
        //此时的导航速度
        velocity = enemyAgent.velocity;
        //计时器
        remainLookAtTime = lookAtTime;

    }

    private void Start()
    {
        //OpenOrCloseKinematic(isDead);

        //enemyAgent.updateRotation = false

        //初始赋予巡逻的状态
        enemyState = EnemyState.PATROL;

        //攻击冷却时间
        lastAttackTime = characterStats.attackData.coolDown;

        //FindWalls();
        //FindWalls2();
        //FindWalls3();
        //一开始就寻找一个最近的墙的子物体点
        FindWalls4();

        //enemyAgent.updateRotation = false;
    }


    private void FixedUpdate()
    {
        /*if (enemyAgent.hasPath)
        {
            RotateToTarget(enemyAgent.path.corners[1]);
        }*/

        //Debug.Log("isFinding:" + isFindingPoint);


        if (isAlreadyDied) return;

        //如果存在攻击目标
        if (attackTarget != null)
        {   
            //绘制射线
            Debug.DrawRay(transform.position, attackTarget.transform.position - transform.position, Color.red);

            /*//如果不是正在寻找掩护点
            if (!isFindingPoint)
            {
                //发出一条射线去检测Tag - player（并影响isExposure（bool
                IsExposureSelf();
            }*/

            IsExposureSelf();
        }


        //判断是否死亡
        if (characterStats.CurrentHealth == 0)
            isDead = true;

        /*if (attackTarget != null)
        {
            transform.LookAt(attackTarget.transform.position - transform.position);
        }
        else if (wayPoint != null)
        {
            transform.LookAt(wayPoint - transform.position);
        }*/



        SwitchStates();
        SwitchAnimation();
        lastAttackTime -= Time.deltaTime;
    }

    //敌人状态的切换
    void SwitchStates()
    {
        //只有在非死亡状态
        if (isDead)
            enemyState = EnemyState.DEAD;
        else if (FoundPlayer())
        {
            enemyAgent.isStopped = false;
            enemyState = EnemyState.CHASE;
        }
        /*else if(!FoundPlayer())
        {
            enemyState = EnemyState.PATROL;
        }*/

        //Debug.Log(enemyState);


        switch (enemyState)
        {
            case EnemyState.Attack:




                break;
            case EnemyState.GUARD:
                break;

            //寻找一个最近的掩体点
            case EnemyState.PATROL:
                //巡逻速度
                enemyAgent.speed = speed * 0.5f;
                //Debug.Log(" enemyAgent.speed" + enemyAgent.speed);


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
                    //Debug.Log("正在走到巡逻点中");
                    //Debug.Log("我与wayPoint点的距离为：" + Convert.ToString(Vector3.Distance(wayPoint, transform.position) <= enemyAgent.stoppingDistance));
                    //Debug.Log("我与wayPoint点的距离为：" + Convert.ToString(Vector3.Distance(wayPoint, transform.position)));

                    isGuard = false;
                    enemyAgent.destination = wayPoint;
                    //Debug.Log("enemyAgent.destination" + enemyAgent.destination);
                }*/

                if (Vector3.Distance(targetPoint.transform.position, transform.position) <= enemyAgent.stoppingDistance)
                {
                    //enemyAgent.stoppingDistance += 1f;
                    isGuard = true;
                    //enemyAgent.isStopped = true;

                    /*if (remainLookAtTime > 0)
                    {
                        isGuard = true;
                        remainLookAtTime -= Time.deltaTime;
                    }
                    else
                    {
                        //ChangeTheWallPoint();
                        ChangeThePointNextLists();
                    }*/

                    /*if(!isDead)
                    {
                        if (IsExposureSelf())
                        {
                            Debug.Log("Enemy 已经暴露了");
                            ChangeThePointNextLists();
                        }
                    }*/


                }
                else
                {
                    //Debug.Log("正在前往导航点");
                    enemyAgent.destination = targetPoint.transform.position;
                    
                }
                break;


            //进入攻击状态
            case EnemyState.CHASE:
                //更新追击速度
                enemyAgent.speed = speed;


                /*//更新追击位置
                if (attackTarget != null)
                {
                    isGuard = false;
                    enemyAgent.destination = attackTarget.transform.position;
                }

                //如果被拉脱，则停在原地
                if (!FoundPlayer())
                {
                    //观察状态 -- 计时器的时间
                    if (remainLookAtTime2 > 0)
                    {
                        //留在原地
                        //TODO：留在玩家最后出现的地方
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


                /*//如果玩家在攻击范围内
                if (TargetInAttackRange())
                {
                    isGuard = true;
                    enemyAgent.isStopped = true;
                    if (lastAttackTime < 0)
                    {
                        //Debug.Log("lastAttackTime:" + lastAttackTime);
                        lastAttackTime = characterStats.attackData.coolDown;
                        EnemyAttack();
                    }
                }*/

                /* //如果没有暴露
                if(!isExposure)
                {
                    Debug.Log("isExposure:" + isExposure);
                    //如果玩家在攻击范围内
                    if (TargetInAttackRange())
                    {
                        isGuard = true;
                        //enemyAgent.isStopped = true;
                        if (lastAttackTime < 0)
                        {
                            //Debug.Log("lastAttackTime:" + lastAttackTime);
                            lastAttackTime = characterStats.attackData.coolDown;
                            EnemyAttack();
                        }
                    }
                }
                //暴露                                            //isExposure 是否能够shot  //isFindingPoint 能否发出射线检测玩家
                else
                {
                    //enemyAgent.isStopped = false;
                    isGuard = false;
                    
                    //改变掩体（切换前后排（随机
                    ChangeThePointNextLists();

                    //AI接近改变掩体后的点
                    //此时到达掩体点
                    if (Vector3.Distance(targetPoint.transform.position, transform.position) <= enemyAgent.stoppingDistance)
                    {
                        isGuard = true;
                        isFindingPoint = false;
                    }
                    else
                    {
                        //Debug.Log("正在前往导航点");
                        enemyAgent.destination = targetPoint.transform.position;
                    }
                }*/



                //AI接近改变掩体后的点
                //此时到达掩体点
                if (Vector3.Distance(targetPoint.transform.position, transform.position) <= enemyAgent.stoppingDistance)
                {
                    isGuard = true;
                    isFindingPoint = false;

                    if (TargetInAttackRange())
                    {
                        isGuard = true;
                        //enemyAgent.isStopped = true;
                        if (lastAttackTime < 0)
                        {
                            //Debug.Log("lastAttackTime:" + lastAttackTime);
                            lastAttackTime = characterStats.attackData.coolDown;
                            EnemyAttack();
                        }
                    }
                }
                else
                {
                    //Debug.Log("正在前往导航点");
                    enemyAgent.destination = targetPoint.transform.position;
                }

                break;
            case EnemyState.DEAD:
                //直接关闭导航
                enemyAgent.enabled = false;

                //关闭碰撞体
                //coll.enabled = false;
                enemyRigdbody.isKinematic = true;
                //animator.SetTrigger("Dead");

                //Idle状态
                animator.SetFloat("Velocity", 0f);

                //关闭动画机
                //animator.enabled = false;

                //关闭血条
                ownHealthImage.SetActive(false);

                //关闭动画
                animator.enabled = false;
                isAlreadyDied = true;

                //Destroy(gameObject);

                OpenOrCloseKinematic();

                break;
        }
    }

    //切换动画
    /*private void SwitchAnimation()
    {
        if (isGuard)
        {
            //animator.SetFloat("Horizontal", 0, 0.2f, Time.deltaTime);
            *//*animator.SetFloat("Vertical", 0, 0.2f, Time.deltaTime);
            animator.SetFloat("Horizontal", 0, 0.2f, Time.deltaTime);*//*

        }
        else
        {
            *//*velocity = enemyAgent.velocity;*/

    /*if (attackTarget == null)
    {
        lookAtPoint.transform.LookAt(wayPoint);
    }
    else
    {
        lookAtPoint.transform.LookAt(attackTarget.transform.position);
    }*/



    /*animator.SetFloat("Horizontal", velocity.x, 0.2f, Time.deltaTime);
    animator.SetFloat("Vertical", velocity.z, 0.2f, Time.deltaTime);*//*
    //Debug.Log("Horizontal:" + velocity.x);
    //Debug.Log("Vertical:" + velocity.z);

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


    public GameObject body;
    //Enemy的Rigidbody Is Kinematic
    private void OpenOrCloseKinematic()
    {
        var colliders = body.GetComponentsInChildren<Collider>();
        var rigidBodys = body.GetComponentsInChildren<Rigidbody>();
        foreach(var rigidBody in rigidBodys)
        {
            rigidBody.isKinematic = false;
        }
        foreach (var collider in colliders)
        {
            collider.enabled = true;
        }
    }



    private void EnemyAttack()
    {
        if (TargetInAttackRange())
        {

            //animator.SetTrigger("Attack");
            AttackPlayer();

        }
    }

    //在范围内寻找玩家
    bool FoundPlayer()
    {
        //圆形内检测碰撞体
        var colliders = Physics.OverlapSphere(transform.position, sightRadius);
        foreach (var target in colliders)
        {
            if (target.CompareTag("Player"))
            {
                //找到攻击目标
                attackTarget = target.gameObject;
                return true;
            }
        }

        //没有找到攻击目标
        attackTarget = null;
        return false;
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

        if (attackTarget != null)
            return Vector3.Distance(transform.position, attackTarget.transform.position) <= characterStats.attackData.attackRange;
        else
        {
            isGuard = false;
            enemyAgent.isStopped = false;
            return false;
        }

    }

    /*void Hit()
    {
        if (attackTarget != null)
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();
            targetStats.TakeDamage(characterStats, targetStats);
            playerHealthImage.fillAmount = (float) targetStats.CurrentHealth / targetStats.MaxHealth;
        }

    }*/

    private void AttackPlayer()
    {
        Vector3 lookPosition = attackTarget.transform.position;

        Vector3 tmp_TargertDire = lookPosition - transform.position;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(tmp_TargertDire), Time.deltaTime * 50);

        //enemyAgent.isStopped = true;



        //枪焰特效
        muzzleParticle.Play();

        //音效
        firearmsShootingAudioSource.clip = fireArmsAudioData.shootingAudioClip;
        firearmsShootingAudioSource.Play();

        //实例化子弹
        CreateBullet();

        //弹壳特效
        casingParticle.Play();


    }


    private void CreateBullet()
    {

        //muzzlePoint.transform.TransformPoint(muzzlePoint.transform.localPosition);

        Vector3 lookRoation = attackTarget.transform.position - muzzlePoint.position;


        GameObject tmp_Bullet = Instantiate(bulletPrefab, muzzlePoint.position,
            Quaternion.LookRotation(lookRoation));

        //tmp_Bullet.AddComponent<Rigidbody>();


        tmp_Bullet.GetComponent<Bullet>().impactPrefabs = bulletImpactPrefabs;
        //获取impactAudioData
        tmp_Bullet.GetComponent<Bullet>().impactAudioData = impactAudioData;
        tmp_Bullet.GetComponent<Bullet>().bulletSpeed = 100;

        //销魂对象
        if (tmp_Bullet != null)
        {
            Destroy(tmp_Bullet, 5);
        }

        enemyAgent.isStopped = false;
    }


    //改变掩体(去离AI最远的一个点
    public void ChangeTheWallPoint()
    {
        remainLookAtTime = lookAtTime;
        isGuard = false;
        float maxDistance = 0f;
        foreach (var point in points)
        {
            if (Vector3.Distance(transform.position, point.transform.position) > maxDistance)
            {
                maxDistance = Vector3.Distance(transform.position, point.transform.position);
                targetPoint = point.transform.gameObject;
            }
        }
    }

    //改变掩体（去离自身的这个点最近的一个点
    private void ChangeThePointNextLists()
    {   
        //正在寻找的状态（此时是不能开枪的(也不能发出射线去检测玩家
        isFindingPoint = true;
        //remainLookAtTime = lookAtTime;
        

        //TODO：AI应该是前往一个能够掩护自己的一个位置
        //如果此时在前排（前往随机的一个后排
        if (prePoints.Contains(targetPoint.transform))
        {
            var tmp_Index = UnityEngine.Random.Range(0, afterPoints.Count);
            targetPoint = afterPoints[tmp_Index].transform.gameObject;
        }
        //同理
        else if(afterPoints.Contains(targetPoint.transform))
        {
            var tmp_Index = UnityEngine.Random.Range(0, prePoints.Count);
            targetPoint = prePoints[tmp_Index].transform.gameObject;
        }
    }


    /*//寻找墙体3（用的字典
    private void FindWalls3()
    {
        //范围内寻找Wall
        var colliders = Physics.OverlapSphere(transform.position, findWallPointRange);
        foreach (var colldier in colliders)
        {
            if (colldier.gameObject.CompareTag("Wall"))
            {
                //循环添加该墙的点
                for (int i = 0; i < 4; i++)
                {
                    //把这个墙体的点和距离放进去
                    pointDit[Vector3.Distance(colldier.transform.GetChild(0).transform.GetChild(i).transform.position, transform.position)] =
                        colldier.transform.GetChild(0).transform.GetChild(i);

                    //存贮距离的列表
                    distances.Add(Vector3.Distance(colldier.transform.GetChild(0).transform.GetChild(i).transform.position, transform.position));

                }
            }
        }

        FindThePoint3();
    }


    //寻找的点分为前排的点和后排的点（配合寻找墙体3
    private void FindThePoint3()
    {   
        float maxDistance = 10000f;
        distances.Sort();
        //存入前排的点
        for(int i = 0; i < 2; i++)
        {
            prePoints.Add(pointDit[distances[i]]);
            Debug.Log("前排的点分别为：" + prePoints[i].transform.gameObject.name);
        }
        //存入后排的点
        for(int j = 2; j < 4; j++)
        {
            afterPoints.Add(pointDit[distances[j]]);
            Debug.Log("后排的点分别为：" + afterPoints[j-2].transform.gameObject.name);
        }

        //设置最近的前排点为导航点
        foreach(var point in prePoints)
        {
            if(Vector3.Distance(transform.position, point.transform.position) < maxDistance)
            {
                targetPoint = point.transform.gameObject;
            }
        }
    }*/


    //寻找墙体4
    private void FindWalls4()
    {
        //范围内寻找Wall
        var colliders = Physics.OverlapSphere(transform.position, findWallPointRange);
        foreach (var colldier in colliders)
        {
            if (colldier.gameObject.CompareTag("Wall"))
            {
                
                //前排
                for(int i = 0; i < 2; i++)
                {
                    prePoints.Add(colldier.transform.GetChild(0).transform.GetChild(i));
                }

                //后排
                for(int j = 0; j < 2; j++)
                {
                    afterPoints.Add(colldier.transform.GetChild(1).transform.GetChild(j));
                }

                FindThePoint4();

                break;
            }
        }

        
    }

    ////寻找的点分为前排的点和后排的点（配合寻找墙体4
    private void FindThePoint4()
    {
        var tmp_Index = UnityEngine.Random.Range(0, prePoints.Count);
        targetPoint = prePoints[tmp_Index].transform.gameObject;
    }


    //如果暴露
    private void IsExposureSelf()
    {
        var dirction = attackTarget.transform.position - transform.position;
        //var Is_Hit =  Physics.Raycast(muzzlePoint.transform.position, dirction.normalized, out RaycastHit tmp_Hit, 5f);
        var Is_Hit =  Physics.Raycast(transform.position, dirction.normalized, out RaycastHit tmp_Hit, 7f);

        if (Is_Hit)
        {
            if (tmp_Hit.collider.gameObject.CompareTag("Player") && !isFindingPoint)
            {
                isGuard = false;
                ChangeThePointNextLists();
            }
        }
        /*else
        {
            isExposure = false;
        }*/
    }


    //时刻转向targetPosition
    void RotateToTarget(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, angle, 0f);
    }

    

    

}
