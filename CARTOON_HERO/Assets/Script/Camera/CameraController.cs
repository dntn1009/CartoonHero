using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    Transform m_target; // 카메라 위치 조절할 타겟
    [SerializeField]
    [Range(0f, 30f)] // 범위
    float m_distance = 5f; // 카메라 확대 거리
    [SerializeField]
    [Range(0f, 30f)]
    float m_height = 3f; // 카메라 높이
    [SerializeField]
    [Range(-90f, 90f)]
    float m_angle = 45f; // 카메라 각도
    [SerializeField]
    [Range(0.1f, 5f)]
    float m_speed = 0.1f; // 카메라 속도
    /////////
    Transform m_prevTransform; // 화면이 이동할떄 프레임을 고려하기위해

    public void targetassign()
    {
        m_target = GameObject.FindWithTag("Player").transform;
        m_prevTransform = transform;
        Application.targetFrameRate = 60;// 프레임 고정, 상관없음 게임설정으로 만들수잇음
        //근데 모바일은 30 or 60 DB로 설정하게 하자
    }

    public void targetMove()
    {
        transform.position = new Vector3(m_target.transform.position.x,
       Mathf.Lerp(m_prevTransform.position.y, m_target.transform.position.y + m_height, m_speed * Time.deltaTime),
       Mathf.Lerp(m_prevTransform.position.z, m_target.transform.position.z - m_distance, m_speed * Time.deltaTime));
        transform.rotation = Quaternion.Lerp(m_prevTransform.rotation, Quaternion.Euler(m_angle, 0f, 0f), m_speed * Time.deltaTime);
        m_prevTransform = transform;
    }

    // Start is called before the first frame update
    void Start()
    {
        Invoke("targetassign", 1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        Invoke("targetMove", 1.5f);
    }
}
