using UnityEngine;

namespace SLOTC.Utils.StateMachine.Behaviour
{
    public abstract class StateBehaviour : MonoBehaviour
    {
        public virtual string GetID()
        {
            return GetType().ToString();
        }

        public virtual void OnEnter()
        {
        }

        public virtual void OnExit()
        {
        }

        public virtual void OnUpdate(float deltaTime)
        {
        }
    }
}