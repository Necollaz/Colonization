using UnityEngine;

public class Resource : MonoBehaviour 
{
    [SerializeField] private int _amount;

    public int GetAmount() => _amount;

    public void Set(Resource resource)
    {
        resource.gameObject.SetActive(true);
    }
}