using TMPro;
using UnityEngine;

public class ResourceUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Base _base;

    private void OnEnable()
    {
        _base.ResourceChanged += UpdateText;
    }

    private void OnDisable()
    {
        _base.ResourceChanged -= UpdateText;
    }

    private void Start()
    {
        UpdateText();
    }

    private void UpdateText()
    {
        int resources = _base.GetResources();

        _text.text = $"Ресурсы: {resources}\n";
    }
}