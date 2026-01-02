using UnityEngine;

public class TeleportTree : MonoBehaviour, IHittable
{
    [SerializeField] private Transform teleportPosition;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnHit()
    {
        PlayerManager.Instance.Teleport(teleportPosition);
    }
}
