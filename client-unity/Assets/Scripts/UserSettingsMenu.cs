using SpacetimeDB;
using SpacetimeDB.Types;
using System;
using TMPro;
using UnityEngine;

public class UserSettingsMenu : NetworkedMonobehavior
{
    [SerializeField]
    private TMP_InputField usernameField;
    [SerializeField] 
    private TMP_InputField colorHexField;


    protected override void OnConnectedToDB(DbConnection connection)
    {

    }

    protected override void OnDisconnectedToDB(DbConnection connection)
    {

    }

    protected override void OnEnable()
    {
        base.OnEnable();
        if (!isConnected) return;

        Identity identity = NetworkManager.Instance.LocalIdentity;
        DbConnection conn = NetworkManager.Instance.Connection;
        UserTable user = conn.Db.User.Identity.Find(identity);

        usernameField.text = user.Settings.Name;
        colorHexField.text = user.Settings.Color;
    }

    public void SaveSettings()
    {
        string username = usernameField.text;
        string colorHex = colorHexField.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(colorHex))
        {
            Debug.Log("Invalid values");
            return;
        }

        // save settings
        UserSettings settings = new()
        {
            Name = username,
            Color = colorHex
        };

        InputManager.Instance.EnqueueCommand("SetUserSettings", settings);

        gameObject.SetActive(false);
    }
}
