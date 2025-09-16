using SpacetimeDB.Types;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserItemSetter : MonoBehaviour
{
    [SerializeField]
    private Image onlineImage;
    [SerializeField]
    private TMP_Text usernameField;

    public void SetData(UserTable user)
    {
        onlineImage.color = user.Online ? Color.green : Color.red;
        usernameField.text = user.Settings.Name;

        if (ColorUtility.TryParseHtmlString(user.Settings.Color, out Color color))
        {
            //usernameField.color = color;
        }
    }
}
