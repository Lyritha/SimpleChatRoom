using SpacetimeDB;
using SpacetimeDB.Types;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UserListHandler : NetworkedMonobehavior
{
    [SerializeField]
    private RectTransform container;
    [SerializeField]
    private UserItemSetter prefab;
    [SerializeField]
    private Dictionary<Identity, UserItemSetter> items = new();

    protected override void OnConnectedToDB(DbConnection connection)
    {
        foreach (User user in connection.Db.User.Iter().OrderBy(m => m.Identity))
        {
            if (!string.IsNullOrEmpty(user.Settings.Name))
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


    private void CreateItem(EventContext ctx = null, User user = null)
    {
        RunOnMainThread(() =>
        {
            UserItemSetter userItem = Instantiate(prefab);
            userItem.gameObject.transform.SetParent(container.transform, false);

            items[user.Identity] = userItem;
            userItem.SetData(user);
        });
    }

    private void UpdateItem(EventContext ctx, User oldValue, User newValue)
    {
        if (string.IsNullOrEmpty(oldValue.Settings.Name) && !string.IsNullOrEmpty(newValue.Settings.Name))
        {
            CreateItem(ctx, newValue);
        }

        RunOnMainThread(() =>
        {
            UserItemSetter item = items[newValue.Identity];
            item.SetData(newValue);
        });
    }
}
