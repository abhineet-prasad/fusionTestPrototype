using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Opsive.UltimateCharacterController.Character;
using UnityEngine.UIElements;
using Opsive.Shared.Camera;
using Opsive.UltimateCharacterController.Camera.ViewTypes;
using Opsive.UltimateCharacterController.Camera;
using Opsive.Shared.Input;

public class FusionLookSource : NetworkBehaviour, ILookSource
{

 
    public GameObject GameObject { get { return m_GameObject; } }

    public Transform Transform => _lookSourceLooseTransform;
    private Transform _lookSourceLooseTransform = null;

    private Transform m_Transform;
    public float LookDirectionDistance {
        get {
            return _activeViewType.LookDirectionDistance;
        }
    }


    private ViewType _activeViewType;
    private GameObject m_GameObject;
    [SerializeField] protected Transform m_LookTransform;
    Camera _camera;
    CameraController _cameraController;
    FusionUnityInputSystem _fusionInputSystem;

    private void Awake()
    {
        m_Transform = transform;
        m_GameObject = gameObject;
        _lookSourceLooseTransform = new GameObject("CameraSimulator").transform;
        _camera = CameraUtility.FindCamera(null);
        _cameraController = _camera.GetComponent<CameraController>(); 
        _activeViewType = _cameraController.ActiveViewType;
        _fusionInputSystem = (FusionUnityInputSystem)GetComponent<FusionPlayerProxy>().PlayerInput;

        if (m_LookTransform == null)
        {
            var animator = GetComponent<Animator>();
            if (animator != null && animator.isHuman)
            {
                m_LookTransform = animator.GetBoneTransform(HumanBodyBones.Head);
            }

            if (m_LookTransform == null)
            {
                m_LookTransform = m_Transform;
            }
        }
    }


    public float Pitch { get { return _activeViewType.Pitch; } }

    public Vector3 LookDirection(bool characterLookDirection)
    {
        return _lookSourceLooseTransform.forward;

    }

    public Vector3 LookDirection(Vector3 lookPosition, bool characterLookDirection, int layerMask, bool includeRecoil, bool includeMovementSpread)
    {
        return new Vector3(_fusionInputSystem.NetworkInput.CurrentInput.LookDirectionX,
                   _fusionInputSystem.NetworkInput.CurrentInput.LookDirectionY,
                   _fusionInputSystem.NetworkInput.CurrentInput.LookDirectionZ);
    }

    public Vector3 LookPosition(bool characterLookPosition)
    {
        return m_LookTransform.position;
    }

    void Update()
    {
        if (_fusionInputSystem != null)
        {
            _lookSourceLooseTransform.SetPositionAndRotation(new Vector3(
                    _fusionInputSystem.NetworkInput.CurrentInput.LookSourcePositionX,
                    _fusionInputSystem.NetworkInput.CurrentInput.LookSourcePositionY,
                    _fusionInputSystem.NetworkInput.CurrentInput.LookSourcePositionZ),
                Quaternion.Euler(_fusionInputSystem.NetworkInput.CurrentInput.LookSourceRotationX,
                    _fusionInputSystem.NetworkInput.CurrentInput.LookSourceRotationY,
                    _fusionInputSystem.NetworkInput.CurrentInput.LookSourceRotationZ));
        }
    }
}
