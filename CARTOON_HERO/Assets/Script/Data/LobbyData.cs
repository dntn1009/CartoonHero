using Firebase.Database;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyData : DonDestory<LobbyData>
{
    [System.Serializable]
    public class UserData
    {

        public string email;
        public string uid;
        public string ch_num;

        public static UserData guserdata = new UserData();
        public UserData()
        {
        }

        public UserData(string email, string uid, string ch_num)
        {
            this.email = email;
            this.uid = uid;
            this.ch_num = ch_num;
        }
        public void setnum(string ch_num)
        {
            this.ch_num = ch_num;
        }
    }

    Firebase.Auth.FirebaseAuth auth;
    Firebase.Auth.FirebaseUser user;
    DatabaseReference reference;
    Lobby_UIManager uimanager;
  //  User users;

    //auth data var
    string email;
    string uid;

    //bool auth true check
    bool Auth_Current = false;

    //ch_num Check bool
    public bool chnumCheck = false;

    //slot check bool
    public bool slotCheck = false;

    public bool DestoryCheck = false; // 오브젝트 파괴용

    //UserData
    [Header("유저 데이터")]
    [SerializeField]
    public UserData userdata;


    #region Event Methods


    public void Auth_Check()
    {
        if (auth.CurrentUser != null)
        {
            if (user != null)
            {
                email = user.Email;
                uid = user.UserId;
                Auth_Current = true;
                Debug.Log("Email : " + email + "uid : " + uid);
            }
            else
            {
                uimanager.QuitScreen();
                uimanager.Guide_Update("잘못된 로그인 방식입니다. (User Data NULL)");
                Debug.Log("user 값이 안뜨네용");
            }
        }
        else
        {
            uimanager.QuitScreen();
            uimanager.Guide_Update("잘못된 로그인 방식입니다. (User Data NULL)");
            Debug.Log("값이 안들어왔으용");
        }
    }

    public void WriteUser(string userID, string uid, string num)
    {
        userdata = new UserData(userID, uid, num);
        string json = JsonUtility.ToJson(userdata);
        reference.Child("Users").Child(uid).SetRawJsonValueAsync(json);

    }

    public void Confirm_database()
    {
        
        bool Tuid = false;
        FirebaseDatabase.DefaultInstance
     .GetReference("Users")
     .GetValueAsync().ContinueWithOnMainThread(task => {
         if (task.IsFaulted)
         {
             // Handle the error...
         }
         else if (task.IsCompleted)
         {
             DataSnapshot snapshot = task.Result;
             foreach (DataSnapshot data in snapshot.Children)
             {
                 string name = data.Key.ToString();
                 string myData = null;
                 if(uid.Equals(name))
                 {
                     Tuid = true;
                     myData = data.GetRawJsonValue();
                     userdata = JsonUtility.FromJson<UserData>(myData);
                     if(userdata.ch_num.Equals(""))
                     {
                         chnumCheck = true;
                     }
                 }

             }
             if (!Tuid)
             {
                 WriteUser(email, uid, "0");
             }
         }

     });
    }

    public void ch_numModify()
    {
        FirebaseDatabase.DefaultInstance
     .GetReference("Users").Child(userdata.uid).Child("ch_num")
     .GetValueAsync().ContinueWithOnMainThread(task => {
         if (task.IsFaulted)
         {
             // Handle the error...
         }
         else if (task.IsCompleted)
         {
             DataSnapshot snapshot = task.Result;
             foreach (DataSnapshot data in snapshot.Children)
             {
                 string name = data.Key.ToString();
                 string myData = null;
                 if(name.Equals("1"))
                 {
                     myData = data.GetRawJsonValue();
                     if (myData.Equals("\"\""))
                     {
                         userdata.setnum("1");
                         slotCheck = true;
                     }
                     else if (!myData.Equals("\"\""))
                     {
                         userdata.setnum("2");
                         slotCheck = true;
                     }
                         
                 }

             }
         }
     });
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        user = auth.CurrentUser;
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        Auth_Check();
    }

    // Update is called once per frame
    void Update()
    {
        if(Auth_Current)
        {
            Confirm_database();
            Auth_Current = false;
        }
        if(chnumCheck)
        {
            // 들어오는거 확인
            ch_numModify();
            chnumCheck = false;
        }
        if (DestoryCheck == true)
        {
            Destroy(this.gameObject);
        }
    }
}
