using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using VoidRains.Common;
using VoidRains.Helpers.StateManagement;

namespace VoidRains.Content.Bosses.BlueVeyeral;

[AutoloadBossHead]
public partial class BlueVeyeral : BossNPC
{
    public override void SetStaticDefaults()
    {
        NPCID.Sets.BossBestiaryPriority.Add(Type);
        NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.ShadowFlame] = true;
    }

    public override void SetDefaults()
    {
        // Slightly smaller than the sprite
        NPC.width = 150;
        NPC.height = 150;
        NPC.damage = 50;
        NPC.defense = 30;
        NPC.lifeMax = 75_000;
        NPC.HitSound = SoundRegistry.VoidRainsHit;
        NPC.DeathSound = SoundRegistry.VoidRainsDeath;
        NPC.knockBackResist = 0f;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
        NPC.value = Item.buyPrice(1, 0, 0, 0);
        NPC.SpawnWithHigherTime(30);
        NPC.boss = true;
        NPC.aiStyle = -1;

        if (!Main.dedServ)
        {
            Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/BlueVeyeral");
        }
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        bestiaryEntry.Info.AddRange([
            new MoonLordPortraitBackgroundProviderBestiaryInfoElement(),
            new FlavorTextBestiaryInfoElement("Mods.VoidRains.NPCs.BlueVeyeral.FlavorText")
        ]);
    }

    public override void OnFirstFrame()
    {
        NPC.TargetClosest();

        AttackManager.Reset();
        AttackManager.SetAttackPattern([
            new AttackState(() => ShootArrows(600), 120),
            new AttackState(() => SineSpinner(600, BulletTypes.Radiant, 120, 3, 8), 120),
            new AttackState(() => ShootArrows(600), 120),
            new AttackState(() =>
            {
                TurningBursts(600, 40, MathHelper.ToRadians(0.6f), MathHelper.ToRadians(1f), 10);
                return TurningBursts(600, 40, MathHelper.ToRadians(-0.6f), MathHelper.ToRadians(-1f), 10);
            }, 120),
            new AttackState(() => ShootArrows(600), 120),
            new AttackState(() =>
            {
                RadiantLines(600);
                return CloverShots(600);
            }, 120),

            new AttackState(() => ShootArrows(600), 120),
            new AttackState(() =>
            {
                RadiantLines(600);
                return SineSpinner(600, BulletTypes.VoidBright, 120, 4, 6);
            }, 120)
        ]);

        AttackManager.AiTimer = 120;
    }

    public override void BossAI()
    {
        EyeLookAt(TargetPlayer.Center);
        NPC.DiscourageDespawn(300);

        if (AttackManager.InWindDown)
            NPC.SimpleFlyMovement(NPC.DirectionTo(TargetPlayer.Center + new Vector2(0, -300)) * 10, 0.1f);

        AttackManager.RunAttackPattern();
    }

    public override void ClientBossAI()
    {
        overlayFrame++;
        wobbleTimer++;
        BlinkProcess();
    }
}