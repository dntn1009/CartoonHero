using Firebase.Database;
using Firebase;
using Firebase.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Firebase.Extensions;

public class MonsterController : MonoBehaviour
{
    [System.Serializable]
    public class MonsterInfo
    {
        public int Exp;
        public float attack;
        public float criAttack;
        public float criRate;
        public float defence;
        public float dodgeRate;
        public float hitRate;
        public int hp;
        public int hpMax;
        public int Coin;

        public MonsterInfo(int Exp, float attack, float criAttack, float criRate, float defence, float dodgeRate, float hitRate, int hp, int hpMax, int Coin)
        {

            this.Exp = Exp;
            this.attack = attack;
            this.criAttack = criAttack;
            this.criRate = criRate;
            this.defence = defence;
            this.dodgeRate = dodgeRate;
            this.hitRate = hitRate;
            this.hp = hp;
            this.hpMax = hpMax;
            this.Coin = Coin;
        }
    }

    public enum BehaviourState
    {
        Idle,
        Chase,
        Patrol,
        Attack01,
        Attack02,
        Damaged,
        Die,
        Max
    }
    Vector3 m_dir;
    BehaviourState m_state;
    [Header("몬스터 능력치")]
    [SerializeField]
    public MonsterInfo monInfo;
    [SerializeField]
    WayPointSystems m_waypointSystem;
    [SerializeField]
    HUDController m_hudCtr;
    [SerializeField]
    Collider m_collider;
    [SerializeField]
    GameObject m_attackAreObj_M;
    AttackAreUnitFind_Mon[] m_attackArea_M;
    MonsterAnimController m_animCtr;
    Animator m_animator;
    GameObject m_fxHitPrefab;
    PlayerController m_player;
    NavMeshAgent m_navAgent;
    TweenMove m_tweenMove;
    DatabaseReference reference;
    Dictionary<MonsterAnimController.Motion, SkillData> m_skillTable = new Dictionary<MonsterAnimController.Motion, SkillData>();
    float m_idleDuration = 5f;
    float m_dieDuration = 4f;
    float m_idleTime;
    float m_dieTime;
    float m_attackDist = 1.6f; // 공격이 가능한 거리
    float m_detectDist = 4f;
    float m_sqrAttackDist;
    float m_sqrDetecDist;
    bool m_isMove; //  waypoint 
    int m_curWaypoint; // waypoint
    int m_delayFrame; // Hit Animation
    int mon_pathnum; // 0 : 슬라임 1 : 가시거북
    public int I_path;
    Coroutine m_coroutineDelayMotion; // Hit Animation;
    SkillData m_skilldata;

    public bool IsDie { get { return m_state == BehaviourState.Die; } }

    public bool m_IsAttack;

    public bool m_isRespwan;

    public bool m_ContactPlayer = false;

    #region Animation Event Methods
    void AnimEvent_AttackFinished()
    {
        SetIdle(1f);
    }

    void AnimEvent_Attack()
    {
        SkillData skillData;
        float damage = 0f;
        if (m_skillTable.TryGetValue(m_animCtr.GetAnimState(), out skillData))
        {
            var unitList = m_attackArea_M[skillData.AttackArea].UnitList;
            for (int i = 0; i < unitList.Count; i++)
            {
                var player = unitList[i].GetComponent<PlayerController>();
                var dummy = Util.FindChildObject(unitList[i], "Player_Hit");
                if (dummy != null && player != null)
                {
                    if (m_player.IsDie) continue;

                    AttackType type = AttackProcess(player, skillData, out damage);
                    m_player.SetDemage(type, damage, skillData);
                    if (type == AttackType.Dodge) return;
                    var effect = Instantiate(m_fxHitPrefab);
                    effect.transform.position = dummy.transform.position;
                    effect.transform.rotation = Quaternion.FromToRotation(effect.transform.forward, (unitList[i].transform.position - transform.position).normalized);
                    Destroy(effect, 1.5f);
                }
            }
        }
        for (int i = 0; i < m_attackArea_M.Length; i++)
            m_attackArea_M[i].UnitList.RemoveAll(obj => obj.GetComponent<PlayerController>().IsDie);
        
    }
    IEnumerator Coroutine_DelayMotion(int frame)
    {
        for (int i = 0; i < frame; i++)
            yield return null;
        SetIdle(0f);
        m_delayFrame = 0;
        m_navAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
    }
    void AnimEvent_HitFinished()
    {
        m_coroutineDelayMotion = StartCoroutine(Coroutine_DelayMotion(m_delayFrame));
    }
    #endregion

