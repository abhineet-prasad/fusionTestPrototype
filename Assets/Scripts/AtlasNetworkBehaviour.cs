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

[StructLayout(LayoutKind.Explicit)]
[NetworkStructWeaved(WORDS + 4)]
public unsafe struct AtlasNetworkCCData : INetworkStruct
{
    public const int WORDS = NetworkTRSPData.WORDS + 4;
    public const int SIZE = WORDS * 4;

    [FieldOffset(0)]
    public NetworkTRSPData TRSPData;


    [FieldOffset((NetworkTRSPData.WORDS + 0) * Allocator.REPLICATE_WORD_SIZE)]
    Vector3Compressed _velocityData;


    public Vector3 Velocity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _velocityData;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => _velocityData = value;
    }
}




[DisallowMultipleComponent]
[NetworkBehaviourWeaved(AtlasNetworkCCData.WORDS)]
public class AtlasNetworkBehaviour : NetworkTRSP, INetworkTRSPTeleport, IBeforeAllTicks, IAfterAllTicks, IBeforeCopyPreviousState
{
    new ref AtlasNetworkCCData Data => ref ReinterpretState<AtlasNetworkCCData>();

    public TextMeshPro playerLabel;
    NetworkCharacterLocomotionHandler locomotionHandler;
    CharacterLocomotion characterLocomotion;

    void Awake()
    {
        locomotionHandler = gameObject.GetComponent<NetworkCharacterLocomotionHandler>();
        characterLocomotion = GetComponent<CharacterLocomotion>();
    }
    public override void Spawned()
    {
        base.Spawned();
        if (HasInputAuthority)
        {
            //AttachCamera();
            var lookSource = gameObject.AddComponent<LocalLookSource>();
            EventHandler.ExecuteEvent<ILookSource>(gameObject, "OnCharacterAttachLookSource", lookSource);
        }
        else
        {
            var lookSource = gameObject.AddComponent<LocalLookSource>();
            EventHandler.ExecuteEvent<ILookSource>(gameObject, "OnCharacterAttachLookSource", lookSource);
        }
    }

    public void Initialize(string pname, Color color)
    {
        playerLabel.text = pname;
        playerLabel.color = color;
    }

    public void AttachCamera()
    {
        var cam = Camera.main.GetComponent<CameraController>();
        cam.Character = this.gameObject;
    }

    public override void FixedUpdateNetwork()
    {

        base.FixedUpdateNetwork();
        if (GetInput(out NetworkInputData data))
        {
            data.direction.Normalize();

            //locomotionHandler.SetHorizontalInput(data.direction.x, data.direction.z);
            Move(data.direction);
        }

    }


    public void Move(Vector3 direction)
    {
        var deltaTime = Runner.DeltaTime;
        var previousPos = transform.position;
        var moveVelocity = Data.Velocity;

        direction = direction.normalized;
        characterLocomotion.Move(moveVelocity.x, moveVelocity.z, moveVelocity.x) ;

        Data.Velocity = (transform.position - previousPos) * Runner.TickRate;
       
    }


    public void Teleport(Vector3? position = null, Quaternion? rotation = null)
    {
        characterLocomotion.enabled = false;
        NetworkTRSP.Teleport(this, transform, position, rotation);
        // Re-enable CC
        characterLocomotion.enabled = true;
    }

    void IBeforeAllTicks.BeforeAllTicks(bool resimulation, int tickCount)
    {
        CopyToEngine();
    }

    void IAfterAllTicks.AfterAllTicks(bool resimulation, int tickCount)
    {
        CopyToBuffer();
    }

    void IBeforeCopyPreviousState.BeforeCopyPreviousState()
    {
        CopyToBuffer();
    }

    void CopyToBuffer()
    {
        Data.TRSPData.Position = transform.position;
        Data.TRSPData.Rotation = transform.rotation;
    }

    void CopyToEngine()
    {
        // CC must be disabled before resetting the transform state

        characterLocomotion.enabled = false;
        // set position and rotation
        transform.SetPositionAndRotation(Data.TRSPData.Position, Data.TRSPData.Rotation);

        // Re-enable CC
        characterLocomotion.enabled = true;
    }
}
