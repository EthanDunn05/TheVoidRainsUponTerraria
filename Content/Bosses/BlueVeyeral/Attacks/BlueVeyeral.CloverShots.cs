using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using VoidRains.Common;
using VoidRains.Content.Projectiles;

namespace VoidRains.Content.Bosses.BlueVeyeral;

public partial class BlueVeyeral
{
    public bool CloverShots(int attackLength)
    {
        AttackManager.CountUp = true;
        NPC.SimpleFlyMovement(Vector2.Zero, 0.15f);
        
        const int shotInterval = 6;
        const int bursts = 4;
        const float rotation = 0.14f / shotInterval;
        
        if (AttackManager.AiTimer >= attackLength)
        {
            AttackManager.CountUp = false;
            return true;
        }

        if (AttackManager.AiTimer % shotInterval == 0)
        {
            var seperation = MathHelper.TwoPi / bursts;
            SoundEngine.PlaySound(BulletTypes.VoidBright.ShootSound, NPC.Center);
            var dscl = 0.9f;

            for (var i = 0; i < bursts; i++)
            {
                var dAngle = AttackManager.AiTimer * rotation;
                var dir = dAngle + seperation * i;
                Bullet.SpawnBasic(
                    NPC.GetSource_FromAI(),
                    NPC.Center,
                    dir.ToRotationVector2() * 8,
                    50,
                    0.04f,
                    BulletTypes.VoidBright
                );
                
                dir = -dAngle - seperation * i;
                Bullet.SpawnBasic(
                    NPC.GetSource_FromAI(),
                    NPC.Center,
                    dir.ToRotationVector2() * 8,
                    50,
                    0.04f,
                    BulletTypes.VoidBright
                );
            }
        }
        
        return false;
    }
}