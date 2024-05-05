
using UnityEngine;

namespace SLOTC.Core.Scene
{
    public class Destroyer : MonoBehaviour
    {
        [SerializeField] float _lifetime;

        void Start()
        {
            Destroy(gameObject, _lifetime);
        }
    }
}
