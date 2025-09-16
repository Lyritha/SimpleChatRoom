using SpacetimeDB;

namespace StdbModule
{
    public static partial class Module
    {
        [Reducer(ReducerKind.Init)]
        public static void Init(ReducerContext ctx)
        {
            // Ensure there's always one ClientRulesTable entry with Id = 0
            var rules = ctx.Db.ClientUpdates.ClientVersion.Find(1);
            if (rules is null)
            {
                ctx.Db.ClientUpdates.Insert(new ClientUpdatesTable
                {
                    ClientVersion = 1,
                    MinimumConnectionLevel = AuthorityLevel.User,
                    Reason = "Initial Rules"
                });
            }
        }

        [Reducer(ReducerKind.ClientConnected)]
        public static void ClientConnected(ReducerContext ctx)
        {
            UserTable? user = ctx.Db.User.Identity.Find(ctx.Sender);

            if (user is not null)
            {
                user.Online = true;
                Log.Info($"Client connected: {user}");
                ctx.Db.User.Identity.Update(user);
            }
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
        }
    }
}