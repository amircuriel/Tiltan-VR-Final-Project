using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void Teleport(Transform newTransform)
    {
        transform.position = newTransform.position;
        transform.rotation = newTransform.rotation;
    }
}
