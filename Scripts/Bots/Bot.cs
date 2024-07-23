using System;
using UnityEngine;

[RequireComponent(typeof(BotMovement), typeof(BotPicker))]
public class Bot : MonoBehaviour
{
    [SerializeField] private Base _base;
    [SerializeField] private ResourcePool _resourcePool;

    private BotMovement _botMovement;
    private BotPicker _botPicker;
    private Resource _resource;

    public bool IsBusy { get; private set; }

    private void Awake()
    {
        _botMovement = GetComponent<BotMovement>();
        _botPicker = GetComponent<BotPicker>();
        _botPicker.Discovered += SetTarget;
    }

    public void SetTarget(Resource resource)
    {
        if (!IsBusy && resource != null)
        {
            IsBusy = true;
            _resource = resource;
            _botMovement.SetTarget(resource.transform);
            _botMovement.OnReachTarget += PickUpResource;
        }
    }

    private void PickUpResource()
    {
        if (_resource != null && Vector3.Distance(transform.position, _resource.transform.position) < 0.1f)
        {
            _resource.transform.SetParent(transform);
            _resource.transform.localPosition = Vector3.zero;
            _botMovement.OnReachTarget -= PickUpResource;
            _botMovement.SetTarget(_base.transform);
            _botMovement.OnReachTarget += ReturnBase;
        }
    }

    private void ReturnBase()
    {
        if (_resource != null)
        {
            _resource.transform.SetParent(null);
            _base.Add(_resource.GetResourceType(), _resource.GetAmount());
            IsBusy = false;
            _base.ReleaseResource(_resource);
            _resourcePool.Return(_resource);
            _resource.gameObject.SetActive(false);
            _botMovement.OnReachTarget -= ReturnBase;
            _base.TryAssign(this);
        }
    }
}