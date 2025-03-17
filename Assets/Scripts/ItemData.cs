using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemData
{
    public int id;
    public string itemName;
    public string description;
    public string nameEng;
    public string itemTypeString;
    [NonSerialized]
    public ItemType ItemType;
    public int price;
    public int power;
    public int level;
    public bool isStackable;
    public string iconPath;

    //���ڿ��� ���������� ��ȯ�ϴ� �޼���
    public void initalizeEnums()
    {
        if (Enum.TryParse(itemTypeString, out ItemType parsedType))
        {
            ItemType = parsedType;
        }
        else
        {
            Debug.LogError($"������ '{itemName}'�� ��ȿ���� ���� ������ Ÿ�� : {itemTypeString}");
            //�⺻�� ����
            ItemType = ItemType.Consumable;
        }
    }

}