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
    public class CommandCosmetic : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "cosmetic";

        public string Help => "Sets a custom cosmetic";

        public string Syntax => "/cos <itemdefid/mythics> (mythical effect)";

        public List<string> Aliases => new List<string>() { "cos" };

        public List<string> Permissions => new List<string>() { "cosmetic" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            var color = MCustomCosmetics.Instance.MessageColor;
            if (command.Length < 1)
            {
                UnturnedChat.Say(caller, MCustomCosmetics.Instance.Translate("cos_invalid_syntax"), color);
                return;
            }
            if (command[0].ToLower() == "mythics" || command[0].ToLower() == "m")
            {
                string[] mythicList = { };
                foreach (var x in MCustomCosmetics.Instance.mythics)
                {
                    mythicList[mythicList.Length] = x.Key;
                }
                UnturnedChat.Say(caller, MCustomCosmetics.Instance.Translate("mythicals_available", String.Join(", ", mythicList)));
                return;
            }
            UnturnedPlayer p = caller as UnturnedPlayer;
            if (!MCustomCosmetics.Instance.pData.data.ContainsKey((ulong)p.CSteamID))
            {
                MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID] = new PlayerCosmetic()
                {
                    SelectedFit = "Outfit1",
                    Outfits = new Dictionary<string, Outfit>()
                    {
                        {"Outfit1", new Outfit()
                        {
                            Hat = 0,
                            Mask = 0,
                            Glasses = 0,
                            Backpack = 0,
                            Shirt = 0,
                            Vest = 0,
                            skins = new Dictionary<int, string>()
                        } }
                    },


                };
            }
            if (!MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].Outfits.ContainsKey(MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].SelectedFit))
            {
                UnturnedChat.Say(caller, MCustomCosmetics.Instance.Translate("no_sel_outfit"), color);
                return;
            }
            var search = command[0];
            UnturnedEconInfo cosmetic;
            if (int.TryParse(search, out int searchId)) MCustomCosmetics.EconInfo.TryGetValue(searchId, out cosmetic); else cosmetic = MCustomCosmetics.EconInfo.Values.FirstOrDefault(x => x.name.ToLower().Contains(search.ToLower()));

            if (cosmetic == null)
            {
                UnturnedChat.Say(caller, MCustomCosmetics.Instance.Translate("cos_not_found", search), color);
                return;
            }
            string mythic = "";
            bool allowMythic = false;
            if (command.Length >= 2)
            {
                if (MCustomCosmetics.Instance.mythics.ContainsKey(command[1].ToLower())){
                    mythic = MCustomCosmetics.Instance.mythics[command[1].ToLower()];
                }
                else
                {
                    UnturnedChat.Say(caller, MCustomCosmetics.Instance.Translate("mythic_not_found"), color);
                    return;
                }
            }
            var type = AddCosmetic(cosmetic, (ulong)p.CSteamID, mythic);
            if (type.Contains("skin")) allowMythic = true;
            if (allowMythic && command.Length >= 2)
            {
                UnturnedChat.Say(caller, MCustomCosmetics.Instance.Translate("cos_mythic_success", cosmetic.name, command[1]), color);
            }
            else
            {
                UnturnedChat.Say(caller, MCustomCosmetics.Instance.Translate("cos_added", cosmetic.name), color);
            }
            if (p.HasPermission("CosmeticsAllowSaving")) MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].AllowSaving = true;
            else MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].AllowSaving = false;
            MCustomCosmetics.Instance.pData.CommitToFile();
        }

        public string AddCosmetic(UnturnedEconInfo info, ulong id, string mythic)
        {
            var type = info.display_type.ToLower();
            if (type.Contains("backpack"))
            {
                MCustomCosmetics.Instance.pData.data[id].Outfits[MCustomCosmetics.Instance.pData.data[id].SelectedFit].Backpack = info.itemdefid;
            }
            else if (type.Contains("glasses"))
            {
                MCustomCosmetics.Instance.pData.data[id].Outfits[MCustomCosmetics.Instance.pData.data[id].SelectedFit].Glasses = info.itemdefid;
            }
            else if (type.Contains("hat"))
            {
                MCustomCosmetics.Instance.pData.data[id].Outfits[MCustomCosmetics.Instance.pData.data[id].SelectedFit].Hat = info.itemdefid;
            }
            else if (type.Contains("mask"))
            {
                MCustomCosmetics.Instance.pData.data[id].Outfits[MCustomCosmetics.Instance.pData.data[id].SelectedFit].Mask = info.itemdefid;
            }
            else if (type.Contains("pants"))
            {
                MCustomCosmetics.Instance.pData.data[id].Outfits[MCustomCosmetics.Instance.pData.data[id].SelectedFit].Pants = info.itemdefid;
            }
            else if (type.Contains("shirt"))
            {
                MCustomCosmetics.Instance.pData.data[id].Outfits[MCustomCosmetics.Instance.pData.data[id].SelectedFit].Shirt = info.itemdefid;
            }
            else if (type.Contains("vest"))
            {
                MCustomCosmetics.Instance.pData.data[id].Outfits[MCustomCosmetics.Instance.pData.data[id].SelectedFit].Vest = info.itemdefid;
            }
            else if (type.Contains("skin"))
            {
                MCustomCosmetics.Instance.pData.data[id].Outfits[MCustomCosmetics.Instance.pData.data[id].SelectedFit].skins[info.itemdefid] = mythic;
            }
            return type;
        }
    }
}
