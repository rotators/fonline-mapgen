#define _G
#define _W

namespace fonline_mapgen
{
    public static class UGLY
    {
        public static string[] DataFiles =
        {
#if _G
            @"H:\FOnline\FOnlineStable\MASTER.DAT",
            @"H:\FOnline\FOnlineDev\FONLINE.DAT"
#elif _W
            @"C:\Users\wipe\Desktop\SDK\Client\data\packs\fallout.dat"
            //,@"C:\Users\wipe\Desktop\FO2238.client\data\faction.zip"
#endif
        };
        public static string ServerDir =
#if _G
            @"H:\FOnline\Factions\trunk\";
#elif _W
            @"C:\Users\wipe\Desktop\FO2238\Server\";
#endif
    }
}
