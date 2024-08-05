using UnityEngine;

public class BotFactory : MonoBehaviour
{
    [SerializeField] private Bot _botPrefab;

    public Bot CreateBot(Transform spawnPoint)
    {
        return Instantiate(_botPrefab, spawnPoint.position, spawnPoint.rotation);
    }
}
