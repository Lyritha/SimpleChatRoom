using SpacetimeDB;

namespace StdbModule
{
    public static partial class Module
    {
        [Reducer(ReducerKind.ClientConnected)]
        public static void ClientConnected(ReducerContext ctx)
        {
            UserTable? user = ctx.Db.User.Identity.Find(ctx.Sender);

            if (user is null)
            {
                Log.Info("Creating new user...");

                user = new UserTable
                {
                    Identity = ctx.Sender,
                    Settings = new UserSettings(name: "", color: "#FFFFFF")
                };

                ctx.Db.User.Insert(user);
            }

            user.Online = true;
            Log.Info($"Client connected: {user}");
            ctx.Db.User.Identity.Update(user);
        }


        [Reducer(ReducerKind.ClientDisconnected)]
        public static void ClientDisconnected(ReducerContext ctx)
        {
            var user = ctx.Db.User.Identity.Find(ctx.Sender);

            if (user is not null)
            {
                user.Online = false;
                ctx.Db.User.Identity.Update(user);
            }
            else
            {
                // User does not exist, log warning
                Log.Warn("Warning: No user found for disconnected client.");
            }
        }
    }
}