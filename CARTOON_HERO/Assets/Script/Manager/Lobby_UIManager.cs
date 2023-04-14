using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Lobby_UIManager : MonoBehaviour
{
    public static Lobby_UIManager Instance;

    [SerializeField]
    GameObject QuitUI;
    [SerializeField]
    GameObject StCreateUI;
    [SerializeField]
    UILabel Guide_Label;

    //DonDestory로 데이터 보관용임. 로그인으로 갈때 파괴하기 위해 생성
    [SerializeField]
    GameObject Lobby_data;
    [SerializeField]
    GameObject Character_data;

    [SerializeField]
    GameObject Type1;

    [SerializeField]
    GameObject Type2;

    [SerializeField]
    UILabel slot1_name;

    [SerializeField]
    UILabel slot2_name;

    [SerializeField]
    UILabel Select_Type;
    [SerializeField]
    UILabel Select_Name;

    GameObject Type1_Sprite;
    GameObject Type2_Sprite;

    Firebase.Auth.FirebaseAuth auth;

    int ClickCount = 0;

    bool Select_Slot1 = false;
    bool Select_Slot2 = false;

    public void SlotType_Design()
    {
        if(CharacterData.Instance.slot1.Type.Equals("Hero"))
        {
            Debug.Log("Hero 통과");
            Type1_Sprite = Instantiate(Resources.Load("Image/Hero_Sprite") as GameObject);
            Type1_Sprite.transform.SetParent(Type1.transform, false);
            slot1_name.text = CharacterData.Instance.slot1.Nickname;
        }
        else if (CharacterData.Instance.slot1.Type.Equals("Magician"))
        {
            Debug.Log("Magician 통과");
            Type1_Sprite = Instantiate(Resources.Load("Image/Magician_Sprite") as GameObject);
            Type1_Sprite.transform.SetParent(Type1.transform, false);
            slot1_name.text = CharacterData.Instance.slot1.Nickname;
        }
        else if (CharacterData.Instance.slot1.Type.Equals("Dog"))
        {
            Debug.Log("Dog 통과");
            Type1_Sprite = Instantiate(Resources.Load("Image/Dog_Sprite") as GameObject);
            Type1_Sprite.transform.SetParent(Type1.transform, false);
            slot1_name.text = CharacterData.Instance.slot1.Nickname;
        }
        else
        {
            slot1_name.text = "New Character";
        }


        if (CharacterData.Instance.slot2.Type.Equals("Hero"))
        {
            Debug.Log("Hero 통과");
            Type2_Sprite = Instantiate(Resources.Load("Image/Hero_Sprite") as GameObject);
            Type2_Sprite.transform.SetParent(Type2.transform, false);
            slot2_name.text = CharacterData.Instance.slot1.Nickname;
        }
        else if (CharacterData.Instance.slot2.Type.Equals("Magician"))
        {
            Debug.Log("Magician 통과");
            Type2_Sprite = Instantiate(Resources.Load("Image/Magician_Sprite") as GameObject);
            Type2_Sprite.transform.SetParent(Type2.transform, false);
            slot2_name.text = CharacterData.Instance.slot2.Nickname;
        }
        else if (CharacterData.Instance.slot2.Type.Equals("Dog"))
        {
            Debug.Log("Dog 통과");
            Type2_Sprite = Instantiate(Resources.Load("Image/Dog_Sprite") as GameObject);
            Type2_Sprite.transform.SetParent(Type2.transform, false);
            slot2_name.text = CharacterData.Instance.slot2.Nickname;
        }
        else
        {
            slot2_name.text = "New Character";
        }
    }

    // Start is called before the first frame update
    void Awake()
    {

        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;

        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != null)
        {
            Debug.Log("Instance already exists, destorying object!");
            Destroy(this);
        }
        Invoke("SlotType_Design", 1.0f);
    }
    private void Update()
    {
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
                QuitScreen();
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

    #region Methods

    public void Slot1Btn()
    {
        Select_Type.text = CharacterData.Instance.slot1.Type;
        Select_Name.text = CharacterData.Instance.slot1.Nickname;
        CharacterData.Instance.Select_Slot1 = true;
        CharacterData.Instance.Select_Slot2 = false;
    }

    public void Slot2Btn()
    {
        Select_Type.text = CharacterData.Instance.slot2.Type;
        Select_Name.text = CharacterData.Instance.slot2.Nickname;
        CharacterData.Instance.Select_Slot1 = false;
        CharacterData.Instance.Select_Slot2 = true;
    }

    public void QuitScreen()
    {
        QuitUI.SetActive(true);
    }

    public void BackScreen()
    {
        QuitUI.SetActive(false);
    }

    public void LoginScreen()
    {
        auth.SignOut();
        LobbyData.Instance.DestoryCheck = true;
        CharacterData.Instance.DestoryCheck = true;
        SceneManager.LoadScene("LoginScene");
    }

    public void InGameScreen()
    {
        SceneManager.LoadScene("InGame");
    }

    public void CreateScene()
    {
        if (LobbyData.Instance.userdata.ch_num.Equals("2"))
        {
            StCreateUI.SetActive(true);
        }
        else
        {
            SceneManager.LoadScene("CreateScene");
        }
    }

    public void StCreateCanScreen()
    {
        StCreateUI.SetActive(false);
    }
    public void QuitCanScreen()
    {
        QuitUI.SetActive(false);
    }

    public void Guide_Update(string text)
    {
        Guide_Label.text = text;
    }
    #endregion
}
