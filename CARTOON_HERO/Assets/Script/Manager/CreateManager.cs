using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreateManager : MonoBehaviour
{

    [SerializeField]
    UILabel Introduce_Label;

    [SerializeField]
    GameObject Hero_Sprite;

    [SerializeField]
    GameObject Magician_Sprite;

    [SerializeField]
    GameObject Dog_Sprite;

    [SerializeField]
    GameObject Nick_InputBox;
    UIInput NickInput;

    public string Type;
    public string Nick_string;
    public bool Typechoice = false;

    #region InputField_Methods

    public void Nickname_Input()
    {
            NickInput = Nick_InputBox.GetComponent<UIInput>();
            Nick_string = NickInput.label.text;
    }

    #endregion

    #region Button_Methods
    public void Introduce_text(string text)
    {
        Introduce_Label.text = text;
    }
    public void Hero_Btn()
    {
        Typechoice = true;
        Type = "Hero";
        Hero_Sprite.SetActive(true);
        Magician_Sprite.SetActive(false);
        Dog_Sprite.SetActive(false);
        Introduce_text("경비병 출신의 인간이며, 무기로 검과 방패를 사용한다.");
    }

    public void Magician_Btn()
    {
        Typechoice = true;
        Type = "Magician";
        Hero_Sprite.SetActive(false);
        Magician_Sprite.SetActive(true);
        Dog_Sprite.SetActive(false);
        Introduce_text("숲속에서 지내던 출신없는 마법사 출신으로, 마법을 사용한다.");
    }

    public void Dog_Btn()
    {
        Typechoice = true;
        Type = "Dog";
        Hero_Sprite.SetActive(false);
        Magician_Sprite.SetActive(false);
        Dog_Sprite.SetActive(true);
        Introduce_text("인간과 다른 아종으로, 강아지 모습을 하며, 대검을 사용한다.");
    }

    public void LobbyScene()
    {
        SceneManager.LoadScene("LobbyScene");
    }

    public void CreateScene()
    {
        SceneManager.LoadScene("LobbyScene");
    }

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
