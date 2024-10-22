﻿using Rocket.API;
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
                UnturnedChat.Say(caller, $"No arguments given! {Syntax}", color);
                return;
            }
            if (!MCustomCosmetics.Instance.pData.data.ContainsKey((ulong)p.CSteamID))
            {
                UnturnedChat.Say(caller, "You don't have any cosmetics set!", color);
                return;

            }
            if (!MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].Outfits.ContainsKey(MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].SelectedFit))
            {
                UnturnedChat.Say(caller, "You do not have a selected outfit! Select one with /outfit", color);
                return;
            }
            switch (command[0].ToLower())
            {
                case "all":
                    MCustomCosmetics.Instance.pData.data.Remove((ulong)p.CSteamID);
                    UnturnedChat.Say(caller, "Removed all custom cosmetics", color);
                    break;
                case "hat":
                    MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].Outfits[MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].SelectedFit].Hat = 0;
                    UnturnedChat.Say(caller, "Removed your custom hat", color);
                    break;
                case "mask":
                    MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].Outfits[MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].SelectedFit].Mask = 0;
                    UnturnedChat.Say(caller, "Removed your custom mask", color);
                    break;
                case "glasses":
                    MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].Outfits[MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].SelectedFit].Glasses = 0;
                    UnturnedChat.Say(caller, "Removed your custom glasses", color);
                    break;
                case "backpack":
                    MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].Outfits[MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].SelectedFit].Backpack = 0;
                    UnturnedChat.Say(caller, "Removed your custom backpack", color);
                    break;
                case "shirt":
                    MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].Outfits[MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].SelectedFit].Shirt = 0;
                    UnturnedChat.Say(caller, "Removed your custom shirt", color);
                    break;
                case "vest":
                    MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].Outfits[MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].SelectedFit].Vest = 0;
                    UnturnedChat.Say(caller, "Removed your custom vest", color);
                    break;
                case "pants":
                    MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].Outfits[MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].SelectedFit].Pants = 0;
                    UnturnedChat.Say(caller, "Removed your custom pants", color);
                    break;
                default:
                    var search = command[0];
                    var econInfoField = typeof(SDG.Provider.TempSteamworksEconomy).GetField("econInfo", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
                    var econInfos = econInfoField.GetValue(null) as Dictionary<int, UnturnedEconInfo>;
                    UnturnedEconInfo cosmetic;
                    if (int.TryParse(search, out int searchId)) econInfos.TryGetValue(searchId, out cosmetic); else cosmetic = econInfos.Values.FirstOrDefault(x => x.name.ToLower().Contains(search.ToLower()));
                    if (cosmetic == null)
                    {
                        UnturnedChat.Say(caller, "Cosmetic id " + search + " not found!", color);
                        return;
                    }
                    if (MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].Outfits[MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].SelectedFit].skins.ContainsKey(cosmetic.itemdefid))
                    {
                        UnturnedChat.Say(caller, "Removed " + cosmetic.name, color);
                        MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].Outfits[MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].SelectedFit].skins.Remove(cosmetic.itemdefid);
                    }
                    else
                    {
                        UnturnedChat.Say(caller, "You do not have " + cosmetic.name + " equipped, or it does not exist.", color);
                    }
                    break;
            }
            if (p.HasPermission("CosmeticsAllowSaving")) MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].AllowSaving = true;
            else MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].AllowSaving = false;
            MCustomCosmetics.Instance.pData.CommitToFile();
        }
    }
}
