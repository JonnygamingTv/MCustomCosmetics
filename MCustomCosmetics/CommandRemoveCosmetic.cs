using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCustomCosmetics
{
    internal class CommandRemoveCosmetic : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "removecosmetic";

        public string Help => "remove a cosmetic or all of them";

        public string Syntax => "/rcos <all/id/name/hat/mask/glasses/shirt/backpack/vest/pants>";

        public List<string> Aliases => new List<string>() { "rcos" };

        public List<string> Permissions => new List<string>() { "removecosmetic" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            var color = MCustomCosmetics.Instance.MessageColor;
            UnturnedPlayer p = caller as UnturnedPlayer;
            if (command.Length < 1)
            {
                UnturnedChat.Say(caller, MCustomCosmetics.Instance.Translate("no_args", Syntax), color);
                return;
            }
            if (!MCustomCosmetics.Instance.pData.data.ContainsKey((ulong)p.CSteamID))
            {
                UnturnedChat.Say(caller, MCustomCosmetics.Instance.Translate("no_cos_set"), color);
                return;

            }
            if (!MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].Outfits.ContainsKey(MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].SelectedFit))
            {
                UnturnedChat.Say(caller, MCustomCosmetics.Instance.Translate("no_sel_outfit"), color);
                return;
            }
            switch (command[0].ToLower())
            {
                case "all":
                    MCustomCosmetics.Instance.pData.data.Remove((ulong)p.CSteamID);
                    UnturnedChat.Say(caller, MCustomCosmetics.Instance.Translate("cos_removeall"), color);
                    break;
                case "hat":
                    MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].Outfits[MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].SelectedFit].Hat = 0;
                    UnturnedChat.Say(caller, MCustomCosmetics.Instance.Translate("cos_removehat"), color);
                    break;
                case "mask":
                    MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].Outfits[MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].SelectedFit].Mask = 0;
                    UnturnedChat.Say(caller, MCustomCosmetics.Instance.Translate("cos_removemask"), color);
                    break;
                case "glasses":
                    MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].Outfits[MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].SelectedFit].Glasses = 0;
                    UnturnedChat.Say(caller, MCustomCosmetics.Instance.Translate("cos_removeglasses"), color);
                    break;
                case "backpack":
                    MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].Outfits[MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].SelectedFit].Backpack = 0;
                    UnturnedChat.Say(caller, MCustomCosmetics.Instance.Translate("cos_removebackpack"), color);
                    break;
                case "shirt":
                    MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].Outfits[MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].SelectedFit].Shirt = 0;
                    UnturnedChat.Say(caller, MCustomCosmetics.Instance.Translate("cos_removeshirt"), color);
                    break;
                case "vest":
                    MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].Outfits[MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].SelectedFit].Vest = 0;
                    UnturnedChat.Say(caller, MCustomCosmetics.Instance.Translate("cos_removevest"), color);
                    break;
                case "pants":
                    MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].Outfits[MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].SelectedFit].Pants = 0;
                    UnturnedChat.Say(caller, MCustomCosmetics.Instance.Translate("cos_removepants"), color);
                    break;
                default:
                    var search = command[0];
                    UnturnedEconInfo cosmetic;
                    if (int.TryParse(search, out int searchId)) MCustomCosmetics.EconInfo.TryGetValue(searchId, out cosmetic); else cosmetic = MCustomCosmetics.EconInfo.Values.FirstOrDefault(x => x.name.ToLower().Contains(search.ToLower()));
                    if (cosmetic == null)
                    {
                        UnturnedChat.Say(caller, MCustomCosmetics.Instance.Translate("cos_not_found", search), color);
                        return;
                    }
                    if (MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].Outfits[MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].SelectedFit].skins.ContainsKey(cosmetic.itemdefid))
                    {
                        UnturnedChat.Say(caller, MCustomCosmetics.Instance.Translate("delete_outfit", cosmetic.name), color);
                        MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].Outfits[MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].SelectedFit].skins.Remove(cosmetic.itemdefid);
                    }
                    else
                    {
                        UnturnedChat.Say(caller, MCustomCosmetics.Instance.Translate("not_equipped_notexist", cosmetic.name), color);
                    }
                    break;
            }
            if (p.HasPermission("CosmeticsAllowSaving")) MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].AllowSaving = true;
            else MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].AllowSaving = false;
            MCustomCosmetics.Instance.pData.CommitToFile();
        }
    }
}
