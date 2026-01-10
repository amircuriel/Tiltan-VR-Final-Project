using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class VRGun : MonoBehaviour
{
    [Header("Interactable")]
    [SerializeField] private XRGrabInteractable _interactor;
    
    [Header("Muzzle")]
    [SerializeField] private Transform _muzzle;

    [Header("Laser")]
    [SerializeField] private LineRenderer _line;
    [SerializeField] private float _laserMaxDistance = 60f;
    [SerializeField] private LayerMask _hitMask = ~0;

    [Header("Input")]
    [SerializeField] private InputActionProperty _fireAction;    // trigger
    [SerializeField] private InputActionProperty _reloadAction;  // button
    [SerializeField] private InputActionProperty _summonGunAction;

    [Header("Fire Settings")]
    [SerializeField] private float _fireCooldown = 0.15f;

    [Header("Ammo")]
    [SerializeField] private int _magSize = 6;
    [SerializeField] private float _reloadTime = 1.2f;

    [Header("VFX")]
    [SerializeField] private ParticleSystem _muzzleFlash;
    [SerializeField] private ParticleSystem _hitVFXPrefab;
    [SerializeField] private float _hitVFXLifetime = 1.5f;
    [SerializeField] private Animator _gunAnimator;
    

    [Header("Audio")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _fireClip;
    [SerializeField] private AudioClip _dryFireClip;
    [SerializeField] private AudioClip _reloadClip;

    private float _lastFireTime;
    private bool _isReloading;
    private int _ammo;
    private static readonly int Reload = Animator.StringToHash("Reload");
    
    private bool CanFire => Time.time > _lastFireTime + _fireCooldown && !_isReloading && _interactor.isSelected;

    private void Awake()
    {
        _line.positionCount = 2;
        _ammo = _magSize;
    }

    private void OnEnable()
    {
        if (_fireAction.action != null)
        {
            _fireAction.action.Enable();
            _fireAction.action.performed += OnFire;
        }

        if (_reloadAction.action != null)
        {
            _reloadAction.action.Enable();
            _reloadAction.action.performed += OnReload;
        }
        
        if (_summonGunAction.action != null)
        {
            _summonGunAction.action.Enable();
            _summonGunAction.action.performed += OnSummonGun;
        }
    }

    private void OnDisable()
    {
        if (_fireAction.action != null)
        {
            _fireAction.action.performed -= OnFire;
            _fireAction.action.Disable();
        }

        if (_reloadAction.action != null)
        {
            _reloadAction.action.performed -= OnReload;
            _reloadAction.action.Disable();
        }
        
        if (_summonGunAction.action != null)
        {
            _summonGunAction.action.performed -= OnSummonGun;
            _summonGunAction.action.Disable();
        }
    }

    private void Update()
    {
        UpdateLaser();
    }

    private void UpdateLaser()
    {
        if (!_muzzle) return;
        _line.gameObject.SetActive(CanFire);
        if (!CanFire) return;

        Vector3 start = _muzzle.position;
        Vector3 dir = _muzzle.forward;

        Vector3 end = start + dir * _laserMaxDistance;

        if (Physics.Raycast(start, dir, out RaycastHit hit, _laserMaxDistance, _hitMask, QueryTriggerInteraction.Ignore))
        {
            end = hit.point;
        }

        _line.SetPosition(0, start);
        _line.SetPosition(1, end);
    }

    private void OnFire(InputAction.CallbackContext ctx)
    {
        TryFire();
    }

    private void TryFire()
    {
        if (!_muzzle) return;
        if (!CanFire) return;

        _lastFireTime = Time.time;

        if (_ammo <= 0)
        {
            PlayOneShot(_dryFireClip);
            return;
        }

        _ammo--;
        if (_ammo <= 0)
        {
            OnReload(default);
        }

        if (_muzzleFlash != null) _muzzleFlash.Play();
        PlayOneShot(_fireClip);

        Vector3 start = _muzzle.position;
        Vector3 dir = _muzzle.forward;

        if (Physics.Raycast(start, dir, out RaycastHit hit, _laserMaxDistance, _hitMask, QueryTriggerInteraction.Ignore))
        {
            if (_hitVFXPrefab != null)
            {
                var fx = Instantiate(_hitVFXPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(fx.gameObject, _hitVFXLifetime);
            }

            IHittable target = hit.collider.GetComponentInParent<IHittable>();
            if (target != null)
            {
                target.OnHit();
            }
        }
    }

    private void OnReload(InputAction.CallbackContext ctx)
    {
        if (!_isReloading && _ammo < _magSize)
        {
            StartCoroutine(ReloadRoutine());
        }
    }
    
    private void OnSummonGun(InputAction.CallbackContext ctx)
    {
        if (_interactor.isSelected || !gameObject.activeInHierarchy) return;
        var transform1 = PlayerManager.Instance.transform;
        transform.position = transform1.position + transform1.forward * 0.3f + Vector3.up * 2;
    }

    private System.Collections.IEnumerator ReloadRoutine()
    {
        _isReloading = true;
        yield return new WaitForSeconds(0.5f);
        _gunAnimator.SetTrigger(Reload);
        yield return new WaitForSeconds(0.3f);
        PlayOneShot(_reloadClip);
        yield return new WaitForSeconds(_reloadTime);

        _ammo = _magSize;
        _isReloading = false;
    }

    private void PlayOneShot(AudioClip clip)
    {
        if (_audioSource == null || clip == null) return;
        _audioSource.PlayOneShot(clip);
    }
}
