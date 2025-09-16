using SpacetimeDB;

namespace StdbModule
{
    public static partial class Module
    {
        [Reducer]
        public static void RegisterUser(ReducerContext ctx, UserSettings settings)
        {
            UserTable? user = ctx.Db.User.Identity.Find(ctx.Sender);
            if (user is null)
            {
                user = new UserTable
                {
                    Identity = ctx.Sender,
                    Online = true,
                    AuthorityLevel = AuthorityLevel.User,
                    Name = settings.Name,
                    Color = settings.Color,
                    ConnectSoundID = settings.ConnectSoundID,
                    DisconnectSoundID = settings.DisconnectSoundID
                };

                ctx.Db.User.Insert(user);
            }
            else
            {
                Log.Info($"user {ctx.Identity} has already been registered");
            }
        }

        [Reducer]
        public static void UpdateUser(ReducerContext ctx, UserSettings settings)
        {
            UserTable? user = ctx.Db.User.Identity.Find(ctx.Sender);
            if (user is null) return;

            AssignIfNotNull(ref user.Name, settings.Name);
            AssignIfNotNull(ref user.Color, settings.Color);

            // Assign int fields directly
            user.ConnectSoundID = settings.ConnectSoundID;
            user.DisconnectSoundID = settings.DisconnectSoundID;

            // Save updated user
            ctx.Db.User.Identity.Update(user);
        }

        [Reducer]
        public static void UpdateUserAuthorityDashboard(ReducerContext ctx, string level, string identityHex)
        {
            if (!Enum.TryParse(level, true, out AuthorityLevel parsedLevel))
            {
                parsedLevel = AuthorityLevel.User;
            }

            UpdateUserAuthority(ctx, parsedLevel, IdentityFromHex(identityHex));
        }

        [Reducer]
        public static void UpdateUserAuthority(ReducerContext ctx, AuthorityLevel level, Identity identityToUpdate)
        {
            if (!IsAllowedToUseReducer(ctx, AuthorityLevel.Admin, "UpdateUserAuthority")) return;

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
