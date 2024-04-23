using UnityEngine;

namespace SLOTC.Core.Physics
{
    public interface IForceReceiver
    {
        void AddForce(Vector3 force);
    }
}