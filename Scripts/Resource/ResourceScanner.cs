using System;
using System.Collections;
using UnityEngine;

public class ResourceScanner : MonoBehaviour
{
    [SerializeField] private ParticleSystem _particleEffect;
    [SerializeField] private LayerMask _resourceLayerMask;
    [SerializeField] private float _scanRadius;
    [SerializeField] private float _delay;

    private ResourceReservation _resourceReservation;

    public event Action<Resource> ResourceFound;

    private void Start()
    {
        _resourceReservation = new ResourceReservation();
        StartCoroutine(Scan());
    }

    public void CopySettings(ResourceScanner other)
    {
        _particleEffect = other._particleEffect;
        _particleEffect.transform.SetParent(transform);
        _particleEffect.transform.localPosition = Vector3.zero;
        _resourceLayerMask = other._resourceLayerMask;
        _scanRadius = other._scanRadius;
        _delay = other._delay;
    }

    private IEnumerator Scan()
    {
        WaitForSeconds wait = new WaitForSeconds(_delay);

        while (true)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, _scanRadius, _resourceLayerMask);

            foreach (Collider hit in hits)
            {
                if(hit.TryGetComponent(out Resource resource) && !_resourceReservation.IsReserved(resource))
                {
                    ResourceFound?.Invoke(resource);
                    _particleEffect.Play();
                }
            }

            yield return wait;
        }
    }
}