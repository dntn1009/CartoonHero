using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewCameraController : MonoBehaviour
{
    public Transform CentralAxis;
    public Transform cam;
    public float camSpeed;
    float mouseX;
    float mouseY;
    float wheel = -5;

    void CamMove()
    {
       if(Input.GetMouseButton(1))
        {
            mouseX += Input.GetAxis("Mouse X");
            mouseY += Input.GetAxis("Mouse Y") * -1;


            float Xlimit = CentralAxis.rotation.x + mouseY;

            if(Xlimit < 180f)
            {
                Xlimit = Mathf.Clamp(Xlimit, -1f, 0f);
            }
            else
            {
                Xlimit = Mathf.Clamp(Xlimit, 335f, 0f);
            }

            CentralAxis.rotation = Quaternion.Euler(
                new Vector3(
                    Xlimit,
                    CentralAxis.rotation.y + mouseX,
                    0) * camSpeed);
        }
    }

    void Zoom()
    {
        wheel += Input.GetAxis("Mouse ScrollWheel");
        if (wheel >= -1)
            wheel = -1;
        if (wheel <= -5)
            wheel = -5;
        cam.localPosition = new Vector3(0, 0, wheel);
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        CamMove();
        Zoom();
    }
}
