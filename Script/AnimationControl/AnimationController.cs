using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    //다형성 부모-자식 클래스

    Animator m_animator;
    Dictionary<string, float> m_dicComboInputTime = new Dictionary<string, float>();
    string m_prevMotion;

    // Player - Monster 어택1과 어택2 콤보를 넣어주려면  그안에 클립들의 시간을 구해서
    //어택2로 갈 수 있는 시간을 정해야 한다. 그래서 정해주는 함수.
    public void CalculateCombonputTime()
    {
        //애니메이션안에 등록되어있는 클립들 정보
        var cilps = m_animator.runtimeAnimatorController.animationClips; 
        for (int i = 0; i < cilps.Length; i++)
        {
            if (cilps[i].events.Length >= 2)
            {
                float attackTime = cilps[i].events[0].time;
                float endFrameTime = cilps[i].events[1].time;
                float result = (endFrameTime - attackTime);
                m_dicComboInputTime.Add(cilps[i].name, result);
            }
        }
    }


    public float GetComboInputTime(string animName)
    {
        float time = 0f;
        m_dicComboInputTime.TryGetValue(animName, out time);
        return time;
    }


    //FSM 이용하여 enum에 적힌 string을 보고 애니메이션 Trigger 발동하여
    // 애니메이션 PLAY하게 하는 함수
    public void Play(string animName, bool isBlend = true)
    {
        if (!string.IsNullOrEmpty(m_prevMotion))
        {
            m_animator.ResetTrigger(m_prevMotion);
            m_prevMotion = null;
        }
        if (isBlend)
        {
            m_animator.SetTrigger(animName);
        }
        else
        {
            m_animator.Play(animName, 0, 0f);
        }
        m_prevMotion = animName;
    }

    // Start is called before the first frame update
    void Awake()
    {
        m_animator = GetComponent<Animator>();
        CalculateCombonputTime();
    }
}
