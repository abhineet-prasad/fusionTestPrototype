using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using static Fusion.NetworkBehaviour;

[DefaultExecutionOrder(-10)]
public class FusionPlayerInput : NetworkBehaviour, IBeforeUpdate, IBeforeTick
{

    public GameplayInput CurrentInput => _currentInput;
    public GameplayInput PreviousInput => _previousInput;

    [SerializeField]

    private Vector2 _lookSensitivity = Vector2.one;

    [Networked]
    private GameplayInput _currentInput { get; set; }

    private GameplayInput _previousInput;
    private GameplayInput _accumulatedInput;
    private bool _resetAccumulatedInput;

    private JengaInput _inputAsset;

    protected override bool ReplicateTo(PlayerRef player)
    {
        return player == Object.InputAuthority;
    }

    public override void Spawned()
    {
        _currentInput = default;
        _previousInput = default;
        _accumulatedInput = default;
        _resetAccumulatedInput = default;

        _inputAsset = new JengaInput();
        _inputAsset.RoamingModeInput.Enable();

        if(Object.HasInputAuthority == true)
        {
            NetworkEvents networkEvents = Runner.GetComponent<NetworkEvents>();
            networkEvents.OnInput.AddListener(OnInput);

            if (Application.isMobilePlatform == false || Application.isEditor == true)
            {
                // Hide cursor
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

           
        }

    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        if (runner == null)
            return;

        NetworkEvents networkEvents = runner.GetComponent<NetworkEvents>();
        if (networkEvents != null)
        {
            // Unregister local player input polling.
            networkEvents.OnInput.RemoveListener(OnInput);
        }
    }

    private void OnInput(NetworkRunner runner, NetworkInput networkInput)
    {
      
        networkInput.Set(_accumulatedInput);
        _accumulatedInput.LookRotationDelta = default;
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

        if (Application.isMobilePlatform == false || Application.isEditor == true)
        {
            // Input is tracked only if the cursor is locked.
            if (Cursor.lockState != CursorLockMode.Locked)
                return;
        }

        //accumulate mouse rotation data
        _accumulatedInput.LookRotationDelta += new Vector2(-_inputAsset.RoamingModeInput.MouseY.ReadValue<float>(), _inputAsset.RoamingModeInput.MouseX.ReadValue<float>()) * _lookSensitivity;
        //accumulate movement input data
        _accumulatedInput.MoveDirection = new Vector2(_inputAsset.RoamingModeInput.Horizontal.ReadValue<float>(), _inputAsset.RoamingModeInput.Vertical.ReadValue<float>()).normalized;
        //set jump movement
        _accumulatedInput.Actions.Set(GameplayInput.JUMP_BUTTON, _inputAsset.RoamingModeInput.Jump.IsPressed());
    }

    public void BeforeTick()
    {
        if (Object == null)
            return;

        _previousInput = _currentInput;

        GameplayInput currentInput = _currentInput;
        currentInput.LookRotationDelta = default;
        _currentInput = currentInput;

        if(Object.InputAuthority != PlayerRef.None)
        {
            if(GetInput(out GameplayInput input) == true)
            {
                _currentInput = input;
            }
        }
    }
    

}
