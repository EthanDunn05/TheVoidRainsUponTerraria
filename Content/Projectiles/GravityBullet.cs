using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using VoidRains.Common;
using VoidRains.Helpers;

namespace VoidRains.Content.Projectiles;

public class GravityBullet : BaseBullet
{
    public float Gravity => Projectile.ai[1];
    public float GravityDir => Projectile.ai[2];
    
    public static Projectile? SpawnSimple(IEntitySource source, Vector2 position, Vector2 velocity, int damage,
        float knockback, float gravity, BulletType bulletType)
    {
        var typeIndex = BulletTypes.TypeArray.IndexOf(bulletType);
        var proj = ProjectileHelper.NewUnscaledProjectile(source, position, velocity, ModContent.ProjectileType<GravityBullet>(), damage,
            knockback, ai0: typeIndex, ai1: gravity, ai2: MathHelper.PiOver2);
        
        return proj;
    }
    
    public static Projectile? SpawnDirectional(IEntitySource source, Vector2 position, Vector2 velocity, int damage,
        float knockback, float gravity, float gravDir, BulletType bulletType)
    {
        var typeIndex = BulletTypes.TypeArray.IndexOf(bulletType);
        var proj = ProjectileHelper.NewUnscaledProjectile(source, position, velocity, ModContent.ProjectileType<GravityBullet>(), damage,
            knockback, ai0: typeIndex, ai1: gravity, ai2: gravDir);
        
        return proj;
    }
    
    public override void AI()
    {
        Projectile.velocity += GravityDir.ToRotationVector2() * Gravity;
        
        Projectile.rotation = Projectile.velocity.ToRotation();
    }
}