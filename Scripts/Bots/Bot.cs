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

    public void Initialize(Base baseInstance)
    {
        _base = baseInstance;
    }

    public void SetTarget(Resource resource)
    {
        if (IsBusy || resource == null) return;

        IsBusy = true;
        _resource = resource;
        _botMovement.SetTarget(resource.transform);
        _botMovement.TargetReached += PickUpResource;
    }

    public void SetFlagTarget(Transform flagTransform)
    {
        if (flagTransform == null) return;

        StopCurrentTask();
        IsBusy = true;
        _flagTarget = flagTransform;
        _botMovement.SetTarget(flagTransform);
        _botMovement.TargetReached += CreateNewBase;
    }

    private void StopCurrentTask()
    {
        _botMovement.Stop();
        IsBusy = false;

        if (_resource != null)
        {
            _resource.transform.SetParent(null);
            _base.Release(_resource);
            _resource = null;
        }
    }

    private void PickUpResource()
    {
        if (_resource == null && Vector3.Distance(transform.position, _resource.transform.position) >= _pickupRange) return;

        _resource.transform.SetParent(transform);
        _resource.transform.localPosition = Vector3.zero;
        _botMovement.TargetReached -= PickUpResource;
        _botMovement.SetTarget(_base.transform);
        _botMovement.TargetReached += ReturnBase;
    }

    private void ReturnBase()
    {
        if (_resource == null) return;

        _resource.transform.SetParent(null);
        _base.Add(_resource.GetAmount());
        _base.Release(_resource);
        _resourcePool.Return(_resource);
        IsBusy = false;
        _resource.gameObject.SetActive(false);
        _botMovement.TargetReached -= ReturnBase;
        _base.TryAssign(this);
    }

    private void CreateNewBase()
    {
        _botMovement.TargetReached -= CreateNewBase;
        Base newBase = Instantiate(_base, _flagTarget.position, Quaternion.identity);

        if (newBase.TryGetComponent(out ResourceScanner newScanner) &&
            _base.TryGetComponent(out ResourceScanner currentScanner))
        {
            newScanner.CopySettings(currentScanner);
        }

        _flagTarget.gameObject.SetActive(false);
        newBase.FlagInstance = null;
        StopCurrentTask();
        newBase.ClearBots();
        newBase.RegisterBot(this);
        _base.UnregisterBot(this);
        _base = newBase;
        Initialize(newBase);
    }
}