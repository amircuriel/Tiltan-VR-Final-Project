using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ClayLauncherButton : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private XRBaseInteractable _interactable;
    [SerializeField] private Transform _spawnPoint;

    [Header("Clay")]
    [SerializeField] private GameObject _clayPrefab;
    [SerializeField] private float _launchForce = 12f;
    [SerializeField] private float _launchUpForce = 2f;

    [Header("Cooldown")]
    [SerializeField] private float _cooldown = 0.5f;

    private float _lastLaunchTime;

    private void Reset()
    {
        _interactable = GetComponent<XRBaseInteractable>();
    }

    private void OnEnable()
    {
        if (_interactable != null)
            _interactable.selectEntered.AddListener(OnPressed);
    }

    private void OnDisable()
    {
        if (_interactable != null)
            _interactable.selectEntered.RemoveListener(OnPressed);
    }

    private void OnPressed(SelectEnterEventArgs args)
    {
        LaunchClay();
    }

    private void LaunchClay()
    {
        if (_clayPrefab == null || _spawnPoint == null) return;
        if (Time.time < _lastLaunchTime + _cooldown) return;
        _lastLaunchTime = Time.time;

        GameObject clay = Instantiate(_clayPrefab, _spawnPoint.position, _spawnPoint.rotation);

        Rigidbody rb = clay.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 force = _spawnPoint.forward * _launchForce + _spawnPoint.up * _launchUpForce;
            rb.AddForce(force, ForceMode.VelocityChange);
        }
    }
}