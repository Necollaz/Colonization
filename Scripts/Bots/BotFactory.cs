using UnityEngine;

public class BotFactory : MonoBehaviour
{
    [SerializeField] private Bot _botPrefab;

    public Bot CreateBot(Transform spawnPoint)
    {
        Bot newBot = Instantiate(_botPrefab, spawnPoint.position, spawnPoint.rotation);
        return newBot;
    }
}
