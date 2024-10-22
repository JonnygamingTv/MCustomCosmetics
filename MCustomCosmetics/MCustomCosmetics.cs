﻿using HarmonyLib;
using Rocket.API.Collections;
using Rocket.Core.Assets;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Permissions;
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
    public class MCustomCosmetics : RocketPlugin<MCustomCosmeticsConfig>
    {
        public static MCustomCosmetics Instance { get; set; }
        public Dictionary<string, string> mythics;
        public PlayerData pData;
        public Dictionary<ulong, bool> globalCos;
        public UnityEngine.Color MessageColor { get; set; }
        public override TranslationList DefaultTranslations => new TranslationList()
        {
            {"set_vehicle_skin", "Set your vehicle skin to {0}"},
            {"cos_global_off","You have toggled off global cosmetics. Relog to see the changes"},
            {"cos_global_on","You have toggled on global cosmetics. Relog to see the changes"},
            {"cos_invalid_syntax", "Invalid syntax! /cos <itemdefid/mythics> (mythical effect)"},
            {"cos_not_found", "Cosmetic id {0} not found!"},
            {"mythicals_available", "Mythical effects available: {0}"},
            {"mythic_not_found", "Mythic not found! Use /cos mythics"},
            {"cos_mythic_success", "Added cosmetic {0} with mythic effect {1}" },
            {"cos_added", "Added cosmetic {0}"},
            {"no_cos_set", "You do not have any cosmetics set! Use /cosmetic first"},
            {"no_cos_equipped","You do not have any cosmetics equipped"},
            {"not_equipped_notexist","You do not have {0} equipped, or it does not exist."},
            {"no_sel_outfit", "You do not have a selected outfit! Select one with /outfit"},
            {"remove_hair","Removed the hair on outfit {0}"},
            {"set_hair","Set your hair color!"},
            {"set_item","Set item id {0} / skin id {1} / mythic {2}"},
            {"invalid_item","That is not a valid item"},
            {"not_for_mannequin", "{0} cannot be applied to a mannequin!"},
            {"mannequin_not_owner","You are not the owner of this mannequin!"},
            {"applied_mannequin","Applied {0} to the mannequin"},
            {"clear_mannequin","Please remove any items from the mannequin first!"},
            {"not_a_mannequin","That is not a mannequin!"},
            {"not_looking_at_mannequin","You are not looking at a mannequin!"},
            {"not_owner","You are not the owner of this storage!"},
            {"not_storage","That is not a storage!"},
            {"no_barricade","Could not find a barricade"},
            {"remove_items","Please remove any items from the storage first!"},
            {"reformat_outfit", "Please reformat your outfit name. It must be alphanumeric and at least 1 character long"},
            {"outfit_limit","You cannot create any more outfits! Please remove one first."},
            {"outfit_created", "Created and equipped outfit: {0}"},
            {"rename_outfit","/outfit rename <outfit you own> <new name>"},
            {"selected_outfit","Selected outfit {0}. Relog to see changes"},
            {"invalid_outfit","That is not a valid outfit you own!"},
            {"removed_outfit","Removed your outfit"},
            {"delete_outfit","Removed {0}"},
            {"cloned_outfit","Cloned outfit {0} named {0}2"},
            {"no_vehicle","You are not in a vehicle!"},
            {"wrong_vehicle","This is not your vehicle! Please lock it first."}
        };
        protected override void Load()
        {
            Instance = this;
            pData = new PlayerData();
            pData.Reload();
            pData.CommitToFile();
            Patches.PatchAll();
            MessageColor = (Color)UnturnedChat.GetColorFromHex(Configuration.Instance.TextColor);
            // mythics dict for plugin
            mythics = new Dictionary<string, string>();
            mythics["burning"] = "particle_effect:1";
            mythics["glowing"] = "particle_effect:2";
            mythics["lovely"] = "particle_effect:3";
            mythics["musical"] = "particle_effect:4";
            mythics["shiny"] = "particle_effect:5";
            mythics["glitched"] = "particle_effect:6";
            mythics["wealthy"] = "particle_effect:7";
            //mythics["divine"] = "particle_effect:8";
            mythics["bubbling"] = "particle_effect:9";
            mythics["cosmic"] = "particle_effect:10";
            mythics["electric"] = "particle_effect:11";
            //mythics["rainbow"] = "particle_effect:12";
            mythics["party"] = "particle_effect:13";
            //mythics["haunted"] = "particle_effect:14";
            mythics["freezing"] = "particle_effect:15";
            mythics["energized"] = "particle_effect:16";
            mythics["holiday"] = "particle_effect:17";
            mythics["meta"] = "particle_effect:18";
            //mythics["pyrotechnic"] = "particle_effect:19";
            //mythics["atomic"] = "particle_effect:20";
            mythics["melting"] = "particle_effect:21";
            mythics["confetti"] = "particle_effect:22";
            mythics["radioactive"] = "particle_effect:23";
            mythics["steampunk"] = "particle_effect:24";
            mythics["bloodsucker"] = "particle_effect:25";
            mythics["luckycoins"] = "particle_effect:26";
            mythics["skylantern"] = "particle_effect:27";
            //mythics["firedragon"] = "particle_effect:28";
            //mythics["icedragon"] = "particle_effect:29";
            mythics["blossoming"] = "particle_effect:30";
            //mythics["bananza"] = "particle_effect:31";
            //mythics["hightide"] = "particle_effect:32";
            mythics["deckedout"] = "particle_effect:33";
            mythics["crystalshards"] = "particle_effect:34";
            mythics["soulshattered"] = "particle_effect:35";
            mythics["enchanted"] = "particle_effect:36";
            //mythics["crypticrunes"] = "particle_effect:37";
            //mythics["sacrificial"] = "particle_effect:38";
            mythics["frosty"] = "particle_effect:39";
            mythics["spectralgems"] = "particle_effect:40";
            //mythics["sunrise"] = "particle_effect:41";
            //mythics["sunset"] = "particle_effect:42";
            mythics["electrostatic"] = "particle_effect:43";
            //mythics["wicked"] = "particle_effect:44";
            //mythics["palmnights"] = "particle_effect:45";
            //mythics["icecrown"] = "particle_effect:46";
            //mythics["firecrown"] = "particle_effect:47";
            mythics["firefly"] = "particle_effect:48";
            //mythics["icicles"] = "particle_effect:49";
            //mythics["snowflake"] = "particle_effect:50";

            globalCos = new Dictionary<ulong, bool>();
            Rocket.Core.Logging.Logger.Log($"{Name} {Assembly.GetName().Version} has been loaded!");
            Rocket.Core.Logging.Logger.Log($"Permissions are the command names!");
            Rocket.Core.Logging.Logger.Log($"Bypass outfit permission is \'OutfitBypassLimit\'");
            Rocket.Core.Logging.Logger.Log($"Saving outfit permission is \'CosmeticsAllowSaving\'");
        }

        protected override void Unload()
        {
            if (Configuration.Instance.ClearUnsavedOnReboot)
            {
                foreach (var item in pData.data.ToArray())
                {
                    if (!item.Value.AllowSaving)
                    {
                        pData.data.Remove(item.Key);
                    }
                }
            }
            pData.CommitToFile();
            Patches.UnpatchAll();
        }
    }

    internal class Patches
    {
        private static Harmony PatcherInstance;
        internal static void PatchAll()
        {
            PatcherInstance = new Harmony("MCustomCosmetics");
            PatcherInstance.PatchAll();
        }
        internal static void UnpatchAll()
        {
            PatcherInstance.UnpatchAll("MCustomCosmetics");
        }

        [HarmonyPatch]
        internal class ProviderAccept
        {
            [HarmonyPatch(typeof(Provider))]
            [HarmonyPatch("accept")]
            [HarmonyPatch(new Type[] { typeof(SteamPlayerID), typeof(bool), typeof(bool), typeof(byte), typeof(byte), typeof(byte), typeof(Color), typeof(Color), typeof(Color), typeof(bool), typeof(int), typeof(int), typeof(int), typeof(int), typeof(int), typeof(int), typeof(int), typeof(int[]), typeof(string[]), typeof(string[]), typeof(EPlayerSkillset), typeof(string), typeof(CSteamID), typeof(EClientPlatform) })]
            [HarmonyPrefix]
            static void Accept(SteamPlayerID playerID, bool isPro, bool isAdmin, byte face, byte hair, byte beard, Color skin, ref Color color, Color markerColor, bool hand, ref int shirtItem, ref int pantsItem, ref int hatItem, ref int backpackItem, ref int vestItem, ref int maskItem, ref int glassesItem, ref int[] skinItems, ref string[] skinTags, ref string[] skinDynamicProps, EPlayerSkillset skillset, string language, CSteamID lobbyID)
            {
                if (MCustomCosmetics.Instance.pData.data.ContainsKey((ulong)playerID.steamID))
                {
                    var pData = MCustomCosmetics.Instance.pData.data[(ulong)playerID.steamID];
                    if (!pData.Outfits.ContainsKey(pData.SelectedFit))
                    {
                        pData.SelectedFit = "none";
                        MCustomCosmetics.Instance.pData.data[(ulong)playerID.steamID].SelectedFit = "none";
                        MCustomCosmetics.Instance.pData.CommitToFile();

                    }
                    if (pData.SelectedFit != "none")
                    {
                        if (pData.Outfits[pData.SelectedFit].Hat != 0) hatItem = pData.Outfits[pData.SelectedFit].Hat;
                        if (pData.Outfits[pData.SelectedFit].Mask != 0) maskItem = pData.Outfits[pData.SelectedFit].Mask;
                        if (pData.Outfits[pData.SelectedFit].Glasses != 0) glassesItem = pData.Outfits[pData.SelectedFit].Glasses;
                        if (pData.Outfits[pData.SelectedFit].Backpack != 0) backpackItem = pData.Outfits[pData.SelectedFit].Backpack;
                        if (pData.Outfits[pData.SelectedFit].Shirt != 0) shirtItem = pData.Outfits[pData.SelectedFit].Shirt;
                        if (pData.Outfits[pData.SelectedFit].Vest != 0) vestItem = pData.Outfits[pData.SelectedFit].Vest;
                        if (pData.Outfits[pData.SelectedFit].Pants != 0) pantsItem = pData.Outfits[pData.SelectedFit].Pants;
                        if (pData.Outfits[pData.SelectedFit].Hair != null)
                        {
                            color = new Color(pData.Outfits[pData.SelectedFit].Hair.R / 255, pData.Outfits[pData.SelectedFit].Hair.G / 255, pData.Outfits[pData.SelectedFit].Hair.B / 255);
                        }
                        List<int> newItems = skinItems.ToList();
                        List<string> newTags = skinTags.ToList();
                        List<string> newProps = skinDynamicProps.ToList();
                        foreach (var x in pData.Outfits[pData.SelectedFit].skins)
                        {
                            newItems.Add(x.Key);
                            newTags.Add(x.Value);
                            newProps.Add("");
                        }
                        newItems.Reverse();
                        newTags.Reverse();
                        newProps.Reverse();
                        skinItems = newItems.ToArray();
                        skinTags = newTags.ToArray();
                        skinDynamicProps = newProps.ToArray();
                    }
                }
                if (MCustomCosmetics.Instance.Configuration.Instance.globalCosmeticSettings.Enabled)
                {
                    if (!MCustomCosmetics.Instance.globalCos.ContainsKey((ulong)playerID.steamID))
                    {
                        MCustomCosmetics.Instance.globalCos[(ulong)playerID.steamID] = true;
                    }
                    if (!MCustomCosmetics.Instance.globalCos[(ulong)playerID.steamID])
                    {
                        return;
                    }
                    var gcos = MCustomCosmetics.Instance.Configuration.Instance.globalCosmeticSettings;
                    if (gcos.Hat > 0) 
                        if (gcos.OverridePersonalCosmetics || (!gcos.OverridePersonalCosmetics && hatItem == 0))
                            hatItem = gcos.Hat;
                    else if (gcos.Hat == -1) hatItem = 0;
                    if (gcos.Mask > 0)
                        if (gcos.OverridePersonalCosmetics || (!gcos.OverridePersonalCosmetics && maskItem == 0))
                            maskItem = gcos.Mask;
                    else if (gcos.Mask == -1) maskItem = 0;
                    if (gcos.Glasses > 0)
                        if (gcos.OverridePersonalCosmetics || (!gcos.OverridePersonalCosmetics && glassesItem == 0))
                            glassesItem = gcos.Glasses;
                    else if (gcos.Glasses == -1) glassesItem = 0;
                    if (gcos.Backpack > 0)
                        if (gcos.OverridePersonalCosmetics || (!gcos.OverridePersonalCosmetics && backpackItem == 0))
                            backpackItem = gcos.Backpack;
                    else if (gcos.Backpack == -1) backpackItem = 0;
                    if (gcos.Shirt > 0)
                        if (gcos.OverridePersonalCosmetics || (!gcos.OverridePersonalCosmetics && shirtItem == 0))
                            shirtItem = gcos.Shirt;
                    else if (gcos.Shirt == -1) shirtItem = 0;
                    if (gcos.Vest > 0)
                        if (gcos.OverridePersonalCosmetics || (!gcos.OverridePersonalCosmetics && vestItem == 0))
                            vestItem = gcos.Vest;
                    else if (gcos.Vest == -1) vestItem = 0;
                    if (gcos.Pants > 0)
                        if (gcos.OverridePersonalCosmetics || (!gcos.OverridePersonalCosmetics && pantsItem == 0))
                            pantsItem = gcos.Pants;
                    else if (gcos.Pants == -1) pantsItem = 0;
                }

                //blocked cosmetics by id
                List<int> blockedCosmetics = new List<int>();
                foreach (var str in MCustomCosmetics.Instance.Configuration.Instance.BlockedCosmetics)
                {
                    var strings = str.Split('-');
                    foreach(var c in strings)
                    {
                        if (int.TryParse(c, out int result))
                        {
                            blockedCosmetics.Add(result);
                        }
                    }
                }

                foreach (var b in blockedCosmetics)
                {
                    if (hatItem == b) hatItem = 0;
                    if (maskItem == b) maskItem = 0;
                    if (glassesItem == b) glassesItem = 0;
                    if (backpackItem == b) backpackItem = 0;
                    if (shirtItem == b) shirtItem = 0;
                    if (vestItem == b) vestItem = 0;
                    if (pantsItem == b) pantsItem = 0;
                }


                //blocked cosmetics by type
                if (!MCustomCosmetics.Instance.Configuration.Instance.AllowedCosmeticTypes.Hat) hatItem = 0;
                if (!MCustomCosmetics.Instance.Configuration.Instance.AllowedCosmeticTypes.Mask) maskItem = 0;
                if (!MCustomCosmetics.Instance.Configuration.Instance.AllowedCosmeticTypes.Glasses) glassesItem = 0;
                if (!MCustomCosmetics.Instance.Configuration.Instance.AllowedCosmeticTypes.Backpack) backpackItem = 0;
                if (!MCustomCosmetics.Instance.Configuration.Instance.AllowedCosmeticTypes.Shirt) shirtItem = 0;
                if (!MCustomCosmetics.Instance.Configuration.Instance.AllowedCosmeticTypes.Vest) vestItem = 0;
                if (!MCustomCosmetics.Instance.Configuration.Instance.AllowedCosmeticTypes.Pants) pantsItem = 0;
            }
        }
    }
}
