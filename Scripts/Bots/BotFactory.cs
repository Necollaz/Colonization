using UnityEngine;

public class BotFactory : MonoBehaviour
{
    [SerializeField] private Bot _botPrefab;

    public Bot CreateBot(Transform spawnPoint, Base baseInstance)
    {
        Bot newBot = Instantiate(_botPrefab, spawnPoint.position, spawnPoint.rotation);
        newBot.Initialize(baseInstance);
        return newBot;
    }
}
