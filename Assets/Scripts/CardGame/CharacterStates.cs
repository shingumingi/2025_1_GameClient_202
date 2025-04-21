using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting.Dependencies.Sqlite;

public class CharacterStates : MonoBehaviour
{
    public string characterName;
    public int maxHealth = 100;
    public int currentHealth;

    //UI ���
    public Slider healthBar;
    public TextMeshProUGUI healthText;

    //���� �߰� �Ǵ� ����
    public int maxMana = 10;            //�ִ� ����
    public int currentMana = 0;         //���� ����
    public Slider manaBar;              //���� �� UI
    public TextMeshProUGUI manaText;    //���� �ؽ�Ʈ UI


    // Start is called before the first frame update
    void Start()
    {
        currentMana = maxMana;
        UpdateUI();
    }

    // Update is called once per frame
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
    }

    public void UseMana(int amount)
    {
        currentMana -= amount;
        if(currentMana < 0)
        {
            currentMana = 0;
        }
        UpdateUI();
    }

    public void GainMana(int amount)
    {
        currentMana += amount;
        if(currentMana > maxMana)
        {
            currentMana = maxMana;
        }
        UpdateUI();
    }

    private void UpdateUI()
    {
        if(healthBar != null)
        {
            healthBar.value = (float)currentHealth / maxHealth;
        }
        if(healthText != null)
        {
            healthText.text = $"{currentHealth} / {maxHealth}";
        }
        if(manaBar != null)
        {
            manaBar.value = (float)currentMana / maxMana;
        }
        if(manaText != null)
        {
            manaText.text = $"{currentMana} / {manaText}";
        }
    }
}
