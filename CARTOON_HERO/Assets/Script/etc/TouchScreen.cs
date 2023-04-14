using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchScreen : MonoBehaviour
{
    int Width = Screen.width;
    int height = Screen.height;

    public bool touch_ScreeCheck = true;

    [SerializeField]
    public BoxCollider Colider;

    public void Colider_reset()
    {
        int I_width = Width / 4;

        this.transform.localPosition = new Vector3(I_width, 0, 0);

        Colider.size = new Vector3(I_width * 2, height / 2, 0);
    }
    // Start is called before the first frame update
    void Start()
    {
        Colider_reset();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
