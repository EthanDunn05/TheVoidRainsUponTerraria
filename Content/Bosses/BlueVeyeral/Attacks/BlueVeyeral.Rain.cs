using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using VoidRains.Common;
using VoidRains.Content.Projectiles;

namespace VoidRains.Content.Bosses.BlueVeyeral;

public partial class BlueVeyeral
{
    public bool Rain(int attackLength)
    {
        AttackManager.CountUp = true;
        LookAtPlayer = false;
        LookGoal = NPC.Center - Vector2.UnitY * 50f; // Look up
        NPC.SimpleFlyMovement(Vector2.Zero, 0.15f);

        const int shotDelay = 3;
        
        if (AttackManager.AiTimer >= attackLength)
        {
            AttackManager.CountUp = false;
            LookAtPlayer = true;
            return true;
        }

        if (AttackManager.AiTimer % shotDelay == 0)
        {
            SoundEngine.PlaySound(BulletTypes.VoidDropBullet.ShootSound, NPC.Center);
            
            for (var i = 0; i < 2; i++)
            {
                var dir = -Main.rand.NextFloat(MathHelper.PiOver2) - MathHelper.PiOver4;
                var offset = dir.ToRotationVector2() * 64;
                GravityBullet.SpawnSimple(
                    NPC.GetSource_FromAI(),
                    NPC.Center + offset,
                    dir.ToRotationVector2() * 15,
                    NPC.damage,
                    0.4f,
                    0.1f,
                    BulletTypes.VoidDropBullet
                );
            }
        }

        return false;
    }
}