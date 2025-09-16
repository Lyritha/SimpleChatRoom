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
            [Unique]
            public string Name = "";
            public string Color = "";
            public int ConnectSoundID = 0;
            public int DisconnectSoundID = 0;
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

        [Table(Name = "ClientUpdates", Public = true)]
        public partial class ClientUpdatesTable
        {
            [PrimaryKey]
            public int ClientVersion;
            public AuthorityLevel MinimumConnectionLevel;
            public string Reason = "";
        }
    }
}
