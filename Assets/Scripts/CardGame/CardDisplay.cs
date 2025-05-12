using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Diagnostics.CodeAnalysis;
using UnityEngine.Timeline;
using Unity.VisualScripting;
using UnityEditorInternal;
using TreeEditor;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor.Experimental.GraphView;

public class CardDisplay : MonoBehaviour
{
    public CardData cardData;               //ī�� ������
    public int cardIndex;                   //���п��� �ε���

    //3D ī�� ���
    public MeshRenderer cardRenderer;       //ī�� ������ (icon or �Ϸ���Ʈ)
    public TextMeshPro nameText;            //�̸� �ؽ�Ʈ
    public TextMeshPro costText;            //��� �ؽ�Ʈ
    public TextMeshPro attackText;          //���ݷ�/ȿ�� �ؽ�Ʈ
    public TextMeshPro descriptionText;     //���� �ؽ�Ʈ

    //ī�� ����
    public bool isDragging = false;        //�� ���̾�
    private Vector3 originalPosition;       //�÷��̾� ���̾�

    private CardManager cardManager;        //ī�� �Ŵ��� ���� �߰�

    //���̾� ����ũ
    public LayerMask enemyLayer;
    public LayerMask playerLayer;

    void Start()
    {
        //���̾� ����ũ ����
        playerLayer = LayerMask.GetMask("Player");
        enemyLayer = LayerMask.GetMask("Enemy");

        cardManager = FindObjectOfType<CardManager>();

        SetupCard(cardData);
    }

    //ī�� ������ ����
    public void SetupCard(CardData data)
    {
        cardData = data;

        //3D �ؽ�Ʈ ������Ʈ
        if (nameText != null) nameText.text = data.cardName;
        if (costText != null) costText.text = data.manaCost.ToString();
        if(attackText != null) attackText.text = data.effectAmount.ToString();
        if(descriptionText != null) descriptionText.text = data.description;

        //ī�� �ؽ��� ����
        if (cardRenderer != null && data.artwork != null)
        {
            Material cardMaterial = cardRenderer.material;
            cardMaterial.mainTexture = data.artwork.texture;
        }

        //SetupCard �޼��忡�� ī�� ���� �ؽ�Ʈ�� �߰� ȿ�� �߰�
        if(descriptionText != null)
        {
            descriptionText.text = data.description + data.GetAdditionalEffectDescription();
        }
    }

    private void OnMouseDown()
    {
        //�巹�� ���� �� ���� ��ġ ����
        originalPosition = transform.position;
        isDragging = true;
    }

    private void OnMouseDrag()
    {
        if (isDragging)
        {
            //���콺 ��ġ�� ī�� �̵�
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Camera.main.WorldToScreenPoint(transform.position).z;
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            transform.position = new Vector3(worldPos.x, worldPos.y, transform.position.z);
        }
    }

    private void OnMouseUp()
    {
        isDragging = true;

        // ���� ī�� ���� ��ó ��� �ߴ��� �˻� (���� üũ ��)
        if(cardManager != null)
        {
            float distToDiscard = Vector3.Distance(transform.position, cardManager.discardPosition.position);

            if(distToDiscard < 2.0f)
            {
                cardManager.DiscardCard(cardIndex);         //���� �Ҹ� ���� ī�� ������
                return;
            }
        }

        //���⼭ ���� ī�� ��� ���� (���� üũ)
        CharacterStates playerstats = null;
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if(playerObj != null)
        {
            playerstats = playerObj.GetComponent<CharacterStates>();
        }

        if(playerstats == null || playerstats.currentMana < cardData.manaCost)
        {
            Debug.Log($"������ �����մϴ�! (�ʿ� : {cardData.manaCost}, ���� : {playerstats?.currentMana ?? 0})");
            transform.position = originalPosition;
            return;
        }

        //�����ɽ�Ʈ Ÿ�� ����
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //ī�� ��� ���� ���� ����
        bool cardUsed = false;

        //�� ���� ��� �ߴ��� �˻�
        if(Physics.Raycast(ray, out hit, Mathf.Infinity, enemyLayer))
        {
            CharacterStates enemyStats = hit.collider.GetComponent<CharacterStates>();      //������ ���� ȿ�� ����

            if (enemyStats != null)
            {
                if(cardData.cardType == CardData.CardType.Attack)           //ī�� ȿ���� ����
                {
                    enemyStats.TakeDamage(cardData.effectAmount);
                    Debug.Log($"{cardData.cardName} ī��� ������ {cardData.effectAmount} �������� �ԷȽ��ϴ�.");
                    cardUsed = true;
                }
            }
            else
            {
                Debug.Log("�� ī��� ������ ����� �� �����ϴ�.");
            }
        }
        else if(Physics.Raycast(ray, out hit, Mathf.Infinity, playerLayer))
        {
            if(playerstats != null)
            {
                if(cardData.cardType == CardData.CardType.Heal)             //ī�� ȿ���� ����
                {
                    playerstats.Heal(cardData.effectAmount);
                    Debug.Log($"{cardData.cardName} ī��� �÷��̾��� ü���� {cardData.effectAmount} ȸ�� �߽��ϴ�.");
                    cardUsed = true;
                }
            }
            else
            {
                Debug.Log("�� ī��� �÷��̾�� ����� �� �����ϴ�.");
            }
        }

        if(!cardUsed)           //ī�带 ������� �ʾҴٸ� ���� ��ġ�� �ǵ�����
        {
            transform.position = originalPosition;
            if(cardManager != null)
                cardManager.ArrangeHand();
            return;
        }

        //ī�� ��� �� ���� �Ҹ�
        playerstats.UseMana(cardData.manaCost);
        Debug.Log($"������ {cardData.manaCost} ��� �߽��ϴ�. (���� ���� : {playerstats.currentMana})");

        //�߰� ȿ���� �ִ� ��� ó��
        if(cardData.additionalEffects != null && cardData.additionalEffects.Count > 0)
        {
            ProcessAdditionalEffectsAndDiscard();               //�߰� ȿ�� ����
        }
        else
        {
            if (cardManager != null)
                cardManager.DiscardCard(cardIndex);             //�߰� ȿ���� ������ �ٷ� ������
        }
    }
    

