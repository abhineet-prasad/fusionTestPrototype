using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Opsive.UltimateCharacterController.Character;
using Fusion;
using Opsive.UltimateCharacterController.Utility;
using Opsive.UltimateCharacterController.Game;
using Opsive.Shared.Events;
using Opsive.Shared.StateSystem;

public class FusionUltimateCharacterLocomotion : UltimateCharacterLocomotion
{

    NetworkRunner runner;
    FusionUltimateCharacterNetworkBehaviour _networkedCharacter;

    bool test = true;

    public override Vector2 InputVector
    {
        get { return (_networkedCharacter.HasSpawned && test) ? _networkedCharacter.InputVector : base.InputVector; }

        set {
            base.InputVector = value;
            if (runner.IsServer)
            { 
                _networkedCharacter.InputVector = value;
            }
        }
    }

    public override Vector2 RawInputVector
    {
        get { return (_networkedCharacter.HasSpawned && test) ? _networkedCharacter.RawInputVector : base.RawInputVector; }
        set
        {
            base.RawInputVector = value;
            if (runner.IsServer)
            {
                _networkedCharacter.RawInputVector = value;
            }
        }
    }



    protected override void UpdateCharacter()
    {
        base.UpdateCharacter();

        //trigger animations on all scripts
        if (runner.IsServer)//this check is redundant right now as Update character is happening only on server 
        {
            _networkedCharacter.UpdateAnimationParameters = !_networkedCharacter.UpdateAnimationParameters;
        }
    }




    public override bool Moving
    {
        get { return _networkedCharacter.HasSpawned ? _networkedCharacter.Moving : base.Moving;  }
        set
        {
            if (m_Moving != value)
            {
                m_Moving = value;
                if (runner.IsServer)
                {
                    _networkedCharacter.Moving = value;
                }
            }
        }
    }


    protected override void AwakeInternal()
    {
        runner = FindObjectOfType<NetworkRunner>();
        _networkedCharacter = GetComponent<FusionUltimateCharacterNetworkBehaviour>();
        base.AwakeInternal();

    }

    protected override void UpdatePosition()
    {
        var frictionValue = 1f;
        // The collider may be destroyed before the grounded check runs again.
        if (m_Grounded && m_GroundedRaycastHit.collider != null)
        {
            frictionValue = (1 - Mathf.Clamp01(MathUtility.FrictionValue(m_CharacterGroundedCollider.material, m_GroundedRaycastHit.collider.material, true)));
        }
        if (UsingRootMotionPosition)
        {
            var localDeltaPosition = this.InverseTransformDirection(m_RootMotionDeltaPosition);
            localDeltaPosition.x *= m_SlopeFactor * frictionValue;
            localDeltaPosition.z *= m_SlopeFactor * frictionValue;
            //m_MotorThrottle = this.TransformDirection(localDeltaPosition) / Time.deltaTime;
            m_MotorThrottle = this.TransformDirection(localDeltaPosition) / runner.DeltaTime;
        }
        else
        {
            // Dampen motor forces.
            m_MotorThrottle /= (1 + m_MotorDamping * m_TimeScale * Time.timeScale);

            // Apply a multiplier if the character is moving backwards.
            var backwardsMultiplier = 1f;
            if (InputVector.y < 0)
            {
                backwardsMultiplier *= Mathf.Lerp(1, m_MotorBackwardsMultiplier, Mathf.Abs(InputVector.y));
            }
            // As the character changes rotation the same local motor throttle force should be applied. This is most apparent when the character is being aligned to the ground
            // and the local y direction changes.
            var prevLocalMotorThrottle = MathUtility.InverseTransformDirection(m_MotorThrottle, m_PrevMotorRotation) * m_PreviousAccelerationInfluence;
            var rotation = Quaternion.Slerp(m_PrevMotorRotation, m_Rotation, m_PreviousAccelerationInfluence);
            var acceleration = m_SlopeFactor * backwardsMultiplier * m_MotorAcceleration * m_TimeScale * Time.timeScale;
            // Convert input into motor forces. Normalize the input vector to prevent the diagonal from moving faster.
            var normalizedInputVector = InputVector.normalized * Mathf.Max(Mathf.Abs(InputVector.x), Mathf.Abs(InputVector.y));
            m_MotorThrottle = MathUtility.TransformDirection(new Vector3(prevLocalMotorThrottle.x + normalizedInputVector.x * acceleration.x,
                                        prevLocalMotorThrottle.y, prevLocalMotorThrottle.z + normalizedInputVector.y * acceleration.z), rotation) * frictionValue;
        }
        m_PrevMotorRotation = m_Rotation;


        for (int i = 0; i < m_ActiveAbilityCount; ++i)
        {
            m_ActiveAbilities[i].UpdatePosition();
        }
        for (int i = 0; i < m_ActiveItemAbilityCount; ++i)
        {
            m_ActiveItemAbilities[i].UpdatePosition();
        }
    }

