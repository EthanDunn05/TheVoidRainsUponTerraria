using System;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Terraria;
using Terraria.Audio;
using VoidRains.Common;
using VoidRains.Content.Projectiles;
using Utils = Terraria.Utils;

namespace VoidRains.Content.Bosses.BlueVeyeral;

public partial class BlueVeyeral
{
    public bool ShootArrows(int attackLength)
    {
        AttackManager.CountUp = true;

        const int radiantInterval = 28;
        const int voidInterval = 30;
        const int radiant2Interval = 35;

        const int radiantOffset = 13;
        const int voidOffset = 7;
        const int radiant2Offset = 18;

        NPC.SimpleFlyMovement(new Vector2(0, AttackManager.AiTimer % 120 > 60 ? 5 : -5), 0.2f);

        if (AttackManager.AiTimer >= attackLength)
        {
            AttackManager.CountUp = false;
            return true;
        }


        if ((AttackManager.AiTimer + radiantOffset) % radiantInterval == 0)
        {
            var offset = AttackManager.AiTimer / 98f;
            SoundEngine.PlaySound(BulletTypes.Radiant.ShootSound, NPC.Center);
            for (var i = 0; i < 6; i++)
            {
                var dir = MathHelper.Lerp(0, MathHelper.TwoPi, Utils.GetLerpValue(0, 6, i));
                CommonPatterns.Arrow(NPC.GetSource_FromAI(), NPC.Center, dir + offset, 15, NPC.damage, 0.4f,
                    BulletTypes.Radiant);
            }
        }

        if ((AttackManager.AiTimer + voidOffset) % voidInterval == 0)
        {
            var offset = AttackManager.AiTimer / 120f;
            SoundEngine.PlaySound(BulletTypes.VoidBright.ShootSound, NPC.Center);
            for (var i = 0; i < 7; i++)
            {
                var dir = MathHelper.Lerp(0, MathHelper.TwoPi, Utils.GetLerpValue(0, 7, i));
                CommonPatterns.Arrow(NPC.GetSource_FromAI(), NPC.Center, dir + offset, 12, NPC.damage, 0.4f, BulletTypes.VoidBright);
            }
        }

        if ((AttackManager.AiTimer + radiant2Offset) % radiant2Interval == 0)
        {
            SoundEngine.PlaySound(BulletTypes.Radiant2.ShootSound, NPC.Center);
            CommonPatterns.Arrow(NPC.GetSource_FromAI(), NPC.Center,
                NPC.Center.DirectionTo(TargetPlayer.Center).ToRotation(), 18, NPC.damage, 0.4f, BulletTypes.Radiant2);
        }

        return false;
    }
}