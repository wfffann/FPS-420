using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CharacterStats : MonoBehaviour
{
    public CharacterData_SO templateData;
    public CharacterData_SO characterData;
    public AttackData_SO attackData;

    //Zombie的血量
    public Image enemyHealth;

    
    

    private void Awake()
    {
        //redImageCroutinue = Flash();

        //拿到一份copy的数据
        if (templateData != null)
        {
            characterData = Instantiate(templateData);
        }
    }

    public int MaxHealth
    {
        get => characterData != null ? characterData.maxHealth : 0;

        set => characterData.maxHealth = value;
    }

    public int CurrentHealth
    {
        get => characterData != null ? characterData.currentHealth : 0;

        set => characterData.currentHealth = value;
    }

    public void Update()
    {
        if (enemyHealth == null) return;
        ChangeHealth();
    }


    //造成伤害
    public void TakeDamage(CharacterStats attacker, CharacterStats defener)
    {
        int demage = UnityEngine.Random.Range(attacker.attackData.minDamage, attacker.attackData.maxDemage);
        CurrentHealth = CurrentHealth - demage;

        //StartInJuriedWithScreenCroutinue();
        //StartCoroutine(Flash());
        //StartFlashCourtinue();

        //血量下限
        if (CurrentHealth < 0) CurrentHealth = 0;
    }

    //更新血量
    public void ChangeHealth()
    {
        enemyHealth.fillAmount = (float)CurrentHealth / MaxHealth;
    }


    
   



    
}
