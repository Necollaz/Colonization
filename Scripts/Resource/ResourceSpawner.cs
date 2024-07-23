using System.Collections;
using UnityEngine;

public class ResourceSpawner : MonoBehaviour
{
    [SerializeField] private float _delay;
    [SerializeField] private float _radius;
    [SerializeField] private int _amount;
    [SerializeField] private ResourcePool _pool;

    private void Start()
    {
        StartCoroutine(Spawn());
    }

    private IEnumerator Spawn()
    {
        WaitForSeconds wait = new WaitForSeconds(_delay);

        for (int i = 0; i < _amount; i++)
        {
            Create();
            yield return wait;
        }
    }

    private Resource Create()
    {
        ResourceType resourceType = _pool.GetRandomResourceType();
        Resource resource = _pool.Get(resourceType);

        resource.transform.position = GetRandomPosition();
        resource.transform.rotation = GetRandomRotation();
        return resource;
    }

    private Vector3 GetRandomPosition()
    {
        float minYPosition = 0;
        Vector3 randomPosition = transform.position + new Vector3(Random.Range(-_radius, _radius), 0, Random.Range(-_radius, _radius));
        randomPosition.y = Mathf.Max(randomPosition.y, minYPosition);
        return randomPosition;
    }

    private Quaternion GetRandomRotation()
    {
        return Quaternion.Euler(0, Random.Range(0f, 360f), 0);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
}