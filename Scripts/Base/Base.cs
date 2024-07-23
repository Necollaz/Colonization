using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ResourceScanner))]
public class Base : MonoBehaviour
{
    [SerializeField] private List<Bot> _bots;
    [SerializeField] private BotFactory _botFactory;
    [SerializeField] private ResourceType _resourceType;
    [SerializeField] private int _resourceCost = 3;

    private ResourceScanner _scanner;
    private Dictionary<ResourceType, int> _resources;
    private HashSet<Resource> _reservedResources;
    private Flag _flag;

    public event Action OnResourceChanged;

    public Flag FlagInstance { get => _flag; set { _flag = value; }}

    private void Awake()
    {
        _resources = new Dictionary<ResourceType, int>();
        _reservedResources = new HashSet<Resource>();
        _scanner = GetComponent<ResourceScanner>();
        _scanner.ResourceFound += Found;
    }

    public Dictionary<ResourceType, int> GetResources() => new Dictionary<ResourceType, int>(_resources);

    public void Add(ResourceType resourceType, int amount)
    {
        if (_resources.ContainsKey(resourceType))
            _resources[resourceType] += amount;
        else
            _resources[resourceType] = amount;

        OnResourceChanged?.Invoke();
        TryCreateBot();
    }

    public void ReleaseResource(Resource resource)
    {
        _reservedResources.Remove(resource);
    }

    public void TryAssign(Bot bot)
    {
        foreach (var resource in _scanner.GetAvailableResources())
        {
            if (!_reservedResources.Contains(resource))
            {
                Assign(bot, resource);
                break;  
            }
        }
    }

    private void Found(Resource resource)
    {
        if (_reservedResources.Contains(resource)) return;

        foreach (var bot in _bots)
        {
            if (!bot.IsBusy)
            {
                bot.SetTarget(resource);
                _reservedResources.Add(resource);
                break;
            }
        }
    }

    private void Assign(Bot bot, Resource resource)
    {
        bot.SetTarget(resource);
        _reservedResources.Add(resource);
    }

    private void TryCreateBot()
    {
        if (!_resources.ContainsKey(_resourceType) || _resources[_resourceType] < _resourceCost)
            return;

        _resources[_resourceType] -= _resourceCost;
        OnResourceChanged?.Invoke();

        Bot newBot = _botFactory.CreateBot(transform);
        _bots.Add(newBot);
    }
}