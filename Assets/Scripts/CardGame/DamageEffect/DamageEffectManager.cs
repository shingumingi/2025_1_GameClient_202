using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DamageEffectManager : MonoBehaviour
{
    [SerializeField] private GameObject textPrefab;         //�ؽ�Ʈ ������
    [SerializeField] private Canvas uiCanvas;               //UI ĵ���� ����

    public static DamageEffectManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if (uiCanvas == null)
        {
            uiCanvas = FindObjectOfType<Canvas>();
            if (uiCanvas == null)
            {
                Debug.LogError("UI �������� ã�� �� �����ϴ�.");
            }
        }
    }

    public void ShowDamageText(Vector3 position, string text, Color color, bool isCritical = false, bool isStatusEffect = false)
    {
        if (textPrefab == null || uiCanvas == null) return;

        Vector3 screenPos = Camera.main.WorldToScreenPoint(position);           //���� ��ǥ�� ��ũ�� ��ǥ�� ��ȯ

        if (screenPos.z < 0) return;                                            //UI�� ī�޶� �ڿ� �ִ� ��� ǥ������ ����

        GameObject damageText = Instantiate(textPrefab, uiCanvas.transform);    //������ �ؽ�Ʈ UI ����

        RectTransform rectTransform = damageText.GetComponent<RectTransform>(); //��ũ�� ��ġ ����
        if(rectTransform != null)
        {
            rectTransform.position = screenPos;
        }

        TextMeshProUGUI tmp = damageText.GetComponent<TextMeshProUGUI>();       //�ؽ�Ʈ ������Ʈ ����
        if(tmp != null)
        {
            tmp.text = text;                            //�ؽ�Ʈ ����
            tmp.color = color;                          //���� ����
            tmp.outlineColor = new Color(               //�ƿ����� ���� ����
                Mathf.Clamp01(color.r - 0.3f),
                Mathf.Clamp01(color.g - 0.3f),
                Mathf.Clamp01(color.b - 0.3f),
                color.a
            );

            float scale = 1.0f;                         //ũ�� ����

            int numbericValue;                          //�ؽ�Ʈ�� ������ ��� ���� ���� ũ�� ����
            if(int.TryParse(text.Replace("+","").Replace("CRIT!","").Replace("HEAL CRIT",""), out numbericValue))
            {
                scale = Mathf.Clamp(numbericValue / 15f, 0.8f, 2.5f);
            }

            if (isCritical) scale = 1.4f;               //ũ��Ƽ���̸� ũ�� ����
            if (isStatusEffect) scale *= 0.8f;          //���� ȿ���� �ణ �۰�

            damageText.transform.localScale = new Vector3(scale, scale, scale);
        }

        DamageTextEffect effect = damageText.GetComponent<DamageTextEffect>();
        if(effect != null)
        {
            effect.Initialized(isCritical, isStatusEffect);
            if (isStatusEffect)
            {
                effect.SetVerticalMovement();
            }
        }

    }

    public void ShowDamage(Vector3 position, int amount, bool isCritical = false)       //������ �Լ�
    {
        string text = amount.ToString();
        Color color = isCritical ? new Color(1.0f, 0.8f, 0.0f) : new Color(1.0f, 0.3f, 0.3f);

        if (isCritical)
        {
            text = "CRIT!\n" + text;
        }

        ShowDamageText(position, text, color, isCritical);
    }

    public void ShowHeal(Vector3 position, int amount, bool isCritical = false)         //���� �Լ�
    {
        string text = amount.ToString();
        Color color = isCritical ? new Color(0.4f, 1.0f, 0.4f) : new Color(0.3f, 0.9f, 0.3f);

        if (isCritical)
        {
            text = "Heal CRIT!\n" + text;
        }

        ShowDamageText(position, text, color, isCritical);
    }

    public void ShowMiss(Vector3 position)                  //�̽� �Լ�
    {
        ShowDamageText(position, "MISS", Color.gray, false);
    }

    public void ShowStatusEffect(Vector3 position, string effectName)
    {
        Color color;

        switch (effectName.ToLower())
        {
            case "poison":
                color = new Color(0.5f, 0.1f, 0.5f);        //�����
                break;
            case "burn":
                color = new Color(1.0f, 0.4f, 0.0f);        //��Ȳ��
                break;
            case "freeze":
                color = new Color(0.5f, 0.8f, 1.0f);        //�ϴû�
                break;
            case "stun":
                color = new Color(1.0f, 1.0f, 0.0f);        //�����
                break;
            default:
                color = new Color(1.0f, 1.0f, 1.0f);        //�⺻ ȸ��
                break;
        }

        ShowDamageText(position, effectName.ToLower(), color, false, true);
    }
}
