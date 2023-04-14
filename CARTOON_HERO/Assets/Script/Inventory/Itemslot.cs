using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Itemslot : MonoBehaviour
{
    GameItem m_item;
    PotionItem potion_item;
    [SerializeField]
    bool m_isSelect;
    Inventory m_inven;

    public int usebtn_check = -1;

    public bool IsEmpty { get { return m_item == null && potion_item == null;  } }// get으로 아이템 값이 널값이면 false가 됌.
    //m_item = potion_item, normal_item 에서도 null 인걸 확인해야됌.
    public bool IsSelect { get { return m_isSelect; } set { m_isSelect = value; } }

    public void SetSlot(Inventory inven)
    {
        m_inven = inven;
    }

    public void OnSelect()
    {
        m_inven.OnSelectSlot(this);
        UseBtn_Check();
        if (usebtn_check == -1 || usebtn_check == 0)
        {
            m_inven.Usebtn.GetComponent<UIButton>().isEnabled = false;
        }
        else
        {
            m_inven.Usebtn.GetComponent<UIButton>().isEnabled = true;
        }
    }

    public void InitSlot(GameItem item)//potion인지 noraml인지 확인해야됌
    {
        if (item.Icode == 0 || item.Icode == 1)
            m_item = item;
        else if (item.Icode == 2 || item.Icode == 3)
            potion_item = (PotionItem)item;
        item.transform.SetParent(transform);
        item.transform.localPosition = Vector3.zero;
        item.transform.localScale = Vector3.one;

    }
    public void UseItem() //Postion
    {
        if (IsEmpty) return;
        var count = potion_item.Decrease();
        potion_item.Item_effect(potion_item.Icode);
        if (count == -1)
        {
            Destroy(potion_item.gameObject);
            potion_item = null;
        }
    }

    public void UseBtn_Check()
    {
        if(IsEmpty)
        {
            usebtn_check = -1;
        }
        if(m_item != null && potion_item == null)
        {
            usebtn_check = m_item.usebtn_check();
        }
        else if(m_item == null && potion_item != null)
        {
            usebtn_check = potion_item.usebtn_check();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

}