    IEnumerator Coroutine_SetDestination(int frame, Transform target)
    {
        while (m_state == BehaviourState.Chase)
        {
            for (int i = 0; i < frame; i++)
            {
                yield return null;
            }
            m_navAgent.SetDestination(target.position);

        }

    }

    #region Public Methods and Operators

    public void SetExp_Coin()
    {
        if(CharacterData.Instance.Select_Slot1)
        {
            CharacterData.Instance.slot1.SetCoin(monInfo.Coin);
            reference.Child("Users").Child(LobbyData.Instance.userdata.uid).Child("ch_num").Child("0").Child("Coin").SetValueAsync(CharacterData.Instance.slot1.Coin);
            if (mon_pathnum == 0)
            {
                MonsterManager.Instance.pocket.mon_pocket.SetSlime();
                MonsterManager.Instance.pocket.Pocket_check = true;
            }
            else if(mon_pathnum == 1)
            {
                MonsterManager.Instance.pocket.mon_pocket.SetTurtle();
                MonsterManager.Instance.pocket.Pocket_check = true;
            }
            

            if (CharacterData.Instance.exp_standard.EXP[CharacterData.Instance.slot1.Level - 1] > CharacterData.Instance.slot1.Exp)
            {
                CharacterData.Instance.slot1.SetExp(monInfo.Exp);
                if (CharacterData.Instance.exp_standard.EXP[CharacterData.Instance.slot1.Level - 1] <= CharacterData.Instance.slot1.Exp) // 표준 경험치(레벨 업) <= 현재 캐릭터의 경험치
                {
                    CharacterData.Instance.slot1.LevelUP(CharacterData.Instance.exp_standard.EXP[CharacterData.Instance.slot1.Level - 1]);
                    reference.Child("Users").Child(LobbyData.Instance.userdata.uid).Child("ch_num").Child("0").Child("Exp").SetValueAsync(CharacterData.Instance.slot1.Exp);
                    reference.Child("Users").Child(LobbyData.Instance.userdata.uid).Child("ch_num").Child("0").Child("Level").SetValueAsync(CharacterData.Instance.slot1.Level);
                    CharacterData.Instance.LevelUpCheck = true;
                }
                else // 경험치 올리기
                {
                    reference.Child("Users").Child(LobbyData.Instance.userdata.uid).Child("ch_num").Child("0").Child("Exp").SetValueAsync(CharacterData.Instance.slot1.Exp);
                    CharacterData.Instance.ExpBarCheck = true;
                }  
            }
            else if (CharacterData.Instance.exp_standard.EXP[CharacterData.Instance.slot1.Level - 1] <= CharacterData.Instance.slot1.Exp)
            {
                CharacterData.Instance.slot1.LevelUP(CharacterData.Instance.exp_standard.EXP[CharacterData.Instance.slot1.Level - 1]);
                reference.Child("Users").Child(LobbyData.Instance.userdata.uid).Child("ch_num").Child("0").Child("Exp").SetValueAsync(CharacterData.Instance.slot1.Exp);
                reference.Child("Users").Child(LobbyData.Instance.userdata.uid).Child("ch_num").Child("0").Child("Level").SetValueAsync(CharacterData.Instance.slot1.Level);
                CharacterData.Instance.LevelUpCheck = true;
            }

        }
        else if(CharacterData.Instance.Select_Slot2)
        {
            CharacterData.Instance.slot2.SetCoin(monInfo.Coin);
            if (mon_pathnum == 0)
            {
                MonsterManager.Instance.pocket.mon_pocket.SetSlime();
                MonsterManager.Instance.pocket.Pocket_check = true;
            }
            else if (mon_pathnum == 1)
            {
                MonsterManager.Instance.pocket.mon_pocket.SetTurtle();
                MonsterManager.Instance.pocket.Pocket_check = true;
            }
            reference.Child("Users").Child(LobbyData.Instance.userdata.uid).Child("ch_num").Child("1").Child("Coin").SetValueAsync(CharacterData.Instance.slot2.Coin);

            if (CharacterData.Instance.exp_standard.EXP[CharacterData.Instance.slot2.Level - 1] > CharacterData.Instance.slot2.Exp)
            {
                CharacterData.Instance.slot2.SetExp(monInfo.Exp);
                if (CharacterData.Instance.exp_standard.EXP[CharacterData.Instance.slot2.Level - 1] <= CharacterData.Instance.slot2.Exp)
                {
                    CharacterData.Instance.slot2.LevelUP(CharacterData.Instance.exp_standard.EXP[CharacterData.Instance.slot2.Level - 1]);
                    reference.Child("Users").Child(LobbyData.Instance.userdata.uid).Child("ch_num").Child("1").Child("Exp").SetValueAsync(CharacterData.Instance.slot2.Exp);
                    reference.Child("Users").Child(LobbyData.Instance.userdata.uid).Child("ch_num").Child("1").Child("Level").SetValueAsync(CharacterData.Instance.slot2.Level);
                    CharacterData.Instance.LevelUpCheck = true;
                }
                else 
                {
                    reference.Child("Users").Child(LobbyData.Instance.userdata.uid).Child("ch_num").Child("1").Child("Exp").SetValueAsync(CharacterData.Instance.slot2.Exp);
                    CharacterData.Instance.ExpBarCheck = true;
                }
            }
            else if (CharacterData.Instance.exp_standard.EXP[CharacterData.Instance.slot2.Level - 1] <= CharacterData.Instance.slot2.Exp)
            {
                CharacterData.Instance.slot2.LevelUP(CharacterData.Instance.exp_standard.EXP[CharacterData.Instance.slot2.Level - 1]);
                reference.Child("Users").Child(LobbyData.Instance.userdata.uid).Child("ch_num").Child("1").Child("Exp").SetValueAsync(CharacterData.Instance.slot2.Exp);
                reference.Child("Users").Child(LobbyData.Instance.userdata.uid).Child("ch_num").Child("1").Child("Level").SetValueAsync(CharacterData.Instance.slot2.Level);
                CharacterData.Instance.LevelUpCheck = true;
            }
        }
    }

