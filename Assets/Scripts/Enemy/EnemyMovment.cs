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
    //���
    private NavMeshAgent enemyAgent;
    private Animator animator;
    private CharacterStats characterStats;
    private Collider coll;
    private Rigidbody enemyRigdbody;

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
    private bool isDead = false;
    private bool isExposure;
    private bool isFindingPoint;
    private bool isAlreadyDied;

    //WallPoint
    public float findWallPointRange;
    public List<GameObject> wallPoints;
    private GameObject targetPoint;

    //ĳһ��ǽ�ĵ�
    List<Transform> points = new List<Transform>();

    //��������ʱ�̴�����ǰ��ǽ�ĵ�
    List<Transform> prePoints = new List<Transform>();

    //��������ʱ�̴����ĺ���ǽ�ĵ�
    List<Transform> afterPoints = new List<Transform>();

    //����point �� �����dic
    Dictionary<float, Transform> pointDit = new Dictionary<float, Transform>();

    //������б�
    List<float> distances = new List<float>();

    //��¶�����߲�
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
        //��¼�����ĳ�ʼ�ٶ�
        speed = enemyAgent.speed;
        //��ʱ�ĵ����ٶ�
        velocity = enemyAgent.velocity;
        //��ʱ��
        remainLookAtTime = lookAtTime;

    }

    private void Start()
    {
        //OpenOrCloseKinematic(isDead);

        //enemyAgent.updateRotation = false

        //��ʼ����Ѳ�ߵ�״̬
        enemyState = EnemyState.PATROL;

        //������ȴʱ��
        lastAttackTime = characterStats.attackData.coolDown;

        //FindWalls();
        //FindWalls2();
        //FindWalls3();
        //һ��ʼ��Ѱ��һ�������ǽ���������
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

        //������ڹ���Ŀ��
        if (attackTarget != null)
        {   
            //��������
            Debug.DrawRay(transform.position, attackTarget.transform.position - transform.position, Color.red);

            /*//�����������Ѱ���ڻ���
            if (!isFindingPoint)
            {
                //����һ������ȥ���Tag - player����Ӱ��isExposure��bool
                IsExposureSelf();
            }*/

            IsExposureSelf();
        }


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
            case EnemyState.Attack:




                break;
            case EnemyState.GUARD:
                break;

            //Ѱ��һ������������
            case EnemyState.PATROL:
                //Ѳ���ٶ�
                enemyAgent.speed = speed * 0.5f;
                //Debug.Log(" enemyAgent.speed" + enemyAgent.speed);


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
                    //Debug.Log("�����ߵ�Ѳ�ߵ���");
                    //Debug.Log("����wayPoint��ľ���Ϊ��" + Convert.ToString(Vector3.Distance(wayPoint, transform.position) <= enemyAgent.stoppingDistance));
                    //Debug.Log("����wayPoint��ľ���Ϊ��" + Convert.ToString(Vector3.Distance(wayPoint, transform.position)));

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
                            Debug.Log("Enemy �Ѿ���¶��");
                            ChangeThePointNextLists();
                        }
                    }*/


                }
                else
                {
                    //Debug.Log("����ǰ��������");
                    enemyAgent.destination = targetPoint.transform.position;
                    
                }
                break;


            //���빥��״̬
            case EnemyState.CHASE:
                //����׷���ٶ�
                enemyAgent.speed = speed;


                /*//����׷��λ��
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
                        remainLookAtTime2 = UnityEngine.Random.Range(0, lookAtTime);
                    }
                }*/


                /*//�������ڹ�����Χ��
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

                /* //���û�б�¶
                if(!isExposure)
                {
                    Debug.Log("isExposure:" + isExposure);
                    //�������ڹ�����Χ��
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
                //��¶                                            //isExposure �Ƿ��ܹ�shot  //isFindingPoint �ܷ񷢳����߼�����
                else
                {
                    //enemyAgent.isStopped = false;
                    isGuard = false;
                    
                    //�ı����壨�л�ǰ���ţ����
                    ChangeThePointNextLists();

                    //AI�ӽ��ı������ĵ�
                    //��ʱ���������
                    if (Vector3.Distance(targetPoint.transform.position, transform.position) <= enemyAgent.stoppingDistance)
                    {
                        isGuard = true;
                        isFindingPoint = false;
                    }
                    else
                    {
                        //Debug.Log("����ǰ��������");
                        enemyAgent.destination = targetPoint.transform.position;
                    }
                }*/



                //AI�ӽ��ı������ĵ�
                //��ʱ���������
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
                    //Debug.Log("����ǰ��������");
                    enemyAgent.destination = targetPoint.transform.position;
                }

                break;
            case EnemyState.DEAD:
                //ֱ�ӹرյ���
                enemyAgent.enabled = false;

                //�ر���ײ��
                //coll.enabled = false;
                enemyRigdbody.isKinematic = true;
                //animator.SetTrigger("Dead");

                //Idle״̬
                animator.SetFloat("Velocity", 0f);

                //�رն�����
                //animator.enabled = false;

                //�ر�Ѫ��
                ownHealthImage.SetActive(false);

                //�رն���
                animator.enabled = false;
                isAlreadyDied = true;

                //Destroy(gameObject);

                OpenOrCloseKinematic();

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


    public GameObject body;
    //Enemy��Rigidbody Is Kinematic
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


    //�ı�����(ȥ��AI��Զ��һ����
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

    //�ı����壨ȥ�����������������һ����
    private void ChangeThePointNextLists()
    {   
        //����Ѱ�ҵ�״̬����ʱ�ǲ��ܿ�ǹ��(Ҳ���ܷ�������ȥ������
        isFindingPoint = true;
        //remainLookAtTime = lookAtTime;
        

        //TODO��AIӦ����ǰ��һ���ܹ��ڻ��Լ���һ��λ��
        //�����ʱ��ǰ�ţ�ǰ�������һ������
        if (prePoints.Contains(targetPoint.transform))
        {
            var tmp_Index = UnityEngine.Random.Range(0, afterPoints.Count);
            targetPoint = afterPoints[tmp_Index].transform.gameObject;
        }
        //ͬ��
        else if(afterPoints.Contains(targetPoint.transform))
        {
            var tmp_Index = UnityEngine.Random.Range(0, prePoints.Count);
            targetPoint = prePoints[tmp_Index].transform.gameObject;
        }
    }


    /*//Ѱ��ǽ��3���õ��ֵ�
    private void FindWalls3()
    {
        //��Χ��Ѱ��Wall
        var colliders = Physics.OverlapSphere(transform.position, findWallPointRange);
        foreach (var colldier in colliders)
        {
            if (colldier.gameObject.CompareTag("Wall"))
            {
                //ѭ����Ӹ�ǽ�ĵ�
                for (int i = 0; i < 4; i++)
                {
                    //�����ǽ��ĵ�;���Ž�ȥ
                    pointDit[Vector3.Distance(colldier.transform.GetChild(0).transform.GetChild(i).transform.position, transform.position)] =
                        colldier.transform.GetChild(0).transform.GetChild(i);

                    //����������б�
                    distances.Add(Vector3.Distance(colldier.transform.GetChild(0).transform.GetChild(i).transform.position, transform.position));

                }
            }
        }

        FindThePoint3();
    }


    //Ѱ�ҵĵ��Ϊǰ�ŵĵ�ͺ��ŵĵ㣨���Ѱ��ǽ��3
    private void FindThePoint3()
    {   
        float maxDistance = 10000f;
        distances.Sort();
        //����ǰ�ŵĵ�
        for(int i = 0; i < 2; i++)
        {
            prePoints.Add(pointDit[distances[i]]);
            Debug.Log("ǰ�ŵĵ�ֱ�Ϊ��" + prePoints[i].transform.gameObject.name);
        }
        //������ŵĵ�
        for(int j = 2; j < 4; j++)
        {
            afterPoints.Add(pointDit[distances[j]]);
            Debug.Log("���ŵĵ�ֱ�Ϊ��" + afterPoints[j-2].transform.gameObject.name);
        }

        //���������ǰ�ŵ�Ϊ������
        foreach(var point in prePoints)
        {
            if(Vector3.Distance(transform.position, point.transform.position) < maxDistance)
            {
                targetPoint = point.transform.gameObject;
            }
        }
    }*/


    //Ѱ��ǽ��4
    private void FindWalls4()
    {
        //��Χ��Ѱ��Wall
        var colliders = Physics.OverlapSphere(transform.position, findWallPointRange);
        foreach (var colldier in colliders)
        {
            if (colldier.gameObject.CompareTag("Wall"))
            {
                
                //ǰ��
                for(int i = 0; i < 2; i++)
                {
                    prePoints.Add(colldier.transform.GetChild(0).transform.GetChild(i));
                }

                //����
                for(int j = 0; j < 2; j++)
                {
                    afterPoints.Add(colldier.transform.GetChild(1).transform.GetChild(j));
                }

                FindThePoint4();

                break;
            }
        }

        
    }

    ////Ѱ�ҵĵ��Ϊǰ�ŵĵ�ͺ��ŵĵ㣨���Ѱ��ǽ��4
    private void FindThePoint4()
    {
        var tmp_Index = UnityEngine.Random.Range(0, prePoints.Count);
        targetPoint = prePoints[tmp_Index].transform.gameObject;
    }


    //�����¶
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


    //ʱ��ת��targetPosition
    void RotateToTarget(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, angle, 0f);
    }

    

    

}
