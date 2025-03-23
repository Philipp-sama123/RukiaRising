using UnityEngine;
using UnityEngine.AI;

namespace KrazyKatGames.Enemy
{
    public class AICharacterLocomotionManager : CharacterLocomotionManager
    {
        [Header("Movement Settings")]
        public float pursueSpeed = 1f;
        public float rotationSpeed = 120f;
        public float patrolRadius = 20f;
        public float waypointTolerance = 1f;
        public float patrolSpeed = .5f;

        private NavMeshPath _navPath;
        private int _currentPathIndex;

        private NavMeshAgent _navAgent;

        protected override void Awake()
        {
            base.Awake();
            _navPath = new NavMeshPath();
            _navAgent = GetComponent<NavMeshAgent>();
            if (_navAgent != null)
            {
                _navAgent.updatePosition = false;
                _navAgent.updateRotation = false;
            }
        }

        public void StartPath(Vector3 destination)
        {
            if (_navAgent != null)
            {
                _navAgent.SetDestination(destination);
                _navPath = _navAgent.path;
                _currentPathIndex = 0;
            }
            else if (NavMesh.CalculatePath(character.characterController.transform.position, destination, NavMesh.AllAreas, _navPath))
            {
                _currentPathIndex = 0;
            }
            else
            {
                Debug.LogError("Failed to calculate NavMesh path.");
            }
        }

        public void UpdatePathMovement(float moveSpeed, float rotationSpeed)
        {
            if (_currentPathIndex < _navPath.corners.Length)
            {
                FollowPath(moveSpeed, rotationSpeed);
            }
        }

        private void FollowPath(float moveSpeed, float rotationSpeed)
        {
            if (_currentPathIndex >= _navPath.corners.Length)
            {
                Debug.Log("Path completed.");
                return;
            }

            Vector3 targetPosition = _navPath.corners[_currentPathIndex];

            MoveToAndRotate(targetPosition, moveSpeed, rotationSpeed);

            if (IsAtPosition(character.characterController.transform.position, targetPosition, waypointTolerance))
            {
                _currentPathIndex++;
            }
        }

        public void MoveToAndRotate(Vector3 targetPosition, float moveSpeed, float rotationSpeed)
        {
            Vector3 direction = (targetPosition - character.characterController.transform.position).normalized;
            direction.y = 0;
            Vector3 move = direction * (moveSpeed * Time.deltaTime);
            character.characterController.Move(move);
            if (direction.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                character.characterController.transform.rotation = Quaternion.Slerp(
                    character.characterController.transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );
            }
        }

        public static bool IsAtPosition(Vector3 currentPosition, Vector3 targetPosition, float tolerance)
        {
            return Vector3.Distance(currentPosition, targetPosition) <= tolerance;
        }

        private void OnDrawGizmos()
        {
            if (_navPath == null || _navPath.corners == null || _navPath.corners.Length == 0)
                return;

            Gizmos.color = Color.green;
            for (int i = 0; i < _navPath.corners.Length; i++)
            {
                Gizmos.DrawSphere(_navPath.corners[i], 0.3f);
                if (i < _navPath.corners.Length - 1)
                {
                    Gizmos.DrawLine(_navPath.corners[i], _navPath.corners[i + 1]);
                }
            }
        }
    }
}