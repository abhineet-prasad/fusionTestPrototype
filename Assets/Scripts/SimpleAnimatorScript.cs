using System.Collections;
using System.Collections.Generic;
using Opsive.Shared.Events;
using UnityEngine;

public class SimpleAnimatorScript : MonoBehaviour
{
    protected GameObject m_GameObject;
    private bool m_Moving;
    protected Animator m_Animator;
    private static int s_MovingHash = Animator.StringToHash("Moving");



    private void Awake()
    {
        m_GameObject = this.gameObject;
        m_Animator = gameObject.GetComponent<Animator>();

        GameEvents.OnMoving += OnMoving;
        //EventHandler.RegisterEvent<bool>(m_GameObject, "OnCharacterMoving", OnMoving);
    }

    private void OnDestroy()
    {
        GameEvents.OnMoving -= OnMoving;
        //EventHandler.UnregisterEvent<bool>(m_GameObject, "OnCharacterMoving", OnMoving);
    }

    private void OnMoving(GameObject passedObject, bool moving)
    {
        if (transform.parent.gameObject.Equals(passedObject))
        {
            Debug.Log("on moving called from event");
            SetMovingParameter(moving);
        }
    }

    public bool SetMovingParameter(bool value)
    {
        var change = m_Moving != value;
        if (change)
        {
            if (m_Animator != null)
            {
                m_Animator.SetBool(s_MovingHash, value);
            }
            m_Moving = value;
        }
        return change;
    }
}
