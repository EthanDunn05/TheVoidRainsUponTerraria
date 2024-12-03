using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using VoidRains.Common;
using VoidRains.Content.Projectiles;

namespace VoidRains.Content.Bosses.BlueVeyeral;

public partial class BlueVeyeral
{
    public bool TurningBursts(int attackLength, int shotDelay, float turn, float aimTurn, float shotSpeed)
    {
        AttackManager.CountUp = true;
        NPC.SimpleFlyMovement(Vector2.Zero, 0.15f);

        const int shots = 24;
        
        if (AttackManager.AiTimer >= attackLength)
        {
            AttackManager.CountUp = false;
            return true;
        }

        if (AttackManager.AiTimer % shotDelay == 0)
        {
            SoundEngine.PlaySound(BulletTypes.Radiant.ShootSound, NPC.Center);
            var speration = MathHelper.TwoPi / shots;
            for (var i = 0; i < shots; i++)
            {
                var dir = AttackManager.AiTimer * aimTurn + speration * i;
                var offset = dir.ToRotationVector2() * 64;
                Bullet.SpawnTurning(
                    NPC.GetSource_FromAI(),
                    NPC.Center + offset,
                    dir.ToRotationVector2() * shotSpeed,
                    NPC.damage,
                    0.4f,
                    turn,
                    60,
                    -turn,
                    BulletTypes.Radiant
                );
            }
        }

        return false;
    }
}