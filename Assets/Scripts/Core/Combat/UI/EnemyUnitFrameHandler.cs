using UnityEngine;

namespace SLOTC.Core.Combat.UI
{
    public class EnemyUnitFrameHandler : MonoBehaviour
    {
        [SerializeField] TargetLocker _targetLocker;
        [SerializeField] UnitFrameUI _unitFrameUI;

        private void Update()
        {
            if (_targetLocker.HasTarget)
            {
                var target = _targetLocker.CurrentTarget;
                _unitFrameUI.HitPoint = target.GetComponentInParent<HitPoint>();
                _unitFrameUI.SkillPoint = target.GetComponentInParent<SkillPoint>();
                _unitFrameUI.HasLevel = false;
                _unitFrameUI.gameObject.SetActive(true);
            }
            else
            {
                _unitFrameUI.gameObject.SetActive(false);
            }
        }
    }
}