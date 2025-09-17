using SpacetimeDB;
using SpacetimeDB.Types;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    private Dictionary<ulong, MessageItemSetter> itemsByMessageId = new();
    private Dictionary<Identity, List<MessageItemSetter>> itemsBySender = new();

    protected override void OnConnectedToDB(DbConnection connection, Identity identity)
    {
        StartCoroutine(InstantiateSpaced(connection));

        connection.Db.Message.OnInsert += CreateItem;
        connection.Db.Message.OnUpdate += UpdateItem;
        connection.Db.User.OnUpdate += UpdateUsernameInMessage;
    }

    private IEnumerator InstantiateSpaced(DbConnection connection)
    {
        var messages = connection.Db.Message.Iter().OrderBy(m => m.MessageId).ToList();
        int index = 0;

        while (index < messages.Count)
        {
            CreateItem(null, messages[index]);
            index++;
            yield return new WaitForEndOfFrame();
        }

        Debug.Log("All messages instantiated without dropping below 60 FPS");
    }


    protected override void OnDisconnectedToDB(DbConnection connection)
    {
        connection.Db.Message.OnInsert -= CreateItem;
        connection.Db.Message.OnUpdate -= UpdateItem;
        connection.Db.User.OnUpdate -= UpdateUsernameInMessage;

        RunOnMainThread(() =>
        {
            foreach (MessageItemSetter message in itemsByMessageId.Values)
            {
                Destroy(message.gameObject);
            }

            itemsByMessageId.Clear();
            itemsBySender.Clear();
        });
    }

    private void CreateItem(EventContext ctx = null, MessageTable message = null)
    {
        RunOnMainThread(() =>
        {
            bool doScroll = scrollRect.verticalNormalizedPosition < 0.1f;

            MessageItemSetter messageItem = Instantiate(prefab);
            messageItem.gameObject.transform.SetParent(container.transform, false);
            messageItem.SetData(message);

            // Add to MessageId dictionary, for index based lookup
            itemsByMessageId[message.MessageId] = messageItem;

            // Add to Sender dictionary, create new list if one doesn't exist
            Identity senderId = message.Sender;
            if (!itemsBySender.TryGetValue(senderId, out var list))
            {
                list = new List<MessageItemSetter>();
                itemsBySender[senderId] = list;
            }
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
            if (itemsByMessageId.TryGetValue(newValue.MessageId, out MessageItemSetter messageItem))
                messageItem.SetData(newValue);
        });
    }

    private void UpdateUsernameInMessage(EventContext context, UserTable oldRow, UserTable newRow)
    {
        RunOnMainThread(() =>
        {
            if (itemsBySender.TryGetValue(newRow.Identity, out List<MessageItemSetter> messages))
                foreach (MessageItemSetter item in messages)
                    item.UpdateData();
        });
    }
}
