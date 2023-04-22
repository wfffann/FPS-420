using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSpringUtility
{
    public Vector3 values;

    //过渡频率(值越大抖动越快
    private float frequence;
    //阻尼效果（值越大越快回到中心点
    private float damp;

    private Vector3 dampValues;

    public CameraSpringUtility(float _frequence, float _damp)
    {
        frequence = _frequence;
        damp = _damp;
    }


    //震屏的射击反馈
    public void UpdateSpring(float _deltalTime, Vector3 _target)
    {
        // value 优先衰减 以影响 dampValues的衰减
        values -= _deltalTime * frequence * dampValues;
        dampValues = Vector3.Lerp(dampValues, values - _target, damp * _deltalTime);
    }
}
