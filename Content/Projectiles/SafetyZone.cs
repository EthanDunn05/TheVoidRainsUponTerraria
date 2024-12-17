using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;
using VoidRains.Common;

namespace VoidRains.Content.Projectiles;

public class SafetyZone : ModProjectile
{
    public float Radius => Projectile.ai[0];
    
    private float outlineSin = 0f;
    
    private readonly int collapseTime = 30;
    private readonly int growTime = 30;
    
    private bool firstFrame = true;
    private float trueRadius = 0f;
    private int timeAlive = 0;
    
    public override void SetDefaults()
    {
        Projectile.width = 0;
        Projectile.height = 0;
        Projectile.hostile = true;
        Projectile.tileCollide = false;
    }

    public override void AI()
    {
        if (firstFrame)
        {
            firstFrame = false;
            timeAlive = Projectile.timeLeft;
        }
        
        // Grow / Collapse
        if (Projectile.timeLeft > timeAlive - growTime)
        {
            trueRadius = MathHelper.Lerp(0, Radius,
                Utils.GetLerpValue(timeAlive, timeAlive - growTime, Projectile.timeLeft, true));
        }
        
        if (Projectile.timeLeft < collapseTime)
        {
            trueRadius = MathHelper.Lerp(Radius, 0f,
                Utils.GetLerpValue(collapseTime, 0, Projectile.timeLeft, true));
        }
        
        outlineSin += 0.1f;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        var alpha = 0.2f;
        var col = new Color(0x40, 0xC0, 0xFF);
        var tex = ModContent.Request<Texture2D>(Texture, AssetRequestMode.ImmediateLoad);
        var texOutline = ModContent.Request<Texture2D>(Texture + "Outline", AssetRequestMode.ImmediateLoad);
        
        for (var i = 0; i < 4; i++)
        {
            var r = trueRadius + trueRadius / 64f * MathF.Sin(outlineSin + MathHelper.Pi * i * 0.5f);
            var scale = new Vector2(r, r) * 2f / tex.Size();
            Main.spriteBatch.Draw(
                    texOutline.Value,
                    Projectile.position - Main.screenPosition,
                    tex.Frame(),
                    Color.White * alpha,
                    0f,
                    tex.Size() * 0.5f,
                    scale,
                    SpriteEffects.None,
                    0f
            );
        }

        var scale2 = new Vector2(trueRadius, trueRadius) * 2f / tex.Size();
        Main.spriteBatch.Draw(
            tex.Value,
            Projectile.position - Main.screenPosition,
            tex.Frame(),
            Color.White * 0.2f,
            0f,
            tex.Size() * 0.5f,
            scale2,
            SpriteEffects.None,
            0f
        );
        
        return false;
    }
}