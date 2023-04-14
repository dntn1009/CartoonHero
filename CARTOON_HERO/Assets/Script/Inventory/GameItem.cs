using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameItem : MonoBehaviour
{
    // 아이템 아이콘 + 아이템 개수 관리
    [SerializeField]
    UI2DSprite m_icon;

    [SerializeField]
    UILabel m_count;

    [SerializeField]
    GameObject Use_Btn;

    string m_type;

    public int Icode;
    public int use_check = -1;

    public void SetItem(string type, Sprite icon, int count, int Icode)
    {
        this.Icode = Icode;
        m_type = type;
        m_icon.sprite2D = icon;
        ResetItemInfo(count);

    }

    public int Decrease()
    {
        var count = int.Parse(m_count.text);
        count--;
        if (count <= 0f)
            return -1;
        ResetItemInfo(count);
        return count;
    }

    void ResetItemInfo(int count)
    {
        m_count.text = count.ToString();

        if (count == 1)

            m_count.transform.parent.gameObject.SetActive(false);

        else

            m_count.transform.parent.gameObject.SetActive(true);
    }

    public virtual int usebtn_check()
    {
        use_check = 0;
        return use_check;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
