using KrazyKatGames;
using UnityEngine;

public class EnterBowLocomotion : StateMachineBehaviour
{
    private PlayerManager _playerManager;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_playerManager == null)
            _playerManager = animator.GetComponent<PlayerManager>();

        _playerManager.applyRootMotion = false;
        _playerManager.isPerformingAction = false;
        _playerManager.canMove = true;
        _playerManager.playerCombatManager.isAiming = true;
    }
}