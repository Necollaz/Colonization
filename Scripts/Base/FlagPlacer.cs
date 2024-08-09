using UnityEngine;

public class FlagPlacer : MonoBehaviour
{
    [SerializeField] private Flag _prefab;

    private Base _base;

    private void Update()
    {
        HandleClick();
    }

    private void SelectBase(Base baseComponent)
    {
        _base = baseComponent;
    }

    private void HandleClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.TryGetComponent(out Base baseComponent))
                {
                    SelectBase(baseComponent);
                }
                else if (_base != null && hit.collider.TryGetComponent(out Ground ground))
                {
                    Place(hit, ground);
                }
            }
        }
    }

    private void Place(RaycastHit hit, Ground ground)
    {
        Vector3 hitPoint = hit.point;

        if (ground.TryGetComponent(out Collider collider) && collider.bounds.Contains(hitPoint))
        {
            if (_base.FlagInstance == null)
            {
                _base.FlagInstance = Instantiate(_prefab, hitPoint, Quaternion.identity);
            }
            else
            {
                _base.FlagInstance.SetPosition(hitPoint);
            }

            _base = null;
        }
    }
}