using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Opsive.Shared.Input;
using UnityEngine.InputSystem;
using Opsive.Shared.Game;

public class FusionUCCInputNetworkBehaviour : NetworkBehaviour, IBeforeUpdate
{

    private string _horizontalAxisName = "Horizontal";
    private string _verticalAxisName = "Vertical";
    private string _mouseXAxisName = "Mouse X";
    private string _mouseYAxisName = "Mouse Y";
    private string _controllerHorizontalLookInputName = "Controller X";
    private string _controllerVerticalLookInputName = "Controller Y";


    public UCCInput CurrentInput => _receivedCurrentInput;
    public UCCInput PreviousInput => _receivedPreviousInput;


  
    protected UnityEngine.InputSystem.PlayerInput m_PlayerInput;
    protected Dictionary<InputActionMap, Dictionary<string, InputAction>> m_InputActionByName = new Dictionary<InputActionMap, Dictionary<string, InputAction>>();

  //  [Networked]
    UCCInput _receivedCurrentInput { get; set; }

    UCCInput _receivedPreviousInput;
    UCCInput _accumulatedInput;
    bool _resetAccumulatedInput = false;


    public override void Spawned()
    {
        _receivedCurrentInput = default;
        _receivedPreviousInput = default;
        _accumulatedInput = default;
        _resetAccumulatedInput = false;


        if (Object.HasInputAuthority == true)
        {
            NetworkEvents networkEvents = Runner.GetComponent<NetworkEvents>();
            networkEvents.OnInput.AddListener(OnInput);

            m_PlayerInput = FindObjectOfType<Opsive.Shared.Game.SchedulerBase>().GetComponentInChildren<UnityEngine.InputSystem.PlayerInput>();
            m_PlayerInput.enabled = true;
        }

        
    }

    void OnInput(NetworkRunner runner, NetworkInput networkInput)
    {
        Debug.Log("Dbmarker set input " + _accumulatedInput.horizontalValue + " " + _accumulatedInput.verticalValue);
        networkInput.Set(_accumulatedInput);

        _resetAccumulatedInput = true;
    }

    public void BeforeUpdate()
    {
        if (HasInputAuthority == false)
            return;


        if (_resetAccumulatedInput == true)
        {
            _resetAccumulatedInput = false;
            _accumulatedInput = default;
        }
        _accumulatedInput.mousePosition = Mouse.current.position.ReadValue();

        _accumulatedInput.SetAxisByName(_horizontalAxisName, GetAxisLocalValue(_horizontalAxisName));
        _accumulatedInput.SetAxisByName(_verticalAxisName, GetAxisLocalValue(_verticalAxisName));
        _accumulatedInput.SetAxisByName(_mouseXAxisName, GetAxisLocalValue(_mouseXAxisName));
        _accumulatedInput.SetAxisByName(_mouseYAxisName, GetAxisLocalValue(_mouseYAxisName));
        _accumulatedInput.SetAxisByName(_controllerHorizontalLookInputName, GetAxisLocalValue(_controllerHorizontalLookInputName));
        _accumulatedInput.SetAxisByName(_controllerVerticalLookInputName, GetAxisLocalValue(_controllerVerticalLookInputName));

    }

    public override void FixedUpdateNetwork()
    {
        if ( HasStateAuthority) //   Object.InputAuthority != PlayerRef.None)
        {
            if (GetInput(out UCCInput input) == true)
            {
               
                _receivedCurrentInput = input;
                Debug.Log("Dbmarker received input " + _receivedCurrentInput.horizontalValue + " " + _receivedCurrentInput.verticalValue);
            }
        }

    }


    private float GetAxisLocalValue(string name)
    {
        var action = GetActionByName(name);
        if (action != null)
        {
            return action.ReadValue<float>();
        }
        return 0.0f;
    }


    protected InputAction GetActionByName(string name)
    {
        if (m_PlayerInput.currentActionMap == null)
        {
            return null;
        }

        if (!m_InputActionByName.TryGetValue(m_PlayerInput.currentActionMap, out var inputActionByName))
        {
            inputActionByName = new Dictionary<string, InputAction>();
            m_InputActionByName.Add(m_PlayerInput.currentActionMap, inputActionByName);
        }
        if (!inputActionByName.TryGetValue(name, out var inputAction))
        {
            inputAction = m_PlayerInput.currentActionMap?.FindAction(name);
            inputActionByName.Add(name, inputAction);
        }
        return inputAction;
    }

}
