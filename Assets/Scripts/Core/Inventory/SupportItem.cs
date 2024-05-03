using SLOTC.Core.Combat;
using SLOTC.Core.Stats;
using UnityEngine;

namespace SLOTC.Core.Inventory
{
    [CreateAssetMenu(menuName = "Inventory/Support item")]
    public class SupportItem : ActionItem
    {
        [SerializeField] int _hpRegen;
        [SerializeField] int _spRegen;
        [SerializeField] StatusEffect _statusEffect;

        private void OnValidate()
        {
            //Debug.Log(_statusEffect.GetID());
        }

        public override bool Use(Status user)
        {
            user.GetComponent<HitPoint>().Regen(_hpRegen);
            user.GetComponent<SkillPoint>().Regen(_spRegen);

            if (user.TryGetComponent(out StatusAffectable statusAffectable))
                statusAffectable.AddEffect(user, _statusEffect);

            return true;
        }
    }
}