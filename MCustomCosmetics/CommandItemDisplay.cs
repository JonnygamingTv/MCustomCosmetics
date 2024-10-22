using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Items;
using Rocket.Unturned.Player;
using SDG.Provider;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MCustomCosmetics
{
    public class CommandItemDisplay : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "ItemDisplay";

        public string Help => "Sets the display item on a storage you are looking at";

        public string Syntax => "/itemdisplay <item id or name> (skin itemdefid or name) (mythical)";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "itemdisplay" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            var color = MCustomCosmetics.Instance.MessageColor;
            if (command.Length < 1)
            {
                UnturnedChat.Say(caller, Syntax, color);
                return;
            }
            ushort itemId = 0;
            ushort skinId = 0;
            ushort mythicId = 0;
            // item id
            if (ushort.TryParse(command[0], out ushort checkId))
            {
                itemId = checkId;
            }
            else
            {
                var checkItem = UnturnedItems.GetItemAssetByName(command[0]);
                if (checkItem == null)
                {
                    UnturnedChat.Say(caller, MCustomCosmetics.Instance.Translate("invalid_item"), color);
                    return;
                }
                itemId = checkItem.id;
            }
            var item = new Item(itemId, true);
            // skin id
            if (command.Length >= 2)
            {
                var search = command[1];
                var econInfoField = typeof(SDG.Provider.TempSteamworksEconomy).GetField("econInfo", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
                var econInfos = econInfoField.GetValue(null) as Dictionary<int, UnturnedEconInfo>;
                UnturnedEconInfo cosmetic;
                if (int.TryParse(search, out int searchId)) econInfos.TryGetValue(searchId, out cosmetic); else cosmetic = econInfos.Values.FirstOrDefault(x => x.name.ToLower().Contains(search.ToLower()));
                if (cosmetic == null)
                {
                    UnturnedChat.Say(caller, MCustomCosmetics.Instance.Translate("cos_not_found", search), color);
                    return;
                }
                if (cosmetic.display_type.Contains("skin"))
                {
                    UnturnedChat.Say(caller, MCustomCosmetics.Instance.Translate("not_for_mannequin", cosmetic.name), color);
                    return;
                }
                skinId = Provider.provider.economyService.getInventorySkinID(cosmetic.itemdefid);
            }
            // mythic id 
            if (command.Length >= 3)
            {
                if (MCustomCosmetics.Instance.mythics.ContainsKey(command[2].ToLower()))
                {
                    mythicId = ushort.Parse(MCustomCosmetics.Instance.mythics[command[2]].ToLower().Split(':')[1]);
                }
            }
            // doing the rest
            UnturnedPlayer p = caller as UnturnedPlayer;
            Physics.Raycast(new Ray(p.Player.look.aim.position, p.Player.look.aim.forward), out var hit, 20, RayMasks.BARRICADE);
            if (hit.collider != null)
            {
                var bar = BarricadeManager.FindBarricadeByRootTransform(hit.collider.transform.root);
                if (bar.GetServersideData().owner != (ulong)p.CSteamID)
                {
                    UnturnedChat.Say(caller, MCustomCosmetics.Instance.Translate("not_owner"), color);
                    return;
                }
                if (bar.interactable == null)
                {
                    UnturnedChat.Say(caller, MCustomCosmetics.Instance.Translate("not_storage"), color);
                    return;
                }
                if (bar.interactable is InteractableStorage storage)
                {
                    if (storage.items.getItemCount() > 0)
                    {
                        UnturnedChat.Say(caller, MCustomCosmetics.Instance.Translate("remove_items"), color);
                        return;
                    }
                    BarricadeManager.sendStorageDisplay(hit.collider.transform.root, item, skinId, mythicId, "", "");
                    UnturnedChat.Say(caller, MCustomCosmetics.Instance.Translate("set_item", item.id, skinId, mythicId), color);
                }
                else
                {
                    UnturnedChat.Say(caller, MCustomCosmetics.Instance.Translate("not_storage"), color);
                    return;
                }
            }
            else
            {
                UnturnedChat.Say(caller, MCustomCosmetics.Instance.Translate("no_barricade"), color);
            }
        }
    }
}
