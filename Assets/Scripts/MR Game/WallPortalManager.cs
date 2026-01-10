using System.Collections.Generic;
using Meta.XR.MRUtilityKit;
using Meta.XR.MRUtilityKit.SceneDecorator;
using UnityEngine;

public class WallPortalManager : SingletonMonoBehaviour<WallPortalManager>
{
    [SerializeField] private MRRoomBootstrapper _bootstrapper;
    [SerializeField] private GameObject _portalPrefab;

    [Header("Portal Placement")]
    [SerializeField] private int _portalsToCreate = 2;
    [SerializeField] private float _heightNormalized = 0.6f;
    [SerializeField] private float _sideOffsetNormalized = 0.25f;
    [SerializeField, Range(0f, 0.45f)] private float _edgePadding = 0.15f;

    [SerializeField] private float _portalDistanceFromWall = 0.01f;


    public readonly List<Transform> Portals = new();
    public MRRoomBootstrapper Bootstrapper => _bootstrapper;

    private void OnEnable()
    {
        _bootstrapper.RoomReady.AddListener(BuildPortals);
    }

    private void OnDisable()
    {
        _bootstrapper.RoomReady.RemoveListener(BuildPortals);
    }

    private void BuildPortals()
    {
        var room = _bootstrapper.Room;
        if (room == null || _portalPrefab == null) return;

        // Cleanup old portals
        for (int i = Portals.Count - 1; i >= 0; i--)
        {
            if (Portals[i] != null) Destroy(Portals[i].gameObject);
        }
        Portals.Clear();

        if (room.WallAnchors == null || room.WallAnchors.Count == 0)
        {
            Debug.LogWarning("No WallAnchors found.");
            return;
        }

        // Copy and shuffle walls so we pick random unique walls
        List<MRUKAnchor> walls = new List<MRUKAnchor>(room.WallAnchors);
        Shuffle(walls);

        int count = Mathf.Min(_portalsToCreate, walls.Count);

        for (int i = 0; i < count; i++)
        {
            var wall = walls[i];
            Rect? rectOpt = wall.PlaneRect;
            if (rectOpt == null) continue;

            Rect rect = rectOpt.Value;

            // Random point in wall plane, with padding away from edges
            float x = Mathf.Lerp(rect.xMin, rect.xMax, Random.Range(_edgePadding, 1f - _edgePadding));
            float y = Mathf.Lerp(rect.yMin, rect.yMax, Random.Range(_edgePadding, 1f - _edgePadding));

            Vector3 localPos = new Vector3(x, y, 0f);
            Vector3 worldPos = wall.transform.TransformPoint(localPos);

            // Push portal slightly off the wall so it doesn't z-fight
            Vector3 outDir = wall.transform.forward; // flip if needed
            worldPos += outDir * _portalDistanceFromWall;

            Quaternion rot = Quaternion.LookRotation(outDir, wall.transform.up);

            var portal = Instantiate(_portalPrefab, worldPos, rot, transform);
            Portals.Add(portal.transform);
        }
    }

    private static void Shuffle<T>(IList<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int j = Random.Range(i, list.Count);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
