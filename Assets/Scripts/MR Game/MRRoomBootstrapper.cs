using Meta.XR.MRUtilityKit;
using UnityEngine;
using UnityEngine.Events;

public class MRRoomBootstrapper : MonoBehaviour
{
    [SerializeField] private EffectMesh _effectMesh;
    [SerializeField] private PhysicsMaterial _environmentPhysicMaterial;
    [SerializeField] private GameObject _testRoom;
    
    public UnityEvent RoomReady;

    public MRUKRoom Room { get; private set; }
    
    private void Awake()
    {
        if (_testRoom != null)
            _testRoom.SetActive(false);
    }

    private void Start()
    {
        if (MRUK.Instance == null)
        {
            Debug.LogError("MRUK.Instance not found. Put the MRUK prefab in the scene.");
            return;
        }

        MRUK.Instance.SceneLoadedEvent.AddListener(OnSceneLoaded);
    }

    private void OnDestroy()
    {
        if (MRUK.Instance != null)
            MRUK.Instance.SceneLoadedEvent.RemoveListener(OnSceneLoaded);
    }

    private void OnSceneLoaded()
    {
        Room = MRUK.Instance.GetCurrentRoom();
        if (Room == null)
        {
            Debug.LogError("Room loaded event fired but current room is null.");
            return;
        }

        if (_effectMesh != null)
        {
            var labels =
                MRUKAnchor.SceneLabels.FLOOR |
                MRUKAnchor.SceneLabels.WALL_FACE |
                MRUKAnchor.SceneLabels.TABLE;

            var filter = new LabelFilter(labels);
            _effectMesh.AddColliders(filter);

            ApplyPhysicMaterialToEffectMesh(_effectMesh.transform);
        }

        RoomReady?.Invoke();
    }

    private void ApplyPhysicMaterialToEffectMesh(Transform root)
    {
        if (_environmentPhysicMaterial == null) return;

        foreach (var col in root.GetComponentsInChildren<Collider>(true))
        {
            col.sharedMaterial = _environmentPhysicMaterial;
        }
    }
}