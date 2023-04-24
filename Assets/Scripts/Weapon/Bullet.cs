using System.Collections;
using System.Collections.Generic;
using Scripts.Weapon;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    //���
    private Rigidbody bulletRigidbody;
    private Transform bulletTransform;
    public CharacterStats characterStats;

    //��Ч
    //public GameObject impactPrefab;
    public List<GameObject> impactPrefabs;
    
    //�ӵ�����
    public float bulletSpeed;
    private Vector3 prevPosition;

    //��Ч
    public ImpactAudioData impactAudioData;


    private void Start()
    {
        
        characterStats = GetComponent<CharacterStats>();
        bulletTransform = transform;
        prevPosition = bulletTransform.position;
    }

    private void Update()
    {
        //������һ֡�ӵ���λ��
        prevPosition = bulletTransform.position;
        //�ӵ�λ�õĸ���
        bulletTransform.Translate(0, 0, bulletSpeed * Time.deltaTime);
        //���ӵ���һ֡����һ֡�������߼���ڼ���ײ����Ϣ 
        if (Physics.Raycast(prevPosition,
                (bulletTransform.position - prevPosition).normalized,
                out RaycastHit tmp_Hit,
                (bulletTransform.position - prevPosition).magnitude))
        {

            

            //������е��ǽ�ʬ
            if (tmp_Hit.collider.gameObject.CompareTag("Zombie"))
            {
                //������ǽ�ʬ��characterStats
                BulletTakeDamage(characterStats, tmp_Hit);
                //Debug.Log("������Zombie��");
            }
            //���������Player
            if (tmp_Hit.collider.gameObject.CompareTag("Player"))
            {
                BulletTakeDamage(characterStats, tmp_Hit);
                //Debug.Log("Enemy������Player��");
            }

            //var tmp_BulletImpact =  Instantiate(impactPrefab, tmp_Hit.point, Quaternion.LookRotation(tmp_Hit.normal, Vector3.up));
            //Ѱ��BulletImpact������Ч

            foreach (var tmp_Impact in impactPrefabs)
            {
                //Debug.Log("����Ѱ����Ч");
                if (tmp_Impact.transform.gameObject.tag == tmp_Hit.collider.gameObject.tag)
                {
                    Instantiate(tmp_Impact, tmp_Hit.point, Quaternion.LookRotation(tmp_Hit.normal, Vector3.up));
                }
            }

            //Ѱ��Tag����ײ��Ч
            var tmp_TagsWithAudio = impactAudioData.impactTagsWithAudios.Find((_audioData) => { return _audioData.Tag.Equals(tmp_Hit.collider.tag); });

            //���ӽ�׳��
            if (tmp_TagsWithAudio != null)
            {
                int tmp_Length = tmp_TagsWithAudio.impactAudioClips.Count;
                AudioClip tmp_AudioClip = tmp_TagsWithAudio.impactAudioClips[Random.Range(0, tmp_Length)];

                //����ײ��������Ч
                AudioSource.PlayClipAtPoint(tmp_AudioClip, tmp_Hit.point);
            }


            //Destroy(tmp_BulletImpact, 3);
            Destroy(transform.gameObject);
        }
        
    }

    //�ӵ�����˺�
    private void BulletTakeDamage(CharacterStats _characterStat, RaycastHit _Hit)
    {
        /*var tmp_Damage = Random.Range(_characterStat.attackData.minDamage, _characterStat.attackData.maxDemage);
        _Hit.collider.transform.gameObject.GetComponent<CharacterStats>().characterData.currentHealth -= tmp_Damage;*/
        var tmp_Target = _Hit.collider.gameObject.GetComponent<CharacterStats>();
        tmp_Target.TakeDamage(_characterStat, tmp_Target);

    }

}
