using System.Collections;
using UnityEngine;

public class SkeetWaveSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _clayPrefab;
    [SerializeField] private AudioSource _audioSource;

    [Header("Wave")]
    [SerializeField] private int _claysPerWave = 4;
    [SerializeField] private float _timeBetweenClays = 0.6f;
    [SerializeField] private float _waveCooldown = 1.0f;

    [Header("Throw")] [SerializeField] private float _forwardSpawnOffset = 0.25f;
    [SerializeField] private float _speed = 13f;
    [SerializeField] private float _upSpeed = 3.5f;
    [SerializeField] private float _yawSpreadDeg = 20f;

    private bool _isSpawning;
    private float _lastWaveTime;

    // Called by your button event
    public void LaunchWave()
    {
        if (_clayPrefab == null) return;
        if (WallPortalManager.Instance.Portals.Count == 0) return;
        if (_isSpawning) return;
        if (Time.time < _lastWaveTime + _waveCooldown) return;

        _lastWaveTime = Time.time;
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        _isSpawning = true;

        for (int i = 0; i < _claysPerWave; i++)
        {
            SpawnOne();
            yield return new WaitForSeconds(_timeBetweenClays);
        }

        _isSpawning = false;
    }

    private void SpawnOne()
    {
        var portal = WallPortalManager.Instance.Portals[Random.Range(0, WallPortalManager.Instance.Portals.Count)];
        var clay = Instantiate(_clayPrefab, portal.position + portal.forward * _forwardSpawnOffset, portal.rotation);

        var rb = clay.GetComponent<Rigidbody>();
        if (rb == null) return;

        float yaw = Random.Range(-_yawSpreadDeg, _yawSpreadDeg);
        Vector3 dir = Quaternion.Euler(0f, yaw, 0f) * portal.forward;

        Vector3 vel = dir * _speed + Vector3.up * _upSpeed;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.AddForce(vel, ForceMode.VelocityChange);
        rb.AddTorque(Random.insideUnitSphere * 0.8f, ForceMode.VelocityChange);
        
        ScoreManager.Instance.OnPotShot();
        _audioSource.Play();
    }
}
