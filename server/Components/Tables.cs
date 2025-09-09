using SpacetimeDB;

namespace StdbModule
{
    public static partial class Module
    {
        // ---------- TABLES ----------
        [Table(Name = "user", Public = true)]
        public partial class User
        {
            [PrimaryKey]
            public Identity Identity;
            public bool Online;

            // user settings
            public UserSettings Settings;
        }

        [Table(Name = "message", Public = true)]
        public partial class Message
        {   
            public Identity Sender;
            public Timestamp Sent;
            public string ChannelID = "";
            public string Text = "";
        }
    }
}
