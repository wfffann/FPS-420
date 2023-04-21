using System.Collections;
using System.Collections.Generic;
using Scripts.Weapon;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    //组件
    private Rigidbody bulletRigidbody;
    private Transform bulletTransform;

    //特效
    public GameObject impactPrefab;

    //子弹数据
    public float bulletSpeed;
    private Vector3 prevPosition;

    //音效
    public ImpactAudioData impactAudioData;

    private void Start()
    {
        
        //characterStats = GetComponent<CharacterStats>();
        bulletTransform = transform;
        prevPosition = bulletTransform.position;
    }

    private void Update()
    {
        prevPosition = bulletTransform.position;
        bulletTransform.Translate(0, 0, bulletSpeed * Time.deltaTime);
        if (Physics.Raycast(prevPosition,
                (bulletTransform.position - prevPosition).normalized,
                out RaycastHit tmp_Hit,
                (bulletTransform.position - prevPosition).magnitude))
        {
            var tmp_BulletImpact =  Instantiate(impactPrefab, tmp_Hit.point, Quaternion.LookRotation(tmp_Hit.normal, Vector3.up));

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

            Destroy(tmp_BulletImpact, 3);
        }
        
    }

}
