using System.Collections;
using System.Collections.Generic;
using Scripts.Weapon;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyMovment : MonoBehaviour
{
    //组件
    private NavMeshAgent enemyAgent;
    private Animator animator;
    private CharacterStats characterStats;
    private Collider coll;

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
    private bool isDead;

    private void Awake()
    {
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
        //初始赋予巡逻的状态
        enemyState = EnemyState.PATROL;
        //攻击冷却时间
        lastAttackTime = characterStats.attackData.coolDown;

        //enemyAgent.updateRotation = false;
    }


    private void FixedUpdate()
    {
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
            case EnemyState.GUARD:
                break;
            case EnemyState.PATROL:
                //巡逻速度
                enemyAgent.speed = speed * 0.5f;
                //Debug.Log(" enemyAgent.speed" + enemyAgent.speed);


                //判断是否到了随机巡逻点
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
                    //没有到随机巡逻的点
                    isGuard = false;
                    enemyAgent.destination = wayPoint;
                    //Debug.Log("enemyAgent.destination" + enemyAgent.destination);
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
                        remainLookAtTime2 = Random.Range(0, lookAtTime);
                    }
                }

                //如果玩家在攻击范围内
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
                }

                break;
            case EnemyState.DEAD:
                //直接关闭导航
                enemyAgent.enabled = false;
                //coll.enable = false;
                //animator.SetTrigger("Dead");
                //关闭血条
                ownHealthImage.SetActive(false);
                animator.enabled = false;


                Destroy(gameObject);
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

    void EnemyAttack()
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
        float randomX = Random.Range(-patrolRange, patrolRange);
        float randomZ = Random.Range(-patrolRange, patrolRange);
        Vector3 randomPoint = new Vector3(randomX + originalPosition.x, transform.position.y, randomZ + originalPosition.z);

        //排除不能导航到的点（返回的是bool
        NavMeshHit hit;
        wayPoint = NavMesh.SamplePosition(randomPoint, out hit, patrolRange, 1) ? hit.position : transform.position;

        //刷新计时器
        remainLookAtTime = Random.Range(0, lookAtTime);
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
}
