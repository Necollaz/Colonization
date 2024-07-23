using UnityEngine;

public class Resource : MonoBehaviour 
{
    [SerializeField] private ResourceType _resourceType;
    [SerializeField] private int _amount;

    public ResourceType GetResourceType() => _resourceType;

    public int GetAmount() => _amount;

    public void Set(ResourceType resourceType)
    {
        _resourceType = resourceType;
        gameObject.SetActive(true);
    }
}