using canjewelry.src.jewelry;
using canjewelry.src.CB;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Server;
using Vintagestory.GameContent;
using canjewelry.src.blocks;

namespace canjewelry.src
{
    public class canjewelry: ModSystem
    {
        public static Harmony harmonyInstance;
        public const string harmonyID = "canjewelry.Patches";
        public static ICoreClientAPI capi;
        public static ICoreServerAPI sapi;

        public static Dictionary<string, Dictionary<string, float>> gemBuffValuesByLevel;
        public static Dictionary<string, HashSet<string>> buffNameToPossibleItem;
        public override void Start(ICoreAPI api)
        {
            base.Start(api);
            harmonyInstance = new Harmony(harmonyID);

          
            harmonyInstance.Patch(typeof(Vintagestory.API.Common.CollectibleObject).GetMethod("GetHeldItemInfo"), postfix: new HarmonyMethod(typeof(harmPatch).GetMethod("Postfix_GetHeldItemInfo")));

            api.RegisterBlockClass("JewelerSetBlock", typeof(JewelerSetBlock));
            api.RegisterBlockEntityClass("JewelerSetBE", typeof(JewelerSetBE));

            api.RegisterCollectibleBehaviorClass("Encrustable", typeof(EncrustableCB));

            api.RegisterBlockClass("BlockJewelGrinder", typeof(BlockJewelGrinder));
            api.RegisterBlockEntityClass("BEJewelGrinder", typeof(BEJewelGrinder));

            api.RegisterBlockClass("GrindLayerBlock", typeof(GrindLayerBlock));
            api.RegisterItemClass("ProcessedGem", typeof(ProcessedGem));
            // api.ModLoader.Systems.ElementAt(1)

        }
        public override void StartClientSide(ICoreClientAPI api)
        {
            base.StartClientSide(api);

            harmPatch.preparedEncrustedGemsImages = new Dictionary<string, AssetLocation>();

            harmPatch.socketsTextureDict = new Dictionary<string, AssetLocation>();
            capi = api;
            harmonyInstance = new Harmony(harmonyID);
            harmPatch.socketsTextureDict.Add("socket-1", new AssetLocation("canjewelry:textures/item/tinbronze.png"));
            /* harmPatch.socketsTextureDict.Add("socket-bismuthbronze", new AssetLocation("game:textures/item/bismuthbronze.png"));
             harmPatch.socketsTextureDict.Add("socket-blackbronze", new AssetLocation("game:textures/item/blackbronze.png"));*/
            harmPatch.socketsTextureDict.Add("socket-2", new AssetLocation("canjewelry:textures/item/iron.png"));
            //harmPatch.socketsTextureDict.Add("socket-meteoriciron", new AssetLocation("game:textures/item/meteoriciron.png"));
            harmPatch.socketsTextureDict.Add("socket-3", new AssetLocation("canjewelry:textures/item/steel.png"));

            //capi.Assets.Get
            harmPatch.preparedEncrustedGemsImages.Add("diamond", new AssetLocation("canjewelry:item/gem/diamond.png"));

            harmPatch.preparedEncrustedGemsImages.Add("corundum", new AssetLocation("canjewelry:item/gem/corundum.png"));

            harmPatch.preparedEncrustedGemsImages.Add("emerald", new AssetLocation("canjewelry:item/gem/emerald.png"));

            harmPatch.preparedEncrustedGemsImages.Add("fluorite", new AssetLocation("canjewelry:item/gem/fluorite.png"));

            harmPatch.preparedEncrustedGemsImages.Add("lapislazuli", new AssetLocation("canjewelry:item/gem/lapislazuli.png"));

            harmPatch.preparedEncrustedGemsImages.Add("malachite", new AssetLocation("canjewelry:item/gem/malachite.png"));

            harmPatch.preparedEncrustedGemsImages.Add("olivine", new AssetLocation("canjewelry:item/gem/olivine.png"));

            harmPatch.preparedEncrustedGemsImages.Add("quartz", new AssetLocation("canjewelry:item/gem/quartz.png"));

            harmPatch.preparedEncrustedGemsImages.Add("uranium", new AssetLocation("canjewelry:item/gem/uranium.png"));
            //ItemSlot inSlot, double posX, double posY, double posZ, float size, int color, float dt, bool shading = true, bool origRotate = false, bool showStackSize = true
            harmonyInstance.Patch(typeof(Vintagestory.API.Client.GuiElementItemSlotGridBase).GetMethod("ComposeSlotOverlays", BindingFlags.NonPublic | BindingFlags.Instance), transpiler: new HarmonyMethod(typeof(harmPatch).GetMethod("Transpiler_ComposeSlotOverlays_Add_Socket_Overlays_Not_Draw_ItemDamage")));
        }
        public void onPlayerPlaying(IServerPlayer byPlayer)
        {
            IInventory charakterInv = byPlayer.InventoryManager.GetOwnInventory("character");
            InventoryBasePlayer playerHotbar = (InventoryBasePlayer)byPlayer.InventoryManager.GetOwnInventory("hotbar");
            charakterInv.SlotModified += (int slotId) => {
                if (charakterInv[slotId].Itemstack != null && charakterInv[slotId].Itemstack.Attributes.HasAttribute("canencrusted"))
                {
                    //do stuff
                }
            };
            playerHotbar.SlotModified += (int slotId) => {
                if (playerHotbar[slotId].Itemstack != null && playerHotbar[slotId].Itemstack.Attributes.HasAttribute("canencrusted"))
                {
                    ITreeAttribute tree = playerHotbar[slotId].Itemstack.Attributes.GetTreeAttribute("canencrusted");
                    for (int i = 0; i < tree.GetInt("socketsnumber"); i++)
                    {
                        ITreeAttribute treeSocket = tree.GetTreeAttribute("slot" + i);
                        if (treeSocket.GetInt("size") > 0)
                        {

                        }
                    }
                }
            };
        }
        public static void onPlayerRespawnRecalculateGemsBuffs(IServerPlayer player)
        {
            //go through all stats and delete "canencrusted" part
            foreach (KeyValuePair<string, EntityFloatStats> stat in player.Entity.Stats)
            {
                foreach (KeyValuePair<string, EntityStat<float>> keyValuePair in stat.Value.ValuesByKey)
                {
                    if (keyValuePair.Key == "canencrusted")
                    {
                        stat.Value.Set(keyValuePair.Key, 0);
                        //stat.Value.Remove(keyValuePair.Key);
                        break;
                    }
                }
                player.Entity.WatchedAttributes.MarkPathDirty("stats");
            }
            //go through hotbar active slot, character slots and apply all buffs
            IInventory playerBackpacks = player.InventoryManager.GetHotbarInventory();
            {
                //playerBackpacks.Player
                if (playerBackpacks != null)
                {
                    for (int i = 0; i < playerBackpacks.Count; ++i)
                    {
                        if (i != player.InventoryManager.ActiveHotbarSlotNumber || playerBackpacks[i].Itemstack == null || playerBackpacks[i].Itemstack.Item is ItemWearable)
                        {
                            continue;
                        }
                        if (playerBackpacks[i] != null)
                        {
                            ItemSlot itemSlot = playerBackpacks[i];
                            ItemStack itemStack = itemSlot.Itemstack;
                            if (itemStack != null)
                            {
                                if (itemStack.Attributes.HasAttribute("canencrusted"))
                                {
                                    ITreeAttribute encrustTree = itemStack.Attributes.GetTreeAttribute("canencrusted");
                                    if (encrustTree == null)
                                    {
                                        return;
                                    }
                                    for (int j = 0; j < encrustTree.GetInt("socketsnumber"); j++)
                                    {
                                        ITreeAttribute socketSlot = encrustTree.GetTreeAttribute("slot" + j.ToString());
                                        if (!socketSlot.HasAttribute("attributeBuff"))
                                        {
                                            continue;
                                        }
                                        if (player.Entity.Stats[socketSlot.GetString("attributeBuff")].ValuesByKey.ContainsKey("canencrusted"))
                                        {
                                            player.Entity.Stats.Set(socketSlot.GetString("attributeBuff"), "canencrusted", player.Entity.Stats[socketSlot.GetString("attributeBuff")].ValuesByKey["canencrusted"].Value + socketSlot.GetFloat("attributeBuffValue"), true);
                                        }
                                        else
                                        {
                                            player.Entity.Stats.Set(socketSlot.GetString("attributeBuff"), "canencrusted", socketSlot.GetFloat("attributeBuffValue"), true);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            IInventory charakterInv = player.InventoryManager.GetOwnInventory("character");
            {
                //playerBackpacks.Player
                if (charakterInv != null)
                {
                    for (int i = 0; i < 4; ++i)
                    {
                        if (charakterInv[i] != null)
                        {
                            ItemSlot itemSlot = charakterInv[i];
                            ItemStack itemStack = itemSlot.Itemstack;
                            if (itemStack != null)
                            {
                                if (itemStack.Attributes.HasAttribute("canencrusted"))
                                {
                                    ITreeAttribute encrustTree = itemStack.Attributes.GetTreeAttribute("canencrusted");
                                    if (encrustTree == null)
                                    {
                                        return;
                                    }
                                    for (int j = 0; j < encrustTree.GetInt("socketsnumber"); j++)
                                    {
                                        ITreeAttribute socketSlot = encrustTree.GetTreeAttribute("slot" + j.ToString());
                                        if (!socketSlot.HasAttribute("attributeBuff"))
                                        {
                                            continue;
                                        }
                                        if (player.Entity.Stats[socketSlot.GetString("attributeBuff")].ValuesByKey.ContainsKey("canencrusted"))
                                        {
                                            player.Entity.Stats.Set(socketSlot.GetString("attributeBuff"), "canencrusted", player.Entity.Stats[socketSlot.GetString("attributeBuff")].ValuesByKey["canencrusted"].Value + socketSlot.GetFloat("attributeBuffValue"), true);
                                        }
                                        else
                                        {
                                            player.Entity.Stats.Set(socketSlot.GetString("attributeBuff"), "canencrusted", socketSlot.GetFloat("attributeBuffValue"), true);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        public void initBuffNameToPossibleItem()
        {
            buffNameToPossibleItem.Add("diamond", new HashSet<string>() { "brigandine", "plate", "chain", "scale" }); //walkspeed
            buffNameToPossibleItem.Add("corundum", new HashSet<string>() { "pickaxe" });//miningSpeedMul
            buffNameToPossibleItem.Add("emerald", new HashSet<string>() { "brigandine", "plate", "chain", "scale" });//maxhealthExtraPoints
            buffNameToPossibleItem.Add("fluorite", new HashSet<string>() { "halberd", "mace", "spear", "rapier", "longsword", "zweihander", "messer" });//meleeWeaponsDamage
            buffNameToPossibleItem.Add("lapislazuli", new HashSet<string>() { "brigandine", "plate", "chain", "scale" });//hungerrate
            buffNameToPossibleItem.Add("malachite", new HashSet<string>() { "brigandine", "plate", "chain", "scale", "knife" });//wildCropDropRate
            buffNameToPossibleItem.Add("olivine", new HashSet<string>() { "brigandine", "plate", "chain", "scale" });//armorDurabilityLoss
            buffNameToPossibleItem.Add("quartz", new HashSet<string>() { "pickaxe" });//oreDropRate
            buffNameToPossibleItem.Add("uranium", new HashSet<string>() { "brigandine", "plate", "chain", "scale" });//healingeffectivness
        }
        public override void StartServerSide(ICoreServerAPI api)
        {
            base.StartServerSide(api);

            gemBuffValuesByLevel = new Dictionary<string, Dictionary<string, float>>();
            buffNameToPossibleItem = new Dictionary<string, HashSet<string>>();
            harmonyInstance = new Harmony(harmonyID);
            sapi = api;

           
            harmonyInstance.Patch(typeof(Vintagestory.API.Common.ItemSlot).GetMethod("TryPutInto", new[] { typeof(ItemSlot), typeof(ItemStackMoveOperation).MakeByRefType() }), postfix: new HarmonyMethod(typeof(harmPatch).GetMethod("Postfix_Collectible_DidModifyItemSlot")));
            harmonyInstance.Patch(typeof(Vintagestory.API.Common.ItemSlot).GetMethod("TakeOut"), prefix: new HarmonyMethod(typeof(harmPatch).GetMethod("Postfix_ItemSlot_TakeOut")));
            harmonyInstance.Patch(typeof(Vintagestory.API.Common.ItemSlot).GetMethod("TryFlipWith"), postfix: new HarmonyMethod(typeof(harmPatch).GetMethod("Postfix_ItemSlot_TryFlipWith")));
            harmonyInstance.Patch(typeof(Vintagestory.Server.CoreServerEventManager).GetMethod("TriggerAfterActiveSlotChanged"), postfix: new HarmonyMethod(typeof(harmPatch).GetMethod("Postfix_TriggerAfterActiveSlotChanged")));

            harmonyInstance.Patch(typeof(Vintagestory.API.Common.ItemSlot).GetMethod("ActivateSlotLeftClick", BindingFlags.NonPublic | BindingFlags.Instance), postfix: new HarmonyMethod(typeof(harmPatch).GetMethod("Postfix_ItemSlot_ActivateSlotLeftClick")));
            //ActivateSlotRightClick
            harmonyInstance.Patch(typeof(Vintagestory.API.Common.ItemSlot).GetMethod("ActivateSlotRightClick", BindingFlags.NonPublic | BindingFlags.Instance), postfix: new HarmonyMethod(typeof(harmPatch).GetMethod("Postfix_ItemSlot_ActivateSlotLeftClick")));

           
            //api.RegisterCommand("can", "", "", canHandlerCommand);
            //api.RegisterCommand("canadm", "", "", canAdmHandlerCommand);


            gemBuffValuesByLevel = api.Assets.Get("config/gems-buffs.json").ToObject<Dictionary<string, Dictionary<string, float>>>();

            api.Event.PlayerNowPlaying += onPlayerPlaying;
            api.Event.PlayerRespawn += onPlayerRespawnRecalculateGemsBuffs;
            initBuffNameToPossibleItem();

        }

    }
}
