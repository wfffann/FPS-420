using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPMouseLook : MonoBehaviour
{
    //���
    [SerializeField] private Transform characterTransform;
    private Vector3 cameraRotation;
    private Transform cameraTransform;

    //����
    public float mouseSensitivity;
    public Vector2 maxMinAngle;

    //������
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

        //������
        CalculateRecoilOffset();

        //TODO������������̶��ˣ�
        cameraRotation.x -= currentRecoil.x;
        cameraRotation.y += currentRecoil.y;

        //����ȡֵ��Χ
        cameraRotation.x = Mathf.Clamp(cameraRotation.x, maxMinAngle.x, maxMinAngle.y);
        //����������Ƕ�
        cameraTransform.rotation = Quaternion.Euler(cameraRotation.x, cameraRotation.y, 0);
        //��������Ƕ�
        characterTransform.rotation = Quaternion.Euler(0, cameraRotation.y, 0);

    }

    //�������ߺ�ʱ��Ķ�Ӧ������ֵ�����Ͻ��͵�ǰ�ĺ�����
    private void CalculateRecoilOffset()
    {
        currentRecoilTime += Time.deltaTime;
        //���㵱ǰ����
        float tmp_recoilFraction = currentRecoilTime / recoilFadeOutTime;
        //������߶�Ӧ��yֵ������Խ��ֵԽС
        float tmp_recoilValue = recoilCurve.Evaluate(tmp_recoilFraction);
        //���Բ�ֵ�Ĵ�����С
        currentRecoil = Vector2.Lerp(Vector2.zero, currentRecoil, tmp_recoilValue);
    }

    //������
    public void FirngForTest()
    {
        //ÿ���һ�Σ�����һ���������Ļ���
        currentRecoil += recoilRange;
        //���������Ч�� 
        //cameraSpring.StartCameraSpring();
        //���ü�ʱ���������ú����ĺ�����ֵ��С
        currentRecoilTime = 0;
    }

}
