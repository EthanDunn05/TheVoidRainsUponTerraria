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
        wobbleTimer %= 60;

        // Wobble
        if (wobbleTimer < 30)
        {
            wobbleScale = Vector2.Lerp(new Vector2(1.05f, 0.95f), new Vector2(0.95f, 1.05f),
                EasingHelper.QuadInOut(Utils.GetLerpValue(0, 30, wobbleTimer)));
        }
        else
        {
            wobbleScale = Vector2.Lerp(new Vector2(0.95f, 1.05f), new Vector2(1.05f, 0.95f),
                EasingHelper.QuadInOut(Utils.GetLerpValue(30, 60, wobbleTimer)));
        }

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
            NPC.rotation, origin, wobbleScale,
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
            NPC.rotation, origin, wobbleScale,
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
            NPC.rotation, origin, wobbleScale,
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
            NPC.rotation, origin, wobbleScale,
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
            NPC.rotation, origin, wobbleScale * 3,
            SpriteEffects.None, 0f);
    }

    private void EyeLookAt(Vector2 position)
    {
        const float maxEyeDist = 30f;
        const float maxPupilDist = 5f;

        eyePos = position - NPC.Center;
        if (eyePos.Length() > maxEyeDist)
        {
            eyePos.Normalize();
            eyePos *= maxEyeDist;
        }

        pupilPos = position - eyePos - NPC.Center;
        if (pupilPos.Length() > maxPupilDist)
        {
            pupilPos.Normalize();
            pupilPos *= maxPupilDist;
        }
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