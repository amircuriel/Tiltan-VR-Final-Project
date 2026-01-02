using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PhysicsHandProxy : MonoBehaviour
{
    [SerializeField] private Transform _target; // tracked controller or palm
    [SerializeField] private float _positionFollowSpeed = 40f;
    [SerializeField] private float _rotationFollowSpeed = 40f;

    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.isKinematic = true;
        _rb.useGravity = false;
    }

    private void FixedUpdate()
    {
        if (!_target) return;

        Vector3 newPos = Vector3.Lerp(_rb.position, _target.position, Time.fixedDeltaTime * _positionFollowSpeed);
        Quaternion newRot = Quaternion.Slerp(_rb.rotation, _target.rotation, Time.fixedDeltaTime * _rotationFollowSpeed);

        _rb.MovePosition(newPos);
        _rb.MoveRotation(newRot);
    }
}
