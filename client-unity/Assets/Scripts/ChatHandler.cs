using SpacetimeDB;
using SpacetimeDB.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;

public class ChatHandler : NetworkedMonobehavior
{
    [SerializeField]
    private RectTransform container;
    [SerializeField]
    private ScrollRect scrollRect;
    [SerializeField]
    private MessageItemSetter prefab;
    [SerializeField]
    private Dictionary<Identity, List<MessageItemSetter>> items = new();


    protected override void OnConnectedToDB(DbConnection connection)
    {
        foreach (MessageTable message in connection.Db.Message.Iter().OrderBy(m => m.Sent))
        {
            CreateItem(null, message);
        }

        connection.Db.Message.OnInsert += CreateItem;
        connection.Db.Message.OnUpdate += UpdateItem;
        connection.Db.User.OnUpdate += UpdateUsernameInMessage;
    }

    protected override void OnDisconnectedToDB(DbConnection connection)
    {
        connection.Db.Message.OnInsert -= CreateItem;
        connection.Db.Message.OnUpdate -= UpdateItem;
        connection.Db.User.OnUpdate -= UpdateUsernameInMessage;
    }


    private void CreateItem(EventContext ctx = null, MessageTable message = null)
    {
        RunOnMainThread(() =>
        {
            bool doScroll = scrollRect.verticalNormalizedPosition < 0.1f;

            if (!items.TryGetValue(message.Sender, out List<MessageItemSetter> list))
            {
                list = new List<MessageItemSetter>();
                items[message.Sender] = list;
            }

            MessageItemSetter messageItem = Instantiate(prefab);
            messageItem.gameObject.transform.SetParent(container.transform, false);
            messageItem.SetData(message);

            list.Add(messageItem);

            if (doScroll)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(container);
                scrollRect.verticalNormalizedPosition = 0f;
            }
        });
    }

    private void UpdateItem(EventContext ctx, MessageTable oldValue, MessageTable newValue)
    {
        RunOnMainThread(() =>
        {
            foreach (MessageItemSetter setter in items[newValue.Sender])
                setter.SetData(newValue);
        });
    }

    private void UpdateUsernameInMessage(EventContext context, UserTable oldRow, UserTable newRow)
    {
        RunOnMainThread(() =>
        {
            if (items.TryGetValue(newRow.Identity, out List<MessageItemSetter> setters))
            {
                foreach (MessageItemSetter setter in setters)
                    setter.UpdateData();
            }
        });
    }
}

