using SpacetimeDB.Types;
using TMPro;
using UnityEngine;

public class MessageInputHandler : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField inputField;

    public void SendMessage()
    {
        MessageData data = new(inputField.text, "");
        InputManager.Instance.EnqueueCommand("SendMessage", data);

        inputField.text = "";
    }
}
