using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Login_UIManager : MonoBehaviour
{
    public static Login_UIManager Instance;

    [SerializeField]
    GameObject LoginUI;
    [SerializeField]
    GameObject RegisterUI;


    // Start is called before the first frame update
    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else if( Instance != null)
        {
            Debug.Log("Instance already exists, destorying object!");
            Destroy(this);
        }
    }

    public void LoginScreen()
    {
        LoginUI.SetActive(true);
        RegisterUI.SetActive(false);
    }

    public void RegisterScreen()
    {
        LoginUI.SetActive(false);
        RegisterUI.SetActive(true);
    }
}
