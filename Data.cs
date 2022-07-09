namespace Discord_RoomTemp_RPC
{
    internal static class Data
    {
        internal static class Settings
        {
            public const int UPDATE_INTERVAL_MS = 60000; // 60000 / 60 (sec) / 1000 (ms) = 1 minute
        }

        internal static class Tuya
        {
            public const string ACCESS_ID = "access_id";
            public const string API_SECRET = "api_secret";
            public const string DEVICE_ID = "device_id";
        }

        internal static class Discord
        {
            public const string CLIENT_ID = "client_id";
            public const string IMAGE_KEY = "image_key";
        }

    }
}