    private void ProcessAdditionalEffectsAndDiscard()
    {
        //ī�� ������ �� �ε��� ����
        CardData cardDataCopy = cardData;
        int cardIndexCopy = cardIndex;

        //�߰� ȿ�� ����
        foreach(var effect in cardDataCopy.additionalEffects)
        {
            switch (effect.effectType)
            {
                case CardData.AdditionalEffectType.DrawCard:            // ��ο� ī�� ����
                    for(int i = 0; i < effect.effectAmount; i++)
                    {
                        if(cardManager != null)
                        {
                            cardManager.DrawCard();
                        }
                    }
                    Debug.Log($"{effect.effectAmount} ���� ī�带 ��ο� �߽��ϴ�.");
                    break;

                case CardData.AdditionalEffectType.DiscardCard:         //ī�� ������ ���� (���� ������)
                    for(int i = 0; i < effect.effectAmount; i++)
                    {
                        if(cardManager != null && cardManager.handCards.Count > 0)
                        {
                            int randomIndex = Random.Range(0, cardManager.handCards.Count);     // ���� ũ�� �������� ���� �ε��� ����

                            Debug.Log($"���� ī�� ������ : ���õ� �ε��� {randomIndex}, ���� ���� ũ�� : {cardManager.handCards.Count}");

                            if(cardIndexCopy < cardManager.handCards.Count)
                            {
                                if(randomIndex != cardIndexCopy)
                                {
                                    cardManager.DiscardCard(randomIndex);

                                    // ���� ���� ī���� �ε����� ���� ī���� �ε������� �۴ٸ� ���� ī���� �ε����� 1 ���� ���Ѿ� ��
                                    if(randomIndex < cardIndexCopy)
                                    {
                                        cardIndexCopy--;
                                    }
                                }
                                else if (cardManager.handCards.Count > 1)
                                {
                                    //�ٸ� ī�� ����
                                    int newIndex = (randomIndex + 1)% cardManager.handCards.Count;
                                    cardManager.DiscardCard(newIndex);

                                    if (randomIndex < cardIndexCopy)
                                    {
                                        cardIndexCopy--;
                                    }
                                }
                            }
                            else
                            {
                                //CardIndexCopy �� ���̻� ��ȿ���� ���� ���, �ƹ� ī�峪 ����
                                cardManager.DiscardCard(randomIndex);
                            }
                        }
                    }
                    Debug.Log($"�������� {effect.effectAmount} ���� ī�带 ���Ƚ��ϴ�.");
                    break;

                case CardData.AdditionalEffectType.GainMana:        // �÷��̾� ���� ȹ��
                    GameObject playerObject = GameObject.FindGameObjectWithTag("Player");   // �±׸� ����Ͽ� �÷��̾� ĳ���� ã��
                    if(playerObject != null)
                    {
                        CharacterStates playerStates = playerObject.GetComponent<CharacterStates>();
                        if(playerStates != null)
                        {
                            playerStates.GainMana(effect.effectAmount);
                            Debug.Log($"������ {effect.effectAmount} ȹ�� �߽��ϴ�! (���� ���� : {playerStates.currentMana})");
                        }
                    }
                    break;

                case CardData.AdditionalEffectType.ReduceEnemyMana:        // �� ���� ����
                    GameObject[] enemies = GameObject.FindGameObjectsWithTag("Ebentg");   // �±׸� ����Ͽ� �÷��̾� ĳ���� ã��
                    foreach(var enemy in enemies)
                    {
                        CharacterStates enemyStates = enemy.GetComponent<CharacterStates>();
                        if (enemyStates != null)
                        {
                            enemyStates.GainMana(effect.effectAmount);
                            Debug.Log($"������ {enemyStates.characterName} �� ������ {effect.effectAmount} ���� ���׽��ϴ�");
                        }
                    }
                    break;

                case CardData.AdditionalEffectType.ReduceCardCost:      //���� ī�� ��� ���� ȿ��(�ð������θ�. ���� ȿ�� X)
                    for(int i = 0; i < cardManager.cardObjects.Count; i++)
                    {
                        CardDisplay display = cardManager.cardObjects[i].GetComponent<CardDisplay>();
                        if(display != null && display != this)      //���� ī�� ����
                        {
                            TextMeshPro costText = display.costText;
                            if(costText != null)
                            {
                                int originalCost = display.cardData.manaCost;
                                int newCost = Mathf.Max(0, originalCost - effect.effectAmount);
                                costText.text = newCost.ToString();
                                costText.color = Color.green;       //���ҵ� ����� ������� ǥ��
                            }
                        }
                    }
                    break;
            }
        }

        //ȿ�� ���� �� ���� ī�� ������
        if (cardManager != null)
            cardManager.DiscardCard(cardIndexCopy);
    }
}
