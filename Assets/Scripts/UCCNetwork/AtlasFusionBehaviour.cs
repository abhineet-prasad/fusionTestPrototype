using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.TextCore.Text;
using Opsive;
using Opsive.UltimateCharacterController;
using Opsive.UltimateCharacterController.Character;
using Opsive.Shared.Events;
using Opsive.UltimateCharacterController.Utility.Builders;
using TMPro;
using Opsive.Shared.Character;
using Opsive.Shared.StateSystem;
using Opsive.UltimateCharacterController.Camera;
using Opsive.Shared.Camera;

public class AtlasFusionBehaviour : NetworkBehaviour
{

    [SerializeField] TextMeshPro playerLabel;
    UltimateCharacterLocomotion m_CharacterLocomotion;
    CameraController _cameraController;
    FusionLookSource _fusionLookSource;

    public void Awake()
    {
  
        m_CharacterLocomotion = GetComponent<FusionUltimateCharacterLocomotion>();
        _cameraController = CameraUtility.FindCamera(null).GetComponent<CameraController>();
        _fusionLookSource = GetComponent<FusionLookSource>();
    }

    public bool IsLocalPlayer => _isLocalPlayer;
    private bool _isLocalPlayer;

    [Networked]
    public Vector3 ServerPosition { get; set; }

    [Networked]
    public Quaternion ServerRotation { get; set; }


    private ChangeDetector _changeDetector;


    public override void Spawned()
    {
        base.Spawned();
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);

        if (HasInputAuthority)
        {
            _isLocalPlayer = true;
            playerLabel.text = "Local";
            playerLabel.color = Color.cyan;
            InitializePlayer(true);
            var simNetOb = SimulationManager.Instance.GetComponent<NetworkObject>();
            simNetOb.AssignInputAuthority(Runner.LocalPlayer);
            
        }
        else
        {
            _isLocalPlayer = false;
            playerLabel.text = "Remote";
            playerLabel.color = Color.red;
            InitializePlayer(false);
        }
    }

    public void Initialize(bool isLocalPlayer)
    {
        if (isLocalPlayer)
        {
            InitializeLocalPlayer();
            
        }
        else
        {
            InitializeRemotePlayer();
        }

    }

    void InitializeRemotePlayer()
    {
        var character = this.gameObject;
        if (character.GetComponent<LocalLookSource>() == null)
        {
            character.AddComponent<LocalLookSource>();
        }

        var inputProxy = character.GetComponent<Opsive.Shared.Input.PlayerInputProxy>();
        if (inputProxy != null)
        {
            DestroyImmediate(inputProxy,true);
            
        }

        var unityInput = character.GetComponentInChildren<Opsive.Shared.Input.UnityInput>();
        if (unityInput != null)
        {
            DestroyImmediate(unityInput.gameObject, true);
        }

        //RemoveUnityInput(character);
    }


    void InitializePlayer(bool isLocalPlayer)
    {
        if (isLocalPlayer)
        {
            _cameraController.Character = gameObject;
        }
        _fusionLookSource.enabled = !isLocalPlayer;
        ILookSource characterLookSource = isLocalPlayer ? _cameraController : _fusionLookSource;
        EventHandler.ExecuteEvent<ILookSource>(gameObject, "OnCharacterAttachLookSource", null);
        EventHandler.ExecuteEvent<ILookSource>(gameObject, "OnCharacterAttachLookSource", characterLookSource);
    }

    void InitializeLocalPlayer()
    {
        var character = this.gameObject;
        m_CharacterLocomotion = character.GetComponent<UltimateCharacterLocomotion>();

        Animator characterAnimator;
        var modelManager = character.GetComponent<ModelManager>();
        if (modelManager != null)
        {
            characterAnimator = modelManager.ActiveModel.GetComponent<Animator>();
        }
        else
        {
            characterAnimator = character.GetComponentInChildren<AnimatorMonitor>(true).GetComponent<Animator>();
        }

        var cameraController = Opsive.Shared.Camera.CameraUtility.FindCamera(character).GetComponent<Opsive.UltimateCharacterController.Camera.CameraController>();


        var lookAtViewType = cameraController.GetViewType<Opsive.UltimateCharacterController.ThirdPersonController.Camera.ViewTypes.LookAt>();
        if (lookAtViewType != null)
        {
            lookAtViewType.Target = characterAnimator.GetBoneTransform(HumanBodyBones.Head);
        }

#if UNITY_2023_1_OR_NEWER
            var pseudo3DPath = GameObject.FindFirstObjectByType<Opsive.UltimateCharacterController.Motion.Path>(FindObjectsInactive.Include);
#else
        var pseudo3DPath = GameObject.FindObjectOfType<Opsive.UltimateCharacterController.Motion.Path>(true);
#endif
        if (pseudo3DPath != null)
        {
            for (int i = 0; i < m_CharacterLocomotion.MovementTypes.Length; ++i)
            {
                if (m_CharacterLocomotion.MovementTypes[i] is Opsive.UltimateCharacterController.ThirdPersonController.Character.MovementTypes.Pseudo3D)
                {
                    var pseudo3DMovementType = m_CharacterLocomotion.MovementTypes[i] as Opsive.UltimateCharacterController.ThirdPersonController.Character.MovementTypes.Pseudo3D;
                    pseudo3DMovementType.Path = pseudo3DPath;
                    break;
                }
            }
        }

        cameraController.ThirdPersonViewTypeFullName = "Opsive.UltimateCharacterController.ThirdPersonController.Camera.ViewTypes.Adventure";

        cameraController.SetPerspective(false, true);

        var existingLookSource = character.GetComponent<LocalLookSource>();
        if(existingLookSource != null)
        {
            DestroyImmediate(existingLookSource);
        }


        cameraController.Character = character;


        if (character.activeInHierarchy)
        {
            EventHandler.ExecuteEvent(character, "OnCharacterSnapAnimator", true);
        }
    }


    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        if (HasStateAuthority)
        {

        }
    }

    public override void Render()
    {

        foreach (var change in _changeDetector.DetectChanges(this))
        {
            switch (change)
            {
                case nameof(ServerPosition):
                    {
                        m_CharacterLocomotion.ServerPosition = ServerPosition ;
                    }
                    break;
                case nameof(ServerRotation):
                    {
                        m_CharacterLocomotion.ServerRotation = ServerRotation;
                    }
                    break;
            }
        }
    }

}
