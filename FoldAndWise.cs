using Terraria;
using Terraria.ModLoader;

namespace XlalaruneBoss
{
    public class FoldAndWise : ModItem 
    {

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 32;
            Item.height = 32;
            Item.maxStack = 1;
            Item.value = Item.sellPrice(0, 10);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<AccessoryPlayer>().HasFoldAndWise = true;
        }

    }
}
