using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSpringUtility
{
    public Vector3 values;

    //����Ƶ��(ֵԽ�󶶶�Խ��
    private float frequence;
    //����Ч����ֵԽ��Խ��ص����ĵ�
    private float damp;

    private Vector3 dampValues;

    public CameraSpringUtility(float _frequence, float _damp)
    {
        frequence = _frequence;
        damp = _damp;
    }


    //�������������
    public void UpdateSpring(float _deltalTime, Vector3 _target)
    {
        // value ����˥�� ��Ӱ�� dampValues��˥��
        values -= _deltalTime * frequence * dampValues;
        dampValues = Vector3.Lerp(dampValues, values - _target, damp * _deltalTime);
    }
}
