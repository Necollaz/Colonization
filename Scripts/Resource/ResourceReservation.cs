using System.Collections.Generic;

public class ResourceReservation
{
    private HashSet<Resource> _reservedResources;

    public ResourceReservation()
    {
        _reservedResources = new HashSet<Resource>();
    }

    public bool IsReserved(Resource resource)
    {
        return _reservedResources.Contains(resource);
    }

    public void Reserve(Resource resource)
    {
        _reservedResources.Add(resource);
    }

    public void Release(Resource resource)
    {
        _reservedResources.Remove(resource);
    }
}
