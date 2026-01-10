using System.Collections;
using Meta.XR.MRUtilityKit;
using UnityEngine;

public class MRRoomColliderConfigurator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MRRoomBootstrapper _bootstrapper;
    [SerializeField] private EffectMesh _effectMesh;

    [Header("Physics")]
    [SerializeField] private PhysicsMaterial _environmentPhysicMaterial;
    [SerializeField] private float _wallThickness = 0.06f;

    private void Awake()
    {
        if (_bootstrapper == null) Debug.LogError("Assign MRRoomBootstrapper.");
        if (_effectMesh == null) Debug.LogError("Assign EffectMesh.");
    }

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
        StartCoroutine(ConfigureRoomNextFrame());
    }

    private IEnumerator ConfigureRoomNextFrame()
    {
        // Let EffectMesh finish creating objects/colliders this frame.
        yield return null;
        yield return new WaitForEndOfFrame();

        var labels = MRUKAnchor.SceneLabels.FLOOR |
                     MRUKAnchor.SceneLabels.WALL_FACE |
                     MRUKAnchor.SceneLabels.TABLE;

        var filter = new LabelFilter(labels);

        // Make sure colliders exist and are enabled.
        _effectMesh.AddColliders(filter);
        _effectMesh.ToggleEffectMeshColliders(true, filter);

        // Wait one more frame in case AddColliders created new components.
        yield return null;

        ReplaceWallMeshCollidersWithBoxes();
        ApplyPhysicMaterialToAllEffectColliders();
    }

    private void ReplaceWallMeshCollidersWithBoxes()
    {
        foreach (var kvp in _effectMesh.EffectMeshObjects)
        {
            MRUKAnchor anchor = kvp.Key;
            var effectObj = kvp.Value;

            if ((anchor.Label & MRUKAnchor.SceneLabels.WALL_FACE) == 0)
                continue;

            // PlaneRect describes the wall dimensions.
            Rect? planeRect = anchor.PlaneRect;
            if (planeRect == null)
                continue;

            var go = effectObj.effectMeshGO;

            // Remove the concave mesh collider for walls.
            var meshCol = go.GetComponent<MeshCollider>();
            if (meshCol != null)
                Destroy(meshCol);

            // Add a thin box collider that is double-sided and stable.
            var box = go.GetComponent<BoxCollider>();
            if (!box)
                box = go.AddComponent<BoxCollider>();

            // PlaneRect is in the anchor's local plane space.
            // Most MRUK wall planes are local X-Y with forward as normal.
            var r = planeRect.Value;

            box.center = new Vector3(r.center.x, r.center.y, 0f);
            box.size = new Vector3(Mathf.Abs(r.width), Mathf.Abs(r.height), _wallThickness);
            box.isTrigger = false;
        }
    }

    private void ApplyPhysicMaterialToAllEffectColliders()
    {
        if (_environmentPhysicMaterial == null) return;

        foreach (var kvp in _effectMesh.EffectMeshObjects)
        {
            var effectObj = kvp.Value;
            var go = effectObj.effectMeshGO;

            foreach (var col in go.GetComponents<Collider>())
            {
                col.sharedMaterial = _environmentPhysicMaterial;
            }
        }
    }
}
