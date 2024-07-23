using System.Collections.Generic;
using UnityEngine;

public class ResourcePool : MonoBehaviour
{
    [SerializeField] private ResourceType[] _resourceTypes;
    [SerializeField] private int _initialPoolSize = 10;

    private Dictionary<ResourceType, Queue<Resource>> _pools;

    private void Awake()
    {
        InitializePools();
    }

    public Resource Get(ResourceType resourceType)
    {
        if (_pools.TryGetValue(resourceType, out Queue<Resource> pool) && pool.Count > 0)
        {
            Resource resource = pool.Dequeue();
            resource.gameObject.SetActive(true);
            return resource;
        }
        else
        {
            return CreateInstance(resourceType);
        }
    }

    public ResourceType GetRandomResourceType() => _resourceTypes[Random.Range(0, _resourceTypes.Length)];

    public void Release(Resource resource)
    {
        Return(resource);
    }

    public void Return(Resource resource)
    {
        ResourceType resourceType = resource.GetResourceType();

        if (_pools.TryGetValue(resourceType, out Queue<Resource> pool))
        {
            resource.gameObject.SetActive(false);
            pool.Enqueue(resource);
        }
    }

    private void InitializePools()
    {
        _pools = new Dictionary<ResourceType, Queue<Resource>>();

        foreach (var resourceType in _resourceTypes)
        {
            Queue<Resource> pool = new Queue<Resource>();

            for (int i = 0; i < _initialPoolSize; i++)
            {
                Resource resource = CreateInstance(resourceType);
                resource.gameObject.SetActive(false);
                pool.Enqueue(resource);
            }

            _pools[resourceType] = pool;
        }
    }

    private Resource CreateInstance(ResourceType resourceType)
    {
        var instance = Instantiate(resourceType.Prefab, transform);

        if (instance.TryGetComponent(out Resource resource))
        {
            resource.Set(resourceType);
            resource.transform.SetParent(transform);
            return resource;
        }
        else
        {
            Destroy(instance);
            return null;
        }
    }
}