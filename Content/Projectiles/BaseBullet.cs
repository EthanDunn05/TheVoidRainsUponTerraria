using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using VoidRains.Common;

namespace VoidRains.Content.Projectiles;

/// <summary>
/// This takes care of some of the specifics that are common to all bullet types.
/// </summary>
public abstract class BaseBullet : ModProjectile
{
    // The index of the type is synced
    private BulletType BulletType => BulletTypes.TypeArray[(int) Projectile.ai[0]];
    public override string Texture => TextureRegistry.InvisPath;
    
    public override void SetDefaults()
    {
        Projectile.width = 5;
        Projectile.height = 5;
        Projectile.hostile = true;
        Projectile.tileCollide = false;
    }

    public override void SetStaticDefaults()
    {
    }

    public override void PostAI()
    {
        // Visuals
        Projectile.frameCounter++;
        if (Projectile.frameCounter >= 2)
        {
            Projectile.frame = (Projectile.frame + 1) % BulletType.Frames;
            Projectile.frameCounter = 0;
        }
    }
    
    public override bool PreDraw(ref Color lightColor)
    {
        var drawPos = Projectile.Center - Main.screenPosition;
        var texture = BulletType.Texture.Value;
        var frame = texture.Frame(verticalFrames: BulletType.Frames, frameY: Projectile.frame);
        var origin = texture.Size() / new Vector2(1, BulletType.Frames) * 0.5f;
        origin += BulletType.CenterOffset;

        Main.spriteBatch.Draw(
            texture, drawPos,
            frame, Color.White,
            Projectile.rotation, origin, Projectile.scale,
            SpriteEffects.None, 0f);

        return false;
    }
}