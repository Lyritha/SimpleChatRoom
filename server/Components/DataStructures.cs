namespace StdbModule
{
    public static partial class Module
    {
        [SpacetimeDB.Type]
        public partial struct UserSettings
        {
            public string Name;
            public string Color;
            public int ConnectSoundID;
            public int DisconnectSoundID;

            public UserSettings(string name = "", string color = "#FFFFFF", int connectSoundID = 0, int disconnectSoundID = 0)
            {
                Name = name;
                Color = color;
                ConnectSoundID = connectSoundID;
                DisconnectSoundID = disconnectSoundID;
            }
        }

        [SpacetimeDB.Type]
        public partial struct MessageData
        {
            public string Text;
            public string ChannelID;

            public MessageData(string text = "", string channelID = "")
            {
                Text = text;
                ChannelID = channelID;
            }
        }

    }
}
