using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using VoidRains.Common;
using VoidRains.Helpers;

namespace VoidRains.Content.Projectiles;

public class TrackingBeam : Beam
{
    private Player? trackingPlayer => Main.player.Where(p => p.active && !p.dead).MinBy(p => Projectile.Distance(p.Center));
    private Vector2 rotateAround = Vector2.Zero;
    private float lerpAggression = 0f;

    private float distance = 0f;
    
    public static Projectile? SpawnTracking(IEntitySource source, Vector2 position, Vector2 velocity, int damage,
        float knockback, BeamType beamType, int length, Vector2 rotateAround, float lerpAgression)
    {
        var typeIndex = BeamTypes.TypeArray.IndexOf(beamType);
        var proj = ProjectileHelper.NewUnscaledProjectile(source, position, velocity, ModContent.ProjectileType<TrackingBeam>(), damage,
            knockback, ai0: typeIndex, ai1: length);
        
        var modProj = (TrackingBeam) proj.ModProjectile;
        modProj.rotateAround = rotateAround;
        modProj.lerpAggression = lerpAgression;
        proj.Sync();
        
        return proj;
    }
    
    public override void AI()
    {
        if (FirstFrame)
        {
            Projectile.rotation = rotateAround.DirectionTo(Projectile.position).ToRotation();
            distance = Projectile.position.Distance(rotateAround);
        }
        
        base.AI();

        if (trackingPlayer is null)
        {
            Projectile.active = false;
            return;
        }
        
        // Track player
        var targetRot = Projectile.DirectionTo(trackingPlayer.Center);
        var rotate = Vector2.Lerp(Projectile.rotation.ToRotationVector2(), targetRot, lerpAggression);
        Projectile.position = rotateAround + distance * rotate;
        Projectile.rotation = rotate.ToRotation();
    }

    public override void SendExtraAI(BinaryWriter writer)
    {
        writer.WritePackedVector2(rotateAround);
        writer.Write(lerpAggression);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        rotateAround = reader.ReadPackedVector2();
        lerpAggression = reader.ReadSingle();
    }
}