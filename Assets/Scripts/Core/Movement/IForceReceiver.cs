using UnityEngine;

namespace SLOTC.Core.Movement
{
    public interface IForceReceiver
    {
        void AddForce(Vector3 force);
    }
}