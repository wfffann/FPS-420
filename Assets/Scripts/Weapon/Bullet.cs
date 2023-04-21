using System.Collections;
using System.Collections.Generic;
using Scripts.Weapon;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    //���
    private Rigidbody bulletRigidbody;
    private Transform bulletTransform;

    //��Ч
    public GameObject impactPrefab;

    //�ӵ�����
    public float bulletSpeed;
    private Vector3 prevPosition;

    //��Ч
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

            Destroy(tmp_BulletImpact, 3);
        }
        
    }

}
