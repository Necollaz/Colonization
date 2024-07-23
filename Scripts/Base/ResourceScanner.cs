using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceScanner : MonoBehaviour
{
    [SerializeField] private float _scanRadius;
    [SerializeField] private float _delay;
    [SerializeField] private ParticleSystem _particleEffect;

    private HashSet<Resource> _detectedResources = new HashSet<Resource>();

    public event Action<Resource> ResourceFound;

    private void Start()
    {
        StartCoroutine(Scan());
    }

    public IEnumerable<Resource> GetAvailableResources()
    {
        return _detectedResources.Where(r => r != null && r.gameObject.activeSelf);
    }

    public void RemoveResource(Resource resource)
    {
        if (_detectedResources.Contains(resource))
        {
            _detectedResources.Remove(resource);
        }
    }

    private IEnumerator Scan()
    {
        WaitForSeconds wait = new WaitForSeconds(_delay);

        while (true)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, _scanRadius);

            foreach (Collider hit in hits)
            {
                if(hit.TryGetComponent(out Resource resource) && !_detectedResources.Contains(resource))
                {
                    _detectedResources.Add(resource);
                    ResourceFound?.Invoke(resource);
                    _particleEffect.Play();
                }
            }

            yield return wait;
        }
    }
}