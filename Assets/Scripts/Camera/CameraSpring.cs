using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSpring : MonoBehaviour
{
    public float frequence = 25;
    public float damp = 15;


    public Vector2 minRecoilRange;
    public Vector2 maxRecoilRange;

    private CameraSpringUtility cameraSpringUtility;
    private Transform cameraSpringTransform;

    private void Start()
    {
        cameraSpringUtility = new CameraSpringUtility(frequence, damp);
        cameraSpringTransform = transform;
    }

    private void Update()
    {
        //˥���Ƕ�
        cameraSpringUtility.UpdateSpring(Time.deltaTime, Vector3.zero);
        //��ת�Ƕȵ���Ҫ�õ�Slerp ���β�ֵ
        cameraSpringTransform.localRotation = Quaternion.Slerp(cameraSpringTransform.localRotation,
            Quaternion.Euler(cameraSpringUtility.values), Time.deltaTime * 10);
    }

    //���һ�������ĽǶ�
    public void StartCameraSpring()
    {
        cameraSpringUtility.values = new Vector3(0,
            Random.Range(minRecoilRange.x, maxRecoilRange.x),
            Random.Range(minRecoilRange.y, maxRecoilRange.y));
    }
}

