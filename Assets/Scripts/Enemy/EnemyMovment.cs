using System.Collections;
using System.Collections.Generic;
using Scripts.Weapon;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyMovment : MonoBehaviour
{
    //���
    private NavMeshAgent enemyAgent;
    private Animator animator;
    private CharacterStats characterStats;
    private Collider coll;

    //public Image playerHealthImage;
    public GameObject ownHealthImage;

    /*public GameObject lookAtPoint;*/


    //ǹе
    public ParticleSystem muzzleParticle;
    public ParticleSystem casingParticle;
    public Transform muzzlePoint;
    public Transform casingPoint;

    public GameObject bulletPrefab;

    //��Ч
    public AudioSource firearmsShootingAudioSource;
    public AudioSource firearmsReloadAudioSource;
    public FireArmsAudioData fireArmsAudioData;
    public ImpactAudioData impactAudioData;

    //ImpactPrefab
    public List<GameObject> bulletImpactPrefabs;



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

    private void Awake()
    {
        enemyAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();
        originalPosition = transform.position;
        //��¼�����ĳ�ʼ�ٶ�
        speed = enemyAgent.speed;
        //��ʱ�ĵ����ٶ�
        velocity = enemyAgent.velocity;
        //��ʱ��
        remainLookAtTime = lookAtTime;

    }

    private void Start()
    {
        //��ʼ����Ѳ�ߵ�״̬
        enemyState = EnemyState.PATROL;
        //������ȴʱ��
        lastAttackTime = characterStats.attackData.coolDown;

        //enemyAgent.updateRotation = false;
    }


    private void FixedUpdate()
    {
        //�ж��Ƿ�����
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

    //����״̬���л�
    void SwitchStates()
    {
        //ֻ���ڷ�����״̬
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
                //Ѳ���ٶ�
                enemyAgent.speed = speed * 0.5f;
                //Debug.Log(" enemyAgent.speed" + enemyAgent.speed);


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
                    //û�е����Ѳ�ߵĵ�
                    isGuard = false;
                    enemyAgent.destination = wayPoint;
                    //Debug.Log("enemyAgent.destination" + enemyAgent.destination);
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
                        //TODO��������������ֵĵط�
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
                        remainLookAtTime2 = Random.Range(0, lookAtTime);
                    }
                }

                //�������ڹ�����Χ��
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
                //ֱ�ӹرյ���
                enemyAgent.enabled = false;
                //coll.enable = false;
                //animator.SetTrigger("Dead");
                //�ر�Ѫ��
                ownHealthImage.SetActive(false);
                animator.enabled = false;


                Destroy(gameObject);
                break;
        }
    }

    //�л�����
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
        float randomX = Random.Range(-patrolRange, patrolRange);
        float randomZ = Random.Range(-patrolRange, patrolRange);
        Vector3 randomPoint = new Vector3(randomX + originalPosition.x, transform.position.y, randomZ + originalPosition.z);

        //�ų����ܵ������ĵ㣨���ص���bool
        NavMeshHit hit;
        wayPoint = NavMesh.SamplePosition(randomPoint, out hit, patrolRange, 1) ? hit.position : transform.position;

        //ˢ�¼�ʱ��
        remainLookAtTime = Random.Range(0, lookAtTime);
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



        //ǹ����Ч
        muzzleParticle.Play();

        //��Ч
        firearmsShootingAudioSource.clip = fireArmsAudioData.shootingAudioClip;
        firearmsShootingAudioSource.Play();

        //ʵ�����ӵ�
        CreateBullet();

        //������Ч
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
        //��ȡimpactAudioData
        tmp_Bullet.GetComponent<Bullet>().impactAudioData = impactAudioData;
        tmp_Bullet.GetComponent<Bullet>().bulletSpeed = 100;

        //�������
        if (tmp_Bullet != null)
        {
            Destroy(tmp_Bullet, 5);
        }

        enemyAgent.isStopped = false;
    }
}
