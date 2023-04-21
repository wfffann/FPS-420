using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPMouseLook : MonoBehaviour
{
    //组件
    [SerializeField] private Transform characterTransform;
    private Vector3 cameraRotation;
    private Transform cameraTransform;

    //数据
    public float mouseSensitivity;
    public Vector2 maxMinAngle;

    //后坐力
    public AnimationCurve recoilCurve;
    public Vector2 recoilRange;
    private float currentRecoilTime;
    private Vector2 currentRecoil;
    public float recoilFadeOutTime = 0.3f;
    //private CameraSpring cameraSpring;

    private void Start()
    {
        cameraTransform = transform;
        //currentRecoil = recoilRange;
        //cameraSpring = GetComponentInChildren<CameraSpring>();

        Cursor.lockState = CursorLockMode.Locked;
    }


    private void Update()
    {
        var temp_MouseX = Input.GetAxis("Mouse X");
        var temp_MouseY = Input.GetAxis("Mouse Y");

        cameraRotation.x -= temp_MouseY * mouseSensitivity;
        cameraRotation.y += temp_MouseX * mouseSensitivity;

        //后坐力
        CalculateRecoilOffset();

        //TODO：后坐力方向固定了？
        cameraRotation.x -= currentRecoil.x;
        cameraRotation.y += currentRecoil.y;

        //限制取值范围
        cameraRotation.x = Mathf.Clamp(cameraRotation.x, maxMinAngle.x, maxMinAngle.y);
        //更新摄像机角度
        cameraTransform.rotation = Quaternion.Euler(cameraRotation.x, cameraRotation.y, 0);
        //更新人物角度
        characterTransform.rotation = Quaternion.Euler(0, cameraRotation.y, 0);

    }

    //根据曲线和时间的对应分数的值，不断降低当前的后坐力
    private void CalculateRecoilOffset()
    {
        currentRecoilTime += Time.deltaTime;
        //计算当前分数
        float tmp_recoilFraction = currentRecoilTime / recoilFadeOutTime;
        //求得曲线对应的y值（分数越大值越小
        float tmp_recoilValue = recoilCurve.Evaluate(tmp_recoilFraction);
        //线性插值的处理（减小
        currentRecoil = Vector2.Lerp(Vector2.zero, currentRecoil, tmp_recoilValue);
    }

    //后坐力
    public void FirngForTest()
    {
        //每射击一次，增加一个后坐力的缓冲
        currentRecoil += recoilRange;
        //射击的震屏效果 
        //cameraSpring.StartCameraSpring();
        //重置计时器，这样让后续的后坐力值减小
        currentRecoilTime = 0;
    }

}
