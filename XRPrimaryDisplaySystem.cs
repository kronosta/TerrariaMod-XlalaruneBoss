using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace XlalaruneBoss
{
    public class XRPrimaryDisplaySystem : ModSystem
    {
        public static Texture2D ToDisplay;
        public static float ToDisplayScale = 1f;
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "XlalaruneBoss: XR Primary Messages",
                    delegate
                    {
                        DrawXlalaruneMessage(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }

        public void DrawXlalaruneMessage(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (ToDisplay == null) return;

            int drawPositionX = Main.screenWidth / 2;
            int drawPositionY = Main.screenHeight / 2;
            drawPositionX -= (int)(ToDisplay.Width * ToDisplayScale / 2);
            drawPositionY -= (int)(ToDisplay.Height * ToDisplayScale / 2);
            spriteBatch.Draw(
                ToDisplay,
                new Vector2(drawPositionX, drawPositionY),
                null,
                Color.White,
                0f,
                new Vector2(0f, 0f),
                ToDisplayScale,
                SpriteEffects.None,
                0f
            );
        }

        public override void OnWorldUnload()
        {
            if (XRPrimary.MainInstance != null && XRPrimary.MainInstance.musicSync != null)
            {
                if (XRPrimary.MainInstance.musicSync.Enabled) XRPrimary.MainInstance.musicSync.Stop();
                XRPrimary.MainInstance.musicSync.Dispose();
                XRPrimary.MainInstance.musicSync = null;
            }
            XRPrimary.MainInstance = null;
            ToDisplay = null;
            ToDisplayScale = 1f;
        }
    }
}
