using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using VoidRains.Common;
using VoidRains.Helpers;

namespace VoidRains.Content.Bosses.BlueVeyeral;

public partial class BlueVeyeral
{
    // <0, 0> is the center of the boss
    private Vector2 eyePos = Vector2.Zero;
    public Vector2 LookGoal = Vector2.Zero;
    public bool LookAtPlayer = true;
    public float lookEase = 0.15f;

    // <0, 0> is the center of the eye
    private Vector2 pupilPos = Vector2.Zero;

    private int blinkFrame = 0;
    private int overlayFrame = 0;

    private float blinkTimer = 0;
    private float wobbleTimer = 0;

    private Vector2 wobbleScale = Vector2.One;

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        // Limit frames
        blinkFrame %= 7;
        overlayFrame %= 24;

        // Wobble
        wobbleScale = new Vector2(1 + 0.05f * MathF.Sin(wobbleTimer), 1 - 0.05f * MathF.Sin(wobbleTimer));

        DrawBody(spriteBatch, drawColor);
        DrawIris(spriteBatch, drawColor);
        DrawPupil(spriteBatch, drawColor);
        DrawEyelid(spriteBatch, drawColor);
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

    private void DrawEyelid(SpriteBatch spriteBatch, Color drawColor)
    {
        var drawPos = NPC.Center + eyePos - Main.screenPosition;
        var texture = TextureRegistry.Boss("BlueVeyeral/Eyelid").Value;
        var frame = texture.Frame(verticalFrames: 7, frameY: blinkFrame);

        var origin = texture.Size() / new Vector2(1f, 7f) * 0.5f;

        spriteBatch.Draw(
            texture, drawPos,
            frame, NPC.GetAlpha(drawColor),
            NPC.rotation, origin, NPC.scale,
            SpriteEffects.None, 0f);
    }

    private void DrawIris(SpriteBatch spriteBatch, Color drawColor)
    {
        var drawPos = NPC.Center + eyePos - Main.screenPosition;
        var texture = TextureRegistry.Boss("BlueVeyeral/Iris").Value;
        var frame = texture.Frame();
        var origin = texture.Size() * 0.5f;

        spriteBatch.Draw(
            texture, drawPos,
            frame, NPC.GetAlpha(drawColor),
            NPC.rotation, origin, NPC.scale,
            SpriteEffects.None, 0f);
    }

    private void DrawPupil(SpriteBatch spriteBatch, Color drawColor)
    {
        var drawPos = NPC.Center + eyePos + pupilPos - Main.screenPosition;
        var texture = TextureRegistry.Boss("BlueVeyeral/Pupil").Value;
        var frame = texture.Frame();
        var origin = texture.Size() * 0.5f;

        spriteBatch.Draw(
            texture, drawPos,
            frame, NPC.GetAlpha(drawColor),
            NPC.rotation, origin, NPC.scale,
            SpriteEffects.None, 0f);
    }

    private void DrawOverlay(SpriteBatch spriteBatch, Color drawColor)
    {
        var drawPos = NPC.Center - Main.screenPosition;
        var texture = TextureRegistry.Boss("BlueVeyeral/Overlay").Value;
        var frame = texture.Frame(verticalFrames: 24, frameY: overlayFrame);
        var origin = texture.Size() / new Vector2(1, 24) * 0.5f;
        var col = NPC.GetAlpha(drawColor);
        col *= 0.15f;

        spriteBatch.Draw(
            texture, drawPos,
            frame, col,
            NPC.rotation, origin, wobbleScale * 3 * NPC.scale,
            SpriteEffects.None, 0f);
    }

    private void LookProcess()
    {
        const float maxEyeDist = 30f;
        const float maxPupilDist = 5f;

        var goalEyePos = LookGoal - NPC.Center;
        if (goalEyePos.Length() > maxEyeDist)
        {
            goalEyePos.Normalize();
            goalEyePos *= maxEyeDist;
        }
        
        eyePos = Vector2.Lerp(eyePos, goalEyePos, lookEase);

        var goalPupilPos = LookGoal - eyePos - NPC.Center;
        if (goalPupilPos.Length() > maxPupilDist)
        {
            goalPupilPos.Normalize();
            goalPupilPos *= maxPupilDist;
        }
        
        pupilPos = Vector2.Lerp(pupilPos, goalPupilPos, lookEase);
    }

    private void BlinkProcess()
    {
        blinkTimer += 0.5f;

        if (blinkTimer >= 120)
        {
            blinkFrame = (int) (blinkTimer - 120);
        }

        if (blinkTimer > 127)
        {
            blinkTimer = 0;
            blinkFrame = 0;
        }
    }
}