    public void ContactFalse()
    {
        m_ContactPlayer = false;
    }

    public void SetDemage(AttackType attackType, float damage, SkillData skillData)
    {
        if (IsDie) return;

        m_hudCtr.ActiveUI();
        monInfo.hp -= Mathf.CeilToInt(damage);
        m_hudCtr.DisplayDamage(attackType, damage, monInfo.hp / (float)monInfo.hpMax);
        /*m_status.hp -= Mathf.CeilToInt(damage);
        m_hudCtr.DisplayDamage(attackType, damage, m_status.hp / (float)m_status.hpMax);*/

        if (attackType == AttackType.Dodge) return;

        m_navAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        if (m_coroutineDelayMotion != null)
        {
            StopCoroutine(m_coroutineDelayMotion);
            m_coroutineDelayMotion = null;
        }
        m_animCtr.Play(MonsterAnimController.Motion.Hit, false);
        if (!m_ContactPlayer)
        {
            m_ContactPlayer = true;
        }
        SetState(BehaviourState.Damaged);
        m_delayFrame = skillData.delayFrame;
        if (skillData.knockBack > 0f)
        {
            var duration = SkillData.MaxKnockBackDuration * (skillData.knockBack / SkillData.MaxKnockBackDist);
            m_tweenMove.Play(transform.position, transform.position + (transform.position - m_player.transform.position).normalized * skillData.knockBack, duration);
        }
        if (monInfo.hp <= 0f)
        {
            SetState(BehaviourState.Die);
            m_animCtr.Play(MonsterAnimController.Motion.Die);
        }

    }
    #endregion

