using SpacetimeDB.Types;
using TMPro;
using UnityEngine;

public class MessageInputHandler : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField inputField;

    private void Start()
    {
        inputField.onSubmit.AddListener(text =>
        {
            ProcessMessageInput(text);
        });
    }

    public void ProcessMessageInput(string input)
    {
        string text = input.Trim();

        if (!string.IsNullOrWhiteSpace(text))
        {
            MessageData data = new(text, "", ulong.MaxValue);
            InputManager.Instance.EnqueueCommand("SendMessage", data);

            inputField.text = "";
        }

        inputField.ActivateInputField();
    }
}
