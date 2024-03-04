using System.Collections;
using System.Collections.Generic;
using Opsive.UltimateCharacterController.Character;
using Opsive.UltimateCharacterController.Utility;
using UnityEngine;

public class FusionAnimatorMonitor : AnimatorMonitor
{

    FusionUltimateCharacterNetworkBehaviour _networkedCharacter;

    protected override void UpdateAnimatorParameters()
    {
        //SetHorizontalMovementParameter(m_CharacterLocomotion.InputVector.x, m_CharacterLocomotion.TimeScale, m_HorizontalMovementDampingTime);
        //SetForwardMovementParameter(m_CharacterLocomotion.InputVector.y, m_CharacterLocomotion.TimeScale, m_ForwardMovementDampingTime);

        SetHorizontalMovementParameter(_networkedCharacter.InputVector.x, m_CharacterLocomotion.TimeScale, m_HorizontalMovementDampingTime);
        SetForwardMovementParameter(_networkedCharacter.InputVector.y, m_CharacterLocomotion.TimeScale, m_ForwardMovementDampingTime);

        if (m_CharacterLocomotion.LookSource != null)
        {
            SetPitchParameter(m_CharacterLocomotion.LookSource.Pitch, m_CharacterLocomotion.TimeScale, m_PitchDampingTime);
        }
        float yawAngle;
        if (m_CharacterLocomotion.UsingRootMotionRotation)
        {
            yawAngle = MathUtility.ClampInnerAngle(m_CharacterLocomotion.DeltaRotation.y);
        }
        else
        {
            yawAngle = MathUtility.ClampInnerAngle((m_CharacterLocomotion.LastDesiredRotation * Quaternion.Inverse(m_CharacterLocomotion.MovingPlatformRotation)).eulerAngles.y);
        }
        SetYawParameter(yawAngle * m_YawMultiplier, m_CharacterLocomotion.TimeScale, m_YawDampingTime);
        if (!m_SpeedParameterOverride)
        {
            //SetSpeedParameter(m_CharacterLocomotion.Moving ? m_MovingSpeedParameterValue : 0, m_CharacterLocomotion.TimeScale);
            SetSpeedParameter(_networkedCharacter.Moving ? m_MovingSpeedParameterValue : 0, m_CharacterLocomotion.TimeScale);
        }

        UpdateDirtyAbilityAnimatorParameters();
        UpdateItemIDParameters();
    }
}
