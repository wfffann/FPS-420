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
        //衰减角度
        cameraSpringUtility.UpdateSpring(Time.deltaTime, Vector3.zero);
        //旋转角度的需要用到Slerp 球形插值
        cameraSpringTransform.localRotation = Quaternion.Slerp(cameraSpringTransform.localRotation,
            Quaternion.Euler(cameraSpringUtility.values), Time.deltaTime * 10);
    }

    //随机一个抖动的角度
    public void StartCameraSpring()
    {
        cameraSpringUtility.values = new Vector3(0,
            Random.Range(minRecoilRange.x, maxRecoilRange.x),
            Random.Range(minRecoilRange.y, maxRecoilRange.y));
    }
}

