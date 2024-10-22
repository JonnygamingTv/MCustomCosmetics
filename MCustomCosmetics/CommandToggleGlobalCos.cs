using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCustomCosmetics
{
    public class CommandToggleGlobalCos : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "togglecos";

        public string Help => "Toggles the forced global cosmetics";

        public string Syntax => "";

        public List<string> Aliases => new List<string>() { "tcos" };

        public List<string> Permissions => new List<string>() { "togglecos" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            var color = MCustomCosmetics.Instance.MessageColor;
            ulong playerId = (ulong)((UnturnedPlayer)caller).CSteamID;
            if (!MCustomCosmetics.Instance.globalCos.ContainsKey(playerId))
            {
                MCustomCosmetics.Instance.globalCos[playerId] = false;
            }
            if (MCustomCosmetics.Instance.globalCos[playerId])
            {
                MCustomCosmetics.Instance.globalCos[playerId] = false;
                UnturnedChat.Say(caller, MCustomCosmetics.Instance.Translate("cos_global_off"), color);
            }
            else
            {
                MCustomCosmetics.Instance.globalCos[playerId] = true;
                UnturnedChat.Say(caller, MCustomCosmetics.Instance.Translate("cos_global_on"), color);
            }
        }
    }
}
