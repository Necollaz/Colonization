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
    private static ResourceReservation _resourceReservation;

    private int _resourceCollectedForNewBase = 0;
    private int _resourcesRequired = 5;

    public event Action OnResourceChanged;

    public Flag FlagInstance { get; set; }
    private void Awake()
    {
        if (_resourceReservation == null)
        {
            _resourceReservation = new ResourceReservation();
        }

        _resources = new Dictionary<ResourceType, int>();
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

        if (FlagInstance != null)
        {
            _resourceCollectedForNewBase += amount;

            if(_resourceCollectedForNewBase >= _resourcesRequired)
            {
                TrySendBot();
            }
        }
        else
        {
            TryCreateBot();
        }
    }

    public void ReleaseResource(Resource resource)
    {
        _resourceReservation.Release(resource);
    }

    public void TryAssign(Bot bot)
    {
        foreach (var resource in _scanner.GetAvailableResources())
        {
            if (!_resourceReservation.IsReserved(resource))
            {
                Assign(bot, resource);
                break;  
            }
        }
    }

    private void Assign(Bot bot, Resource resource)
    {
        bot.SetTarget(resource);
        _resourceReservation.Reserve(resource);
    }

    private void Found(Resource resource)
    {
        if (_resourceReservation.IsReserved(resource)) return;

        foreach (var bot in _bots)
        {
            if (!bot.IsBusy && !_resourceReservation.IsReserved(resource))
            {
                Assign(bot, resource);
                break;
            }
        }
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

    private void TrySendBot()
    {
        if(_resourceCollectedForNewBase >= _resourcesRequired && FlagInstance != null)
        {
            foreach (var bot in _bots)
            {
                if (!bot.IsBusy)
                {
                    bot.SetFlagTarget(FlagInstance.transform);
                    _resourceCollectedForNewBase = 0;
                    FlagInstance = null;
                    return;
                }
            }
        }
    }
}