using SpacetimeDB;
using SpacetimeDB.Types;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;

public class UserInfoSetter : NetworkedMonobehavior
{
    [SerializeField]
    private TMP_Text usernameText;

    protected override void OnConnectedToDB(DbConnection connection, Identity identity)
    {
        connection.Db.User.OnUpdate += DbUpdate;
        connection.Db.User.OnInsert += DbInsert;
    }

    protected override void OnDisconnectedToDB(DbConnection connection)
    {
        connection.Db.User.OnUpdate -= DbUpdate;
        connection.Db.User.OnInsert -= DbInsert;
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        if (isConnected)
        {
            UserTable user = connection.Db.User.Identity.Find(localIdentity);
            if (user != null)
            {
                SetElements(user);
            }
        }
    }

    private void DbInsert(EventContext ctx = null, UserTable user = null)
    {
        if (user.Identity == localIdentity)
        {
            RunOnMainThread(() => SetElements(user));
        }
    }

    private void DbUpdate(EventContext ctx, UserTable oldValue, UserTable newValue)
    {
        if (newValue.Identity == localIdentity)
        {
            RunOnMainThread(() => SetElements(newValue));
        }
    }

    private void SetElements(UserTable user)
    {
        usernameText.text = user.Name;
        if (ColorUtility.TryParseHtmlString(user.Color, out Color color))
        {
            usernameText.color = color;
        }
    }
}
