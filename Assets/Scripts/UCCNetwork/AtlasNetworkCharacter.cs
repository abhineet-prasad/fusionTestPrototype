using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Opsive.Shared.Networking;
using Fusion;
using Opsive.UltimateCharacterController.Networking.Character;
using Opsive.UltimateCharacterController.Items.Actions.Modules;
using Opsive.UltimateCharacterController.Items.Actions;
using Opsive.UltimateCharacterController.Items.Actions.Impact;
using Opsive.UltimateCharacterController.Items.Actions.Modules.Melee;
using Opsive.UltimateCharacterController.Items.Actions.Modules.Shootable;
using Opsive.UltimateCharacterController.Character;
using Opsive.Shared.Events;

using Opsive.UltimateCharacterController.Character.Abilities;

public class AtlasNetworkCharacter : NetworkBehaviour, INetworkCharacter
{

    public override void Spawned()
    {
        base.Spawned();
        var lookSource = gameObject.AddComponent<LocalLookSource>();
        EventHandler.ExecuteEvent<ILookSource>(gameObject, "OnCharacterAttachLookSource", lookSource);
        
    }


    #region unusedCallbacks

    public void ChangeModels(int modelIndex)
    {
        throw new System.NotImplementedException();
    }

    public void EnableThrowableObjectMeshRenderers(ActionModule module, bool enable)
    {
        throw new System.NotImplementedException();
    }

    public void EquipUnequipItem(uint itemIdentifierID, int slotID, bool equip)
    {
        throw new System.NotImplementedException();
    }

    public void ExecuteBoolEvent(string eventName, bool value)
    {
        throw new System.NotImplementedException();
    }

    public void InvokeGenericEffectModule(ActionModule module)
    {
        throw new System.NotImplementedException();
    }

    public void InvokeMagicBeginEndModules(CharacterItemAction itemAction, ActionModuleGroupBase moduleGroup, int invokedBitmask, bool start, MagicUseDataStream data)
    {
        throw new System.NotImplementedException();
    }

    public void InvokeMagicCastEffectsModules(CharacterItemAction itemAction, ActionModuleGroupBase moduleGroup, int invokedBitmask, INetworkCharacter.CastEffectState state, MagicUseDataStream data)
    {
        throw new System.NotImplementedException();
    }

    public void InvokeMagicImpactModules(CharacterItemAction itemAction, ActionModuleGroupBase moduleGroup, int invokedBitmask, ImpactCallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void InvokeMeleeAttackEffectModule(ActionModule module, MeleeUseDataStream data)
    {
        throw new System.NotImplementedException();
    }

    public void InvokeMeleeAttackModule(MeleeAttackModule module, MeleeUseDataStream data)
    {
        throw new System.NotImplementedException();
    }

    public void InvokeMeleeImpactModules(CharacterItemAction itemAction, ActionModuleGroupBase moduleGroup, int invokedBitmask, MeleeImpactCallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void InvokeShootableDryFireEffectModules(CharacterItemAction itemAction, ActionModuleGroupBase moduleGroup, int invokedBitmask, ShootableUseDataStream data)
    {
        throw new System.NotImplementedException();
    }

    public void InvokeShootableFireEffectModules(CharacterItemAction itemAction, ActionModuleGroupBase moduleGroup, int invokedBitmask, ShootableUseDataStream data)
    {
        throw new System.NotImplementedException();
    }

    public void InvokeShootableImpactModules(CharacterItemAction itemAction, ActionModuleGroupBase moduleGroup, int invokedBitmask, ShootableImpactCallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void InvokeThrowableEffectModules(CharacterItemAction itemAction, ActionModuleGroupBase moduleGroup, int invokedBitmask, ThrowableUseDataStream data)
    {
        throw new System.NotImplementedException();
    }

    public void InvokeUseAttributeModifierToggleModule(ActionModule module, bool on)
    {
        throw new System.NotImplementedException();
    }

    public void ItemIdentifierPickup(uint itemIdentifierID, int slotID, int amount, bool immediatePickup, bool forceEquip)
    {
        throw new System.NotImplementedException();
    }

    public void ItemReloadComplete(ShootableReloaderModule module, bool success, bool immediateReload)
    {
        throw new System.NotImplementedException();
    }

    public void LoadDefaultLoadout()
    {
        throw new System.NotImplementedException();
    }

    public void PushRigidbody(Rigidbody targetRigidbody, Vector3 force, Vector3 point)
    {
        throw new System.NotImplementedException();
    }

    public void ReloadItem(ShootableReloaderModule itemAction, bool fullClip)
    {
        throw new System.NotImplementedException();
    }

    public void RemoveAllItems()
    {
        throw new System.NotImplementedException();
    }

    public void RemoveItemIdentifierAmount(uint itemIdentifierID, int slotID, int amount, bool drop, bool removeCharacterItem, bool destroyCharacterItem)
    {
        throw new System.NotImplementedException();
    }

    public void ResetRotationPosition()
    {
        throw new System.NotImplementedException();
    }

    public void SetActive(bool active, bool uiEvent)
    {
        throw new System.NotImplementedException();
    }

    public void StartItemReload(ShootableReloaderModule module)
    {
        throw new System.NotImplementedException();
    }

    #endregion

    UltimateCharacterLocomotion _characterLocomotion;
    MoveTowards _moveTowards;

    void Awake()
    {
        _characterLocomotion = GetComponent<UltimateCharacterLocomotion>();
        _moveTowards = _characterLocomotion.GetAbility<MoveTowards>();
    }

    public Vector3 targetLocation;

    void Start()
    {
        
    }

    public void SetPosition(Vector3 position, bool snapAnimator)
    {
        transform.position = position;
    }

    public void SetPositionAndRotation(Vector3 position, Quaternion rotation, bool snapAnimator, bool stopAllAbilities)
    {
        transform.SetPositionAndRotation(position, rotation);
        //_characterLocomotion.DesiredMovement = position ;
    }

    public void SetRotation(Quaternion rotation, bool snapAnimator)
    {
        transform.rotation = rotation;
    }


    Vector3 _targetPosition;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            //_characterLocomotion.MoveTowardsAbility.MoveTowardsLocation(targetLocation);
            _characterLocomotion.MoveTowardsAbility.MoveTowardsLocation(transform.position + Vector3.forward * 10 * Runner.DeltaTime);
        }
    }


    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        if (GetInput(out NetworkInputData data))
        {
            data.direction.Normalize();

         //   _characterLocomotion.MoveTowardsAbility.MoveTowardsLocation(transform.position + data.direction * 10 * Runner.DeltaTime);


            //_characterLocomotion.DesiredMovement = data.direction * 10 * Runner.DeltaTime;
            //_characterLocomotion.SetPositionAndRotation(transform.position + data.direction * 10 * Runner.DeltaTime, transform.rotation);
            //_characterLocomotion.DesiredMovement = Vector3.forward * 1 * Runner.DeltaTime;

            //_characterLocomotion.DesiredMovement = data.direction * 10 * Time.deltaTime;

        }
    }

}
