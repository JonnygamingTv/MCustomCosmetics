using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.NetTransport;
using SDG.Provider;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MCustomCosmetics
{
    public static class Util
    {
        public static UnturnedEconInfo GetCosmetic (string search)
        {
            var econInfoField = typeof(SDG.Provider.TempSteamworksEconomy).GetField("econInfo", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            var econInfos = econInfoField.GetValue(null) as Dictionary<int, UnturnedEconInfo>;
            UnturnedEconInfo cosmetic;
            if (int.TryParse(search, out int searchId)) econInfos.TryGetValue(searchId, out cosmetic); else cosmetic = econInfos.Values.FirstOrDefault(x => x.name.ToLower().Contains(search.ToLower()));
            return cosmetic;
        }
    }
}
