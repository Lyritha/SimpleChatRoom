using SpacetimeDB;

namespace StdbModule
{
    public static partial class Module
    {
        [Reducer]
        public static void SendMessage(ReducerContext ctx, MessageData data)
        {
            if (string.IsNullOrWhiteSpace(data.Text))
            {
                throw new ArgumentException("Messages must not be empty or just whitespace");
            }

            Log.Info($"{ctx.Sender} sent message: {data.Text}");
            ctx.Db.Message.Insert(
                new MessageTable
                {
                    Sender = ctx.Sender,
                    Text = data.Text,
                    ChannelID = data.ChannelID,
                    Sent = ctx.Timestamp,
                }
            );
        }

        [Reducer]
        public static void UpdateMessage(ReducerContext ctx, MessageData data)
        {
            if (string.IsNullOrWhiteSpace(data.Text))
            {
                throw new ArgumentException("Messages must not be empty or just whitespace");
            }

            MessageTable? message = ctx.Db.Message.MessageId.Find(data.MessageId);

            if (message != null)
            {
                message.Text = data.Text;
                message.HasBeenEdited = true;

                Log.Info($"{ctx.Sender} updated message: {data.Text}");
                ctx.Db.Message.MessageId.Update(message);
            }
        }
    }
}
