using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MCustomCosmetics
{
    public class CommandOutfit : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "outfit";

        public string Help => "Change or manage outfits";

        public string Syntax => "/outfit <create/delete/select/list/clone/rename> (name)";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "outfit" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            var color = MCustomCosmetics.Instance.MessageColor;
            UnturnedPlayer p = caller as UnturnedPlayer;
            if (!MCustomCosmetics.Instance.pData.data.ContainsKey((ulong)p.CSteamID))
            {
                UnturnedChat.Say(caller, MCustomCosmetics.Instance.Translate("no_cos_set"), color);
                return;
            }
            if (command.Length < 1)
            {
                UnturnedChat.Say(caller, MCustomCosmetics.Instance.Translate("current_cos", MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].SelectedFit, Syntax), color);
                return;
            }
            List<string> fits = new List<string>();
            foreach (var f in MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].Outfits)
            {
                fits.Add(f.Key);
            }
            if (command[0].ToLower() == "list")
            {
                UnturnedChat.Say(caller, MCustomCosmetics.Instance.Translate("list_outfits", string.Join(", ", fits)), color);
                return;
            }
            else if (command.Length < 2)
            {
                UnturnedChat.Say(caller, Syntax, color);
                return;
            }
            command[1] = Regex.Replace(command[1], "[^A-Za-z0-9]", "");
            if (command[1].Length < 1)
            {
                UnturnedChat.Say(caller, MCustomCosmetics.Instance.Translate("reformat_outfit"), color);
                return;
            }
            switch (command[0].ToLower())
            {
                case "create":
                    if (MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].Outfits.Count >= MCustomCosmetics.Instance.Configuration.Instance.OutfitLimit && !p.HasPermission("OutfitBypassLimit"))
                    {
                        UnturnedChat.Say(caller, MCustomCosmetics.Instance.Translate("outfit_limit"), color);
                        return;
                    }
                    MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].Outfits[command[1]] = new Outfit()
                    {
                        Hat = 0,
                        Mask = 0,
                        Glasses = 0,
                        Backpack = 0,
                        Shirt = 0,
                        Pants = 0,
                        Vest = 0,
                        skins = new Dictionary<int, string>()
                    };
                    MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].SelectedFit = command[1];
                    UnturnedChat.Say(caller, MCustomCosmetics.Instance.Translate("outfit_created", command[1]), color);
                    MCustomCosmetics.Instance.pData.CommitToFile();
                    break;
                case "clone":
                    if (MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].Outfits.Count >= MCustomCosmetics.Instance.Configuration.Instance.OutfitLimit && !p.HasPermission("OutfitBypassLimit"))
                    {
                        UnturnedChat.Say(caller, MCustomCosmetics.Instance.Translate("outfit_limit"), color);
                        return;
                    }
                    if (!MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].Outfits.ContainsKey(command[1]))
                    {
                        UnturnedChat.Say(caller, MCustomCosmetics.Instance.Translate("invalid_outfit"), color);
                        return;
                    }
                    MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].Outfits[$"{command[1]}2"] = new Outfit()
                    {
                        Hat = MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].Outfits[command[1]].Hat,
                        Mask = MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].Outfits[command[1]].Mask,
                        Glasses = MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].Outfits[command[1]].Glasses,
                        Backpack = MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].Outfits[command[1]].Backpack,
                        Shirt = MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].Outfits[command[1]].Shirt,
                        Pants = MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].Outfits[command[1]].Pants,
                        Vest = MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].Outfits[command[1]].Vest,
                        skins = new Dictionary<int, string>(MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].Outfits[command[1]].skins),
                        Hair = new HairColor(MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].Outfits[command[1]].Hair.R, MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].Outfits[command[1]].Hair.G, MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].Outfits[command[1]].Hair.B)
                    };
                    UnturnedChat.Say(caller, MCustomCosmetics.Instance.Translate("cloned_outfit",command[1]), color);
                    MCustomCosmetics.Instance.pData.CommitToFile();
                    break;
                case "delete":
                    if (MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].Outfits.ContainsKey(command[1]))
                    {
                        MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].Outfits.Remove(command[1]);
                        UnturnedChat.Say(caller, MCustomCosmetics.Instance.Translate("delete_outfit",command[1]), color);
                        if (MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].SelectedFit == command[1])
                        {
                            MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].SelectedFit = "none";
                        }
                        MCustomCosmetics.Instance.pData.CommitToFile();
                    }
                    else
                    {
                        UnturnedChat.Say(caller, MCustomCosmetics.Instance.Translate("invalid_outfit"), color);
                        return;
                    }
                    break;
                case "select":
                    if (command[1].ToLower() == "none")
                    {
                        MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].SelectedFit = "none";
                        UnturnedChat.Say(caller, MCustomCosmetics.Instance.Translate("removed_outfit"), color);
                        MCustomCosmetics.Instance.pData.CommitToFile();
                    }
                    else if (MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].Outfits.ContainsKey(command[1]))
                    {
                        MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].SelectedFit = command[1];
                        UnturnedChat.Say(caller,MCustomCosmetics.Instance.Translate("selected_outfit",command[1]), color);
                        MCustomCosmetics.Instance.pData.CommitToFile();
                    }
                    else
                    {
                        UnturnedChat.Say(caller, MCustomCosmetics.Instance.Translate("invalid_outfit"), color);
                        return;
                    }
                    break;
                case "rename":
                    if (command.Length < 3)
                    {
                        UnturnedChat.Say(caller, MCustomCosmetics.Instance.Translate("rename_outfit"), color);
                        return;
                    }
                    if (!MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].Outfits.ContainsKey(command[1]))
                    {
                        UnturnedChat.Say(caller, MCustomCosmetics.Instance.Translate("invalid_outfit"), color);
                        return;
                    }
                    command[2] = Regex.Replace(command[2], "[^A-Za-z0-9]", "");
                    if (command[2].Length < 1)
                    {
                        UnturnedChat.Say(caller, MCustomCosmetics.Instance.Translate("reformat_outfit"), color);
                        return;
                    }
                    var fit = MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].Outfits[command[1]];
                    MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].Outfits.Remove(command[1]);
                    MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].Outfits[command[2]] = fit;
                    if (MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].SelectedFit == command[1])
                    {
                        MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].SelectedFit = command[2];
                    }
                    UnturnedChat.Say(caller, MCustomCosmetics.Instance.Translate("renamed_outfit", command[1], command[2]), color);
                    break;
                default:
                    UnturnedChat.Say(caller, Syntax, color);
                    return;
            }
            if (p.HasPermission("CosmeticsAllowSaving")) MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].AllowSaving = true;
            else MCustomCosmetics.Instance.pData.data[(ulong)p.CSteamID].AllowSaving = false;
            MCustomCosmetics.Instance.pData.CommitToFile();
        }
    }
}
