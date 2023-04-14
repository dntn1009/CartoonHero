using Firebase.Database;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [System.Serializable]
    public class PlayerInfo
    {
        public float attack;
        public float criAttack;
        public float criRate;
        public float defence;
        public float dodgeRate;
        public float hitRate;
        public int hp;
        public int hpMax;

        public PlayerInfo(float attack, float criAttack, float criRate, float defence, float dodgeRate, float hitRate, int hp, int hpMax)
        {
            this.attack = attack;
            this.criAttack = criAttack;
            this.criRate = criRate;
            this.defence = defence;
            this.dodgeRate = dodgeRate;
            this.hitRate = hitRate;
            this.hp = hp;
            this.hpMax = hpMax;
        }
    }
    [Header("주인공 능력치")]
    [SerializeField]
    public PlayerInfo playInfo;
    //Var
    Vector3 m_dir;
    Vector3 move_dir;
    float m_accel;
    CharacterController m_charCtr;
    PlayerAnimController m_animCtr;
    Animator m_animator;
    AttackAreUnitFind[] m_attackArea;
    GameObject m_fxHitPrefab;
    Dictionary<PlayerAnimController.Motion, SkillData> m_skillTable = new Dictionary<PlayerAnimController.Motion, SkillData>();
    ///////////////
    //SerializeField
    [SerializeField]
    float m_speed = 2f;
    [SerializeField]
    GameObject m_attackAreaObj;
    [SerializeField]
    public HUDController m_hudCtr;
    ////////////////
    ////////Die respwan
    float timer;
    int waitingtime;

    ///////
    public bool IsDie { get { return m_animCtr.GetAnimState() == PlayerAnimController.Motion.Die; } }
    public bool IsAttack
    {
        get
        {
            if (m_animCtr.GetAnimState() == PlayerAnimController.Motion.Attack01 ||
                 m_animCtr.GetAnimState() == PlayerAnimController.Motion.Attack02)
                return true;
            return false;
        }
    }

    bool m_isPressAttack;
    Queue<KeyCode> m_keyBuffer = new Queue<KeyCode>();
    List<PlayerAnimController.Motion> m_comboList = new List<PlayerAnimController.Motion>() { PlayerAnimController.Motion.Attack01, PlayerAnimController.Motion.Attack02 };
    int m_comboIndex; // 몇번쨰 ATTACk 인지 알려주려는 변수

    public bool target_player = false;
    ////////////////
    #region Animation Event Methods
    void AnimEvent_Attack()
    {
        SkillData skillData;
        float damage = 0f;
        if (m_skillTable.TryGetValue(m_animCtr.GetAnimState(), out skillData))
        {
            var unitList = m_attackArea[skillData.AttackArea].UnitList;
            for (int i = 0; i < unitList.Count; i++)
            {
                var mon = unitList[i].GetComponent<MonsterController>();
                var dummy = Util.FindChildObject(unitList[i], "Monster_Hit");
                if (dummy != null && mon != null)
                {
                    if (mon.IsDie) continue;

                    AttackType type = AttackProcess(mon, skillData, out damage);
                    mon.SetDemage(type, damage, skillData);
                    if (type == AttackType.Dodge) return;
                    var effect = Instantiate(m_fxHitPrefab);
                    effect.transform.position = dummy.transform.position;
                    effect.transform.rotation = Quaternion.FromToRotation(effect.transform.forward, (unitList[i].transform.position - transform.position).normalized);
                    Destroy(effect, 1.5f);
                }
            }
        }
        for (int i = 0; i < m_attackArea.Length; i++)
            m_attackArea[i].UnitList.RemoveAll(obj => obj.GetComponent<MonsterController>().IsDie);
    }

    void AnimaEvent_AttackFinished()
    {
        bool IsCombo = false; // 연타했을경우를 위한 변수
        if (m_isPressAttack)
            IsCombo = true;
        if (m_keyBuffer.Count == 1)
        {
            var key = m_keyBuffer.Dequeue();
            if (key == KeyCode.Space)
                IsCombo = true;
        }
        else if (m_keyBuffer.Count > 1)
        {
            ReleaseKeyBuffer();
            IsCombo = false;
        }
        if (IsCombo)
        {
            m_comboIndex++;
            if (m_comboIndex >= m_comboList.Count)
                m_comboIndex = 0;
            m_animCtr.Play(m_comboList[m_comboIndex]);
        }
        else
        {
            m_animCtr.Play(PlayerAnimController.Motion.Idle);
            m_comboIndex = 0;
        }
    }
    void AnimEvent_HitFinished()
    {
        m_animCtr.Play(PlayerAnimController.Motion.Idle);
    }
    #endregion
    ////////////////

    #region Virtual Controller Event Methods
    Vector3 GetPadDir()
    {
        var Paddir = GamePad.Instance.GetAxis();
        Vector3 dir = Paddir;
       /* Vector3 dir = Vector3.zero;
        if (Paddir.x < 0.0f)
        {
            dir += Vector3.left * Mathf.Abs(Paddir.x);
        }
        if (Paddir.x > 0.0f)
        {
            dir += Vector3.right * Paddir.x;
        }
        if (Paddir.y < 0.0f)
        {
            dir += Vector3.back * Mathf.Abs(Paddir.y);
        }
        if (Paddir.y > 0.0f)
        {
            dir += Vector3.forward * Paddir.y;
        }*/
        return dir;
    }

    public void SetPlayer(Camera uiCamera, Transform hudPanel)
    {
        m_hudCtr.SetHud(uiCamera, hudPanel);
        if (CharacterData.Instance.Select_Slot1)
        {
            m_hudCtr.m_name.text = CharacterData.Instance.slot1.Nickname;
        }
        else if (CharacterData.Instance.Select_Slot2)
        {
            m_hudCtr.m_name.text = CharacterData.Instance.slot2.Nickname;
        }
        m_hudCtr.InitHud();
    }

    public void SetRead()
    {
        if (CharacterData.Instance.Select_Slot1)
        {
            string th_name = CharacterData.Instance.slot1.Type;
            readstat(th_name);
        }
        else if(CharacterData.Instance.Select_Slot2)
        {
            string th_name = CharacterData.Instance.slot2.Type;
            readstat(th_name);
        }
    }

    public void OnPressAttack()
    {
        if (IsAttack)
        {
            if (IsInvoking("ReleaseKeyBuffer")) // 아직 리셋이 안되었단 이야기
                CancelInvoke("ReleaseKeyBuffer");
            float time = m_animCtr.GetComboInputTime(m_comboList[m_comboIndex].ToString());
            Invoke("ReleaseKeyBuffer", time); ;
            m_keyBuffer.Enqueue(KeyCode.Space);
        }// 눌린시점으로부터 일정시간동안 값이 안들어오면 이값을 비울것이란 예약
        if (m_animCtr.GetAnimState() == PlayerAnimController.Motion.Idle || m_animCtr.GetAnimState() == PlayerAnimController.Motion.Move)
        {
            m_animCtr.Play(PlayerAnimController.Motion.Attack01);
        }
        m_isPressAttack = true;
    }

    public void OnReleaseAttack()
    {
        m_isPressAttack = false;
    }

    AttackType AttackProcess(MonsterController mon, SkillData skillData, out float damage)
    {
        AttackType type = AttackType.Dodge;
        damage = 0f;
        if (CalculationDamage.AttackDecision(playInfo.hitRate, mon.monInfo.dodgeRate))
        {
            type = AttackType.Normal;
            damage = CalculationDamage.NormalDamage(playInfo.attack, skillData.attack, mon.monInfo.defence);
            if (CalculationDamage.CriticalDecision(playInfo.criRate))
            {
                type = AttackType.Critical;
                damage = CalculationDamage.CriticalDamage(damage, playInfo.criAttack);
            }
        }
        return type;
    }

    void ReleaseKeyBuffer()
    {
        m_keyBuffer.Clear();
    }
    #endregion

    private void readstat(string player_name)
    {

        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        FirebaseDatabase.DefaultInstance
     .GetReference("Player")
     .GetValueAsync().ContinueWithOnMainThread(task => {
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

                 if (player_name.Equals(name))
                 {
                     myData = data.GetRawJsonValue();
                     playInfo = JsonUtility.FromJson<PlayerInfo>(myData);
                 }

             }
         }

     });
    }

    public void SetDemage(AttackType attackType, float damage, SkillData skillData)
    {
        if (IsDie) return;
        playInfo.hp -= Mathf.CeilToInt(damage);
        m_hudCtr.DisplayDamage(attackType, damage, playInfo.hp / (float)playInfo.hpMax);

        if (attackType == AttackType.Dodge) return;

        if(attackType== AttackType.Critical)
        m_animCtr.Play(PlayerAnimController.Motion.Hit, false);

        if (playInfo.hp <= 0f)
        {
            m_animCtr.Play(PlayerAnimController.Motion.Die);
        }

    }

    void InitSkillData()
    {
        m_skillTable.Add(PlayerAnimController.Motion.Attack01, new SkillData() { AttackArea = 0, knockBack = 0.5f, delayFrame = 2 });
        m_skillTable.Add(PlayerAnimController.Motion.Attack02, new SkillData() { AttackArea = 1, knockBack = 0.5f, delayFrame = 3 });
        //DB로 만들자
    }

    // Start is called before the first frame update
    void Start()
    {
        ///die respwan;
        timer = 0.0f;
        waitingtime = 4;
        ///
        m_charCtr = GetComponent<CharacterController>(); // character Controller 활성화
        m_animCtr = GetComponent<PlayerAnimController>(); // PlayerAnim Controller 활성화 다형성
        m_animator = GetComponent<Animator>();
        m_attackArea = m_attackAreaObj.GetComponentsInChildren<AttackAreUnitFind>();
        m_fxHitPrefab = Resources.Load("Prefab/Effect/FX_Attack_Basic") as GameObject;
        SetRead();
        InitSkillData();
    }

    // Update is called once per frame
    void Update()
    {
        var padDir = GetPadDir();
        m_dir = padDir;
        //m_dir.Normalize(); // 1값으로 고정시켜줌
        m_animator.SetFloat("Speed", padDir.magnitude );
        move_dir = CharacterManager.Instance.Move_Camerafoward(m_dir);
      
        if (m_dir != Vector3.zero && !IsAttack)
        {
            if (m_animCtr.GetAnimState() == PlayerAnimController.Motion.Idle)
                m_animCtr.Play(PlayerAnimController.Motion.Move);
            transform.forward = move_dir; //캐릭터의 정면을 뜻함
        }
        else
        {
            if (m_animCtr.GetAnimState() == PlayerAnimController.Motion.Move)
                m_animCtr.Play(PlayerAnimController.Motion.Idle);
        }
        if (!m_charCtr.isGrounded)
        {
            m_accel += Mathf.Abs(Physics.gravity.y) * Time.deltaTime;
            m_dir += Vector3.down * m_accel;
        }
        else
        {
            m_accel = 0f;
        }
        if (!IsAttack)
            m_charCtr.Move(move_dir * m_speed * Time.deltaTime);
        if (IsDie)
        {
            timer += Time.deltaTime;

            if (timer >= waitingtime)
            {
                m_hudCtr.SetPlayerHP();
                playInfo.hp = playInfo.hpMax;
                Vector3 move = new Vector3(-(transform.position.x), 21, -(transform.position.z));
                m_charCtr.Move(move);
                m_animCtr.Play(PlayerAnimController.Motion.Idle);
                timer = 0;
            }
        }
    }
}
