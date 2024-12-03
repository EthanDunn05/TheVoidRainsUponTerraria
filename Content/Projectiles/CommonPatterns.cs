using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using VoidRains.Common;

namespace VoidRains.Content.Projectiles;

public static class CommonPatterns
{
    public static void Arrow(IEntitySource source, Vector2 position, float direction, float speed, int damage,
        float kb, BulletType bulletType)
    {
        for (var i = 0; i < 6; i++)
        {
            var start = position + direction.ToRotationVector2() * 80;
            var leftOff = i * 16 * (direction + 2.62f).ToRotationVector2();
            Bullet.SpawnBasic(source, start + leftOff, direction.ToRotationVector2() * speed, damage, kb, bulletType);

            if (i > 0)
            {
                var rightOff = i * 16 * (direction - 2.62f).ToRotationVector2();
                Bullet.SpawnBasic(source, start + rightOff, direction.ToRotationVector2() * speed, damage,
                    kb, bulletType);
            }
        }
    }
}