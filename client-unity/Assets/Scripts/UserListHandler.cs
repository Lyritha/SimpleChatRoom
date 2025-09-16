using SpacetimeDB;
using SpacetimeDB.Types;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UserListHandler : NetworkedMonobehavior
{
    [SerializeField]
    private RectTransform onlineContainer;
    [SerializeField]
    private RectTransform offlineContainer;
    [SerializeField]
    private UserItemSetter prefab;
    [SerializeField]
    private Dictionary<Identity, UserItemSetter> items = new();

    protected override void OnConnectedToDB(DbConnection connection, Identity identity)
    {
        foreach (UserTable user in connection.Db.User.Iter().OrderBy(m => m.Identity))
        {
            if (!string.IsNullOrEmpty(user.Name))
            {
                CreateItem(null, user);
            }
        }

        connection.Db.User.OnInsert += CreateItem;
        connection.Db.User.OnUpdate += UpdateItem;
    }

    protected override void OnDisconnectedToDB(DbConnection connection)
    {
        connection.Db.User.OnInsert -= CreateItem;
        connection.Db.User.OnUpdate -= UpdateItem;
    }


    private void CreateItem(EventContext ctx = null, UserTable user = null)
    {
        RunOnMainThread(() =>
        {
            UserItemSetter userItem = Instantiate(prefab);

            RectTransform parent = user.Online ? onlineContainer : offlineContainer;
            userItem.gameObject.transform.SetParent(parent.transform, false);

            items[user.Identity] = userItem;
            userItem.SetData(user);
        });
    }

    private void UpdateItem(EventContext ctx, UserTable oldValue, UserTable newValue)
    {
        RunOnMainThread(() =>
        {
            UserItemSetter item = items[newValue.Identity];

            RectTransform parent = newValue.Online ? onlineContainer : offlineContainer;
            item.gameObject.transform.SetParent(parent.transform, false);

            item.SetData(newValue);
        });
    }
}
