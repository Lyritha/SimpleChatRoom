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
    }
}
