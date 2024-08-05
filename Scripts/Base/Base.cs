using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ResourceScanner))]
public class Base : MonoBehaviour
{
    [SerializeField] private List<Bot> _bots;
    [SerializeField] private BotFactory _botFactory;
    [SerializeField] private int _resourceCost = 3;

    private ResourceScanner _scanner;
    private int _resources = 0;
    private int _resourcesRequired = 5;
    private HashSet<Resource> _reservedResources = new HashSet<Resource>();

    public event Action ResourceChanged;

    public Flag FlagInstance { get; set; }

    private void Awake()
    {
        _scanner = GetComponent<ResourceScanner>();
        _scanner.ResourceFound += Found;
    }

    public int GetResources() => _resources;

    public void Add(int amount)
    {
        _resources += amount;

        ResourceChanged?.Invoke();

        if (FlagInstance != null)
        {
            if(_resources >= _resourcesRequired)
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
        _reservedResources.Remove(resource);
    }

    public void TryAssign(Bot bot)
    {
        foreach (Resource resource in _reservedResources)
        {
            if (!_reservedResources.Contains(resource))
            {
                Assign(bot, resource);
                break;  
            }
        }
    }

    private void Assign(Bot bot, Resource resource)
    {
        bot.SetTarget(resource);
        _reservedResources.Add(resource);
    }

    private void Found(Resource resource)
    {
        if (_reservedResources.Contains(resource)) return;

        foreach (Bot bot in _bots)
        {
            if (!bot.IsBusy)
            {
                Assign(bot, resource);
                break;
            }
        }
    }

    private void TryCreateBot()
    {
        if (_resources < _resourceCost) return;

        _resources -= _resourceCost;
        ResourceChanged?.Invoke();

        Bot newBot = _botFactory.CreateBot(transform);
        _bots.Add(newBot);
    }

    private void TrySendBot()
    {
        if(_resources >= _resourcesRequired && FlagInstance != null)
        {
            foreach (Bot bot in _bots)
            {
                if (!bot.IsBusy)
                {
                    bot.SetFlagTarget(FlagInstance.transform);
                    _resources -= _resourcesRequired;
                    ResourceChanged?.Invoke();
                    FlagInstance = null;
                    return;
                }
            }
        }
    }
}