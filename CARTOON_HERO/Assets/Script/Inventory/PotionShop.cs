using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionShop : MonoBehaviour
{
    DatabaseReference reference;

    [SerializeField]
    TouchScreen touchscreen;

    [SerializeField]
    UILabel a_count;

    [SerializeField]
    UILabel b_count;

    int a_price = 20;
    int b_price = 40;

    [SerializeField]
    UILabel Coin;

    [SerializeField]
    UILabel Price_all; // 총 구매 금액

    [SerializeField]
    UILabel Warning_Label; // 금액 부족 경고

    int a = 0; // 회복 포션 (소)

    int b = 0; // 회복 포션 (중)

    int price = 0;
    public void A_CountPlus()
    {
        a = int.Parse(a_count.text) + 1;
        a_count.text = a.ToString();

        price = (a_price * a) + (b_price * b);
        Price_all.text = price.ToString();
    }

    public void B_CountPlus()
    {
        b = int.Parse(b_count.text) + 1;
        b_count.text = b.ToString();

        price = (a_price * a) + (b_price * b);
        Price_all.text = price.ToString();
    }

    public void Buy_Btn()
    {
        if (CharacterData.Instance.Select_Slot1) // 계정이 Slot1일 경우
        {
            if (price <= CharacterData.Instance.slot1.Coin)
            {
                if (a != 0)
                {
                    CharacterManager.Instance.pocket.shop_pocket.SetA(a);
                    CharacterManager.Instance.pocket.Pocket_Shop = true;
                }

                if (b != 0)
                {
                    CharacterManager.Instance.pocket.shop_pocket.SetB(b);
                    CharacterManager.Instance.pocket.Pocket_Shop = true;
                }
                CharacterData.Instance.slot1.MinusCoin(price);
                reference.Child("Users").Child(LobbyData.Instance.userdata.uid).Child("ch_num").Child("0").Child("Coin").SetValueAsync(CharacterData.Instance.slot1.Coin);
                a_count.text = "0";
                b_count.text = "0";
                Price_all.text = "0";
                Coin.text = CharacterData.Instance.slot1.Coin.ToString();
            }
            else
            {
                Warning_Label.text = "금액 부족!";
                a_count.text = "0";
                b_count.text = "0";
                Price_all.text = "0";
            }
        }
        else if( CharacterData.Instance.Select_Slot2) // 계정이 Slot2일 경우
        {
            if (price <= CharacterData.Instance.slot2.Coin)
            {
                if (a != 0)
                {
                    CharacterManager.Instance.pocket.shop_pocket.SetA(a);
                    CharacterManager.Instance.pocket.Pocket_Shop = true;
                }

                if (b != 0)
                {
                    CharacterManager.Instance.pocket.shop_pocket.SetB(b);
                    CharacterManager.Instance.pocket.Pocket_Shop = true;
                }
                CharacterData.Instance.slot2.MinusCoin(price);
                reference.Child("Users").Child(LobbyData.Instance.userdata.uid).Child("ch_num").Child("1").Child("Coin").SetValueAsync(CharacterData.Instance.slot2.Coin);
                a_count.text = "0";
                b_count.text = "0";
                Price_all.text = "0";
                Coin.text = CharacterData.Instance.slot2.Coin.ToString();
            }
            else
            {
                Warning_Label.text = "금액 부족!";
                a_count.text = "0";
                b_count.text = "0";
                Price_all.text = "0";
            }
        }
    }

    public void ShowUI()
    {
        gameObject.SetActive(true);
        Warning_Label.text = "";
        a_count.text = "0";
        b_count.text = "0";
        Price_all.text = "0";
        touchscreen.touch_ScreeCheck = false;

        
        Pre_Coin();
    }

    public void HideUI()
    {
        gameObject.SetActive(false);
        touchscreen.touch_ScreeCheck = true;
    }

    public void Pre_Coin()
    {
        if(CharacterData.Instance.Select_Slot1)
            Coin.text = CharacterData.Instance.slot1.Coin.ToString();
        else if(CharacterData.Instance.Select_Slot2)
            Coin.text = CharacterData.Instance.slot2.Coin.ToString();
    }

    // Start is called before the first frame update
    void Start()
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
