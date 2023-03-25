using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;

namespace canjewelry.src.jewelry
{
    public class GuiDialogJewelerSet : GuiDialogBlockEntity
    {
        GuiElementHorizontalTabs groupOfInterests;
        public GuiDialogJewelerSet(string dialogTitle, InventoryBase inventory, BlockPos blockEntityPos, ICoreClientAPI capi) : base(dialogTitle, inventory, blockEntityPos, capi)
        {            
            if (IsDuplicate)
            {
                return;
            }
            capi.World.Player.InventoryManager.OpenInventory((IInventory)inventory);
            SetupDialog();
        }
        public void SetupDialog()
        {
            int chosenGroupTab = groupOfInterests == null ? 0 : groupOfInterests.activeElement;
            int fixedY1 = 60;
            ElementBounds elementBounds = ElementStdBounds.AutosizedMainDialog.WithAlignment(EnumDialogArea.CenterMiddle);
            ElementBounds bounds1 = ElementBounds.Fill.WithFixedPadding(GuiStyle.ElementToDialogPadding);
            ElementBounds bounds2 = ElementBounds.FixedPos(EnumDialogArea.LeftTop, 100, 40).WithFixedHeight(30.0).WithFixedWidth(140);
            int fixedY2 = fixedY1 + 28;
            ElementBounds bounds3 = ElementBounds.FixedPos(EnumDialogArea.LeftTop, bounds1.fixedX + GuiElement.scaledi(10), bounds1.fixedY + GuiElement.scaledi(50)).WithFixedHeight(GuiElement.scaledi(70)).WithFixedWidth(GuiElement.scaledi(305));

            ElementBounds bounds4 = ElementBounds.FixedPos(EnumDialogArea.LeftTop, 160, 80).WithFixedHeight(150).WithFixedWidth(250);

            ElementBounds bounds5 = ElementBounds.FixedPos(EnumDialogArea.LeftTop, GuiElement.scaledi(10), GuiElement.scaledi(10)).WithFixedHeight(GuiElement.scaledi(80)).WithFixedWidth(GuiElement.scaledi(80));
            ElementBounds bounds6 = ElementBounds.FixedPos(EnumDialogArea.CenterTop, 0, GuiElement.scaledi(10)).WithFixedHeight(GuiElement.scaledi(40)).WithFixedWidth(GuiElement.scaledi(160)); ;
            bounds3.WithChild(bounds5);
            bounds3.WithChild(bounds6);
            int fixedY3 = fixedY2 + 28;
            // ElementBounds bounds4 = ElementBounds.Fixed(0.0, (double)fixedY3, 140.0, 200.0);
            //int fixedY4 = fixedY3 + 4;
            //ElementBounds bounds5 = ElementBounds.FixedOffseted(EnumDialogArea.LeftBottom, 20.0, -12.0, 100.0, 24.0);
            elementBounds.BothSizing = ElementSizing.FitToChildren;
            elementBounds.WithChild(bounds1);
            bounds1.BothSizing = ElementSizing.FitToChildren;
            GuiTab[] tabs1 = new GuiTab[2];

            tabs1[0] = new GuiTab();
            tabs1[0].Name = "Sockets";
            tabs1[0].DataInt = 0;

            tabs1[1] = new GuiTab();
            tabs1[1].Name = "Gems";
            tabs1[1].DataInt = 1;

            this.SingleComposer = this.capi.Gui.CreateCompo("fgtabledlg" + this.BlockEntityPosition?.ToString(), elementBounds).
                  AddShadedDialogBG(bounds1).
                  AddDialogTitleBar(Lang.Get("canjewelry:jewelset_gui_name"), new Action(this.OnTitleBarClose));
            SingleComposer.AddHorizontalTabs(tabs1, bounds2, new Action<int>(this.OnGroupTabClicked), CairoFont.WhiteSmallText(), CairoFont.WhiteSmallText(), "GroupTabs");
            this.groupOfInterests = this.SingleComposer.GetHorizontalTabs("GroupTabs");
            this.groupOfInterests.activeElement = chosenGroupTab;
            bounds1.WithChildren(bounds2, bounds3, bounds4);
            if (this.groupOfInterests.activeElement == 0)
            {

                bounds3.WithFixedWidth(GuiElement.scaledi(200));
                bounds6.WithFixedWidth(GuiElement.scaledi(40));

                double elementToDialogPadding = GuiStyle.ElementToDialogPadding;
                double unscaledSlotPadding = GuiElementItemSlotGridBase.unscaledSlotPadding;
                //ElementBounds elementBoundsbig = ElementStdBounds.SlotGrid(EnumDialogArea.None, 0.0, 40.0, 3, 3).FixedGrow(unscaledSlotPadding);


                SingleComposer.AddItemSlotGrid((IInventory)this.Inventory, new Action<object>(((GuiDialogJewelerSet)this).DoSendPacket), 1, new int[1]
                {
                   0
                }, bounds5, "craftinggrid");

                       SingleComposer.AddItemSlotGrid((IInventory)this.Inventory, new Action<object>(((GuiDialogJewelerSet)this).DoSendPacket), 1, new int[1]
               {

                   1
               }, bounds6, "craftinggrid2");
                SingleComposer.AddInset(bounds3);
                SingleComposer.AddButton(Lang.Get("canjewelry:gui_add_socket"), () => onClickBackButtonPutSocket(), ElementBounds.Fixed(55, (double)bounds2.fixedY + 150, 100, 40), CairoFont.WhiteDetailText().WithFontSize(16));
            }
            else
            {
                //gems input
                
                double elementToDialogPadding = GuiStyle.ElementToDialogPadding;
                double unscaledSlotPadding = GuiElementItemSlotGridBase.unscaledSlotPadding;
                ElementBounds elementBoundsbig = ElementStdBounds.SlotGrid(EnumDialogArea.None, 100.0, 40.0, 4, 3).FixedGrow(unscaledSlotPadding);



                SingleComposer.AddItemSlotGrid((IInventory)this.Inventory, new Action<object>(((GuiDialogJewelerSet)this).DoSendPacket), 1, new int[1]
                            {
                                0
                            }, bounds5, "encrusteditem");
                if (this.Inventory.Count > 1)
                {
                    if(this.Inventory[0].Itemstack != null)
                    {
                        if (this.Inventory[0].Itemstack.Attributes.HasAttribute("canencrusted"))
                        {

                            //tree
                            var canencrustedTree = this.Inventory[0].Itemstack.Attributes.GetTreeAttribute("canencrusted");
                            if (canencrustedTree.GetInt("socketsnumber") > 0)
                            {
                                int[] intArr = new int[canencrustedTree.GetInt("socketsnumber")];
                                for(int i = 0; i < intArr.Length; i++)
                                {
                                    intArr[i] = i + 1;
                                }
                                SingleComposer.AddItemSlotGrid((IInventory)this.Inventory, new Action<object>(((GuiDialogJewelerSet)this).DoSendPacket), 4, intArr, bounds6, "socketsslots");
                            }
                        }
                    }
                }
                SingleComposer.AddInset(bounds3);
               // SingleComposer.AddInset(bounds6);
                SingleComposer.AddButton(Lang.Get("canjewelry:gui_add_gem"), () => onClickBackButtonPutGem(), ElementBounds.Fixed(55, (double)bounds2.fixedY + 150, 90, 40), CairoFont.WhiteDetailText().WithFontSize(16));


            }      
            SingleComposer.Compose();
            this.SingleComposer.UnfocusOwnElements();


            /*double elementToDialogPadding = GuiStyle.ElementToDialogPadding;
            double unscaledSlotPadding = GuiElementItemSlotGridBase.unscaledSlotPadding;
            ElementBounds bounds1 = ElementBounds.Fill.WithFixedPadding(GuiStyle.ElementToDialogPadding);
            ElementBounds elementBounds = ElementStdBounds.SlotGrid(EnumDialogArea.None, 0.0, 40.0, 3, 3).FixedGrow(unscaledSlotPadding);
            ElementBounds bounds2 = ElementStdBounds.SlotGrid(EnumDialogArea.None, 60.0, 90.0, 1, 1).RightOf(elementBounds, 50.0).FixedGrow(unscaledSlotPadding);
            ElementBounds bounds3 = ElementBounds.FixedOffseted(EnumDialogArea.None, 0.0, 40.0, 20.0, 20.0).RightOf(elementBounds, 20.0);
            bounds1.BothSizing = ElementSizing.FitToChildren;
            bounds1.WithChildren(elementBounds, bounds3, bounds2);
            ElementBounds bounds4 = ElementStdBounds.AutosizedMainDialog.WithAlignment(EnumDialogArea.RightMiddle).WithFixedAlignmentOffset(-GuiStyle.DialogToScreenPadding, 0.0);
           
            this.SingleComposer = GuiComposerHelpers.
                AddShadedDialogBG(this.capi.Gui.CreateCompo("fgtabledlg" + this.BlockEntityPosition?.ToString(), bounds4), bounds1, true, 5.0).
                AddDialogTitleBar("Crafting Table", new Action(this.OnTitleBarClose));
            //SingleComposer.AddStaticText(Lang.Get("claimsext:gui-type-name"), CairoFont.WhiteSmallishText(), ElementBounds.Fixed(60, (double)bounds3.fixedY + 20, 200, 20));
            SingleComposer.AddTextInput(ElementBounds.Fixed(40, (double)bounds3.fixedY + 60, 220, 34), (name) => collectedStringValue = name, null, "collectedValue");
            SingleComposer.AddButton(Lang.Get("canmods:gui-rename-ok"), () => onClickRenameCollectible(), ElementBounds.Fixed(80, (double)bounds3.fixedY + 120, 90, 40));
            SingleComposer.Compose();
            this.SingleComposer.UnfocusOwnElements();*/
        }
        private void OnTitleBarClose() => this.TryClose();
        public override void OnGuiClosed()
        {
           // var a = this.opened;
            //this.Inventory.SlotModified -= new Action<int>(this.OnSlotModified);
            this.capi.Network.SendPacketClient(this.capi.World.Player.InventoryManager.CloseInventory((IInventory)this.Inventory));
           // this.SingleComposer.GetSlotGrid("craftinggrid").OnGuiClosed(this.capi);
           // this.SingleComposer.GetSlotGrid("outputslot").OnGuiClosed(this.capi);
            base.OnGuiClosed();
        }
        private void OnGroupTabClicked(int clicked)
        {
            this.groupOfInterests.activeElement = clicked;
            this.SetupDialog();
        }
        public bool onClickBackButtonPutSocket()
        {
            this.capi.Network.SendBlockEntityPacket(this.BlockEntityPosition, 1004);
            //this.chosenCommand = enumChosenCommand.NO_CHOSEN_COMMAND;
            // this.buildWindow();
            return true;
        }
        public bool onClickBackButtonPutGem()
        {
            this.capi.Network.SendBlockEntityPacket(this.BlockEntityPosition, 1005);
            //this.chosenCommand = enumChosenCommand.NO_CHOSEN_COMMAND;
            // this.buildWindow();
            return true;
        }
    }
}
