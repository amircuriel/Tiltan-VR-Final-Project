using System;
using System.Collections;
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
    private bool _broken;

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