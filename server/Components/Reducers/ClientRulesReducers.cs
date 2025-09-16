using SpacetimeDB;

namespace StdbModule
{
    public static partial class Module
    {
        [Reducer]
        public static void CreateOrUpdateClientUpdate(ReducerContext ctx, int version, string minimumConnectionLevel, string reason)
        {
            if (!IsAllowedToUseReducer(ctx, AuthorityLevel.Admin, "UpdateClientRules")) return;

            ClientUpdatesTable? rules = ctx.Db.ClientUpdates.ClientVersion.Find(version);

            // Convert string to enum
            if (!Enum.TryParse(minimumConnectionLevel, true, out AuthorityLevel parsedLevel))
            {
                parsedLevel = AuthorityLevel.User;
            }

            if (rules != null)
            {
                // Update existing record
                rules.ClientVersion = version;
                rules.MinimumConnectionLevel = parsedLevel;
                rules.Reason = reason;
                ctx.Db.ClientUpdates.ClientVersion.Update(rules);
            }
            else
            {
                // Insert new record
                ctx.Db.ClientUpdates.Insert(new ClientUpdatesTable
                {
                    ClientVersion = version,
                    MinimumConnectionLevel = parsedLevel,
                    Reason = reason
                });
            }
        }
    }
}
