using SpacetimeDB;
using SpacetimeDB.Types;
using UnityEngine;

public class FirstConnect : NetworkedMonobehavior
{
    [SerializeField]
    private RectTransform settingsMenu;

    protected override void OnConnectedToDB(DbConnection connection)
    {
        Identity identity = NetworkManager.Instance.LocalIdentity;
        DbConnection conn = NetworkManager.Instance.Connection;
        UserTable user = conn.Db.User.Identity.Find(identity);

        if (string.IsNullOrEmpty(user.Settings.Name))
        {
            settingsMenu.gameObject.SetActive(true);
        }
    }

    protected override void OnDisconnectedToDB(DbConnection connection)
    {
    }
}
