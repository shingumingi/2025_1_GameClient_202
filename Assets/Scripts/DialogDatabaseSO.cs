using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogDatabase", menuName = "Dialog System/Database")]

public class DialogDatabaseSO : ScriptableObject
{
    public List<DialogSO> dialogs = new List<DialogSO>();

    private Dictionary<int, DialogSO> dialogsById;      //ĳ���� ���� ��ųʸ� ���

    public void Initailize()
    {
        dialogsById = new Dictionary<int, DialogSO>();

        foreach(var dialog in dialogs)
        {
            if (dialog != null)
            {
                dialogsById[dialog.id] = dialog;
            }
        }
    }

    public DialogSO GetDialogById(int id)
    {
        if(dialogsById == null)
            Initailize();

        if (dialogsById.TryGetValue(id,out DialogSO dialog))
            return dialog;

        return null;
    }
}
