using System;
using UnityEngine;

public class BotPicker : MonoBehaviour
{
    private Resource _targetResource;

    public event Action<Resource> ResourcePicked;

    public void PickResource(Resource targetResource)
    {
        _targetResource = targetResource;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out Resource resource) && resource == _targetResource)
        {
            ResourcePicked?.Invoke(resource);
            _targetResource = null;
        }
    }
}