using System.Collections;
using System.Collections.Generic;
using Opsive.Shared.Input;
using Opsive.UltimateCharacterController;
using UnityEngine;

public class FusionPlayerProxy : PlayerInputProxy
{
    protected override void Awake()
    {

        GetComponentInChildren<FusionUnityInputSystem>().NetworkInput = GetComponent<FusionUCCInputNetworkBehaviour>();

        if (m_PlayerInput == null)
        {
            m_PlayerInput = GetComponentInChildren<PlayerInput>();

            if (m_PlayerInput == null)
            {
                Debug.LogError("Error: Unable to find the PlayerInput component.");
                enabled = false;
                return;
            }
        }


        Transform parent;
        // Move the PlayerInput GameObject to a GameObject that will never be disabled.
#if UNITY_2023_1_OR_NEWER
            var scheduler = FindFirstObjectByType<Game.SchedulerBase>();
#else
        var scheduler = SimulationManager.Instance;//FindObjectOfType<Game.SchedulerBase>();
#endif
        if (scheduler != null)
        {
            parent = scheduler.transform;
        }
        else
        {
            parent = new GameObject(gameObject.name + "Input").transform;
        }
        m_PlayerInput.transform.parent = parent;
        m_PlayerInput.transform.localPosition = Vector3.zero;
        m_PlayerInput.transform.localRotation = Quaternion.identity;
        m_PlayerInput.transform.localPosition = Vector3.zero;
        m_PlayerInput.RegisterEvents(gameObject);

#if FIRST_PERSON_CONTROLLER || THIRD_PERSON_CONTROLLER
        Opsive.Shared.StateSystem.StateManager.LinkGameObjects(gameObject, m_PlayerInput.gameObject, true);
#endif
    }
}
