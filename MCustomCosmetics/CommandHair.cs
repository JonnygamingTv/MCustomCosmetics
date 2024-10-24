﻿using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCustomCosmetics
{
    public class CommandHair : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "hair";

        public string Help => "Set or remove the hair color for your outfit";

        public string Syntax => "/hair <none> or <r> <g> <b>";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "hair" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            var color = MCustomCosmetics.Instance.MessageColor;
            UnturnedPlayer p = caller as UnturnedPlayer;
            if (!MCustomCosmetics.Instance.pData.data.ContainsKey((ulong)p.CSteamID))
            {
                UnturnedChat.Say(caller, MCustomCosmetics.Instance.Translate("no_cos_set"), color);
                return;
            }
            string wrongSyntax = "";
            if (MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].Outfits[MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].SelectedFit].Hair != null)
            {
                var hair = MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].Outfits[MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].SelectedFit].Hair;
                wrongSyntax = $"{hair.R}, {hair.G}, {hair.B} | ";
            }
            wrongSyntax += Syntax;
            if (command.Length < 1)
            {                
                UnturnedChat.Say(caller, wrongSyntax, color);
                return;
            }
            if (MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].SelectedFit == "none")
            {
                UnturnedChat.Say(caller, MCustomCosmetics.Instance.Translate("no_sel_outfit"), color);
                return;
            }
            if (command[0].ToLower() == "none")
            {
                MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].Outfits[MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].SelectedFit].Hair = null;
                UnturnedChat.Say(caller, MCustomCosmetics.Instance.Translate("remove_hair", MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].SelectedFit), color);
                MCustomCosmetics.Instance.pData.CommitToFile();
                return;
            }
            if (command.Length < 3)
            {
                UnturnedChat.Say(caller, wrongSyntax, color);
                return;
            }
            if (float.TryParse(command[0], out float r) && float.TryParse(command[1], out float g) && float.TryParse(command[2], out float b))
            {
                if (r > 255) r = 255;
                if (g > 255) g = 255;
                if (b > 255) b = 255;
                MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].Outfits[MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].SelectedFit].Hair = new HairColor(r, g, b);
                UnturnedChat.Say(caller, MCustomCosmetics.Instance.Translate("set_hair"), color);
                MCustomCosmetics.Instance.pData.CommitToFile();
            }
            else
            {
                UnturnedChat.Say(caller, wrongSyntax, color);
                return;
            }
            if (p.HasPermission("CosmeticsAllowSaving")) MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].AllowSaving = true;
            else MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].AllowSaving = false;
            MCustomCosmetics.Instance.pData.CommitToFile();
        }
    }
}
