using Meta.XR.MRUtilityKit;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlaceConsoleOnSurface : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MRRoomBootstrapper _bootstrapper;
    [SerializeField] private Transform _aimOrigin; // controller or hand aim transform

    [Header("Prefabs")]
    [SerializeField] private GameObject _ghostPrefab;
    [SerializeField] private GameObject _consolePrefab;

    [Header("Placement")]
    [SerializeField] private float _maxDistance = 4f;
    [SerializeField] private KeyCode _editorConfirmKey = KeyCode.Space; // for quick editor testing

    private GameObject _ghost;
    private bool _placed;

    private void OnEnable()
    {
        _bootstrapper.RoomReady.AddListener(OnRoomReady);
    }

    private void OnDisable()
    {
        _bootstrapper.RoomReady.RemoveListener(OnRoomReady);
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

        // 1) Try TABLE
        var tableFilter = new LabelFilter(MRUKAnchor.SceneLabels.TABLE);
        bool hit = room.Raycast(ray, _maxDistance, tableFilter, out var hitInfo, out var hitAnchor);

        // 2) Fallback to FLOOR
        if (!hit)
        {
            var floorFilter = new LabelFilter(MRUKAnchor.SceneLabels.FLOOR);
            hit = room.Raycast(ray, _maxDistance, floorFilter, out hitInfo, out hitAnchor);
        }

        if (!hit) return;

        _ghost.transform.position = hitInfo.point;

        // Align to surface. Use the surface normal if you captured it, otherwise anchor transform up.
        Vector3 up = (hitAnchor) ? hitAnchor.transform.up : Vector3.up;
        Vector3 forward = Vector3.ProjectOnPlane(_aimOrigin.forward, up).normalized;
        if (forward.sqrMagnitude < 0.001f) forward = Vector3.forward;

        _ghost.transform.rotation = Quaternion.LookRotation(forward, up);

        // Confirm placement (replace with your OVR input once you want)
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            var console = Instantiate(_consolePrefab, _ghost.transform.position, _ghost.transform.rotation);
            Destroy(_ghost);
            _placed = true;

            // Optional: call a method on console to finish wiring
            console.SendMessage("OnPlaced", SendMessageOptions.DontRequireReceiver);
        }
    }
}
