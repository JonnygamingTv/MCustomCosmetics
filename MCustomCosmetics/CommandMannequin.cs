﻿using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Provider;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MCustomCosmetics
{
    public class CommandMannequin : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "mannequin";

        public string Help => "Applies cosmetics to a mannequin";

        public string Syntax => "/mannequin <itemdefid or name>";

        public List<string> Aliases => new List<string>() { "ma" };

        public List<string> Permissions => new List<string>() { "mannequin" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            var color = MCustomCosmetics.Instance.MessageColor;
            if (command.Length < 1)
            {
                UnturnedChat.Say(caller, Syntax, color);
                return;
            }
            UnturnedPlayer p = caller as UnturnedPlayer;
            Physics.Raycast(new Ray(p.Player.look.aim.position, p.Player.look.aim.forward), out var hit, 20, RayMasks.BARRICADE);
            if (hit.collider != null)
            {
                var bar = BarricadeManager.FindBarricadeByRootTransform(hit.collider.transform.root);
                if (bar.GetServersideData().owner != (ulong)p.CSteamID)
                {
                    UnturnedChat.Say(caller, MCustomCosmetics.Instance.Translate("mannequin_not_owner"), color);
                    return;
                }
                if (bar.interactable == null)
                {
                    UnturnedChat.Say(caller, MCustomCosmetics.Instance.Translate("not_a_mannequin"), color);
                    return;
                }
                if (bar.interactable is InteractableMannequin man)
                {
                    if (man.hat != 0 || man.backpack != 0 || man.glasses != 0 || man.mask != 0 || man.shirt != 0 || man.vest != 0 || man.pants != 0)
                    {
                        UnturnedChat.Say(caller, MCustomCosmetics.Instance.Translate("clear_mannequin"), color);
                        return;
                    }
                    var search = command[0];
                    UnturnedEconInfo cosmetic;
                    if (int.TryParse(search, out int searchId)) MCustomCosmetics.EconInfo.TryGetValue(searchId, out cosmetic); else cosmetic = MCustomCosmetics.EconInfo.Values.FirstOrDefault(x => x.name.ToLower().Contains(search.ToLower()));
                    if (cosmetic == null)
                    {
                        UnturnedChat.Say(caller, MCustomCosmetics.Instance.Translate("cos_not_found",search), color);
                        return;
                    }
                    if (cosmetic.display_type.Contains("skin"))
                    {
                        UnturnedChat.Say(caller, MCustomCosmetics.Instance.Translate("not_for_mannequin",cosmetic.name), color);
                        return;
                    }
                    var backpack = man.visualBackpack;
                    var glasses = man.visualGlasses;
                    var hat = man.visualHat;
                    var mask = man.visualMask;
                    var pants = man.visualPants;
                    var vest = man.visualVest;
                    var shirt = man.visualShirt;
                    //UnturnedChat.Say(caller, $"{backpack} {glasses} {hat} {mask} {pants} {vest} {shirt}");
                    var type = cosmetic.display_type.ToLower();
                    if (type.Contains("backpack"))
                    {
                        backpack = cosmetic.itemdefid;
                    }
                    else if (type.Contains("glasses"))
                    {
                        glasses = cosmetic.itemdefid;
                    }
                    else if (type.Contains("hat"))
                    {
                        hat = cosmetic.itemdefid;
                    }
                    else if (type.Contains("mask"))
                    {
                        mask = cosmetic.itemdefid;
                    }
                    else if (type.Contains("pants"))
                    {
                        pants = cosmetic.itemdefid;
                    }
                    else if (type.Contains("shirt"))
                    {
                        shirt = cosmetic.itemdefid;
                    }
                    else if (type.Contains("vest"))
                    {
                        vest = cosmetic.itemdefid;
                    }
                    //UnturnedChat.Say(caller, $"{backpack} {glasses} {hat} {mask} {pants} {vest} {shirt}");
                    (bar.interactable as InteractableMannequin).updateVisuals(shirt, pants, hat, backpack, vest, mask, glasses);
                    (bar.interactable as InteractableMannequin).rebuildState();
                    var newpos = bar.GetServersideData().point;
                    BarricadeManager.tryGetInfo(hit.collider.transform.root, out byte xx, out byte y, out ushort plant, out ushort index, out BarricadeRegion region);
                    BarricadeManager.dropNonPlantedBarricade(bar.GetServersideData().barricade, newpos, hit.collider.transform.root.rotation, bar.GetServersideData().owner, bar.GetServersideData().group);
                    BarricadeManager.destroyBarricade(bar, xx, y, plant);
                    UnturnedChat.Say(caller, MCustomCosmetics.Instance.Translate("applied_mannequin", cosmetic.name), color);
                }
                else
                {
                    UnturnedChat.Say(caller, MCustomCosmetics.Instance.Translate("not_a_mannequin"), color);
                    return;
                }
            }
            else
            {
                UnturnedChat.Say(caller, MCustomCosmetics.Instance.Translate("not_looking_at_mannequin"), color);
            }
        }
    }
}
