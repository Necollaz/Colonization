using System.Collections.Generic;
using UnityEngine;

public class ResourcePool : MonoBehaviour
{
    [SerializeField] private Resource _resource;
    [SerializeField] private int _initialPoolSize = 10;

    private Queue<Resource> _pool;

    private void Awake()
    {
        InitializePool();
    }

    public Resource Get()
    {
        if (_pool.Count > 0)
        {
            Resource resource = _pool.Dequeue();
            resource.gameObject.SetActive(true);
            return resource;
        }
        else
        {
            return CreateInstance();
        }
    }

    public void Return(Resource resource)
    {
        resource.gameObject.SetActive(false);
        _pool.Enqueue(resource);
    }

    private void InitializePool()
    {
        _pool = new Queue<Resource>();

        for (int i = 0; i < _initialPoolSize; i++)
        {
            Resource resource = CreateInstance();
            resource.gameObject.SetActive(false);
            _pool.Enqueue(resource);
        }
    }

    private Resource CreateInstance()
    {
        Resource instance = Instantiate(_resource, transform);
        instance.Set(instance);
        instance.transform.SetParent(transform);
        return instance;
    }
}