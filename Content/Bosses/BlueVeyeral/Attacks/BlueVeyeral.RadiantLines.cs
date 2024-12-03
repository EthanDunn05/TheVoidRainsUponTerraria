using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using VoidRains.Common;
using VoidRains.Content.Projectiles;

namespace VoidRains.Content.Bosses.BlueVeyeral;

public partial class BlueVeyeral
{
    public bool RadiantLines(int attackLength)
    {
        AttackManager.CountUp = true;
        NPC.SimpleFlyMovement(Vector2.Zero, 0.15f);

        const int shootInterval = 45;
        const int shots = 7;
        
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
            SoundEngine.PlaySound(BulletTypes.Radiant.ShootSound, NPC.Center);
            Bullet.SpawnBasic(
                NPC.GetSource_FromAI(),
                NPC.Center + eyePos,
                NPC.Center.DirectionTo(TargetPlayer.Center) * 15,
                NPC.damage,
                0.04f,
                BulletTypes.Radiant
            );
        }

        if (shootTimer <= 0) shootTimer = shootInterval;

        return false;
    }
}