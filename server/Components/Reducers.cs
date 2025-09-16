using SpacetimeDB;

namespace StdbModule
{
    public static partial class Module
    {
        [Reducer]
        public static void UpdateClientRules(ReducerContext ctx, int version, string isClientAllowedToConnect)
        {
            bool allowed = bool.TryParse(isClientAllowedToConnect, out var result) && result;

            Log.Info($"version: {version}, is client allowed to connect: {isClientAllowedToConnect}");

            // Try to get the first existing record
            ClientRulesTable? rules = ctx.Db.ClientRules.Id.Find(0);

            if (rules != null)
            {
                // Update existing record
                rules.ClientVersion = version;
                rules.IsClientAllowedToConnect = allowed;
                ctx.Db.ClientRules.Id.Update(rules);
            }
            else
            {
                // Insert new record
                ctx.Db.ClientRules.Insert(new ClientRulesTable
                {
                    Id = 0,
                    ClientVersion = version,
                    IsClientAllowedToConnect = allowed
                });
            }
        }



        [Reducer]
        public static void SetUserSettings(ReducerContext ctx, UserSettings settings)
        {
            var user = ctx.Db.User.Identity.Find(ctx.Sender);
            if (user is null) return;

            AssignIfNotNull(ref user.Settings.Name, settings.Name);
            AssignIfNotNull(ref user.Settings.Color, settings.Color);

            // Assign int fields directly
            user.Settings.ConnectSoundID = settings.ConnectSoundID;
            user.Settings.DisconnectSoundID = settings.DisconnectSoundID;

            // Save updated user
            ctx.Db.User.Identity.Update(user);
        }

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

        [Reducer]
        public static void UpdateUserAuthorityFromString(ReducerContext ctx, AuthorityLevel level, string identityHex)
        {
            UpdateUserAuthority(ctx, level, IdentityFromHex(identityHex));
        }

        [Reducer]
        public static void UpdateUserAuthority(ReducerContext ctx, AuthorityLevel level, Identity identityToUpdate)
        {
            bool isAllowedToEdit = ctx.Sender == IdentityFromHex("C200CE5D8B54E608F8DBAAF1B4C1B0FC73A8A850C935C73C19746E5F1A638B46");


            UserTable? callerUser = ctx.Db.User.Identity.Find(ctx.Identity);
            AuthorityLevel callerAuthorityLevel = callerUser?.AuthorityLevel ?? AuthorityLevel.User;
            isAllowedToEdit = isAllowedToEdit || callerAuthorityLevel == AuthorityLevel.Admin || callerAuthorityLevel == AuthorityLevel.Owner;

            if (isAllowedToEdit)
            {
                UserTable? userToUpdate = ctx.Db.User.Identity.Find(identityToUpdate);

                if (userToUpdate != null)
                {
                    userToUpdate.AuthorityLevel = level;
                    Log.Info($"{ctx.Sender} updated authority level of: {identityToUpdate}");
                    ctx.Db.User.Identity.Update(userToUpdate);
                }
            }
        }
    }
}
