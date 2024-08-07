using System.Collections.Concurrent;
using System.Collections.Generic;

public class ResourceReservation
{
    private static ConcurrentDictionary<int, Resource> _reservedResources = new ConcurrentDictionary<int, Resource>();

    public bool IsReserved(Resource resource)
    {
        return _reservedResources.ContainsKey(resource.GetInstanceID());
    }

    public bool Reserve(Resource resource)
    {
        return _reservedResources.TryAdd(resource.GetInstanceID(), resource);
    }

    public bool Release(Resource resource)
    {
        return _reservedResources.TryRemove(resource.GetInstanceID(), out _);
    }

    public IEnumerable<Resource> GetReservedResources()
    {
        return _reservedResources.Values;
    }
}