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

public class AtlasFusionBehaviour : NetworkBehaviour
{

    [SerializeField] TextMeshPro playerLabel;
    UltimateCharacterLocomotion m_CharacterLocomotion;

    public void Awake()
    {
        if (gameObject.GetComponent<LocalLookSource>() == null)
        {
            gameObject.AddComponent<LocalLookSource>();
        }
    }

    public bool IsLocalPlayer => _isLocalPlayer;
    private bool _isLocalPlayer;

    public override void Spawned()
    {
        base.Spawned();
        if (HasInputAuthority)
        {
            _isLocalPlayer = true;
            playerLabel.text = "Local";
            playerLabel.color = Color.cyan;
            InitializeLocalPlayer();
            var simNetOb = SimulationManager.Instance.GetComponent<NetworkObject>();
            simNetOb.AssignInputAuthority(Runner.LocalPlayer);
        }
        else
        {
            _isLocalPlayer = false;
            playerLabel.text = "Remote";
            playerLabel.color = Color.red;
            InitializeRemotePlayer();
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
    
}