    private void readstat(string mon_name)
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        FirebaseDatabase.DefaultInstance
     .GetReference("Monster")
     .GetValueAsync().ContinueWithOnMainThread(task =>
     {
         if (task.IsFaulted)
         {
                // Handle the error...
            }
         else if (task.IsCompleted)
         {
             DataSnapshot snapshot = task.Result;
             foreach (DataSnapshot data in snapshot.Children)
             {
                 string name = data.Key.ToString();
                 string myData = null;
                 if (mon_name.Equals(name))
                 {
                     myData = data.GetRawJsonValue();
                     monInfo = JsonUtility.FromJson<MonsterInfo>(myData);
                 }
             }
         }
     });
    }

    public void SetMonster(Camera uiCamera, Transform hudPool)
    {
        m_hudCtr.SetHud(uiCamera, hudPool);
    }

    void SetState(BehaviourState state)
    {
        m_state = state;
    }

    void SetIdleDuration(float duration)
    {
        m_idleTime = m_idleDuration - duration;
        if (m_idleTime < 0f) m_idleTime = 0f;
    }
    void SetIdle(float duration)
    {
        m_isMove = false;
        m_navAgent.ResetPath();
        m_navAgent.isStopped = false;
        SetState(BehaviourState.Idle);
        m_animCtr.Play(MonsterAnimController.Motion.Idle);
        SetIdleDuration(duration);
    }

    public void InitMonster(WayPointSystems path)
    {
        m_waypointSystem = path;
        m_curWaypoint = -1;
        transform.position = path.m_waypoints[0].transform.position;
        gameObject.SetActive(true);
        SetIdle(1f);
        monInfo.hp = monInfo.hpMax;
        /*m_status.hp = m_status.hpMax;*/
        m_collider.enabled = true;
        m_hudCtr.InitHud();
    }

    bool IsInSetArea(float curDist, float targetdist)
    {
        if (Mathf.Approximately(curDist, targetdist) || curDist < targetdist)
        {
            return true;
        }
        return false;
    }

