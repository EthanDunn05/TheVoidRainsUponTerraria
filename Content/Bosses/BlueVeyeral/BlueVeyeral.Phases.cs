using Microsoft.Xna.Framework;
using VoidRains.Common;
using VoidRains.Helpers.StateManagement;

namespace VoidRains.Content.Bosses.BlueVeyeral;

public partial class BlueVeyeral
{
    public PhaseState Phase1 => new(Phase1AI, EnterPhase1);

    public void EnterPhase1()
    {
        AttackManager.Reset();
        AttackManager.SetAttackPattern([
            new AttackState(() => ShootArrows(600), 120),
            new AttackState(() => SineSpinner(600, BulletTypes.Radiant, 120, 3, 8), 120),
            new AttackState(() => ShootArrows(600), 120),
            new AttackState(() =>
            {
                TurningBursts(600, 40, MathHelper.ToRadians(0.6f), MathHelper.ToRadians(1f), 10);
                return TurningBursts(600, 40, MathHelper.ToRadians(-0.6f), MathHelper.ToRadians(-1f), 10);
            }, 120),
            new AttackState(() => ShootArrows(600), 120),
            new AttackState(() =>
            {
                RadiantLines(600);
                return CloverShots(600);
            }, 120),

            new AttackState(() => ShootArrows(600), 120),
            new AttackState(() =>
            {
                RadiantLines(600);
                return SineSpinner(600, BulletTypes.VoidBright, 120, 4, 6);
            }, 120)
        ]);

        AttackManager.AiTimer = 120;
    }

    public void Phase1AI()
    {
        EyeLookAt(TargetPlayer.Center);
        
        if (AttackManager.InWindDown)
        {
            NPC.SimpleFlyMovement(NPC.DirectionTo(TargetPlayer.Center + new Vector2(0, -300)) * 10, 0.1f);
        }

        AttackManager.RunAttackPattern();
    }
}