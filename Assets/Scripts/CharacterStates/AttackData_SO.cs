using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Attack", menuName = "Attack")]
public class AttackData_SO : ScriptableObject
{

    public float attackRange;

    public int minDamage;

    public int maxDemage;

    public float coolDown;

    //±©»÷ºóµÄÉËº¦±¶ÂÊ
    public float criticalMultiplier;
}
