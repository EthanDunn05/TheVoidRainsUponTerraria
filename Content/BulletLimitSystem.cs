using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using VoidRains.Content.Projectiles;

namespace VoidRains.Content;

/// <summary>
/// Limits bullet projectiles to 750 so they don't override player projectiles
/// </summary>
public class BulletLimitSystem : ModSystem
{
    // Types of projectiles to limit
    private int[] limitedTypes;

    public override void Load()
    {
        On_Projectile.NewProjectile_IEntitySource_float_float_float_float_int_int_float_int_float_float_float +=
            LimitProjectile;
    }

    public override void OnModLoad()
    {
        limitedTypes =
        [
            ModContent.ProjectileType<Bullet>(),
            ModContent.ProjectileType<GravityBullet>()
        ];
    }

    // Jesus, that method signature
    private int LimitProjectile(
        On_Projectile.orig_NewProjectile_IEntitySource_float_float_float_float_int_int_float_int_float_float_float orig,
        IEntitySource spawnsource, float x, float y, float speedx, float speedy, int type, int damage, float knockback,
        int owner, float ai0, float ai1, float ai2)
    {
        var res = orig(spawnsource, x, y, speedx, speedy, type, damage, knockback, owner, ai0, ai1, ai2);
        
        // Don't check if not trying to spawn a bullet
        if (!limitedTypes.Contains(type)) return res;
        
        // Limit to 750 bullets to leave room for player projectiles
        if (Main.projectile.Count(p => p.active) > 750)
        {
            var result = 1000;
            var min = 9999999;
            for (var i = 0; i < 1000; i++)
            {
                var proj = Main.projectile[i];
                if (!proj.netImportant && proj.timeLeft < min && limitedTypes.Contains(proj.type))
                {
                    result = i;
                    min = proj.timeLeft;
                }
            }

            Main.projectile[result].active = false;
        }

        return res;
    }

    public override void Unload()
    {
        On_Projectile.NewProjectile_IEntitySource_float_float_float_float_int_int_float_int_float_float_float -=
            LimitProjectile;
    }
}