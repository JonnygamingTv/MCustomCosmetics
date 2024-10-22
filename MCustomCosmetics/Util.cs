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
            UnturnedEconInfo cosmetic;
            if (int.TryParse(search, out int searchId)) MCustomCosmetics.EconInfo.TryGetValue(searchId, out cosmetic); else cosmetic = MCustomCosmetics.EconInfo.Values.FirstOrDefault(x => x.name.ToLower().Contains(search.ToLower()));
            return cosmetic;
        }
    }
}
