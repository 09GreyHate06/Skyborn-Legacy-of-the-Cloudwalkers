using UnityEngine;
using Animancer;
using SLOTC.Core.Combat.Animation;

namespace SLOTC.Core.Combat
{
    [CreateAssetMenu(fileName = "New Attack", menuName = "Combat/Single Attack")]
    public class SingleAttack : Attack
    {
        [Space(10)]
        [Header("Animation Settings")]
        [EventNames(CombatAnimationEventNames.ApplyForce, CombatAnimationEventNames.ActivateWeapon, CombatAnimationEventNames.DeactivateWeapon, CombatAnimationEventNames.Exit)]
        [SerializeField] ClipTransition _attackAnim;

        // copy event and add callback to it
        public ClipTransition AttackAnim { get { return _attackAnim; } }
    }
}