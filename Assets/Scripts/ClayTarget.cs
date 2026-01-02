using UnityEngine;

public class ClayTarget : MonoBehaviour
{
    [Header("Hit FX")]
    [SerializeField] private ParticleSystem _breakVFXPrefab;
    [SerializeField] private AudioClip _breakSound;
    [SerializeField] private float _breakVFXLifetime = 2.5f;

    [Header("Auto Cleanup")]
    [SerializeField] private float _maxLifetime = 12f;

    private bool _broken;

    private void Start()
    {
        if (_maxLifetime > 0f)
            Destroy(gameObject, _maxLifetime);
    }

    public void Break()
    {
        if (_broken) return;
        _broken = true;

        if (_breakVFXPrefab != null)
        {
            ParticleSystem fx = Instantiate(_breakVFXPrefab, transform.position, transform.rotation);
            Destroy(fx.gameObject, _breakVFXLifetime);
        }

        if (_breakSound != null)
        {
            AudioSource.PlayClipAtPoint(_breakSound, transform.position, 1f);
        }

        Destroy(gameObject);
    }
}