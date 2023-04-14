using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpBarController : MonoBehaviour
{
    [SerializeField]
    UIProgressBar m_ExpBar;

    [SerializeField]
    UILabel m_RecLabel;

    [SerializeField]
    UILabel m_MaxLabel;

    [SerializeField]
    UILabel m_PerLabel;

    #region Methods

    public void Up_ExpBar()
    {
        float exp;
        if (CharacterData.Instance.Select_Slot1)
        {
            exp = CharacterData.Instance.slot1.Exp / (float)CharacterData.Instance.exp_standard.EXP[CharacterData.Instance.slot1.Level - 1];
            m_ExpBar.value = exp;
            m_RecLabel.text = CharacterData.Instance.slot1.Exp.ToString();
            m_MaxLabel.text = CharacterData.Instance.exp_standard.EXP[CharacterData.Instance.slot1.Level - 1].ToString();
            m_PerLabel.text = "(" + exp * 100 + "%)";
            CharacterManager.Instance.Lv_Label.text = "Lv " + CharacterData.Instance.slot1.Level;
            /*if (exp <= 0f)
            {
                m_ExpBar.alpha = 0f;
            }*/
        }
        else if (CharacterData.Instance.Select_Slot2)
        {
            exp = CharacterData.Instance.slot2.Exp / (float)CharacterData.Instance.exp_standard.EXP[CharacterData.Instance.slot2.Level - 1];
            m_ExpBar.value = exp;
            m_RecLabel.text = CharacterData.Instance.slot2.Exp.ToString();
            m_MaxLabel.text = CharacterData.Instance.exp_standard.EXP[CharacterData.Instance.slot2.Level - 1].ToString();
            m_PerLabel.text = "(" + exp * 100 + "%)";
            CharacterManager.Instance.Lv_Label.text = "Lv " + CharacterData.Instance.slot2.Level;
            /* if (exp <= 0f)
             {
                 m_ExpBar.alpha = 0f;
             }*/
        }
    }
    public void GetExpBar()
    {
        float exp;
        if (CharacterData.Instance.Select_Slot1)
        {
            exp = CharacterData.Instance.slot1.Exp / (float)CharacterData.Instance.exp_standard.EXP[CharacterData.Instance.slot1.Level - 1];
            m_ExpBar.value = exp;
            m_RecLabel.text = CharacterData.Instance.slot1.Exp.ToString();
            m_PerLabel.text = "(" + exp * 100 + "%)";
            /*if (exp <= 0f)
            {
                m_ExpBar.alpha = 0f;
            }*/
        }
        else if (CharacterData.Instance.Select_Slot2)
        {
            exp = CharacterData.Instance.slot2.Exp / (float)CharacterData.Instance.exp_standard.EXP[CharacterData.Instance.slot2.Level - 1];
            m_ExpBar.value = exp;
            m_RecLabel.text = CharacterData.Instance.slot2.Exp.ToString();
            m_PerLabel.text = "(" + exp * 100 + "%)";
            /*if (exp <= 0f)
            {
                m_ExpBar.alpha = 0f;
            }*/
        }
    }

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        Up_ExpBar();
    }

    // Update is called once per frame
    void Update()
    {
        if(CharacterData.Instance.ExpBarCheck)
        {
            GetExpBar();
            CharacterData.Instance.ExpBarCheck = false;
        }
        if(CharacterData.Instance.LevelUpCheck)
        {
            Up_ExpBar();
            CharacterData.Instance.LevelUpCheck = false;
        }
    }
}
