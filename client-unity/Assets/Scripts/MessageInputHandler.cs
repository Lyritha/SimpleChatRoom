using SpacetimeDB.Types;
using System.Text.RegularExpressions;
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
            string cleaned = Regex.Replace(text, @"[\u2800-\u28FF]", "");
            MessageData data = new(cleaned, "", ulong.MaxValue);
            InputManager.Instance.EnqueueCommand("SendMessage", data);

            inputField.text = "";
        }

        inputField.ActivateInputField();
    }
}
