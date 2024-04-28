using SLOTC.Core.Stats;
using UnityEngine;

namespace SLOTC.Core.Combat
{
    [RequireComponent(typeof(Collider))]
    public class Hitbox : MonoBehaviour
    {
        [field: SerializeField] public Status Owner { get; private set; }
    }
}