    protected override void UpdateDesiredMovement()
    {
        //m_DesiredMovement += (m_MotorThrottle + m_ExternalForce) * Time.deltaTime;
        m_DesiredMovement += (m_MotorThrottle + m_ExternalForce) * runner.DeltaTime;

        if (!m_Grounded && UsingGravity)
        {
            //m_GravityAccumulation += m_GravityAmount * m_TimeScale * Time.timeScale * Time.deltaTime;
            m_GravityAccumulation += m_GravityAmount * m_TimeScale * Time.timeScale * runner.DeltaTime;
            m_DesiredMovement += m_GravityDirection * (m_GravityAccumulation * m_TimeScale * Time.timeScale);
        }

        for (int i = 0; i < m_ActiveAbilityCount; ++i)
        {
            m_ActiveAbilities[i].UpdateDesiredMovement();
        }
        for (int i = 0; i < m_ActiveItemAbilityCount; ++i)
        {
            m_ActiveItemAbilities[i].UpdateDesiredMovement();
        }
    }

    protected override void DetectGroundCollision(bool sendGroundedEvents = true)
    {
        if (!UsingGroundCollisionDetection)
        {
            return;
        }

        m_GroundedRaycastHit = m_EmptyRaycastHit;
        m_CharacterGroundedCollider = null;

        var grounded = false;
        var localDesiredMovement = MathUtility.InverseTransformDirection(m_DesiredMovement - m_MovingPlatformMovement, m_Rotation);
        var localMovingPlatformMovement = MathUtility.InverseTransformDirection(m_MovingPlatformMovement, m_Rotation);
        // The target position should be above the current position to account for slopes.
        var castOffset = m_Radius + m_MaxStepHeight + (m_SlopedGround ? localDesiredMovement.magnitude : 0) + Mathf.Abs(localMovingPlatformMovement.y) + m_VerticalCastBuffer;
        var targetPosition = m_Position + MathUtility.TransformDirection(
                            new Vector3(localDesiredMovement.x, (localDesiredMovement.y < 0 ? 0 : localDesiredMovement.y) + castOffset, localDesiredMovement.z),
                            m_Rotation) + m_MovingPlatformMovement;
        var stickyGround = StickingToGround && m_Grounded;
        var hitCount = CombinedCast(targetPosition, m_GravityDirection,
                                    (stickyGround ? m_StickToGroundDistance : 0) +
                                    Mathf.Abs(localDesiredMovement.y) + m_SkinWidth + castOffset + c_ColliderSpacing);
        if (hitCount > 0)
        {
            for (int i = 0; i < hitCount; ++i)
            {
                var closestRaycastHit = QuickSelect.SmallestK(m_CombinedCastResults, hitCount, i, m_CastHitComparer);
                var activeCollider = m_ColliderCount > 1 ? m_Colliders[m_ColliderIndexMap[closestRaycastHit]] : m_Colliders[m_ColliderIndex];
                if (closestRaycastHit.distance == 0)
                {
                    if (ResolvePenetrations(activeCollider, targetPosition - m_Position, out var offset) && closestRaycastHit.collider.Raycast(new Ray(targetPosition + m_Up * castOffset, m_GravityDirection), out var hit, Mathf.Infinity) &&
                                (closestRaycastHit.distance == 0 || MathUtility.InverseTransformPoint(m_Position + m_MovingPlatformMovement, m_Rotation * m_MovingPlatformRotation, closestRaycastHit.point).y > m_MaxStepHeight))
                    {
                        var colliderIndex = 0;
                        if (m_ColliderCount > 1)
                        {
                            colliderIndex = m_ColliderIndexMap[closestRaycastHit];
                        }
                        closestRaycastHit = hit;
                        closestRaycastHit.distance = MathUtility.InverseTransformDirection(targetPosition - hit.point, m_Rotation).y;

                        // The raycast result may already exist if there are multiple m_CombinedCastResults with a 0 distance.
                        if (m_ColliderCount > 1 && !m_ColliderIndexMap.ContainsKey(closestRaycastHit))
                        {
                            m_ColliderIndexMap.Add(closestRaycastHit, colliderIndex);
                        }
                    }
                    else
                    {
                        continue;
                    }
                }

                if (this.InverseTransformPoint(closestRaycastHit.point).y > m_Radius + m_MaxStepHeight + (m_SlopedGround ? localDesiredMovement.magnitude : 0) + localMovingPlatformMovement.y + m_VerticalCastBuffer)
                {
                    continue;
                }

                if (closestRaycastHit.rigidbody != null && !closestRaycastHit.rigidbody.isKinematic)
                {
                    var groundPoint = MathUtility.InverseTransformPoint(m_Position, m_Rotation, closestRaycastHit.point - m_MovingPlatformMovement);
                    if (groundPoint.y > m_MaxStepHeight)
                    {
                        continue;
                    }
                }

                // Determine which collider caused the intersection.
                var distance = closestRaycastHit.distance - castOffset;

                // The character is grounded if:
                // - The character should not be force ungrounded OR the desired movement is down.
                // - The character is sticking to the ground or the desired vertical movement is less than the distance to the ground.
                // - The distance to the ground is less than the skin width. If the desired movement is up then the character can still be grounded if the ground is near.
                grounded = !m_ForceUngrounded && (stickyGround || localDesiredMovement.y <= Mathf.Abs(distance) || (closestRaycastHit.transform.gameObject.layer == LayerManager.MovingPlatform && distance < m_SkinWidth)) &&
                                                ((Mathf.Abs(localDesiredMovement.y) + (stickyGround ? (m_StickToGroundDistance + c_ColliderSpacing) : 0) > distance) || distance < m_SkinWidth) &&
                                                (distance + Mathf.Min(localDesiredMovement.y, 0) <= m_SkinWidth || stickyGround);
                if (grounded)
                {
                    m_GravityAccumulation = 0;
                    if (!m_MovingPlatformOverride && m_GroundedRaycastHit.transform != closestRaycastHit.transform)
                    {
                        UpdateMovingPlatformTransform(closestRaycastHit.transform);
                    }
                    m_GroundedRaycastHit = closestRaycastHit;
                    m_CharacterGroundedCollider = activeCollider;

                    if (UsingVerticalCollisionDetection)
                    {
                        var desiredMagnitude = m_DesiredMovement.magnitude;
                        var bouncinessValue = MathUtility.BouncinessValue(activeCollider.material, closestRaycastHit.collider.material);
                        if (bouncinessValue > 0)
                        {
                            //var magnitude = desiredMagnitude * bouncinessValue * m_GroundBounceModifier / Time.deltaTime;
                            var magnitude = desiredMagnitude * bouncinessValue * m_GroundBounceModifier / runner.DeltaTime;
                            AddForce(closestRaycastHit.normal * magnitude, 1, false);
                        }
                        // Do not slide on the ground unless the ground friction material is explicitly set.
                        var frictionValue = (Mathf.Clamp01(MathUtility.FrictionValue(m_CharacterGroundedCollider.material, closestRaycastHit.collider.material, true)));
                        if (frictionValue > 0)
                        {
                            var magnitude = desiredMagnitude * frictionValue * m_GroundFrictionModifier;
                            var direction = Vector3.Cross(Vector3.Cross(closestRaycastHit.normal, -m_Up), closestRaycastHit.normal);
                            AddForce(direction * magnitude, 1, false);
                        }

                        PushRigidbody(closestRaycastHit.rigidbody, closestRaycastHit.point, closestRaycastHit.normal, desiredMagnitude);

                        // The character should be snapped to the ground if the ground is sticky or the character would go through the ground (the distance is negative).
                        localDesiredMovement.y -= (stickyGround || distance < 0 || localDesiredMovement.y - distance < 0) ? (distance + Mathf.Min(localDesiredMovement.y, 0) - c_ColliderSpacing) : 0;
                        m_DesiredMovement = MathUtility.TransformDirection(localDesiredMovement, m_Rotation) + m_MovingPlatformMovement;

                        // Others may be interested in the collision.
                        if (m_OnCollision != null)
                        {
                            m_OnCollision(closestRaycastHit);
                        }
                    }
                }
                break;
            }
        }

        // Ensure there is enough space above the character.
        if (UsingVerticalCollisionDetection && UsingHorizontalCollisionDetection && localDesiredMovement.y > 0)
        {
            for (int i = 0; i < m_ColliderCount; ++i)
            {
                var activeCollider = m_Colliders[i];
                if (!activeCollider.gameObject.activeInHierarchy)
                {
                    continue;
                }
                hitCount = OverlapColliders(activeCollider, activeCollider.transform.position + m_DesiredMovement + m_Up * c_ColliderSpacing, activeCollider.transform.rotation, -c_ColliderSpacing * 2);
                if (hitCount > 0)
                {
                    m_DesiredMovement = Vector3.zero;
                    break;
                }
            }
        }

        if (UpdateGroundState(grounded, sendGroundedEvents))
        {
            if (grounded)
            {
                if (!m_MovingPlatformOverride)
                {
                    UpdateMovingPlatformTransform(m_GroundedRaycastHit.transform);
                }
                // The vertical external force should be cancelled out.
                if (!m_ForceUngrounded)
                {
                    var localExternalForce = this.InverseTransformDirection(m_ExternalForce);
                    if (localExternalForce.y < 0)
                    {
                        localExternalForce.y = 0;
                        m_ExternalForce = this.TransformDirection(localExternalForce);
                    }
                }
            }
            else
            {
                m_ForceUngrounded = false;

                if (m_MovingPlatform != null && !m_MovingPlatformOverride)
                {
                    UpdateMovingPlatformTransform(null);
                }
            }
        }
        m_SlopedGround = false;
        UpdateSlopeFactor();
    }

    protected override void ApplyPosition()
    {
        //m_Velocity = (m_DesiredMovement - m_MovingPlatformMovement) / Time.deltaTime;
        m_Velocity = (m_DesiredMovement - m_MovingPlatformMovement) / runner.DeltaTime;
        m_Position += m_DesiredMovement;

        if (m_MovingPlatform != null)
        {
            m_MovingPlatformRelativePosition = m_MovingPlatform.InverseTransformPoint(m_Position);
        }

        // Cancel out the vertical external force if the character changes directions.
        if (LocalDesiredMovement.y > 0)
        {
            var localExternalForce = m_Transform.InverseTransformDirection(m_ExternalForce);
            if (localExternalForce.y < 0)
            {
                localExternalForce.y = 0;
                m_ExternalForce = m_Transform.TransformDirection(localExternalForce);
            }
        }

        m_LastDesiredMovement = m_DesiredMovement;
        m_DesiredMovement = m_RootMotionDeltaPosition = Vector3.zero;
    }
}
