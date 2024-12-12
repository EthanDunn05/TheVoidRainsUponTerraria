using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace VoidRains.Helpers;

public static class ProjectileHelper
{
    /// <summary>
    /// A helper method for spawning projectiles with some changes.
    /// Spawning projectiles with this method automatically handles sidedness, retuning null if trying to spawn the
    /// projectile on an invalid host.
    /// The damage scaling due to difficulty is also counteracted for a more sensible scaling that actually makes sense.
    /// </summary>
    /// <returns>The projectile that was spawned. Null if called from the wrong host such as calling from a multiplayer client for an NPC projectile.</returns>
    public static Projectile? NewUnscaledProjectile(IEntitySource source, float X, float spawnY, float velocityX, float velocityY, int type, int damage, float knockback, int owner = -1, float ai0 = 0f, float ai1 = 0f, float ai2 = 0f)
    {
        // Check for server side spawning here
        if (owner == -1 && Main.netMode == NetmodeID.MultiplayerClient) return null;
        
        // Only allow a player to spawn the projectile if they own it
        if (owner >= 0 && owner != Main.myPlayer) return null;
        
        // Blatantly stolen from luminance
        // The only difference between this and Luminance's implementation is that this doesn't default to the player
        // as the owner and defaults to the server like normal
        float damageJankCorrectionFactor = 0.5f;
        if (Main.expertMode)
            damageJankCorrectionFactor = 0.25f;
        if (Main.masterMode)
            damageJankCorrectionFactor = 0.1667f;
        damage = (int)(damage * damageJankCorrectionFactor);

        int index = Projectile.NewProjectile(source, X, spawnY, velocityX, velocityY, type, damage, knockback, owner, ai0, ai1, ai2);
        if (index >= 0 && index < Main.maxProjectiles)
            Main.projectile[index].netUpdate = true;

        return Main.projectile[index];
    }
    
    /// <summary>
    /// A helper method for spawning projectiles with some changes.
    /// Spawning projectiles with this method automatically handles sidedness, retuning null if trying to spawn the
    /// projectile on an invalid host.
    /// The damage scaling due to difficulty is also counteracted for a more sensible scaling that actually makes sense.
    /// </summary>
    /// <returns>The projectile that was spawned. Null if called from the wrong host such as calling from a multiplayer client for an NPC projectile.</returns>
    public static Projectile? NewUnscaledProjectile(IEntitySource source, Vector2 center, Vector2 velocity, int type, int damage, float knockback, int owner = -1, float ai0 = 0f, float ai1 = 0f, float ai2 = 0f)
    {
        return NewUnscaledProjectile(source, center.X, center.Y, velocity.X, velocity.Y, type, damage, knockback, owner, ai0, ai1, ai2);
    }

    public static void Sync(this Projectile self)
    {
        NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, self.whoAmI);
    }
}