/*    void InitStatus()
    {
        m_status = new Status(500, 50f, 5f, 5f, 80f, 35f, 10f);
    }*/

    void InitSkillData()
    {
        m_skillTable.Add(MonsterAnimController.Motion.Attack01, new SkillData() { AttackArea = 0, knockBack = 0.0f, delayFrame = 0 });
        m_skillTable.Add(MonsterAnimController.Motion.Attack02, new SkillData() { AttackArea = 1, knockBack = 0.0f, delayFrame = 0 });
    }

    bool FindTarget(Transform target, float distance)
    {
        var dir = target.position - transform.position;
        dir.y = 0f;
        RaycastHit hit;
        Debug.DrawRay(transform.position + Vector3.up * 1f, dir.normalized * distance, Color.red);
        if (Physics.Raycast(transform.position + Vector3.up * 1f, dir.normalized, out hit, distance, 1 << LayerMask.NameToLayer("Ground") | 1 << LayerMask.NameToLayer("Player")))
        {
            if (hit.collider.CompareTag("Player"))
                return true;
        }
        return false;
    }
    AttackType AttackProcess(PlayerController player, SkillData skillData, out float damage)
    {
        AttackType type = AttackType.Dodge;
        damage = 0f;
        /* if (CalculationDamage.AttackDecision(MyStatus.hitRate, player.MyStatus.dodgeRate))
         {
             type = AttackType.Normal;
             damage = CalculationDamage.NormalDamage(MyStatus.attack, skillData.attack, player.MyStatus.defence);
             if (CalculationDamage.CriticalDecision(MyStatus.criRate))
             {
                 type = AttackType.Critical;
                 damage = CalculationDamage.CriticalDamage(damage, MyStatus.criAttack);
             }
         }*/
        if (CalculationDamage.AttackDecision(monInfo.hitRate, player.playInfo.dodgeRate))
        {
            type = AttackType.Normal;
            damage = CalculationDamage.NormalDamage(monInfo.attack, skillData.attack, player.playInfo.defence);
            if (CalculationDamage.CriticalDecision(monInfo.criRate))
            {
                type = AttackType.Critical;
                damage = CalculationDamage.CriticalDamage(damage, monInfo.criAttack);
            }
        }
        return type;
    }

    void BehaviourProcess()
    {
        float dist = 0f;
        switch (m_state)
        {
            case BehaviourState.Idle:
                m_idleTime += Time.deltaTime;
                if (m_idleTime > m_idleDuration)
                {
                    m_idleTime = 0f;
                    m_navAgent.isStopped = false;

                    if (FindTarget(m_player.transform, m_attackDist))
                    {
                        if (m_IsAttack)
                        {
                            SetState(BehaviourState.Attack02);
                            m_animCtr.Play(MonsterAnimController.Motion.Attack02);
                            m_IsAttack = false;
                            return;
                        }
                        else
                        {
                            SetState(BehaviourState.Attack01);
                            m_animCtr.Play(MonsterAnimController.Motion.Attack01);
                            m_IsAttack = true;
                            return;
                        }
                    }
                    if (FindTarget(m_player.transform, m_detectDist))
                    {
                        SetState(BehaviourState.Chase);
                        m_animCtr.Play(MonsterAnimController.Motion.Move);
                        StartCoroutine(Coroutine_SetDestination(15, m_player.transform));
                        m_navAgent.stoppingDistance = m_attackDist;
                        m_IsAttack = false;
                        return;
                    }
                    SetState(BehaviourState.Patrol);
                    m_animCtr.Play(MonsterAnimController.Motion.Move);
                    m_navAgent.stoppingDistance = m_navAgent.radius;
                }
                break;
            case BehaviourState.Attack01:
                break;
            case BehaviourState.Chase:
                // m_navAgent.SetDestination(m_player.transform.position);
                dist = (m_player.transform.position - transform.position).sqrMagnitude;
                m_animator.SetFloat("Speed", 1f);
                if (IsInSetArea(dist, m_sqrAttackDist))
                {
                    m_animator.SetFloat("Speed", 0f);
                    m_navAgent.ResetPath();
                    m_navAgent.isStopped = true;
                    SetIdle(0f);
                }
                break;
            case BehaviourState.Patrol:
                if (!FindTarget(m_player.transform, m_detectDist))
                {
                    if (!m_isMove)
                    {
                        m_curWaypoint++;
                        if (m_curWaypoint > m_waypointSystem.m_waypoints.Length - 1)
                            m_curWaypoint = 0;
                        m_navAgent.SetDestination(m_waypointSystem.m_waypoints[m_curWaypoint].transform.position);
                        m_isMove = true;
                    }
                    else
                    {
                        dist = (m_waypointSystem.m_waypoints[m_curWaypoint].transform.position - transform.position).sqrMagnitude;
                        if (IsInSetArea(dist, Mathf.Pow(m_navAgent.stoppingDistance, 2f)))
                        {
                            SetIdle(2f);
                        }
                    }
                }
                else
                {
                    m_navAgent.ResetPath();
                    m_navAgent.isStopped = true;
                    SetIdle(0f);
                }
                break;
            case BehaviourState.Die:
                if(m_ContactPlayer)
                {
                    SetExp_Coin();
                    m_ContactPlayer = false;
                }
                m_dieTime += Time.deltaTime;
                if (m_dieTime >= m_dieDuration)
                {
                    //0 = 슬라임, 1 = 가시거북
                    if(mon_pathnum == 0)
                    {
                        MonsterManager.Instance.RemoveMonster(this);
                        MonsterManager.Instance.CreateMonster(mon_pathnum, I_path);
                    }
                    else if(mon_pathnum == 1)
                    {
                        MonsterManager.Instance.RemoveMonster2(this);
                        MonsterManager.Instance.CreateMonster2(mon_pathnum, I_path);
                    }
                    m_dieTime = 0f;
                }
                break;
        }
    }
    // Start is called before the first frame update
    void Awake()
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        m_curWaypoint = -1;
        m_sqrAttackDist = Mathf.Pow(m_attackDist, 2f);
        m_sqrDetecDist = Mathf.Pow(m_detectDist, 2f);
        m_animCtr = GetComponent<MonsterAnimController>();
        m_navAgent = GetComponent<NavMeshAgent>();
        m_tweenMove = GetComponent<TweenMove>();
        m_animator = GetComponent<Animator>();
        m_attackArea_M = m_attackAreObj_M.GetComponentsInChildren<AttackAreUnitFind_Mon>();
        m_player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        m_fxHitPrefab = Resources.Load("Prefab/Effect/FX_Attack_Basic") as GameObject;
        InitSkillData();
        string la_name = m_hudCtr.m_name.text;
        if(la_name.Equals("슬라임"))
        {
            mon_pathnum = 0;
        }
        else if(la_name.Equals("가시거북"))
        {
            mon_pathnum = 1;
        }
        readstat(la_name);
    }

    // Update is called once per frame
    void Update()
    {
        BehaviourProcess();

        /*if(m_ContactPlayer)
        {

        }*/
    }
}
