using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using VoidRains.Common;
using VoidRains.Helpers;

namespace VoidRains.Content.Bosses.BlueVeyeral;

public class BlueBall : ModNPC
{
    private int overlayFrame = 0;
    private float wobbleTimer = 0;
    private Vector2 wobbleScale = Vector2.One;

    public override string Texture => "VoidRains/Content/Bosses/BlueVeyeral/BlueVeyeral";

    private bool firstFrame = true;

    public override void SetStaticDefaults()
    {
        NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.ShadowFlame] = true;
        NPCID.Sets.NeverDropsResourcePickups[Type] = true; // No free heals :P
    }
    
    public override void SetDefaults()
    {
        // Slightly smaller than the sprite
        NPC.width = 150;
        NPC.height = 150;
        NPC.damage = 50;
        NPC.defense = 30;
        NPC.lifeMax = 1000;
        NPC.HitSound = SoundRegistry.VoidRainsHit;
        NPC.DeathSound = SoundRegistry.VoidRainsDeath;
        NPC.knockBackResist = 0f;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
        NPC.value = 0f; // no money :)
        NPC.SpawnWithHigherTime(5);
        NPC.aiStyle = -1;
    }

    public override void AI()
    {
        if (firstFrame)
        {
            NPC.Opacity = 0;
            NPC.velocity = new Vector2(5 * NPC.ai[0], 0);
            firstFrame = false;
        }
        
        overlayFrame++;
        wobbleTimer += 0.1f;
        
        // Fade in
        NPC.Opacity = MathF.Min(1f, NPC.Opacity + 0.05f);
        if (NPC.Opacity < 1f) NPC.damage = 0;
        else NPC.damage = NPC.defDamage;
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        // Limit frames
        overlayFrame %= 24;
        wobbleScale = new Vector2(1 + 0.05f * MathF.Sin(wobbleTimer), 1 - 0.05f * MathF.Sin(wobbleTimer));

        DrawBody(spriteBatch, drawColor);
        DrawOverlay(spriteBatch, drawColor);

        return false;
    }
    
    private void DrawBody(SpriteBatch spriteBatch, Color drawColor)
    {
        var drawPos = NPC.Center - Main.screenPosition;
        var texture = TextureAssets.Npc[Type].Value;
        var origin = texture.Size() * 0.5f;

        spriteBatch.Draw(
            texture, drawPos,
            NPC.frame, NPC.GetAlpha(drawColor),
            NPC.rotation, origin, wobbleScale * NPC.scale,
            SpriteEffects.None, 0f);
    }
    
    private void DrawOverlay(SpriteBatch spriteBatch, Color drawColor)
    {
        var drawPos = NPC.Center - Main.screenPosition;
        var texture = TextureRegistry.Boss("BlueVeyeral/Overlay").Value;
        var frame = texture.Frame(verticalFrames: 24, frameY: overlayFrame);
        var origin = texture.Size() / new Vector2(1, 24) * 0.5f;
        var col = NPC.GetAlpha(drawColor);
        col *= 0.25f;

        spriteBatch.Draw(
            texture, drawPos,
            frame, col,
            NPC.rotation, origin, wobbleScale * 3 * NPC.scale,
            SpriteEffects.None, 0f);
    }
}