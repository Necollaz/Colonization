using UnityEngine;

[RequireComponent(typeof(BotMovement), typeof(BotPicker))]
public class Bot : MonoBehaviour
{
    [SerializeField] private Base _base;
    [SerializeField] private ResourcePool _resourcePool;

    private BotMovement _botMovement;
    private BotPicker _botPicker;
    private Resource _resource;
    private Transform _flagTarget;
    private float _pickupRange = 0.1f;

    public bool IsBusy { get; private set; }

    private void Awake()
    {
        _botMovement = GetComponent<BotMovement>();
        _botPicker = GetComponent<BotPicker>();
        _botPicker.ResourcePicked += SetTarget;
    }

    private void OnDestroy()
    {
        _botPicker.ResourcePicked -= SetTarget;
    }

    public void SetTarget(Resource resource)
    {
        if (!IsBusy && resource != null)
        {
            IsBusy = true;
            _resource = resource;
            _botMovement.SetTarget(resource.transform);
            _botMovement.TargetReached += PickUpResource;
        }
    }

    public void SetFlagTarget(Transform flagTransform)
    {
        if(flagTransform != null)
        {
            StopCurrentTask();
            IsBusy = true;
            _flagTarget = flagTransform;
            _botMovement.SetTarget(flagTransform);
            _botMovement.TargetReached += CreateNewBase;
        }
    }

    private void StopCurrentTask()
    {
        _botMovement.Stop();
        _botMovement.TargetReached -= ReturnBase;
        _botMovement.TargetReached -= CreateNewBase;
    }

    private void PickUpResource()
    {
        if (_resource != null && Vector3.Distance(transform.position, _resource.transform.position) < _pickupRange)
        {
            _resource.transform.SetParent(transform);
            _resource.transform.localPosition = Vector3.zero;
            _botMovement.TargetReached -= PickUpResource;
            _botMovement.SetTarget(_base.transform);
            _botMovement.TargetReached += ReturnBase;
        }
    }

    private void ReturnBase()
    {
        if (_resource != null)
        {
            _resource.transform.SetParent(null);
            _base.Add(_resource.GetAmount());
            IsBusy = false;
            _base.ReleaseResource(_resource);
            _resourcePool.Return(_resource);
            _resource.gameObject.SetActive(false);
            _botMovement.TargetReached -= ReturnBase;
            _base.TryAssign(this);
        }
    }

    private void CreateNewBase()
    {
        _botMovement.TargetReached -= CreateNewBase;
        Base newBase = Instantiate(_base, _flagTarget.position, Quaternion.identity);

        if(newBase.TryGetComponent(out ResourceScanner scanner) && _base.TryGetComponent(out ResourceScanner currentScanner))
        {
            scanner.CopySettings(currentScanner);
        }

        _flagTarget.gameObject.SetActive(false);
        newBase.FlagInstance = null;
        newBase.TryAssign(this);
        IsBusy = false;
        _base = newBase;
    }
}