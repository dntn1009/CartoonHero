using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : SingletonMonobehaviour<MonsterManager>
{
 
    GameObjectPool<MonsterController> m_monsterPool;
    GameObjectPool<MonsterController> m_monsterPool2;
    [SerializeField]
    GameObject[] m_monsterPrefab;
    [SerializeField]
    Camera m_uiCamera;
    [SerializeField]
    Transform m_hudPool;
    [SerializeField]
    WayPointSystems[] m_path;

    [SerializeField]
    GameObject[] Way_Path;

    int Max_monster = 5;
    
    [SerializeField]
    public Pocket pocket;

    public void CreateMonster(int num, int i)
    {
        var mon = m_monsterPool.Get();
        mon.I_path = i;
        WayPointSystems wayPoint = Way_Path[num].transform.Find("Path_1_" + i).GetComponent<WayPointSystems>();
        mon.InitMonster(wayPoint);
    }

    public void CreateMonster2(int num, int i)
    {
        var mon = m_monsterPool2.Get();
        mon.I_path = i;
        WayPointSystems wayPoint = Way_Path[num].transform.Find("Path_2_" + i).GetComponent<WayPointSystems>();
        mon.InitMonster(wayPoint);
    }

    public void RemoveMonster(MonsterController mon)
    {
        mon.gameObject.SetActive(false);
        m_monsterPool.Set(mon);
    }

    public void RemoveMonster2(MonsterController mon)
    {
        mon.gameObject.SetActive(false);
        m_monsterPool2.Set(mon);
    }

    protected override void OnStart()
    {
        // m_monsterPrefab = Resources.LoadAll<GameObject>("Prefab/Monsters");

        m_monsterPool = new GameObjectPool<MonsterController>(Max_monster, () =>
            {
                var obj = Instantiate(m_monsterPrefab[0]);
                    obj.SetActive(false);
                    obj.transform.SetParent(transform);
                    obj.transform.localPosition = Vector3.zero;
                    obj.transform.localScale = Vector3.one;
                var mon = obj.GetComponent<MonsterController>();
                    mon.SetMonster(m_uiCamera, m_hudPool);
                return mon;
            });
          m_monsterPool2 = new GameObjectPool<MonsterController>(Max_monster, () =>
           {
            var obj = Instantiate(m_monsterPrefab[1]);
            obj.SetActive(false);
            obj.transform.SetParent(transform);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;
            var mon = obj.GetComponent<MonsterController>();
            mon.SetMonster(m_uiCamera, m_hudPool);
            return mon;
            });


        //몬스터 생성
        for (int mon_pre = 0; mon_pre < m_monsterPrefab.Length; mon_pre++)
        {
            for (int i = 0; i < Max_monster; i++)
            {
                if (mon_pre == 0)
                    CreateMonster(mon_pre, i);
                else if (mon_pre == 1)
                    CreateMonster2(mon_pre, i);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}

