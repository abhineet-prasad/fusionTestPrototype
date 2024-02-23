using System;
using Opsive.Shared.Utility;
using Opsive.UltimateCharacterController.Character;
using Opsive.UltimateCharacterController.Character.Abilities;
using Opsive.UltimateCharacterController.Character.Abilities.AI;
using Opsive.UltimateCharacterController.Character.MovementTypes;
using UnityEngine;

namespace Runtime.Gameplay.Controllers.MovementTypes
{
    public class ThirdPersonAutoNavigationMovementType : MovementType
    {
        [Tooltip("The minimum distance required in order to move.")] [SerializeField]
        protected float minDistance = 1f;

        [Tooltip(
            "The speed of the character as they move towards the target. The SpeedChange ability must be added to the character.")]
        [SerializeField]
        protected MinMaxFloat moveSpeed = new(1f, 2f);

        [Tooltip(
            "The character will run towards the destination when the squared distance is greater than the specified value.")]
        [SerializeField]
        protected float runMaxSquaredDistance = 140;

        public override bool FirstPersonPerspective => false;
        private PathfindingMovement _pathfindingMovement;
        private MoveTowards _moveTowards;
        private SpeedChange _speedChange;
        private float _minPointClickDistanceSqr;

        private const float Tolerance = 0.001f;

        public override void Initialize(UltimateCharacterLocomotion characterLocomotion)
        {
            base.Initialize(characterLocomotion);
            _pathfindingMovement = characterLocomotion.GetAbility<PathfindingMovement>();
            if (_pathfindingMovement == null)
            {
                Debug.LogError("Error: The Point Click Movement Type requires a PathfindingMovement ability.");
                return;
            }

            _moveTowards = characterLocomotion.GetAbility<MoveTowards>();
            if (_moveTowards == null)
            {
                Debug.LogError("Error: The Point Click Movement Type requires the MoveTowards ability.");
                return;
            }

            _speedChange = characterLocomotion.GetAbility<SpeedChange>();
            _minPointClickDistanceSqr = minDistance * minDistance;
        }

        public override float GetDeltaYawRotation(float characterHorizontalMovement, float characterForwardMovement,
            float cameraHorizontalMovement, float cameraVerticalMovement)
        {
            return 0;
        }

        public void NavigateTowards(Vector3 location)
        {
            // Do not allow movement if the location is close to the character.
            var sqrDistance = Vector3.SqrMagnitude(m_CharacterLocomotion.Position - location);
            if (sqrDistance >= _minPointClickDistanceSqr)
            {
                _moveTowards.MoveTowardsLocation(location);
            }
        }

        public override Vector2 GetInputVector(Vector2 inputVector)
        {
            // If a SpeedChange ability exists then the character can move at a faster speed further away from the target.
            if (_moveTowards.IsActive && _speedChange != null &&
                Math.Abs(moveSpeed.MinValue - moveSpeed.MaxValue) > Tolerance)
            {
                var distance = (_pathfindingMovement.GetDestination() - m_CharacterLocomotion.Position).sqrMagnitude;
                _speedChange.MaxSpeedChangeValue = _speedChange.SpeedChangeMultiplier = Mathf.Lerp(moveSpeed.MinValue,
                    moveSpeed.MaxValue, Mathf.Clamp01(distance / runMaxSquaredDistance));
                _speedChange.MinSpeedChangeValue = -_speedChange.MaxSpeedChangeValue;
            }

            return Vector2.zero;
        }
    }
}