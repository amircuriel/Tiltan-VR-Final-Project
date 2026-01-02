using System.Collections;
using UnityEngine;

public class ClayPotLauncher : MonoBehaviour
{
    [Header("Spawn")]
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private GameObject _clayPrefab;
    [SerializeField] private AudioSource _audioSource;

    [Header("Wave Settings")]
    [SerializeField] private int _claysPerWave = 3;
    [SerializeField] private float _timeBetweenClays = 0.6f;
    [SerializeField] private float _waveCooldown = 1.0f;

    [Header("Throw Feel")]
    [SerializeField] private float _speed = 14f;
    [SerializeField] private float _upwardSpeed = 4f;
    [SerializeField] private float _randomYawDegrees = 18f;
    [SerializeField] private float _randomPitchDegrees = 6f;

    [Header("Anti Spam")]
    [SerializeField] private bool _allowWhileRunning = false;

    private bool _isRunning;
    private float _lastWaveTime;

    [ContextMenu("Launch Wave")]
    public void LaunchWave()
    {
        if (_clayPrefab == null || _spawnPoint == null) return;

        if (!_allowWhileRunning && _isRunning) return;

        if (Time.time < _lastWaveTime + _waveCooldown) return;
        _lastWaveTime = Time.time;

        StartCoroutine(LaunchWaveRoutine());
    }

    private IEnumerator LaunchWaveRoutine()
    {
        _isRunning = true;

        for (int i = 0; i < _claysPerWave; i++)
        {
            SpawnAndThrow();
            yield return new WaitForSeconds(_timeBetweenClays);
        }

        _isRunning = false;
    }

    private void SpawnAndThrow()
    {
        GameObject clay = Instantiate(_clayPrefab, _spawnPoint.position, _spawnPoint.rotation);
        Rigidbody rb = clay.GetComponent<Rigidbody>(); //sorry for this lol
        if (rb == null) return;

        float yaw = Random.Range(-_randomYawDegrees, _randomYawDegrees);
        float pitch = Random.Range(-_randomPitchDegrees, _randomPitchDegrees);

        Quaternion spread = Quaternion.Euler(-pitch, yaw, 0f);
        Vector3 dir = spread * _spawnPoint.forward;

        Vector3 velocity = dir * _speed + Vector3.up * _upwardSpeed;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        //Adds linear velocity
        rb.AddForce(velocity, ForceMode.VelocityChange);

        //Adds torque
        rb.AddTorque(Random.insideUnitSphere * 0.8f, ForceMode.VelocityChange);

        ScoreManager.Instance.OnPotShot();
        _audioSource.Play();
    }
}
