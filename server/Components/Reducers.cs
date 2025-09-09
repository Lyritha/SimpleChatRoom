using SpacetimeDB;

namespace StdbModule
{
    public static partial class Module
    {
        [Reducer]
        public static void SetUserSettings(ReducerContext ctx, UserSettings settings)
        {
            var user = ctx.Db.user.Identity.Find(ctx.Sender);
            if (user is null) return;

            AssignIfNotNull(ref user.Settings.Name, settings.Name);
            AssignIfNotNull(ref user.Settings.Color, settings.Color);

            // Assign int fields directly
            user.Settings.ConnectSoundID = settings.ConnectSoundID;
            user.Settings.DisconnectSoundID = settings.DisconnectSoundID;

            // Save updated user
            ctx.Db.user.Identity.Update(user);
        }

        [Reducer]
        public static void SendMessage(ReducerContext ctx, MessageData data)
        {
            if (string.IsNullOrEmpty(data.Text))
            {
                throw new ArgumentException("Messages must not be empty");
            }

            Log.Info(data.Text);
            ctx.Db.message.Insert(
                new Message
                {
                    Sender = ctx.Sender,
                    Text = data.Text,
                    ChannelID = data.ChannelID,
                    Sent = ctx.Timestamp,
                }
            );
        }
    }
}
