using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Firebase.Database;

public class AuthManager : MonoBehaviour
{
    [SerializeField]
    Login_UIManager uimanager;

    //Login_InputBox
    [Header("Login_InputBox")]
    [SerializeField]
    GameObject ID_InputBox;
    [SerializeField]
    GameObject PWD_InputBox;

    //Register_InputBox
    [Header("Register_InputBox")]
    [SerializeField]
    GameObject IDC_InputBox;
    [SerializeField]
    GameObject PWDC_InputBox;
    [SerializeField]
    GameObject PWDCC_InputBox;

    //Login
    UIInput IDInput;
    UIInput PWDInput;

    //Register
    UIInput IDCInput;
    UIInput PWDCInput;
    UIInput PWDCCInput;

    bool Register_Check = false;

    //Firebase Auth
    FirebaseAuth auth;
    DatabaseReference reference;

    //string Text
    string LID;
    string LPWD;
    string RID;
    string RPWD;
    string RPWDC;

    //RPWD Length
    int RPWD_Len;
    int ClickCount = 0;

    //LoadScene
    bool LoadTrigger = false;

    //GuideLabel
    [SerializeField]
    UILabel Guide_Login;
    [SerializeField]
    UILabel Guide_Register;

    public class User
    {
        public string uid;
        public int ch_num;

        public User()
        {
        }

        public User(string uid, int ch_num)
        {
            this.uid = uid;
            this.ch_num = ch_num;
        }
    }

    #region EventMethods
    public void getMessage_ID()
    {
        IDInput = ID_InputBox.GetComponent<UIInput>();
        LID = IDInput.label.text;
    }
    public void getMessage_PWD()
    {
        PWDInput = PWD_InputBox.GetComponent<UIInput>();
        LPWD = PWDInput.label.text;
    }

    public void getMessage_IDC()
    {
        IDCInput = IDC_InputBox.GetComponent<UIInput>();
        RID = IDCInput.label.text;
    }

    public void getMessage_PWDC()
    {
        PWDCInput = PWDC_InputBox.GetComponent<UIInput>();
        RPWD = PWDCInput.label.text;
    }

    public void getMessage_PWDCC()
    {
        PWDCCInput = PWDCC_InputBox.GetComponent<UIInput>();
        RPWDC = PWDCCInput.label.text;
    }

    void DoubleClick()
    {
        ClickCount = 0;
    }

    public void LoadSceneTrigeer()
    {
        SceneManager.LoadScene("LobbyScene");
    }

    public void WriteUser(string userID, string uid, int num)
    {
        User User = new User(uid, num);
        string json = JsonUtility.ToJson(User);
        reference.Child("Users").Child(userID).SetRawJsonValueAsync(json);
    }
    #endregion

    #region Login and Register

    public void Test()
    {
        SceneManager.LoadScene("LobbyScene");
    }
    public void Login()
    {
        auth.SignInWithEmailAndPasswordAsync(LID, LPWD).ContinueWith(
           task => {
               if (task.IsCompleted && !task.IsFaulted && !task.IsCanceled)
               {
                   Debug.Log(LID + " 로 로그인 하셨습니다.");
                   LoadTrigger = true;
                   return;
               }
               else
               {
                   Debug.Log("로그인에 실패하셨습니다.");
                   Guide_Login.text = "아이디 혹은 비밀번호가 틀렸습니다.";
                   Guide_Login.color = Color.red;
                   return;
               }
           }
       );
    }

    public void Register()
    {
        RPWD_Len = RPWD.Length;
        if (RPWD.Length > 7)
        {
            if (RPWD.Equals(RPWDC))
            {
                auth.CreateUserWithEmailAndPasswordAsync(RID, RPWD).ContinueWith(
                task =>
                {
                    if (!task.IsCanceled && !task.IsFaulted)
                    {
                        Register_Check = true;
                        Debug.Log(RID + "로 회원가입 되었습니다.");
                    }
                    else
                    {
                        Guide_Register.text = "이미 가입된 아이디, 혹은 오류입니다.";
                        Guide_Register.color = Color.red;
                        Debug.Log("이미 가입된 아이디, 혹은 오류입니다.");
                    }
                }
                );
            }
            else
            {
                Guide_Register.text = "비밀번호가 동일하지 않습니다.";
                Guide_Register.color = Color.red;
                Debug.Log("비밀번호가 동일하지 않습니다.");
            }
        }
        else
        {
            Guide_Register.text = "비밀번호는 8자 이상이어야 합니다.";
            Guide_Register.color = Color.red;
            Debug.Log("비밀번호는 8자 이상이여야 합니다.");
        }
    }

    #endregion

    // Start is called before the first frame update
    void Awake()
    {
        auth = FirebaseAuth.DefaultInstance;
        reference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    // Update is called once per frame
    void Update()
    {
        if(LoadTrigger)
        {
            LoadSceneTrigeer();
            LoadTrigger = false;
        }

        if(Register_Check)
        {
            uimanager.LoginScreen();
            Register_Check = false;
        }
        if (Application.platform == RuntimePlatform.Android)
        {

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ClickCount++;
                if (!IsInvoking("DoubleClick"))
                    Invoke("DoubleClick", 1.0f);
            }
            else if (ClickCount == 2)
            {
                CancelInvoke("DoubleClick");
                Application.Quit();
            }

            if (Input.GetKeyDown(KeyCode.Home))

            {

                // Home Button

            }

            if (Input.GetKeyDown(KeyCode.Menu))

            {

                // Menu Button

            }

        }
    }

}
