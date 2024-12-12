using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;
using VoidRains.Common;
using VoidRains.Helpers;

namespace VoidRains.Content.Projectiles;

public class Beam : ModProjectile
{
    private BeamType BeamType => BeamTypes.TypeArray[(int) Projectile.ai[0]];
    public override string Texture => TextureRegistry.InvisPath;
    private const float MaxDist = 10000f;
    private const int DischargeTime = 30;
    
    private int chargeTime = 0;
    public bool FirstFrame = true;
    private bool playedFire;
    private SlotId contSound;
    
    private bool Charging => chargeTime < BeamType.ChargeFrames;
    private bool Discharging => Projectile.timeLeft < DischargeTime;
    
    public override void SetDefaults()
    {
        Projectile.width = 0;
        Projectile.height = 0;
        Projectile.hostile = true;
        Projectile.tileCollide = false;
        Projectile.hide = true;
    }
    
    public static Projectile? SpawnBasic(IEntitySource source, Vector2 position, Vector2 velocity, int damage,
        float knockback, BeamType beamType, int length)
    {
        var typeIndex = BeamTypes.TypeArray.IndexOf(beamType);
        var proj = ProjectileHelper.NewUnscaledProjectile(source, position, velocity, ModContent.ProjectileType<Beam>(), damage,
            knockback, ai0: typeIndex, ai1: length);
        
        return proj;
    }

    public override void AI()
    {
        if (FirstFrame)
        {
            FirstFrame = false;
            Projectile.timeLeft = (int) Projectile.ai[1] + BeamType.ChargeFrames * 2 + DischargeTime;
            SoundEngine.PlaySound(SoundRegistry.BeamCharge, Projectile.position);
        }
        
        Projectile.frameCounter++;
        if (Projectile.frameCounter % 2 == 0 && Charging)
        {
            chargeTime++;
        }
        
        if (Projectile.frameCounter % 3 == 0)
        {
            Projectile.frame = (Projectile.frame + 1) % BeamType.BeamFrames;
        }
        
        if (chargeTime == BeamType.ChargeFrames && !playedFire)
        {
            SoundEngine.PlaySound(SoundRegistry.BeamFire, Projectile.position);
            playedFire = true;
        }

        if (!Charging && !Discharging && !SoundEngine.TryGetActiveSound(contSound, out var sound1))
        {
            var tracker = new ProjectileAudioTracker(Projectile);
            contSound = SoundEngine.PlaySound(SoundRegistry.BeamCont with
            {
                IsLooped = true,
                SoundLimitBehavior = SoundLimitBehavior.IgnoreNew
            }, Projectile.position, instance =>
            {
                instance.Position = Projectile.position;
                return tracker.IsActiveAndInGame();
            });
        }

        if (Discharging && SoundEngine.TryGetActiveSound(contSound, out var sound))
        {
            sound.Stop();
        }
    }

    public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers,
        List<int> overWiresUI)
    {
        overWiresUI.Add(index);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        var step = BeamType.MiddleTexture.Width();
        var i = 1f;
        
        // Show thin line when discharging
        if (Discharging)
        {
            i = 0.5f;
            for (; i <= MaxDist / step - 1; i++)
            {
                var position = Projectile.position + Projectile.rotation.ToRotationVector2() * i * step;
                DrawPart(BeamType.ThinTexture, position, BeamType.BeamFrames, Projectile.frame);
            }

            return false;
        }
        
        // Beam is under every other part
        if (Charging) i = 0.5f;
        for (; i <= MaxDist / step - 1; i++)
        {
            var position = Projectile.position + Projectile.rotation.ToRotationVector2() * i * step;
            if (Charging) DrawPart(BeamType.StartTexture, position, BeamType.BeamFrames, Projectile.frame);
            else DrawPart(BeamType.MiddleTexture, position, BeamType.BeamFrames, Projectile.frame);
        }
        
        // Show the charge or end piece
        if (Charging)
        {
            DrawPart(BeamType.ChargeTexture, Projectile.position, BeamType.ChargeFrames, chargeTime);
        }
        else DrawPart(BeamType.EndTexture, Projectile.position, BeamType.BeamFrames, Projectile.frame);
        
        // Place the end
        if (!Charging) DrawPart(BeamType.EndTexture, Projectile.position + Projectile.rotation.ToRotationVector2() * MaxDist, BeamType.BeamFrames, Projectile.frame, MathHelper.Pi);
        
        return false;
    }

    // Draws one part of the beam. Mostly here to avoid repeating the long draw call
    private void DrawPart(Asset<Texture2D> tex, Vector2 position, int frames, int frame, float rotOff = 0f)
    {
        var rect = tex.Frame(verticalFrames: frames, frameY: frame);
        Main.spriteBatch.Draw(
            tex.Value, 
            position - Main.screenPosition,
            rect,
            Color.White,
            Projectile.rotation + rotOff,
            rect.Size() / 2f,
            1f,
            SpriteEffects.None,
            0f
        );
    }
    
    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
        var point = 0f;
        
        if (Charging || Discharging) return false;
        
        return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center,
            Projectile.Center + Projectile.rotation.ToRotationVector2() * MaxDist, BeamType.MiddleTexture.Width(), ref point);
    }
}