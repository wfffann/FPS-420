using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterStats : MonoBehaviour
{
    public CharacterData_SO templateData;
    public CharacterData_SO characterData;
    public AttackData_SO attackData;

    public Image enemyHealth;


    private void Awake()
    {
        //�õ�һ��copy������
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


    //����˺�
    public void TakeDamage(CharacterStats attacker, CharacterStats defener)
    {
        int demage = Random.Range(attacker.attackData.minDamage, attacker.attackData.maxDemage);
        CurrentHealth = CurrentHealth - demage;
        if (CurrentHealth < 0) CurrentHealth = 0;
    }

    //����Ѫ��
    public void ChangeHealth()
    {
        enemyHealth.fillAmount = (float)CurrentHealth / MaxHealth;
    }

}
