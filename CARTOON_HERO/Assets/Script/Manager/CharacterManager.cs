using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterManager : DonDestory<CharacterManager>
{
    DatabaseReference reference;

    //Character Manager
    [Header("캐릭터 관리")]
    PlayerController play_control;

    public GameObject player;

    [SerializeField]
    GameObject character;

    [SerializeField]
    Camera m_uiCamera;

    [SerializeField]
    Transform m_hudPool;

    [SerializeField]
    public UILabel Lv_Label;

    GameObject obj;

    //Camera Controller
    [Header("카메라 관리")]
    public Transform CentralAxis;

    public Transform cam;

    public float camSpeed;

    public float m_camSpeed;

    float mouseX;
    float mouseY;
    float wheel = -5;

    int m_fingerID;
    bool finger_Check = false;

    [SerializeField]
    GamePad gamepad;

    [SerializeField]
    GameObject Screen_Touch;

    [SerializeField]
    TouchScreen touchscreen; // Panel 열때 화면 움직이는거 방지

    float init_x = 0;
    float init_y = 0; // mobile touch

    [Header("스텟 관리")]
    [SerializeField]
    GameObject Stat_Panel;

    [SerializeField]
    UILabel STR;
    [SerializeField]
    UILabel DEX;
    [SerializeField]
    UILabel LUK;
    [SerializeField]
    UILabel INT;
    [SerializeField]
    UILabel POINT;

    bool STR_Btn = false;
    bool DEX_Btn = false;
    bool LUK_Btn = false;
    bool INT_Btn = false;

    int Init_Point;

    [SerializeField]
    public Pocket pocket;

    [Header("DonDestroy 관리")]
    [SerializeField]
    GameObject Item_manager;


    // Start is called before the first frame update

    #region Character Methods

    public void OnSelect()
    {
        play_control = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
    }
    public void OnPressAttack()
    {
        play_control.OnPressAttack();
    }

    public void OnReleaseAttack()
    {
        play_control.OnReleaseAttack();
    }

    public void Create_Character()
    {
        if (CharacterData.Instance.Select_Slot1)
        {
            if (CharacterData.Instance.slot1.Type.Equals("Hero"))
            {
                player = Resources.Load<GameObject>("Prefab/Player/Hero");
                obj = Instantiate(player);
                obj.transform.parent = character.transform;
                var phud = obj.GetComponent<PlayerController>();
                phud.SetPlayer(m_uiCamera, m_hudPool);
            }
            else if (CharacterData.Instance.slot1.Type.Equals("Dog"))
            {
                player = Resources.Load<GameObject>("Prefab/Player/Dog");
                obj = Instantiate(player);
                obj.transform.parent = character.transform;
                var phud = obj.GetComponent<PlayerController>();
                phud.SetPlayer(m_uiCamera, m_hudPool);
            }
            else if (CharacterData.Instance.slot1.Type.Equals("Magician"))
            {
                player = Resources.Load<GameObject>("Prefab/Player/Magician");
                obj = Instantiate(player);
                obj.transform.parent = character.transform;
                var phud = obj.GetComponent<PlayerController>();
                phud.SetPlayer(m_uiCamera, m_hudPool);
            }
        }

        if (CharacterData.Instance.Select_Slot2)
        {
            if (CharacterData.Instance.slot2.Type.Equals("Hero"))
            {
                player = Resources.Load<GameObject>("Prefab/Player/Hero");
                obj = Instantiate(player);
                obj.transform.parent = character.transform;
                var phud = obj.GetComponent<PlayerController>();
                phud.SetPlayer(m_uiCamera, m_hudPool);
            }
            else if (CharacterData.Instance.slot2.Type.Equals("Dog"))
            {
                player = Resources.Load<GameObject>("Prefab/Player/Dog");
                obj = Instantiate(player);
                obj.transform.parent = character.transform;
                var phud = obj.GetComponent<PlayerController>();
                phud.SetPlayer(m_uiCamera, m_hudPool);
            }
            else if (CharacterData.Instance.slot2.Type.Equals("Magician"))
            {
                player = Resources.Load<GameObject>("Prefab/Player/Magician");
                obj = Instantiate(player);
                obj.transform.parent = character.transform;
                var phud = obj.GetComponent<PlayerController>();
                phud.SetPlayer(m_uiCamera, m_hudPool);
            }
        }
    }

    #endregion

    #region Camera Methods

    public Vector3 Move_Camerafoward(Vector3 m_dir)
    {
       Vector3 lookForward = new Vector3(CentralAxis.forward.x, 0f, CentralAxis.forward.z).normalized;
       Vector3 lookRight = new Vector3(CentralAxis.right.x, 0f, CentralAxis.right.z).normalized;


        Vector3 movedir = lookForward * m_dir.y + lookRight * m_dir.x;

        return movedir;
    }
    void CamMove()
    {
        //Debug.DrawRay(CentralAxis.position, new Vector3(CentralAxis.forward.x, 0f, CentralAxis.forward.z).normalized, Color.red);
        //Debug.DrawRay(CentralAxis.position, CentralAxis.forward, Color.blue);
#if UNITY_EDITOR || UNITY_STANDALONE

        if (Input.GetMouseButton(1))
        {
            mouseX += Input.GetAxis("Mouse X");
            mouseY += Input.GetAxis("Mouse Y") * -1;


            float Xlimit = CentralAxis.rotation.x + mouseY;

            if (Xlimit < 180f)
            {
                Xlimit = Mathf.Clamp(Xlimit, -1f, 0f);
            }
            else
            {
                Xlimit = Mathf.Clamp(Xlimit, 335f, 0f);
            }

            CentralAxis.rotation = Quaternion.Euler(
                new Vector3(Xlimit, CentralAxis.rotation.y + mouseX, 0) * camSpeed);
        }

#elif UNITY_ANDROID || UNITY_IPONE

        if (touchscreen.touch_ScreeCheck)
        {
            for (int i = 0; i < Input.touchCount; i++)//손가락
            {
                Ray ray = m_uiCamera.ScreenPointToRay(Input.touches[i].position);//손가락으로 눌렀을때 무엇인지 나타내는것
                RaycastHit rayHit;

                if (Physics.Raycast(ray, out rayHit, 100f, 1 << LayerMask.NameToLayer("UI")))
                {
                    if (rayHit.collider.transform == Screen_Touch.transform)
                    {
                        if (Input.touches[i].phase == TouchPhase.Began)
                        {
                            init_x = Input.touches[i].position.x; // 2000
                            init_y = Input.touches[i].position.y;
                        }

                        m_fingerID = Input.touches[i].fingerId;

                        if (Input.touches[i].phase == TouchPhase.Moved)
                        {
                            finger_Check = true;
                        }
                    }
                }

                // 처음 값이 크게뜸 Input.touches potion (Init_x) 가 크게뜨는데
                //여기서 나중에 Input.touch[i].position.x 에 - Init.x를 해주니까 움직이기는 함
                // 결국 첫터치에서 0이 되어야 함
                if ((Input.touches[i].phase == TouchPhase.Ended || Input.touches[i].phase == TouchPhase.Canceled) && Input.touches[i].fingerId == m_fingerID)
                {
                    finger_Check = false;
                    m_fingerID = -1;//손가락을 뗀 경우기 떄문에 -1을 다시 줌
                }

                if (finger_Check)
                {
                    if (Input.touches[i].fingerId == m_fingerID)
                    {
                        float last_x = Input.touches[i].position.x;
                        mouseX += (last_x - init_x); // 이게 값이 어마무시한거임
                        mouseY += (Input.touches[i].position.y - init_y) * -1;

                        float Xlimit = CentralAxis.rotation.x + mouseY;

                        if (Xlimit < 180f)
                        {
                            Xlimit = Mathf.Clamp(Xlimit, -1f, 0f);
                        }
                        else
                        {
                            Xlimit = Mathf.Clamp(Xlimit, 335f, 0f);
                        }

                        CentralAxis.rotation = Quaternion.Euler(new Vector3(Xlimit, CentralAxis.rotation.y + mouseX, 0) * m_camSpeed);


                    }
                }
            }
        }
#endif
    }

    void Zoom()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        wheel += Input.GetAxis("Mouse ScrollWheel");
        if (wheel >= -1)
            wheel = -1;
        if (wheel <= -5)
            wheel = -5;
        cam.localPosition = new Vector3(0, 0, wheel);
#elif UNITY_ANDROID || UNITY_IPONE


#endif
    }

    #endregion

    #region Stat Methods

    public void Stat_Open()
    {
        Stat_Setting();
        Stat_Panel.SetActive(true);
        touchscreen.touch_ScreeCheck = false;
    }

    public void Stat_Close()
    {
        Stat_Panel.SetActive(false);
        touchscreen.touch_ScreeCheck = true;
    }

    public void Stat_Setting()
    {
        if(CharacterData.Instance.Select_Slot1)
        { 
            STR.text = CharacterData.Instance.slot1.Stat[0].ToString();
            DEX.text = CharacterData.Instance.slot1.Stat[1].ToString();
            LUK.text = CharacterData.Instance.slot1.Stat[2].ToString();
            INT.text = CharacterData.Instance.slot1.Stat[3].ToString();
            POINT.text = CharacterData.Instance.slot1.Point.ToString();
            Init_Point = CharacterData.Instance.slot1.Point;
        }
        else if(CharacterData.Instance.Select_Slot2)
        {
            STR.text = CharacterData.Instance.slot2.Stat[0].ToString();
            DEX.text = CharacterData.Instance.slot2.Stat[1].ToString();
            LUK.text = CharacterData.Instance.slot2.Stat[2].ToString();
            INT.text = CharacterData.Instance.slot2.Stat[3].ToString();
            POINT.text = CharacterData.Instance.slot2.Point.ToString();
            Init_Point = CharacterData.Instance.slot2.Point;
        }
    }

    public void STR_Plus()
    {
        STR_Btn = true;
    }
    
    public void DEX_Plus()
    {
        DEX_Btn = true;
    }

    public void LUK_Plus()
    {
        LUK_Btn = true;
    }

    public void INT_Plus()
    {
        INT_Btn = true;
    }
    public void Stat_Plus()
    {
        int stat_point = int.Parse(POINT.text);
        if(stat_point > 0)
        {
            if(STR_Btn)
            {
                STR.text = (int.Parse(STR.text) + 1).ToString();
                STR_Btn = false;
            }
            else if(DEX_Btn)
            {
                DEX.text = (int.Parse(DEX.text) + 1).ToString();
                DEX_Btn = false;
            }
            else  if(LUK_Btn)
            {
                LUK.text = (int.Parse(LUK.text) + 1).ToString();
                LUK_Btn = false;
            }
            else if(INT_Btn)
            {
                INT.text = (int.Parse(INT.text) + 1).ToString();
                INT_Btn = false;
            }
            stat_point = stat_point - 1;
            POINT.text = stat_point.ToString();
        }
    }

    public void Stat_Save()
    {
        int str = int.Parse(STR.text);
        int dex = int.Parse(DEX.text);
        int luk = int.Parse(LUK.text);
        int Int = int.Parse(INT.text);
        int Point = int.Parse(POINT.text);
        int[] new_stat = new int[4] { str, dex, luk, Int };

        if (CharacterData.Instance.Select_Slot1)
        {
            reference.Child("Users").Child(LobbyData.Instance.userdata.uid).Child("ch_num").Child("0").Child("Stat").SetValueAsync(new_stat);
            CharacterData.Instance.slot1.SetStat(new_stat, Point);
        }
        else if (CharacterData.Instance.Select_Slot2)
        {
            reference.Child("Users").Child(LobbyData.Instance.userdata.uid).Child("ch_num").Child("1").Child("Stat").SetValueAsync(new_stat);
            CharacterData.Instance.slot2.SetStat(new_stat, Point);
        }

    }
    #endregion

    #region Inventory Methods

    // 따로 etc/Script로 보관

    #endregion

    public void LobbyScene()
    {
        CharacterData.Instance.Select_Slot1 = false;
        CharacterData.Instance.Select_Slot2 = false;
        Destroy(this.gameObject);
        Destroy(Item_manager);
        SceneManager.LoadScene("LobbyScene");
    }
    void Start()
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        m_fingerID = -1;
        if(CharacterData.Instance.Select_Slot1)
            Lv_Label.text = "Lv. " + CharacterData.Instance.slot1.Level;
        else if(CharacterData.Instance.Select_Slot2)
            Lv_Label.text = "Lv. " + CharacterData.Instance.slot2.Level;
        Invoke("OnSelect", 1.0f);
        Create_Character();
    }

    // Update is called once per frame
    void Update()
    {
        if(STR_Btn || DEX_Btn || LUK_Btn || INT_Btn)
        {
            Stat_Plus();
        }

        character.transform.position = obj.transform.position + new Vector3(0, 4, 0);
        CamMove();
        Zoom();
    }
}
