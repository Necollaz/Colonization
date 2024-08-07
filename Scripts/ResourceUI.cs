using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourceUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _resourceText;

    private List<Base> _allResourceBases = new List<Base>();

    public void AddBase(Base resourceBase)
    {
        _allResourceBases.Add(resourceBase);
        resourceBase.ResourceChanged += (amount) => UpdateResourceText();
        UpdateResourceText();
    }

    private void UpdateResourceText()
    {
        _resourceText.text = string.Empty;

        foreach (var resourceBase in _allResourceBases)
        {
            string newLine = GenerateResourceText(resourceBase);
            _resourceText.text += newLine;
        }
    }

    private string GenerateResourceText(Base resourceBase)
    {
        return $"{resourceBase.name}: Количество ресурсов - {resourceBase.Resources} шт.\n";
    }
}