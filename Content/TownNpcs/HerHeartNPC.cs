using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Personalities;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace VoidRains.Content.TownNpcs;

[AutoloadHead]
public class HerHeartNPC : ModNPC
{
    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[Type] = 25;

        NPCID.Sets.ExtraFramesCount[Type] = 9;
        NPCID.Sets.AttackFrameCount[Type] = 4;
        NPCID.Sets.DangerDetectRange[Type] = 700;
        NPCID.Sets.PrettySafe[Type] = 300;
        NPCID.Sets.AttackType[Type] = 2;
        NPCID.Sets.AttackTime[Type] = 60;
        NPCID.Sets.AttackAverageChance[Type] = 30;
        NPCID.Sets.HatOffsetY[Type] = 5;
        NPCID.Sets.ShimmerTownTransform[Type] = false;

        NPCID.Sets.ActsLikeTownNPC[Type] = true;
        
        var drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers() {
            Velocity = 1f,
            Direction = 1
        };
        NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);

        NPC.Happiness
            .SetBiomeAffection<HallowBiome>(AffectionLevel.Love)
            .SetBiomeAffection<ForestBiome>(AffectionLevel.Like)
            .SetNPCAffection(NPCID.PartyGirl, AffectionLevel.Love)
            .SetNPCAffection(NPCID.Clothier, AffectionLevel.Like);
    }

    public override void SetDefaults()
    {
        NPC.townNPC = true;
        NPC.friendly = true;
        NPC.width = 18;
        NPC.height = 40;
        NPC.aiStyle = 7;
        NPC.damage = 10;
        NPC.defense = 15;
        NPC.lifeMax = 250;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;

        AnimationType = NPCID.Stylist;
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        bestiaryEntry.Info.AddRange([
            BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheHallow,
            new FlavorTextBestiaryInfoElement("Mods.VoidRains.NPCs.HerHeartNPC.FlavorText")
        ]);
    }
    
    public override bool CanTownNPCSpawn(int numTownNPCs)
    {
        // Temporary spawn condition until I make a better one
        return NPC.downedBoss1;
    }

    public override List<string> SetNPCNameList()
    {
        return
        [
            "Her Heart"
        ];
    }

    public override string GetChat()
    {
        var chat = new WeightedRandom<string>();
        
        chat.Add(Language.GetTextValue("Mods.VoidRains.Dialogue.HerHeart.Standard1"));
        chat.Add(Language.GetTextValue("Mods.VoidRains.Dialogue.HerHeart.Standard2"));
        chat.Add(Language.GetTextValue("Mods.VoidRains.Dialogue.HerHeart.Standard3"));
        chat.Add(Language.GetTextValue("Mods.VoidRains.Dialogue.HerHeart.Standard4"));

        return chat;
    }

    public override void SetChatButtons(ref string button, ref string button2)
    {
        button = Language.GetTextValue("LegacyInterface.28"); // Shop
    }

    public override void OnChatButtonClicked(bool firstButton, ref string shopName)
    {
        if (!firstButton) return;
        shopName = "Shop";
    }

    public override void AddShops()
    {
        var shop = new NPCShop(Type, "Shop");
        
        shop.Register();
    }

    public override bool CanGoToStatue(bool toKingStatue) => !toKingStatue;

    public override void TownNPCAttackStrength(ref int damage, ref float knockback)
    {
        damage = 10;
        knockback = 4;
    }

    public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
    {
        cooldown = 30;
        randExtraCooldown = 30;
    }

    public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
    {
        projType = ProjectileID.NebulaBlaze1;
        attackDelay = 1;
    }

    public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
    {
        multiplier = 5f;
        randomOffset = 0f;
    }

    public override void TownNPCAttackMagic(ref float auraLightMultiplier)
    {
        auraLightMultiplier = 1f;
    }
}