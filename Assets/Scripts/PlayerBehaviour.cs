using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.InputSystem.XR;

public class PlayerBehaviour : NetworkBehaviour
{
    private NetworkCharacterController _cc;
    [SerializeField]private Ball _prefabBall;
    [SerializeField] private PhysxBall _prefabPhysxBall;

    PlayerColor _playerColor;

    private CharacterController _scc;

    private void Awake()
    {
        _cc = GetComponent<NetworkCharacterController>();
        _playerColor = GetComponent<PlayerColor>();
        _scc = GetComponent<CharacterController>();
        _scc.detectCollisions = false;
    }

    private void Start()
    {
        var localNetObj = Runner.GetPlayerObject(Runner.LocalPlayer);

        isLocal = this.GetComponent<NetworkObject>().Equals(localNetObj);
        
    }

    public  bool isLocal;

    private Vector3 _forward = Vector3.forward;

    [Networked] private TickTimer delay { get; set; }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        if (GetInput(out NetworkInputData data))
        {
            data.direction.Normalize();

            if (_cc == null)
            {
                _scc.Move(5 * data.direction * Runner.DeltaTime);
                //transform.Translate(5 * data.direction * Runner.DeltaTime);
            }
            else
            {
                _cc.Move(5 * data.direction * Runner.DeltaTime);
            }

            if (HasStateAuthority && delay.ExpiredOrNotRunning(Runner)) //spawning should happen only on server
            {

                if (data.buttons.IsSet(NetworkInputData.MOUSEBUTTON0))
                {
                    delay = TickTimer.CreateFromSeconds(Runner, 0.5f);
                    Runner.Spawn(_prefabBall, transform.position + _forward, Quaternion.LookRotation(_forward), Object.InputAuthority,(runner,o) =>
                    {
                        o.GetComponent<Ball>().Init();
                    });
                    _playerColor.spawnedProjecttile = !_playerColor.spawnedProjecttile;
                }
                else if (data.buttons.IsSet(NetworkInputData.MOUSEBUTTON1))
                {
                    delay = TickTimer.CreateFromSeconds(Runner, 0.5f);
                    Runner.Spawn(_prefabPhysxBall,
                      transform.position + 2 * _forward,
                      Quaternion.LookRotation(_forward),
                      Object.InputAuthority,
                      (runner, o) =>
                      {
                          o.GetComponent<PhysxBall>().Init(10 * _forward);
                      });
                    _playerColor.spawnedProjecttile = !_playerColor.spawnedProjecttile;
                }
            }
            

        }
    }
}
