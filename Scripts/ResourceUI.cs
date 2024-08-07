using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourceUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _resourceText;

    private List<Base> _allResourceBases = new List<Base>();

    public void AddBase(Base resourceBase)
    {
        string newLine = $"{resourceBase.name}: {resourceBase.Resources}\n";
        _resourceText.text += newLine;
        resourceBase.ResourceChanged += (amount) => UpdateResourceText();
    }

    public void AddBaseToList(Base resourceBase)
    {
        _allResourceBases.Add(resourceBase);
        AddBase(resourceBase);
    }

    private void UpdateResourceText()
    {
        _resourceText.text = string.Empty;

        foreach (var resourceBase in _allResourceBases)
        {
            string newLine = $"{resourceBase.name}: {resourceBase.Resources}\n";
            _resourceText.text += newLine;
        }
    }

}