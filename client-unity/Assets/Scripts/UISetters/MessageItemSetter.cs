using SpacetimeDB.Types;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class MessageItemSetter : MonoBehaviour
{
    [SerializeField]
    private TMP_Text usernameField;
    [SerializeField]
    private TMP_Text timestampField;
    [SerializeField]
    private TMP_Text textField;
    [SerializeField]
    private TMP_InputField textInputField;
    [SerializeField]
    private Toggle editToggle;
    [SerializeField]
    private TMP_Text editedText;

    public MessageTable ItemData {  get; private set; }

    private void Start()
    {
        textInputField.onSubmit.AddListener(text =>
        {
            ProcessMessageInput(text);
        });
    }


    public void SetData(MessageTable data)
    {
        var user = NetworkManager.Instance.Connection.Db.User.Identity.Find(data.Sender);
        ItemData = data;

        usernameField.text = user.Name;
        if (ColorUtility.TryParseHtmlString(user.Color, out Color color))
        {
            usernameField.color = color;
        }

        timestampField.text = TimestampFormatter.Format(data.Sent);

        string cleaned = Regex.Replace(data.Text, @"[\u2800-\u28FF]", "");
        bool isEmptyString = string.IsNullOrWhiteSpace(cleaned);
        textField.text = isEmptyString ? "Message contained invalid characters" : cleaned;

        editedText.gameObject.SetActive(data.HasBeenEdited);
        editToggle.gameObject.SetActive(data.Sender == NetworkManager.Instance.LocalIdentity);
    }

    public void UpdateData()
    {
        SetData(ItemData);
    }

    public void EditMessageToggle()
    {
        textInputField.text = textField.text;
        textInputField.gameObject.SetActive(!textInputField.gameObject.activeInHierarchy);

        if (textInputField.gameObject.activeInHierarchy)
        {
            textInputField.ActivateInputField();

            // Force the UI to update first
            Canvas.ForceUpdateCanvases();

            // Set cursor at the end without selecting the text
            int textLength = textInputField.text.Length;
            textInputField.caretPosition = textLength;
            textInputField.selectionAnchorPosition = textLength;
            textInputField.selectionFocusPosition = textLength;
        }

        textField.gameObject.SetActive(!textField.gameObject.activeInHierarchy);
    }

    public void ProcessMessageInput(string input)
    {
        string text = input.Trim();

        if (!string.IsNullOrWhiteSpace(text))
        {
            MessageData data = new(text, "", ItemData.MessageId);
            InputManager.Instance.EnqueueCommand("UpdateMessage", data);

            textInputField.text = "";
        }

        editToggle.isOn = false;
        textInputField.gameObject.SetActive(false);
        textField.gameObject.SetActive(true);
    }
}
