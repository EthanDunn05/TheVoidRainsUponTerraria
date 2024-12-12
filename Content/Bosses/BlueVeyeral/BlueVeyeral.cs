using System.IO;
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
    private PhaseTracker phaseTracker;
    
    public override void SetStaticDefaults()
    {
        NPCID.Sets.BossBestiaryPriority.Add(Type);
        NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.ShadowFlame] = true;
        NPCID.Sets.MPAllowedEnemies[Type] = true;
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
        NPC.hide = true;

        if (!Main.dedServ)
        {
            Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/BlueVeyeral");
        }
    }

    public override void DrawBehind(int index)
    {
        Main.instance.DrawCacheNPCsOverPlayers.Add(index);
    }

    public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
    {
        scale = 1.5f;
        return null;
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

        phaseTracker = new PhaseTracker([
            Phase1,
            Phase2,
        ]);
    }

    public override void BossAI()
    {
        NPC.DiscourageDespawn(300);
        phaseTracker.RunPhaseAI();
    }

    public override void ClientBossAI()
    {
        overlayFrame++;
        wobbleTimer += 0.1f;
        if (LookAtPlayer) LookGoal = TargetPlayer.Center;
        
        BlinkProcess();
        LookProcess();
    }

    public override void SendBossAI(BinaryWriter writer)
    {
        phaseTracker.Serialize(writer);
    }

    public override void RecieveBossAI(BinaryReader reader)
    {
        phaseTracker.Deserialize(reader);
    }
}