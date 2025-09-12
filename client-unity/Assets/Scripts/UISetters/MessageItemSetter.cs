using SpacetimeDB.Types;
using TMPro;
using UnityEngine;

public class MessageItemSetter : MonoBehaviour
{
    [SerializeField]
    private TMP_Text usernameField;
    [SerializeField]
    private TMP_Text timestampField;
    [SerializeField]
    private TMP_Text textField;

    private Message itemData;


    public void SetData(Message data)
    {
        var user = NetworkManager.Instance.Connection.Db.User.Identity.Find(data.Sender);
        itemData = data;

        usernameField.text = user.Settings.Name;
        if (ColorUtility.TryParseHtmlString(user.Settings.Color, out Color color))
        {
            usernameField.color = color;
        }

        timestampField.text = TimestampFormatter.Format(data.Sent);
        textField.text = data.Text;
    }

    public void UpdateData()
    {
        SetData(itemData);
    }
}
