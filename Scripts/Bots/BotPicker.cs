using System;
using UnityEngine;

public class BotPicker : MonoBehaviour
{
    public event Action<Resource> Discovered;

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out Resource resource))
        {
            Discovered?.Invoke(resource);
        }
    }
}