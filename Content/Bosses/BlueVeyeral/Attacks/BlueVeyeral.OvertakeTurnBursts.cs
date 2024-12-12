using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using VoidRains.Common;
using VoidRains.Content.Projectiles;

namespace VoidRains.Content.Bosses.BlueVeyeral;

public partial class BlueVeyeral
{
    public bool OvertakeTurnBurrsts(int attackLength)
    {
        AttackManager.CountUp = true;
        NPC.SimpleFlyMovement(Vector2.Zero, 0.15f);

        const int shootInterval = 45;
        const int shots = 8;
        const int bursts = 8;
        var turn = MathHelper.ToRadians(4);
        
        ref var shootTimer = ref NPC.localAI[0];
        shootTimer--;
        if (AttackManager.AiTimer == 0) shootTimer = shots * 3;

        ref var shootAngle = ref NPC.localAI[1];
        
        if (AttackManager.AiTimer >= attackLength)
        {
            AttackManager.CountUp = false;
            shootTimer = 0;
            shootAngle = 0;
            return true;
        }

        if (shootTimer < shots * 3 && shootTimer % 3 == 0)
        {
            SoundEngine.PlaySound(BulletTypes.Radiant.ShootSound, NPC.Center);
            shootAngle += turn;
            var seperation = MathHelper.TwoPi / bursts;
            
            
            for (var i = 0; i < bursts; i++)
            {
                var dir = shootAngle + seperation * i;
                var spawnOffset = dir.ToRotationVector2() * 64;
                var speed = 10 + 0.25f * (1 - shootTimer / shots * 3);

                Bullet.SpawnBasic(
                    NPC.GetSource_FromAI(),
                    NPC.Center + spawnOffset,
                    dir.ToRotationVector2() * speed,
                    NPC.damage,
                    0.04f,
                    BulletTypes.Radiant
                );
            }
        }

        if (shootTimer <= 0) shootTimer = shootInterval;

        return false;
    }
}