using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using VoidRains.Common;
using VoidRains.Content.Projectiles;

namespace VoidRains.Content.Bosses.BlueVeyeral;

public partial class BlueVeyeral
{
    public bool SineSpinner(int attackLength, BulletType type, float period, int shootInterval, int streams, float timeOffset = 0)
    {
        AttackManager.CountUp = true;
        NPC.SimpleFlyMovement(Vector2.Zero, 0.15f);

        const float amplitude = MathHelper.Pi / 3f;

        var shootAngle = amplitude * MathF.Sin(MathHelper.TwoPi * (AttackManager.AiTimer + timeOffset) / period);

        if (AttackManager.AiTimer >= attackLength)
        {
            AttackManager.CountUp = false;
            return true;
        }

        if (AttackManager.AiTimer % shootInterval == 0)
        {
            SoundEngine.PlaySound(type.ShootSound, NPC.Center);
            var seperation = MathHelper.TwoPi / streams;

            for (var i = 0; i < streams; i++)
            {
                var dir = shootAngle + seperation * i;
                var spawnOffset = dir.ToRotationVector2() * 64;

                Bullet.SpawnBasic(
                    NPC.GetSource_FromAI(),
                    NPC.Center + spawnOffset,
                    dir.ToRotationVector2() * 10,
                    50,
                    0.04f,
                    type
                );
            }
        }

        return false;
    }
}