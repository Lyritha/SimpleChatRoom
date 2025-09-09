using SpacetimeDB;

namespace StdbModule
{
    public static partial class Module
    {
        [Reducer(ReducerKind.ClientConnected)]
        public static void ClientConnected(ReducerContext ctx)
        {
            Log.Info($"Connect {ctx.Sender}");
            var user = ctx.Db.user.Identity.Find(ctx.Sender);

            if (user is null)
            {
                user = new User
                {
                    Identity = ctx.Sender,
                    Settings = new UserSettings(),
                };

                ctx.Db.user.Insert(user);
            }

            user.Online = true;
            ctx.Db.user.Identity.Update(user);
        }


        [Reducer(ReducerKind.ClientDisconnected)]
        public static void ClientDisconnected(ReducerContext ctx)
        {
            var user = ctx.Db.user.Identity.Find(ctx.Sender);

            if (user is not null)
            {
                user.Online = false;
                ctx.Db.user.Identity.Update(user);
            }
            else
            {
                // User does not exist, log warning
                Log.Warn("Warning: No user found for disconnected client.");
            }
        }
    }
}