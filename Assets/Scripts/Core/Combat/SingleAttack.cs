using UnityEngine;
using SLOTC.Core.Stats;
using SLOTC.Core.GlobalSetting;
using System.ComponentModel;

namespace SLOTC.Core.Combat
{
    [CreateAssetMenu(fileName = "New Attack", menuName = "Combat/Single Attack")]
    public class SingleAttack : Attack
    {
        [Space(10)]
        [Header("Animation Settings")]
        [field: SerializeField] string _animName;

        [ReadOnly(true), SerializeField] int _animNameHash;

        public int AnimNameHash { get { return _animNameHash; } }

        private void OnValidate()
        {
            _animNameHash = Animator.StringToHash(_animName);
        }
    }
}