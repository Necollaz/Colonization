using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ResourceScanner))]
public class Base : MonoBehaviour
{
    [SerializeField] private List<Bot> _bots = new List<Bot>();
    [SerializeField] private BotFactory _botFactory;
    [SerializeField] private ResourceUI _resourceUI;

    private ResourceScanner _scanner;
    private ResourceReservation _resourceReservation;
    private int _resources = 0;
    private int _resourceCost = 3;
    private int _resourcesRequired = 5;

    public event Action<int> ResourceChanged;

    public int Resources => _resources;
    public Flag FlagInstance { get; set; }

    private void Awake()
    {
        _scanner = GetComponent<ResourceScanner>();
        _resourceReservation = new ResourceReservation();
        _scanner.ResourceFound += Found;

        foreach (var bot in _bots)
        {
            RegisterBot(bot);
        }

        RegisterBaseInUI();
    }

    private void OnDestroy()
    {
        _scanner.ResourceFound -= Found;
    }

    public void RegisterBaseInUI()
    {
        _resourceUI?.AddBase(this);
    }

    public void RegisterBot(Bot bot)
    {
        if (!_bots.Contains(bot))
        {
            _bots.Add(bot);
        }
    }

    public void UnregisterBot(Bot bot)
    {
        _bots.Remove(bot);
    }

    public void ClearBots()
    {
        _bots.Clear();
    }

    public void Add(int amount)
    {
        _resources += amount;
        ResourceChanged?.Invoke(_resources);

        if (FlagInstance != null)
        {
            if (_resources >= _resourcesRequired)
            {
                TrySendBot();
            }
        }
        else
        {
            TryCreateBot();
        }
    }

    public void Release(Resource resource)
    {
        _resourceReservation.Release(resource);
    }

    public void TryAssign(Bot bot)
    {
        foreach (Resource resource in _resourceReservation.GetReservedResources())
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
    }

    private void Found(Resource resource)
    {
        foreach (Bot bot in _bots)
        {
            if (!bot.IsBusy && !_resourceReservation.IsReserved(resource))
            {
                if (_resourceReservation.Reserve(resource))
                {
                    Assign(bot, resource);
                    break;
                }
            }
        }
    }

    private void TryCreateBot()
    {
        if (_resources < _resourceCost) return;

        _resources -= _resourceCost;
        ResourceChanged?.Invoke(_resources);
        Bot newBot = _botFactory.CreateBot(transform, this);
        RegisterBot(newBot);
    }

    private void TrySendBot()
    {
        if (_resources >= _resourcesRequired && FlagInstance != null)
        {
            foreach (Bot bot in _bots)
            {
                if (!bot.IsBusy)
                {
                    bot.SetFlagTarget(FlagInstance.transform);
                    _resources -= _resourcesRequired;
                    ResourceChanged?.Invoke(_resources);
                    FlagInstance = null;
                    return;
                }
            }
        }
    }
}