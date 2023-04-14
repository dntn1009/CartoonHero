using Firebase.Database;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterData : DonDestory<CharacterData>
{
    [System.Serializable]
    public class Slot1
    {
        public string Nickname;
        public string Type;
        public int[] Stat;
        public int Level;
        public int Exp;
        public int Point;
        public int Coin;
        /*[0] : str
          [1] : dex
          [2] : luk
          [3] : int*/
        public Slot1()
        {
        }

        public Slot1(string Nickname, string Type, int[] Stat, int Level, int Exp, int Point, int Coin)
        {
            this.Nickname = Nickname;
            this.Type = Type;
            this.Stat = Stat;
            this.Level = Level;
            this.Exp = Exp;
            this.Point = Point;
            this.Coin = Coin;
        }
        public void SetExp(int Exp)
        {
            this.Exp = this.Exp + Exp;
        }

        public void LevelUP(int Exp)
        {
            this.Exp = this.Exp - Exp;
            this.Level = this.Level + 1;
            this.Point = this.Point + 5;
        }

        public void SetStat(int[] stat, int Point)
        {
            this.Stat = stat;
            this.Point = Point;
        }

        public void SetCoin(int coin)
        {
            this.Coin = this.Coin + coin;
        }

        public void MinusCoin(int price)
        {
            this.Coin = this.Coin - price;
        }
    }
    
    [System.Serializable]
    public class Slot2
    {
        public string Nickname;
        public string Type;
        public int[] Stat;
        public int Level;
        public int Exp;
        public int Point;
        public int Coin;
        /*[0] : str
          [1] : dex
          [2] : luk
          [3] : int*/
        public Slot2()
        {
        }

        public Slot2(string Nickname, string Type, int[] Stat, int Level, int Exp, int Point, int Coin)
        {
            this.Nickname = Nickname;
            this.Type = Type;
            this.Stat = Stat;
            this.Level = Level;
            this.Exp = Exp;
            this.Point = Point;
            this.Coin = Coin;
        }
        public void SetExp(int Exp)
        {
            this.Exp = this.Exp + Exp;
        }

        public void LevelUP(int Exp)
        {
            this.Exp = this.Exp - Exp;
            this.Level = this.Level + 1;
            this.Point = this.Point + 5;
        }
        public void SetStat(int[] stat, int Point)
        {
            this.Stat = stat;
            this.Point = Point;
        }

        public void SetCoin(int coin)
        {
            this.Coin = this.Coin + coin;
        }

        public void MinusCoin(int price)
        {
            this.Coin = this.Coin - price;
        }
    }

    [System.Serializable]
    public class ExpStandard
    {
        public int[] EXP;

        public ExpStandard()
        {
        }

        public ExpStandard(int[] EXP)
        {
            this.EXP = EXP;
        }
    }

    [Header("슬롯 1 데이터")]
    [SerializeField]
    public Slot1 slot1;
    [Header("슬롯 2 데이터")]
    [SerializeField]
    public Slot2 slot2;
    DatabaseReference reference;

    string slotnum;

    public bool Select_Slot1 = false;
    public bool Select_Slot2 = false;

    public bool ExpBarCheck = false;
    public bool LevelUpCheck = false;

    public bool DestoryCheck = false; // 오브젝트 파괴용

    [SerializeField]
    public ExpStandard exp_standard;

    public void ExpData_Fill()
    {
            FirebaseDatabase.DefaultInstance
           .GetReference("EXPStandard")
           .GetValueAsync().ContinueWithOnMainThread(task => {
               if (task.IsFaulted)
               {
                   Debug.Log("CharacterData Loading Fail");
                    // Handle the error...
                }
               else if (task.IsCompleted)
               {
                   DataSnapshot snapshot = task.Result;
                   foreach (DataSnapshot data in snapshot.Children)
                   {
                       string myData = data.GetRawJsonValue();
                       Debug.Log(myData);
                       exp_standard = JsonUtility.FromJson<ExpStandard>(myData);

                   }
               }
           });
    }
    public void SlotFill()
    {
        if(LobbyData.Instance.userdata.ch_num.Equals("1") || LobbyData.Instance.userdata.ch_num.Equals("2"))
        {

            FirebaseDatabase.DefaultInstance
         .GetReference("Users").Child(LobbyData.Instance.userdata.uid).Child("ch_num")
         .GetValueAsync().ContinueWithOnMainThread(task => {
             if (task.IsFaulted)
             {
                 Debug.Log("CharacterData Loading Fail");
             // Handle the error...
             }
             else if (task.IsCompleted)
             {
                 DataSnapshot snapshot = task.Result;
                 foreach (DataSnapshot data in snapshot.Children)
                 {
                     string name = data.Key.ToString();
                     string myData = null;
                     if (name.Equals("0"))
                     {
                         myData = data.GetRawJsonValue();
                         Debug.Log(myData);
                         slot1 = JsonUtility.FromJson<Slot1>(myData);
                     }
                 }
             }
         });

            if (LobbyData.Instance.userdata.ch_num.Equals("2"))
            {
                FirebaseDatabase.DefaultInstance
             .GetReference("Users").Child(LobbyData.Instance.userdata.uid).Child("ch_num")
             .GetValueAsync().ContinueWithOnMainThread(task => {
                 if (task.IsFaulted)
                 {
                     Debug.Log("CharacterData Loading Fail");
                 // Handle the error...
             }
                 else if (task.IsCompleted)
                 {
                     DataSnapshot snapshot = task.Result;
                     foreach (DataSnapshot data in snapshot.Children)
                     {
                         string name = data.Key.ToString();
                         string myData = null;
                         if (name.Equals("1"))
                         {
                             myData = data.GetRawJsonValue();
                             slot2 = JsonUtility.FromJson<Slot2>(myData);
                         }
                     }
                 }
             });
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        ExpData_Fill();
    }

    // Update is called once per frame
    void Update()
    {
        if(LobbyData.Instance.slotCheck)
        {
            SlotFill();
            LobbyData.Instance.slotCheck = false;
        }

        if(DestoryCheck == true)
        {
            Destroy(this.gameObject);
        }
    }
}
