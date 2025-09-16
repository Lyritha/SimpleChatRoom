using SpacetimeDB;

namespace StdbModule
{
    public static partial class Module
    {
        private static void AssignIfNotNull<T>(ref T target, T? value)
        {
            if (value != null)
                target = value;
        }

        /// <summary>
        /// Converts a 64-character hex string into a SpacetimeDB.Identity.
        /// Throws if the hex is invalid or wrong length.
        /// </summary>
        private static Identity IdentityFromHex(string hex)
        {
            if (string.IsNullOrWhiteSpace(hex))
                throw new ArgumentException("Hex string cannot be null or empty", nameof(hex));

            if (hex.Length != 64)
                throw new ArgumentException("Identity hex string must be 64 characters (32 bytes)", nameof(hex));

            byte[] bytes = [.. Enumerable.Range(0, hex.Length / 2).Select(i => Convert.ToByte(hex.Substring(i * 2, 2), 16))];

            return new Identity(bytes);
        }

        private static bool IsAllowedToUseReducer(ReducerContext ctx, AuthorityLevel minimumAuthority, string reducerName)
        {
            bool isServer = ctx.Sender.ToString() == "C200CE5D8B54E608F8DBAAF1B4C1B0FC73A8A850C935C73C19746E5F1A638B46";
            if (isServer) return true;

            UserTable? user = ctx.Db.User.Identity.Find(ctx.Sender);
            AuthorityLevel authorityLevel = user?.AuthorityLevel ?? AuthorityLevel.User;

            if ((int)authorityLevel >= (int)minimumAuthority) return true;

            Log.Warn($"Client {ctx.Sender} attempted call reducer {reducerName} with insufficient authority");
            return false;
        }
    }
}
