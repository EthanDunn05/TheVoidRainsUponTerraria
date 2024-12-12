using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using VoidRains.Common;
using VoidRains.Content.Projectiles;

namespace VoidRains.Content.Bosses.BlueVeyeral;

public partial class BlueVeyeral
{
    public bool WaveTurnBursts(int attackLength)
    {
        AttackManager.CountUp = true;
        NPC.SimpleFlyMovement(Vector2.Zero, 0.15f);
        
        const float shootInterval = 5;
        var turn = MathHelper.ToRadians(3);
        var streams = 8;
        
        var shootAngle = AttackManager.AiTimer * turn;
        var ySin = AttackManager.AiTimer * 0.025f;

        ref var anchorY = ref NPC.localAI[0];
        if (AttackManager.AiTimer == 0) anchorY = NPC.position.Y;

        if (AttackManager.AiTimer >= attackLength)
        {
            AttackManager.CountUp = false;
            anchorY = 0;
            return true;
        }

        var goalY = anchorY - MathF.Sin(ySin) * 200f;
        NPC.position.Y = MathHelper.Lerp(anchorY, goalY, Utils.GetLerpValue(0, 30, AttackManager.AiTimer, true));

        if (AttackManager.AiTimer % shootInterval == 0)
        {
            SoundEngine.PlaySound(BulletTypes.VoidBright.ShootSound, NPC.Center);
            var seperation = MathHelper.TwoPi / streams;

            for (var i = 0; i < streams; i++)
            {
                var dir = shootAngle + seperation * i;
                var spawnOffset = dir.ToRotationVector2() * 64;
                var speed = 10 + 10 * 0.2f * MathF.Cos(ySin);

                Bullet.SpawnBasic(
                    NPC.GetSource_FromAI(),
                    NPC.Center + spawnOffset,
                    dir.ToRotationVector2() * speed,
                    NPC.damage,
                    0.04f,
                    BulletTypes.VoidBright
                );
            }
        }

        return false;
    }
}