using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    const int BASE_SLOT_COUNT = 24;

    [SerializeField]
    TouchScreen touchscreen;

    [SerializeField]
    GameObject m_itemPrefab;

    [SerializeField]
    GameObject potion_itemPrefab;

    [SerializeField]
    GameObject m_slotPrefab;

    [SerializeField]
    UIGrid m_grid;

    [SerializeField]
    UIScrollView m_scrollView;

    [SerializeField]
    Transform m_cursor;

    [SerializeField]
    UILabel Coin_Label;

    [SerializeField]
    public UIButton Usebtn;

    List<Itemslot> m_slotList = new List<Itemslot>();

    //Slot 선택시에 효과
    public void OnSelectSlot(Itemslot selectslot)
    {
        var curSlot = m_slotList.Find(slot => slot.IsSelect);
        if (curSlot != null)
            curSlot.IsSelect = false;
        selectslot.IsSelect = true;
        if (!m_cursor.gameObject.activeSelf)
            m_cursor.gameObject.SetActive(true);
        m_cursor.transform.position = selectslot.transform.position;
    }

    public void OnUseSlotItem()
    {
        var curSlot = m_slotList.Find(slot => slot.IsSelect);
        if (curSlot != null)
        {
            curSlot.UseItem();
        }
    }

    public void CreateItem(int Icode, int Icount)
    { //  SetItem이면 NpC한테 포션아이템 소를 사면 2 중을사면 3
      // 일반 몬스터 드랍은 0 , 1
      // count 는 고른 count값에 따라 틀려지게
        int index = FindEmptySlot();
        if (index != -1)
        {
            string type = ItemData.Instance.item.Item[Icode];
            Sprite iconSprite = ItemData.Instance.m_iconSprites[Icode];
            int count = Icount;

            GameObject obj;
            GameItem item;
            if (Icode == 0 || Icode == 1)
            {
                obj = Instantiate(m_itemPrefab);
                item = obj.GetComponent<GameItem>();
                item.SetItem(type, iconSprite, count, Icode);
                m_slotList[index].InitSlot(item);
            }
            else if (Icode == 2 || Icode == 3)
            {
                obj = Instantiate(potion_itemPrefab);
                item = obj.GetComponent<PotionItem>();
                item.SetItem(type, iconSprite, count, Icode);
                m_slotList[index].InitSlot(item);
            }
        }
    }


    int FindEmptySlot()
    {
        for (int i = 0; i < m_slotList.Count; i++)
        {
            if (m_slotList[i].IsEmpty)
                return i;
        }
        return -1;
    }

    //Slot 생성
    void CreateSlot(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var obj = Instantiate(m_slotPrefab);
            obj.transform.SetParent(m_grid.transform);
            obj.transform.localPosition = Vector3.zero; // 아이템 프리팹이 엉뚱한 좌표로 있으면 이상해지기 때문에 제로로 설정해놈.
            obj.transform.localRotation = Quaternion.identity;// 이건 생략 가능 회전 되어있을 경우도 생각한거임.
            obj.transform.localScale = Vector3.one; // {1,1,1} 좌표
            //항상 좌표 설정 해줘야 함.
            var slot = obj.GetComponent<Itemslot>();
            slot.SetSlot(this);//만드는 자신의 인벤토리 정보를 넘겨줌.
            m_slotList.Add(slot);
        }

    }

    public void ShowUI()
    {
        gameObject.SetActive(true);
        touchscreen.touch_ScreeCheck = false;
        Monster_itemDrop();
        Shop_itemBuy();
        ShowCoin();
    }

    public void Monster_itemDrop()
    {
        if (MonsterManager.Instance.pocket.Pocket_check)
        {
            if (MonsterManager.Instance.pocket.mon_pocket.Slime_Count != 0)
            {
                CreateItem(0, MonsterManager.Instance.pocket.mon_pocket.Slime_Count);
                MonsterManager.Instance.pocket.mon_pocket.SlimeZero();
            }
            if (MonsterManager.Instance.pocket.mon_pocket.Turtle_Count != 0)
            {
                CreateItem(1, MonsterManager.Instance.pocket.mon_pocket.Turtle_Count);
                MonsterManager.Instance.pocket.mon_pocket.TurtleZero();
            }
            MonsterManager.Instance.pocket.Pocket_check = false;
        }
    }

    public void Shop_itemBuy()
    {
        if (CharacterManager.Instance.pocket.Pocket_Shop)
        {
            if(CharacterManager.Instance.pocket.shop_pocket.A_Count != 0)
            {
                CreateItem(2, CharacterManager.Instance.pocket.shop_pocket.A_Count);
                CharacterManager.Instance.pocket.shop_pocket.AZero();
            }
            
            if(CharacterManager.Instance.pocket.shop_pocket.B_Count != 0)
            {
                CreateItem(3, CharacterManager.Instance.pocket.shop_pocket.B_Count);
                CharacterManager.Instance.pocket.shop_pocket.BZero();
            }
            MonsterManager.Instance.pocket.Pocket_Shop = false;
        }
    }

    public void HideUI()
    {
        gameObject.SetActive(false);
        touchscreen.touch_ScreeCheck = true;
    }

    public void ShowCoin()
    {
        if(CharacterData.Instance.Select_Slot1)
        {
            Coin_Label.text = CharacterData.Instance.slot1.Coin.ToString();
        }
        else if(CharacterData.Instance.Select_Slot2)
        {
            Coin_Label.text = CharacterData.Instance.slot2.Coin.ToString();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        CreateSlot(BASE_SLOT_COUNT);
        Usebtn.GetComponent<UIButton>().isEnabled = false;
        m_cursor.gameObject.SetActive(false);
        HideUI();
    }

    // Update is called once per frame
    void Update()
    {
    }
}
