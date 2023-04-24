using System.Collections;
using System.Collections.Generic;
using Scripts.Weapon;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    //组件
    private Rigidbody bulletRigidbody;
    private Transform bulletTransform;
    public CharacterStats characterStats;

    //特效
    //public GameObject impactPrefab;
    public List<GameObject> impactPrefabs;
    
    //子弹数据
    public float bulletSpeed;
    private Vector3 prevPosition;

    //音效
    public ImpactAudioData impactAudioData;


    private void Start()
    {
        
        characterStats = GetComponent<CharacterStats>();
        bulletTransform = transform;
        prevPosition = bulletTransform.position;
    }

    private void Update()
    {
        //更新上一帧子弹的位置
        prevPosition = bulletTransform.position;
        //子弹位置的更新
        bulletTransform.Translate(0, 0, bulletSpeed * Time.deltaTime);
        //从子弹上一帧到下一帧发射射线检测期间碰撞的信息 
        if (Physics.Raycast(prevPosition,
                (bulletTransform.position - prevPosition).normalized,
                out RaycastHit tmp_Hit,
                (bulletTransform.position - prevPosition).magnitude))
        {

            

            //如果打中的是僵尸
            if (tmp_Hit.collider.gameObject.CompareTag("Zombie"))
            {
                //传入的是僵尸的characterStats
                BulletTakeDamage(characterStats, tmp_Hit);
                //Debug.Log("打中了Zombie！");
            }
            //如果打中了Player
            if (tmp_Hit.collider.gameObject.CompareTag("Player"))
            {
                BulletTakeDamage(characterStats, tmp_Hit);
                //Debug.Log("Enemy打中了Player！");
            }

            //var tmp_BulletImpact =  Instantiate(impactPrefab, tmp_Hit.point, Quaternion.LookRotation(tmp_Hit.normal, Vector3.up));
            //寻找BulletImpact弹孔特效

            foreach (var tmp_Impact in impactPrefabs)
            {
                //Debug.Log("正在寻找特效");
                if (tmp_Impact.transform.gameObject.tag == tmp_Hit.collider.gameObject.tag)
                {
                    Instantiate(tmp_Impact, tmp_Hit.point, Quaternion.LookRotation(tmp_Hit.normal, Vector3.up));
                }
            }

            //寻找Tag的碰撞音效
            var tmp_TagsWithAudio = impactAudioData.impactTagsWithAudios.Find((_audioData) => { return _audioData.Tag.Equals(tmp_Hit.collider.tag); });

            //增加健壮性
            if (tmp_TagsWithAudio != null)
            {
                int tmp_Length = tmp_TagsWithAudio.impactAudioClips.Count;
                AudioClip tmp_AudioClip = tmp_TagsWithAudio.impactAudioClips[Random.Range(0, tmp_Length)];

                //在碰撞点生成音效
                AudioSource.PlayClipAtPoint(tmp_AudioClip, tmp_Hit.point);
            }


            //Destroy(tmp_BulletImpact, 3);
            Destroy(transform.gameObject);
        }
        
    }

    //子弹造成伤害
    private void BulletTakeDamage(CharacterStats _characterStat, RaycastHit _Hit)
    {
        /*var tmp_Damage = Random.Range(_characterStat.attackData.minDamage, _characterStat.attackData.maxDemage);
        _Hit.collider.transform.gameObject.GetComponent<CharacterStats>().characterData.currentHealth -= tmp_Damage;*/
        var tmp_Target = _Hit.collider.gameObject.GetComponent<CharacterStats>();
        tmp_Target.TakeDamage(_characterStat, tmp_Target);

    }

}
