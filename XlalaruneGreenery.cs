using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace XlalaruneBoss
{
    [AutoloadEquip(EquipType.Legs)]
    public class XlalaruneGreenery : ModItem
    {
        public override void SetStaticDefaults()
        {
            ArmorIDs.Legs.Sets.HidesBottomSkin[EquipLoader.GetEquipSlot(Mod, "XlalaruneGreenery", EquipType.Legs)] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;

            // Common values for every boss mask
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 75);
            Item.vanity = true;
            Item.maxStack = 1;
        }
    }
}
