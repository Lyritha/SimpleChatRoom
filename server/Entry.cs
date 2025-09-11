using SpacetimeDB;

namespace StdbModule
{
    public static partial class Module
    {
        [Reducer(ReducerKind.ClientConnected)]
        public static void ClientConnected(ReducerContext ctx)
        {
            User? user = ctx.Db.user.Identity.Find(ctx.Sender);

            if (user is null)
            {
                Log.Info("Creating new user...");

                user = new User
                {
                    Identity = ctx.Sender,
                    Settings = new UserSettings(name: "", color: "#FFFFFF")
                };

                ctx.Db.user.Insert(user);
            }

            user.Online = true;
            Log.Info($"Client connected: {user}");
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