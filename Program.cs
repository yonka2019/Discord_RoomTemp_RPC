
using com.clusterrr.TuyaNet;
using DiscordRPC;
using DiscordRPC.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace Discord_RoomTemp_RPC
{
    internal class Program
    {
        private static DiscordRpcClient rpcClient;
        private static TuyaApi tuyaApi;

        private static void Init()
        {
            tuyaApi = new TuyaApi(region: TuyaApi.Region.CentralEurope, accessId: Data.Tuya.ACCESS_ID, apiSecret: Data.Tuya.API_SECRET);

            rpcClient = new DiscordRpcClient(Data.Discord.CLIENT_ID)
            {
                Logger = new ConsoleLogger() { Level = LogLevel.Warning }
            };

            rpcClient.OnReady += (sender, e) =>
            {
                Console.WriteLine("Received Ready from user {0}", e.User.Username);
            };

            rpcClient.OnPresenceUpdate += (sender, e) =>
            {
                Console.WriteLine("Received Update! {0}", e.Presence.GetString());
            };
            rpcClient.Initialize();
        }

        private static async Task Main()
        {
            Init();
            await UpdateTempAsync(); // start updating
        }

        private static string ExtractTemp(string getResult)
        {
            JObject json = JObject.Parse(getResult);
            string temp = json["status"][0]["value"].ToString(); // status[0] => {"temp", value} => return value
            return temp.Substring(0, temp.Length - 1) + "." + temp[temp.Length - 1]; // temp (300) => return 30.0
        }

        private static async Task UpdateTempAsync()
        {
            while (true)
            {
                string getResult = await tuyaApi.RequestAsync(TuyaApi.Method.GET, $"/v1.0/devices/{Data.Tuya.DEVICE_ID}");
                string roomTemperature = ExtractTemp(getResult);

                rpcClient.SetPresence(new RichPresence()
                {
                    Details = "In Room:",
                    State = $"{roomTemperature} °C",
                    Assets = new Assets()
                    {
                        LargeImageKey = Data.Discord.LARGE_IMAGE_KEY,
                        SmallImageKey = Data.Discord.SMALL_IMAGE_KEY,
                        SmallImageText = "By Yonka"
                    }
                });
                await Task.Delay(Data.Settings.UPDATE_INTERVAL_MS);
            }
        }
    }

    internal static class MExtensions
    {
        internal static string GetString(this BaseRichPresence rpc)
        {
            return $"\n----------\nDetails: {rpc.Details}\nState: {rpc.State}\n----------\n";
        }
    }
}
