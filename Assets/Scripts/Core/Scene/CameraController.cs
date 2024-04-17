using UnityEngine;

namespace SLOTC.Core.Scene
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] Transform _target;
        [SerializeField] float _followSpeed;
        [SerializeField] float _rotateSpeed = 5.0f;

        private void Start()
        {
            //Cursor.visible = false;
            //Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
        }

        private void LateUpdate()
        {
            Vector3 temp = transform.position;
            transform.position = Vector3.Lerp(temp, _target.position, _followSpeed * Time.deltaTime);

            transform.Rotate(0.0f, Input.GetAxisRaw("Mouse X") * _rotateSpeed * Time.deltaTime, 0.0f);
        }

        //public object CaptureState()
        //{
        //    SerializableVector[] sv = new SerializableVector[3];
        //    sv[0] = new SerializableVector(transform.position);
        //    sv[1] = new SerializableVector(transform.eulerAngles);
        //    return sv;
        //}
        //
        //public void RestoreState(object state)
        //{
        //    SerializableVector[] sv = (SerializableVector[])state;
        //
        //    transform.position = sv[0].ToVector3();
        //    transform.eulerAngles = sv[1].ToVector3();
        //}
    }
}