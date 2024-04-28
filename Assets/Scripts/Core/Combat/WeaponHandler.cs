using SLOTC.Core.Stats;
using UnityEngine;

namespace SLOTC.Core.Combat
{
    public abstract class WeaponHandler : MonoBehaviour
    {
        [SerializeField] protected Status _user;
        public abstract void Activate(Attack attack);
        public abstract void Deactivate();
    }
}