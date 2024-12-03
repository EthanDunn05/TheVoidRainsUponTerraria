using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using VoidRains.Common;
using VoidRains.Content.Projectiles;

namespace VoidRains.Content.Bosses.BlueVeyeral;

public partial class BlueVeyeral
{
    public bool SquiggleShots(int attackLength, int direction)
    {
        AttackManager.CountUp = true;
        NPC.SimpleFlyMovement(Vector2.Zero, 0.15f);

        const int shootInterval = 30;
        const int shots = 6;
        const int burst = 8;
        
        ref var shootTimer = ref NPC.localAI[0];
        shootTimer--;
        if (AttackManager.AiTimer == 0) shootTimer = shots * 3;
        
        if (AttackManager.AiTimer >= attackLength)
        {
            AttackManager.CountUp = false;
            shootTimer = 0;
            return true;
        }

        if (shootTimer < shots * 3 && shootTimer % 3 == 0)
        {
            SoundEngine.PlaySound(BulletTypes.Radiant2.ShootSound, NPC.Center);

            for (var i = 0; i < burst; i++)
            {
                var dir = AttackManager.AiTimer + (MathHelper.TwoPi / burst) * i * direction;
                
                Bullet.SpawnWaveTurn(
                    NPC.GetSource_FromAI(),
                    NPC.Center + dir.ToRotationVector2() * 64,
                    dir.ToRotationVector2() * 7,
                    NPC.damage,
                    0.04f,
                    MathHelper.ToRadians(20),
                    0.174f * direction,
                    BulletTypes.Radiant2
                );
            }
        }

        if (shootTimer <= 0) shootTimer = shootInterval;

        return false;
    }
}