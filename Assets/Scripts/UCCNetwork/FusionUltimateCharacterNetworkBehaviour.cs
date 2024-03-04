using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using static Fusion.NetworkBehaviour;
using Opsive.Shared.Events;
using Opsive.Shared.StateSystem;

public class FusionUltimateCharacterNetworkBehaviour : NetworkBehaviour
{
    GameObject m_GameObject;
    FusionUltimateCharacterLocomotion _characterLocomotion;

    [Networked]
    public bool Moving { get; set; }

    [Networked]
    public Vector2 InputVector { get; set; }

    [Networked]
    public Vector2 RawInputVector { get; set; }

    [Networked]
    public bool UpdateAnimationParameters { get; set; }


    private ChangeDetector _changeDetector;

    private bool _hasSpawned = false;
    public bool HasSpawned => _hasSpawned;

    public override void Spawned()
    {
        _hasSpawned = true;
        _characterLocomotion = GetComponent<FusionUltimateCharacterLocomotion>();
        m_GameObject = gameObject;
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
    }

    public override void Render()
    {

        foreach (var change in _changeDetector.DetectChanges(this))
        {
            switch (change)
            {
                case nameof(Moving):
                    {
                        //_characterLocomotion.Moving = Moving;
                        EventHandler.ExecuteEvent(m_GameObject, "OnCharacterMoving", Moving);
                        if (!string.IsNullOrEmpty(_characterLocomotion.MovingStateName))
                        {
                            StateManager.SetState(m_GameObject, _characterLocomotion.MovingStateName, Moving);
                        }
                    }

                    break;

                case nameof(UpdateAnimationParameters):
                    {
                        if (!HasStateAuthority) //this is being called directly on the server, no need to use an event 
                        {
                            EventHandler.ExecuteEvent(m_GameObject, "OnAnimationParametersUpdate");
                        }
                    }
                    break;
               
         
            }
        }
    }

}
