using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using VoidRains.Common;

namespace VoidRains.Content.Bosses.BlueVeyeral;

public partial class BlueVeyeral
{
    public bool TrackingBeam(int attackLength)
    {
        AttackManager.CountUp = true;
        LookAtPlayer = false;
        LookGoal = NPC.Center - Vector2.UnitY * 50f; // Look up
        NPC.SimpleFlyMovement(Vector2.Zero, 0.15f);
        
        if (AttackManager.AiTimer > 30) lookEase = 0.02f;

        if (AttackManager.AiTimer == 0)
        {
            var pos = NPC.DirectionTo(TargetPlayer.Center) * 35 + NPC.Center;
            Projectiles.TrackingBeam.SpawnTracking(NPC.GetSource_FromAI(), pos, Vector2.Zero, NPC.damage, 4, BeamTypes.Void,
                attackLength, NPC.Center, 0.02f);
        }
        
        if (AttackManager.AiTimer >= attackLength + 60)
        {
            lookEase = 0.15f;
            AttackManager.CountUp = false;
            LookAtPlayer = true;
            return true;
        }
        
        var lookAt = Main.player.Where(p => p.active && !p.dead).MinBy(p => NPC.Distance(p.Center));
        if (lookAt != null) LookGoal = lookAt.Center;

        return false;
    }
}