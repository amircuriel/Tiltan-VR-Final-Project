using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class VRPushButton : MonoBehaviour
{
    [Header("Button Parts")]
    [SerializeField] private Transform _buttonVisual;

    [Header("Movement")]
    [SerializeField] private float _pressDepth = 0.03f;
    [SerializeField] private float _returnSpeed = 12f;

    [Header("Press Threshold")]
    [SerializeField] private float _pressThreshold = 0.9f;

    [Header("Events")]
    public UnityEvent OnPressed;

    private Vector3 _initialLocalPos;
    private float _currentPress;
    private bool _hasFired;

    private void Awake()
    {
        _initialLocalPos = _buttonVisual.localPosition;
    }

    private void Update()
    {
        if (_currentPress <= 0f)
        {
            _buttonVisual.localPosition = Vector3.Lerp(
                _buttonVisual.localPosition,
                _initialLocalPos,
                Time.deltaTime * _returnSpeed
            );

            _hasFired = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        Vector3 localPoint = transform.InverseTransformPoint(other.ClosestPoint(transform.position));
        float pressAmount = Mathf.Clamp01(-localPoint.z / _pressDepth);

        _currentPress = pressAmount;

        _buttonVisual.localPosition =
            _initialLocalPos + Vector3.back * (_pressDepth * pressAmount);

        if (!_hasFired && pressAmount >= _pressThreshold)
        {
            _hasFired = true;
            OnPressed?.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _currentPress = 0f;
    }
}