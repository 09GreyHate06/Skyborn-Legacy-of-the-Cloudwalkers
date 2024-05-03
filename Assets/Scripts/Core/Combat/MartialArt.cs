using UnityEngine;
using Animancer;
using SLOTC.Core.Combat.Animation;

using cae = SLOTC.Core.Combat.Animation.CombatAnimationEventNames;
namespace SLOTC.Core.Combat
{
    [CreateAssetMenu(fileName = "New MartialArt", menuName = "Combat/Martial Art")]
    public class MartialArt : ScriptableObject
    {
        [Header("Core Settings")]
        [SerializeField] string _displayName;
        [SerializeField, TextArea(5, 8)] string _description;
        [SerializeField] Attack[] _attackInstances;

        [Space(10)]
        [Header("Animation Settings")]
        [EventNames(cae.ApplyForce + "0", cae.ApplyForce + "1", cae.ApplyForce + "2", cae.ApplyForce + "3", cae.ApplyForce + "4", cae.ApplyForce + "5", cae.ApplyForce + "6", cae.ApplyForce + "7", cae.ApplyForce + "8", cae.ApplyForce + "9",
            cae.ActivateWeapon + "0", cae.ActivateWeapon + "1", cae.ActivateWeapon + "2", cae.ActivateWeapon + "3", cae.ActivateWeapon + "4", cae.ActivateWeapon + "5", cae.ActivateWeapon + "6", cae.ActivateWeapon + "7", cae.ActivateWeapon + "8", cae.ActivateWeapon + "9",
            cae.DeactivateWeapon + "0", cae.DeactivateWeapon + "1", cae.DeactivateWeapon + "2", cae.DeactivateWeapon + "3", cae.DeactivateWeapon + "4", cae.DeactivateWeapon + "5", cae.DeactivateWeapon + "6", cae.DeactivateWeapon + "7", cae.DeactivateWeapon + "8", cae.DeactivateWeapon + "9",
            cae.Exit)]
        [SerializeField] ClipTransition _attackAnim;

        public Attack[] AttackInstances { get { return _attackInstances; } }
        // copy event and add callback to it
        public ClipTransition MartialArtAnim { get { return _attackAnim; } }
    }
}