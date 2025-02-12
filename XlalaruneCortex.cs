using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace XlalaruneBoss
{
    [AutoloadEquip(EquipType.Head)]
    public class XlalaruneCortex : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 22;

            // Common values for every boss mask
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 75);
            Item.vanity = true;
            Item.maxStack = 1;
        }
    }
}
