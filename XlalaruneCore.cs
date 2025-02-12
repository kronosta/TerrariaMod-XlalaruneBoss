using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace XlalaruneBoss
{
    [AutoloadEquip(EquipType.Body)]
    public class XlalaruneCore : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 16;

            // Common values for every boss mask
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 75);
            Item.vanity = true;
            Item.maxStack = 1;
        }
    }
}
