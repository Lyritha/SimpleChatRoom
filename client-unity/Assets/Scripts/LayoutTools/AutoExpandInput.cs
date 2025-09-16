using UnityEngine;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(LayoutElement))]
public class AutoExpandInput : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private float maxHeight = 375f;
    [SerializeField] private float minHeight = 125f;

    private LayoutElement layoutElement;

    private void Awake()
    {
        layoutElement = GetComponent<LayoutElement>();
        inputField.onValueChanged.AddListener(OnTextChanged);
        UpdateHeight(); // initialize
    }

    private void OnTextChanged(string _)
    {
        UpdateHeight();
    }

    private void UpdateHeight()
    {
        inputField.textComponent.ForceMeshUpdate();

        // Let TMP calculate preferred size with wrapping
        float textWidth = inputField.textComponent.rectTransform.rect.width;
        Vector2 preferredValues = inputField.textComponent.GetPreferredValues(inputField.text, textWidth, Mathf.Infinity);

        // Add viewport padding (difference between text area & text rects)
        float viewportPadding = inputField.textViewport.rect.height - inputField.textComponent.rectTransform.rect.height;

        float preferredHeight = preferredValues.y + viewportPadding;
        preferredHeight = Mathf.Clamp(preferredHeight, minHeight, maxHeight);
        layoutElement.preferredHeight = preferredHeight;
    }
}
