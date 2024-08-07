using System;
using UnityEngine;

public class BotPicker : MonoBehaviour
{
    public event Action<Resource> ResourcePicked;

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out Resource resource))
        {
            ResourcePicked?.Invoke(resource);
        }
    }
}