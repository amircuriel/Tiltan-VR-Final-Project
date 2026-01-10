using Meta.XR.MRUtilityKit;
using Meta.XR.MRUtilityKit.SceneDecorator;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlaceConsoleOnSurface : SingletonMonoBehaviour<PlaceConsoleOnSurface>
{
    [Header("References")]
    [SerializeField] private MRRoomBootstrapper _bootstrapper;
    [SerializeField] private Transform _aimOrigin; // controller or hand aim transform
    [SerializeField] private VRGun _gun;

    [Header("Prefabs")]
    [SerializeField] private GameObject _ghostPrefab;
    [SerializeField] private GameObject _consolePrefab;

    [Header("Placement")]
    [SerializeField] private float _maxDistance = 4f;
    
    [Header("Input")]
    [SerializeField] private InputActionProperty _placeButtonAction;

    private GameObject _ghost;
    private bool _placed;
    
    public GameObject Console { get; private set;}

    private void OnEnable()
    {
        _bootstrapper.RoomReady.AddListener(OnRoomReady);
        if (_placeButtonAction.action != null)
        {
            _placeButtonAction.action.Enable();
            _placeButtonAction.action.performed += PlaceConsole;
        }
        _gun.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        _bootstrapper.RoomReady.RemoveListener(OnRoomReady);
        if (_placeButtonAction.action != null)
        {
            _placeButtonAction.action.performed -= PlaceConsole;
            _placeButtonAction.action.Disable();
        }
    }

    private void OnRoomReady()
    {
        if (_ghostPrefab != null)
            _ghost = Instantiate(_ghostPrefab);
    }

    private void Update()
    {
        if (_placed || !_bootstrapper.Room || !_ghost || !_aimOrigin)
            return;

        MRUKRoom room = _bootstrapper.Room;
        Ray ray = new Ray(_aimOrigin.position, _aimOrigin.forward);
        Debug.DrawRay(ray.origin, ray.direction * _maxDistance);

        //Try TABLE / COUCH / BED first (non-floor horizontal surfaces)
        var wallFilter = new LabelFilter(MRUKAnchor.SceneLabels.TABLE | MRUKAnchor.SceneLabels.COUCH | MRUKAnchor.SceneLabels.BED);
        bool hit = room.Raycast(ray, _maxDistance, wallFilter, out var hitInfo, out var hitAnchor);

        //Fallback to FLOOR
        if (!hit)
        {
             var floorFilter = new LabelFilter(MRUKAnchor.SceneLabels.FLOOR);
             hit = room.Raycast(ray, _maxDistance, floorFilter, out hitInfo, out hitAnchor);
        }

        if (!hit) return;

        _ghost.transform.position = hitInfo.point;

        //Couldn't make it work in time, sorry
        /*Vector3 up = (hitAnchor) ? hitAnchor.transform.up : Vector3.up;
        Vector3 forward = Vector3.ProjectOnPlane(_aimOrigin.forward, up).normalized;
        if (forward.sqrMagnitude < 0.001f) forward = Vector3.forward;

        _ghost.transform.rotation = Quaternion.LookRotation(forward, up);*/
    }

    private void PlaceConsole(InputAction.CallbackContext obj)
    {
        PlaceConsole();
    }

    private void PlaceConsole()
    {
        if (_bootstrapper.Room.IsPositionInRoom(_ghost.transform.position) == false)
        {
            Debug.LogWarning("Not in room.");
            return;
        }
        Console = Instantiate(_consolePrefab, _ghost.transform.position, _ghost.transform.rotation);
        Destroy(_ghost);
        _placed = true;
        _gun.gameObject.SetActive(true);
        _gun.transform.position = Console.transform.position;
    }
}
