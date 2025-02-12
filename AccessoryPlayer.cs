using Terraria.ModLoader;

namespace XlalaruneBoss
{
    public class AccessoryPlayer : ModPlayer
    {
        public bool HasFoldAndWise;

        public override void ResetEffects()
        {
            HasFoldAndWise = false;
        }
    }
}
