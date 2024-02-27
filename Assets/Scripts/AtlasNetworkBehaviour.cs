using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Opsive.UltimateCharacterController.Camera;
using Opsive.UltimateCharacterController.Character;
using UnityEngine.TextCore.Text;
using Opsive.Shared.Events;
using TMPro;
using Opsive.UltimateCharacterController;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using Fusion.Addons.SimpleKCC;
using static Fusion.NetworkBehaviour;

[DefaultExecutionOrder(-5)]
public class AtlasNetworkBehaviour : NetworkBehaviour
{
    public TextMeshPro playerLabel;
    [Header("Movement")]
    public float MoveSpeed = 10.0f;
    public float JumpImpulse = 10.0f;
    public float UpGravity = -25.0f;
    public float DownGravity = -40.0f;
    public float GroundAcceleration = 55.0f;
    public float GroundDeceleration = 25.0f;
    public float AirAcceleration = 25.0f;
    public float AirDeceleration = 1.3f;


    public Transform cameraPivot;
    public Transform cameraHandle;

    private FusionPlayerInput _input;
    private SimpleKCC _simpleKCC;


    [Networked]
    private Vector3 _moveVelocity { get; set; }

    [Networked]
    private bool _moving { get; set; }

    private ChangeDetector _moveChangeDetector;

    public override void Spawned()
    {
        base.Spawned();
        _simpleKCC = GetComponent<SimpleKCC>();
        _simpleKCC.SetGravity(Physics.gravity.y);
        _input = GetComponent<FusionPlayerInput>();

        _moveChangeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
        //var playerO = Runner.GetPlayerObject(Runner.play)
        //playerO.GetComponent<AtlasNetworkBehaviour>().Initialize("Player " + player.PlayerId, playerColors[player.PlayerId % playerColors.Length]);

    }

    public void Initialize(string pname, Color color)
    {
        playerLabel.text = pname;
        playerLabel.color = color;
    }


    public override void FixedUpdateNetwork()
    {

        _simpleKCC.AddLookRotation(_input.CurrentInput.LookRotationDelta);

        Vector3 inputDirection = _simpleKCC.TransformRotation * new Vector3(_input.CurrentInput.MoveDirection.x, 0, _input.CurrentInput.MoveDirection.y);
        float jumpImpulse = default;

        if(_input.CurrentInput.Actions.WasPressed(_input.PreviousInput.Actions, GameplayInput.JUMP_BUTTON) == true)
        {
            if(_simpleKCC.IsGrounded == true)
            {
                jumpImpulse = JumpImpulse;
            }
        }

        // It feels better when the player falls quicker.
        _simpleKCC.SetGravity(_simpleKCC.RealVelocity.y >= 0.0f ? UpGravity : DownGravity);

        Vector3 desiredMovementVelocity = inputDirection * MoveSpeed;

        float acceleration;

        if(desiredMovementVelocity == Vector3.zero)
        {
            acceleration = _simpleKCC.IsGrounded == true ? GroundDeceleration : AirDeceleration;
        }
        else
        {
            acceleration = _simpleKCC.IsGrounded == true ? GroundAcceleration : AirAcceleration;
        }

        _moveVelocity = Vector3.Lerp(_moveVelocity, desiredMovementVelocity, acceleration * Runner.DeltaTime);
        _simpleKCC.Move(_moveVelocity, jumpImpulse);
        if(_moving != (_moveVelocity.sqrMagnitude > .001f))
        {
            _moving = !_moving;
        }
    }


    private void LateUpdate()
    {
        if (HasInputAuthority == false)
        {
            return;
        }

        //Update Camera control logic
        Vector2 pitchRotation = _simpleKCC.GetLookRotation(true, false);
        cameraPivot.localRotation = Quaternion.Euler(pitchRotation);

        Camera.main.transform.SetPositionAndRotation(cameraHandle.position, cameraHandle.rotation);
    }

    public override void Render()
    {

        foreach (var change in _moveChangeDetector.DetectChanges(this))
        {
            switch (change)
            {
                case nameof(_moving):
                    Debug.Log("move change detected");
                    GameEvents.OnMoving(gameObject,_moving);
                    //EventHandler.ExecuteEvent(gameObject, "OnCharacterMoving", _moving);
                    break;
            }
        }


    }
}
