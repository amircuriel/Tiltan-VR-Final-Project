using System;
using System.Collections;
using Meta.XR.MRUtilityKit;
using UnityEngine;

public interface IHittable
{
    public void OnHit();
}

public class ClayTarget : MonoBehaviour, IHittable
{
    [Header("Hit FX")]
    [SerializeField] private ParticleSystem _breakVFX;
    [SerializeField] private AudioClip _breakSound;
    [SerializeField] private Collider _collider;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private bool _breakOnFirstHit = true;
    private bool _broken;
    private bool _hitOnce = false;
    private float _lastHitTime;

    private LabelFilter wallFilter = new(MRUKAnchor.SceneLabels.WALL_FACE);

    private void OnEnable()
    {
        _collider.enabled = _meshRenderer.enabled = true;
        _broken = false;
    }

    public void OnHit()
    {
        Break(true);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!_breakOnFirstHit && (!_hitOnce || Time.time - _lastHitTime < 0.2f))
        {
            _hitOnce = true;
            _lastHitTime = Time.time;
            return;
        }
        Break(false);
    }

    [ContextMenu("Break")]
    private void Break(bool fromShot)
    {
        if (_broken) return;
        _broken = true;
        _collider.enabled = _meshRenderer.enabled = _rb.useGravity = false;

        if (_breakVFX != null)
        {
            _breakVFX.Play();
        }

        if (_breakSound != null)
        {
            AudioSource.PlayClipAtPoint(_breakSound, transform.position, 1f);
        }
        
        if (fromShot) ScoreManager.Instance.AddPoint();
        else ScoreManager.Instance.OnPotGone();

        
        Destroy(gameObject, 5f);
    }
}