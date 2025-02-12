using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace XlalaruneBoss
{
    [AutoloadBossHead]
    public class XRPrimary : ModNPC
    {

        public Timer musicSync = null;
        public static XRPrimary MainInstance = null;

        public int SyncIndex = 0;
        public int PhaseXlalaruneBase = 0;
        public int PhaseXlalaruneVariationLength = 64;
        public int PhaseXlalaruneVariationCount = 4;
        public int ProjectileCooldown = 0;
        public bool CloneDisable = false;
        public Random rand = new Random((int)DateTime.Now.Ticks);

        public float FireAngle
        {
            get => NPC.ai[0];
            set => NPC.ai[0] = value;
        }

        // Just in case I get something wrong
        public static double SyncMultiplier = 1.0;
        public static double SyncPreAdder = 0.0;
        public static double SyncPostAdder = 0;

        public static int Sync => (int)((MainInstance.SyncIndex + SyncPreAdder) * SyncMultiplier + SyncPostAdder);
        public static int NumClones => Main.npc.Where(x => x.active && x.type == ModContent.NPCType<XRPrimary>()).Count();

        public override void OnSpawn(IEntitySource source)
        {
            FireAngle = 0f;

            if (MainInstance == null)
            {
                MainInstance = this;
                musicSync = new Timer(50); // Runs 20 times per second to increment a value.
                musicSync.Start();
                musicSync.Elapsed += (sender, e) =>
                {
                    if (!Main.gameInactive)
                    {
                        SyncIndex += 1;
                        if (SyncIndex >= 5140) SyncIndex = 0;
                        if (!MainInstance.NPC.active)
                        {
                            MainInstance.musicSync.Stop();
                            MainInstance.musicSync.Dispose();
                            MainInstance.musicSync = null;
                            MainInstance = null;
                        }
                    }
                };
            }

            MainInstance.musicSync.Elapsed += (sender, e) =>
            {
                this.ProjectileCooldown--;
                if (this.ProjectileCooldown < 0) this.ProjectileCooldown = this == MainInstance ? 8 : 10;
            };

            if (source is EntitySource_Parent)
            {
                Vector2 spawnPosition = ((EntitySource_Parent)source).Entity.position + new Vector2(5f, 0f);
                NPC.Teleport(spawnPosition);
            }
        }

        public override void OnKill()
        {
            if (this == MainInstance)
            {
                MainInstance = null;
                this.musicSync.Stop();
                this.musicSync.Dispose();
                this.musicSync = null;
                ResetMessageDisplay();
            }
        }

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 4;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
        }

        public override void SetDefaults()
        {
            NPC.width = 256;
            NPC.height = 198;
            NPC.scale = 3f;
            NPC.damage = 0;
            NPC.defense = 17;
            NPC.lifeMax = 262144;
            NPC.HitSound = SoundID.MenuOpen;
            NPC.DeathSound = SoundID.MenuClose;
            NPC.knockBackResist = 0f;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.value = Item.buyPrice(gold: 16);
            NPC.boss = true;
            NPC.npcSlots = 10f;
            NPC.aiStyle = -1;
            NPC.BossBar = ModContent.GetInstance<XRPrimaryBossBar>();

            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot("XlalaruneBoss/Music/XRPrimaryTheme");
                Main.musicNoCrossFade[Music] = true;
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            LeadingConditionRule hardmode = new LeadingConditionRule(new Conditions.IsHardmode());
            hardmode.OnSuccess(ItemDropRule.Common(ItemID.CobaltOre, 1, 10, 30));
            hardmode.OnSuccess(ItemDropRule.Common(ItemID.PalladiumOre, 1, 10, 30));
            hardmode.OnSuccess(ItemDropRule.Common(ItemID.MythrilOre, 1, 10, 30));
            hardmode.OnSuccess(ItemDropRule.Common(ItemID.OrichalcumOre, 1, 10, 30));
            hardmode.OnSuccess(ItemDropRule.Common(ItemID.AdamantiteOre, 1, 10, 30));
            hardmode.OnSuccess(ItemDropRule.Common(ItemID.TitaniumOre, 1, 10, 30));
            hardmode.OnFailedConditions(ItemDropRule.Common(ItemID.GoldOre, 1, 10, 30));
            hardmode.OnFailedConditions(ItemDropRule.Common(ItemID.PlatinumOre, 1, 10, 30));
            hardmode.OnFailedConditions(ItemDropRule.Common(ItemID.DemoniteOre, 1, 10, 30));
            hardmode.OnFailedConditions(ItemDropRule.Common(ItemID.CrimtaneOre, 1, 10, 30));
            hardmode.OnFailedConditions(ItemDropRule.Common(ItemID.Obsidian, 1, 10, 30));
            hardmode.OnFailedConditions(ItemDropRule.Common(ItemID.Hellstone, 1, 10, 30));
            hardmode.OnFailedConditions(ItemDropRule.Common(ItemID.Meteorite, 1, 10, 30));

            npcLoot.Add(hardmode);
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<XRPrimaryBossBag>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FoldAndWise>(), 15));
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>
            {
                new MoonLordPortraitBackgroundProviderBestiaryInfoElement(),
                new FlavorTextBestiaryInfoElement(
                    "A sapient chimera of plant and machine, unaggressive and neutral except in case of a threat to the ecosystem or its kind."
                )
            });
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = ImmunityCooldownID.Bosses;
            return true;
        }

        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            if (projectile.type == ProjectileID.GreenLaser) return false;
            return null;
        }

        public override void FindFrame(int frameHeight)
        {
            int startFrame = 0;
            int finalFrame = 3;

            int frameSpeed = 5;
            NPC.frameCounter += 0.5;
            if (NPC.frameCounter > frameSpeed)
            {
                NPC.frameCounter = 0.0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y > finalFrame * frameHeight)
                {
                    NPC.frame.Y = startFrame * frameHeight;
                }
            }
        }

        public override void AI()
        {
            if (MainInstance == null && Main.netMode != NetmodeID.MultiplayerClient)
                this.NPC.StrikeInstantKill();
            if (MainInstance == null) return;
            int sync = Sync;
            if (sync <= 1020)
            {
                ResetMessageDisplay();
                Phase_WeHaveTheRightYouKnow();
            }
            else if (sync <= 1525)
            {
                PhaseXlalaruneBase = 1021;
                Phase_Xlalarune(sync);
            }
            else if (sync <= 1786)
            {
                ResetMessageDisplay();
                Phase_WeHaveTheRightYouKnow();
            }
            else if (sync <= 2298)
            {
                PhaseXlalaruneBase = 1789;
                Phase_Xlalarune(sync);
            }
            else if (sync <= 3064)
            {
                ResetMessageDisplay();
                Phase_RiemannHypothesis();
            }
            else if (sync <= 4090)
            {
                PhaseXlalaruneBase = 3067;
                Phase_Xlalarune(sync);
            }
            else if (sync <= 4602)
            {
                ResetMessageDisplay();
                Phase_WeHaveTheRightYouKnow();
            }
            else if (sync <= 5140)
            {
                ResetMessageDisplay();
                Phase_RiemannHypothesis();
            }
            else
            {
                ResetMessageDisplay();
                MainInstance.SyncIndex = 0;
            }
        }

        public void FireProjectileWithFireAngle(float unrotatedVelocityX, float unrotatedVelocityY)
        {
            Vector2 velocity = new Vector2(unrotatedVelocityX, unrotatedVelocityY).RotatedBy(FireAngle);
            Vector2 position = NPC.Center + velocity * 5f;
            Main.projectile[Projectile.NewProjectile(
                new EntitySource_Parent(this.NPC),
                position,
                velocity,
                ProjectileID.GreenLaser,
                40,
                32f
            )].hostile = true;
        }

        public void ResetMessageDisplay()
        {
            XRPrimaryDisplaySystem.ToDisplay = null;
            XRPrimaryDisplaySystem.ToDisplayScale = 1f;
        }

        public void Phase_WeHaveTheRightYouKnow()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (this == MainInstance)
                {
                    FireAngle += rand.NextSingle() * 0.2f;
                    if (FireAngle > MathHelper.Pi * 2) FireAngle = 0;
                    if (ProjectileCooldown == 0)
                    {
                        FireProjectileWithFireAngle(-2f, 0f);
                        FireProjectileWithFireAngle(2f, 0f);
                        FireProjectileWithFireAngle(0f, 2f);
                        FireProjectileWithFireAngle(0f, -2f);
                        FireProjectileWithFireAngle(1f, 1f);
                        FireProjectileWithFireAngle(1f, -1f);
                        FireProjectileWithFireAngle(-1f, 1f);
                        FireProjectileWithFireAngle(-1f, -1f);
                    }
                }
                else
                {
                    FireAngle += rand.NextSingle() * 0.15f;
                    if (FireAngle > MathHelper.Pi * 2) FireAngle = 0;
                    if (ProjectileCooldown == 0)
                        FireProjectileWithFireAngle(3f, 0f);
                }
            }
        }

        public void Phase_Xlalarune(int sync)
        {
            int displayTime = sync - PhaseXlalaruneBase;
            int displayTimeMod = displayTime % (PhaseXlalaruneVariationCount * PhaseXlalaruneVariationLength);
            int displayTimeVariationMod = displayTime % PhaseXlalaruneVariationLength;

            if (Main.netMode != NetmodeID.Server)
            {
                if (displayTimeVariationMod <= 8)
                {
                    XRPrimaryDisplaySystem.ToDisplay = Mod.Assets.Request<Texture2D>("XRPrimaryMessages/X").Value;
                    XRPrimaryDisplaySystem.ToDisplayScale = 3f;
                }
                else if (displayTimeVariationMod <= 14)
                {
                    XRPrimaryDisplaySystem.ToDisplay = Mod.Assets.Request<Texture2D>("XRPrimaryMessages/Lala").Value;
                    XRPrimaryDisplaySystem.ToDisplayScale = 3f;
                }
                else if (displayTimeVariationMod <= 22)
                {
                    XRPrimaryDisplaySystem.ToDisplay = Mod.Assets.Request<Texture2D>("XRPrimaryMessages/Rune").Value;
                    XRPrimaryDisplaySystem.ToDisplayScale = 3f;
                }
                else
                {
                    XRPrimaryDisplaySystem.ToDisplay = null;
                    XRPrimaryDisplaySystem.ToDisplayScale = 1f;
                }
            }

            if (displayTimeMod == 96 && MainInstance == this)
            {
                ReplacePlayerHandWithGuitar();
                CloneDisable = false;
            }

            if (displayTimeMod == 224 && MainInstance == this)
            {
                if (!CloneDisable) SummonCloneAtPlayer();
                CloneDisable = true;
            }
        }

        public void Phase_RiemannHypothesis ()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                FireAngle += rand.NextSingle() * 0.18f;
                if (FireAngle > MathHelper.Pi * 2) FireAngle = 0;
                if (ProjectileCooldown == 0)
                {
                    for (float i = -2f; i <= 2f; i++)
                    {
                        Main.projectile[Projectile.NewProjectile(
                            new EntitySource_Parent(this.NPC),
                            this.NPC.Center + new Vector2(-3f, i),
                            new Vector2(-2.5f, 0f).RotatedBy(FireAngle),
                            ProjectileID.GreenLaser,
                            40,
                            32f
                        )].hostile = true;
                        Main.projectile[Projectile.NewProjectile(
                            new EntitySource_Parent(this.NPC),
                            this.NPC.Center + new Vector2(3f, i),
                            new Vector2(2.5f, 0f).RotatedBy(FireAngle),
                            ProjectileID.GreenLaser,
                            40,
                            32f
                        )].hostile = true;
                        Main.projectile[Projectile.NewProjectile(
                            new EntitySource_Parent(this.NPC),
                            this.NPC.Center + new Vector2(i, 3f),
                            new Vector2(0f, 2.5f).RotatedBy(FireAngle),
                            ProjectileID.GreenLaser,
                            40,
                            32f
                        )].hostile = true;
                    }
                }
            }
        }

        public void ReplacePlayerHandWithGuitar()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                if (!Main.LocalPlayer.GetModPlayer<AccessoryPlayer>().HasFoldAndWise)
                {
                    Item guitar = new Item(ItemID.IvyGuitar);
                    Main.LocalPlayer.inventory[Main.LocalPlayer.selectedItem] = guitar;
                }
                else
                {
                    SoundStyle foldAndWise = new SoundStyle("XlalaruneBoss/Sounds/FoldAndWise");
                    SoundEngine.PlaySound(foldAndWise);
                }
            }
        }

        public void SummonCloneAtPlayer()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient && NumClones < 5)
            {
                Player[] allPlayers = Main.player.Where(x => x.active).ToArray();
                Player selectedPlayer = allPlayers[rand.Next(allPlayers.Length)];
                int posX = (int)selectedPlayer.Center.X;
                int posY = (int)selectedPlayer.Center.Y;
                NPC.NewNPC(
                    new EntitySource_SpawnNPC(),
                    posX + 10,
                    posY,
                    ModContent.NPCType<XRPrimary>()
                );
                return;
            }
        }
    }
}
