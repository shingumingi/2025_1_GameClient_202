using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Inventory/Database")]

public class ItemDatabaseSO : ScriptableObject
{
    public List<ItemSO> items = new List<ItemSO>();         //ItemSO�� ����Ʈ�� ���� �Ѵ�.

    // ĳ���� ���� ����
    private Dictionary<int, ItemSO> itemByld;               //ID�� ������ ã�� ���� ĳ��
    private Dictionary<string, ItemSO> itemByName;          //�̸����� ������ ã��

    public void Initialize()                                //�ʱ� ���� �Լ�
    {
        itemByld = new Dictionary<int, ItemSO>();           //���� ���� �߱� ������ Dictionary �Ҵ�
        itemByName = new Dictionary<string, ItemSO>();  

        foreach (var item in items)                         //items ����Ʈ�� ���� �Ǿ� �ִ°��� ������ Dictionary�� �Է��Ѵ�.
        {
            itemByld[item.id] = item;                   
            itemByName[item.itemName] = item;
        }
    }

    //ID�� ������ ã��
    public ItemSO GetItemByld(int id)                       
    {
        if (itemByld == null)                               //itemByld �� ĳ���� �Ǿ� ���� �ʴٸ� �ʱ�ȭ �Ѵ�.
        { 
            Initialize();
        }

        if (itemByld.TryGetValue(id, out ItemSO item))      //id ���� ã�Ƽ� ItemSO �� �����Ѵ�
            return item;

        return null;                                        // ������� NULL
    }

    public ItemSO GetItemByName(string name)                
    {
        if (itemByName == null)                             //itemByName �� ĳ���� �Ǿ� ���� �ʴٸ� �ʱ�ȭ �Ѵ�.
        {
            Initialize();
        }
        if (itemByName.TryGetValue(name, out ItemSO item))  // name ���� ã�Ƽ� ItemSO �� �����Ѵ�
            return item;

        return null;
    }

    public List<ItemSO> GetItemByType(ItemType type)
    {
        return items.FindAll(item => item.itemType == type);
    }
}
