using UnityEngine;

[CreateAssetMenu(fileName = "NewResourceType", menuName = "ResourceType")]
public class ResourceType: ScriptableObject
{
    public string ResourceName;
    public GameObject Prefab;
}