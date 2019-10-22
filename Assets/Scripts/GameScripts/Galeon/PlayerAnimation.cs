using UnityEngine;
using System.Collections;


public delegate void OnAnimationEvent(string a_state);
/// <summary>
/// Componente que enmascara la gestión  de las animaciones de mechanim
/// </summary>
public class PlayerAnimation : MonoBehaviour
{
    public string[] m_states;


    private Animator m_animator;
    private OnAnimationEvent m_OnAnimAttackFinish;
    private RuntimeAnimatorController m_defaultAnimationController;
    public OnAnimationEvent RegisterOnAnimationEvent
    {
        set { m_OnAnimAttackFinish += value; }
    }

    public OnAnimationEvent UnRegisterOnAnimationEvent
    {
        set { m_OnAnimAttackFinish -= value; }
    }

    public bool IsWalking
    {
        get { return m_animator.GetBool("IsMove"); }
        set { m_animator.SetBool("IsMove",value); }
    }
    public bool IsAttacking
    {
        get { return m_animator.GetBool("IsAttacking"); }
        set { m_animator.SetBool("IsAttacking", value); }
    }
    public void EventDeath()
    {
        m_animator.SetTrigger("IsDeath");
    }
    public bool IsRunning { get { return m_animator.GetBool("IsRunning"); } set { m_animator.SetBool("IsRunning", value); } }

    public string CurrentState
    {
        get
        {
            string result = "";
            AnimatorStateInfo stateInfo = m_animator.GetCurrentAnimatorStateInfo(0);
            bool found = false;
            for (int i = 0; !found && i < m_states.Length; ++i)
            {
                if(stateInfo.IsName(m_states[i]))
                {
                    found = true;
                    result = m_states[i];
                }
            }
            if (!found)
                Debug.LogWarning(stateInfo+" No es uno de los estados permitidos");
            return result;

        }
    }

    public void ChangeOverrideAnimatorController(AnimatorOverrideController newController = null)
    {
        if(newController == null)
            m_animator.runtimeAnimatorController = m_defaultAnimationController;
        else
            m_animator.runtimeAnimatorController = newController;
    }

    protected void Start()
    {
        m_animator = GetComponent<Animator>();
        m_defaultAnimationController = m_animator.runtimeAnimatorController;
    }


    protected void AnimationFinish()
    {
        if(m_OnAnimAttackFinish != null)
            m_OnAnimAttackFinish(CurrentState);

    }
}
