using SpacetimeDB;

namespace StdbModule
{
    public static partial class Module
    {
        // ---------- TABLES ----------
        [Table(Name = "User", Public = true)]
        public partial class UserTable
        {
            [PrimaryKey]
            public Identity Identity;
            public bool Online;
            public AuthorityLevel AuthorityLevel = AuthorityLevel.User;

            // user settings
            public UserSettings Settings;
        }

        [Table(Name = "Message", Public = true)]
        public partial class MessageTable
        {
            [PrimaryKey]
            [AutoInc]
            public ulong MessageId;
            public Identity Sender;
            public Timestamp Sent;
            public string ChannelID = "";
            public string Text = "";
            public bool HasBeenEdited = false;
        }

        [Table(Name = "ClientRules", Public = true)]
        public partial class ClientRulesTable
        {
            [PrimaryKey]
            public int Id;
            public int ClientVersion;
            public bool IsClientAllowedToConnect;
        }
    }
}
