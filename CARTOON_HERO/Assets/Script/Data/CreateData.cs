using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateData : MonoBehaviour
{
    [SerializeField]
    CreateManager createmanager;


    string Ch_Nick;
    string Ch_Type;
    string ch_num = LobbyData.Instance.userdata.ch_num;

    int[] inti_st = new int[4] { 5, 5, 5, 5}; // 초기 스텟 5555
    int Level = 1;
    int Exp = 0;
    int Point = 0;
    int Coin = 0;
    
    public class Character_Create
    {

        public string Nickname;
        public string Type;
        public int[] Stat;
        /*[0] : str
          [1] : dex
          [2] : luk
          [3] : int*/
        public int Level;
        public int Exp;
        public int Point;
        public int Coin;
        public Character_Create()
        {
        }

        public Character_Create(string Nickname, string Type, int[] Stat, int Level, int Exp, int Point, int Coin)
        {
            this.Nickname = Nickname;
            this.Type = Type;
            this.Stat = Stat;
            this.Level = Level;
            this.Exp = Exp;
            this.Point = Point;
            this.Coin = Coin;
        }
    }

    public class UserModi
    {

        public string uid;
        public string email;
        public string[] ch_num;
        
        public UserModi()
        {

        }
        public UserModi(string uid, string email, string[] ch_num)
        {
            this.uid = uid;
            this.email = email;
            this.ch_num = ch_num;
        }

    }
    DatabaseReference reference;
    Character_Create charcre;
    UserModi usermodi;
    
    #region Event Methods

    public void Modichnum()
    {
        string[] chnum = new string[2];
        usermodi = new UserModi(LobbyData.Instance.userdata.uid, LobbyData.Instance.userdata.email, chnum);
        string json = JsonUtility.ToJson(usermodi);
        reference.Child("Users").Child(LobbyData.Instance.userdata.uid).SetRawJsonValueAsync(json);

    }
    public void WriteCharacter(string Nickname, string Type, int[] Stat, int Level, int Exp, int Point, int Coin)
    {       
        charcre = new Character_Create(Nickname, Type, Stat, Level, Exp, Point, Coin);
        string json = JsonUtility.ToJson(charcre);
        reference.Child("Users").Child(LobbyData.Instance.userdata.uid).Child("ch_num").Child(ch_num).SetRawJsonValueAsync(json);
    }

    public void Create()
    {
        Ch_Nick = createmanager.Nick_string;
        Ch_Type = createmanager.Type;
        if(Ch_Nick.Length < 6 && Ch_Nick.Length > 1)
        {
            if(createmanager.Typechoice == true)
            {
                if (ch_num.Equals("0"))
                {
                    Modichnum();
                    WriteCharacter(Ch_Nick, Ch_Type, inti_st, Level, Exp, Point, Coin);
                     
                    LobbyData.Instance.Confirm_database();
                    if (LobbyData.Instance.chnumCheck)
                    {
                        // 들어오는거 확인
                        LobbyData.Instance.ch_numModify();
                        LobbyData.Instance.chnumCheck = false;
                    }
                    createmanager.CreateScene();
                }
                else if(ch_num.Equals("1"))
                {
                    WriteCharacter(Ch_Nick, Ch_Type, inti_st, Level, Exp, Point, Coin);
                    LobbyData.Instance.Confirm_database();
                    if (LobbyData.Instance.chnumCheck)
                    {
                        // 들어오는거 확인
                        LobbyData.Instance.ch_numModify();
                        LobbyData.Instance.chnumCheck = false;
                    }
                    createmanager.CreateScene();
                }
            }
            else
            {
                //"캐릭터를 선택해주세요"
            }
        }
        else
        {
           //"닉네임을 2자이상, 5자이하로 적어주세요.
        }
        
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;
    }


}
