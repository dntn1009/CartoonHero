using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAreUnitFind_Mon : MonoBehaviour
{
    // 플레이어 공격시 유닛 찾기
    List<GameObject> m_unitList = new List<GameObject>();

    public List<GameObject> UnitList { get { return m_unitList; } }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            m_unitList.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            m_unitList.Remove(other.gameObject);
        }

    }

    // Start is called before the first frame update
    void Start()
    {

    }
}
