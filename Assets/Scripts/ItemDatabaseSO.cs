using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Inventory/Database")]

public class ItemDatabaseSO : ScriptableObject
{
    public List<ItemSO> items = new List<ItemSO>();         //ItemSO를 리스트로 관리 한다.

    // 캐싱을 위한 사전
    private Dictionary<int, ItemSO> itemByld;               //ID로 아이템 찾기 위한 캐싱
    private Dictionary<string, ItemSO> itemByName;          //이름으로 아이템 찾기

    public void Initialize()                                //초기 설정 함수
    {
        itemByld = new Dictionary<int, ItemSO>();           //위에 선언만 했기 때문에 Dictionary 할당
        itemByName = new Dictionary<string, ItemSO>();  

        foreach (var item in items)                         //items 리스트에 선언 되어 있는것을 가지고 Dictionary에 입력한다.
        {
            itemByld[item.id] = item;                   
            itemByName[item.itemName] = item;
        }
    }

    //ID로 아이템 찾기
    public ItemSO GetItemByld(int id)                       
    {
        if (itemByld == null)                               //itemByld 가 캐싱이 되어 있지 않다면 초기화 한다.
        { 
            Initialize();
        }

        if (itemByld.TryGetValue(id, out ItemSO item))      //id 값을 찾아서 ItemSO 를 리턴한다
            return item;

        return null;                                        // 없을경우 NULL
    }

    public ItemSO GetItemByName(string name)                
    {
        if (itemByName == null)                             //itemByName 이 캐싱이 되어 있지 않다면 초기화 한다.
        {
            Initialize();
        }
        if (itemByName.TryGetValue(name, out ItemSO item))  // name 값을 찾아서 ItemSO 를 리턴한다
            return item;

        return null;
    }

    public List<ItemSO> GetItemByType(ItemType type)
    {
        return items.FindAll(item => item.itemType == type);
    }
}
