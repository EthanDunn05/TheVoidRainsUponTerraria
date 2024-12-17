using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VoidRains.Content.Buffs;
using VoidRains.Content.Projectiles;

namespace VoidRains.Content;

public class SafetyZoneSystem : ModSystem
{
    public override void PostUpdateProjectiles()
    {
        var zones = Main.projectile.Where(p => p.active && p.type == ModContent.ProjectileType<SafetyZone>()).ToArray();

        if (zones.Length == 0) return;
        
        // Only check the current player
        if (Main.netMode == NetmodeID.Server) return;

        var inZone = false;
        foreach (var zone in zones)
        {
            if (zone is null) continue;

            // If inside zone
            if (Main.LocalPlayer.Distance(zone.position) < zone.ai[0])
            {
                inZone = true;
                break;
            }
        }

        if (!inZone)
        {
            Main.LocalPlayer.AddBuff(ModContent.BuffType<OutsideZoneBuff>(), 2);
        }
    }
}