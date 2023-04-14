using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStart : MonoBehaviour
{

    int ClickCount = 0;

    public void Login_form()
    {
        SceneManager.LoadScene("LoginScene");
    }

    public void InGame_form()
    {
        SceneManager.LoadScene("InGame");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
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
