using SpacetimeDB.Types;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserItemSetter : MonoBehaviour
{
    [SerializeField]
    private TMP_Text usernameField;

    public void SetData(UserTable user)
    {
        usernameField.text = user.Name;

        usernameField.color = user.Online ? Color.white : new(0.388f, 0.388f, 0.388f);
    }
}
