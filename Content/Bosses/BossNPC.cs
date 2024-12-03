using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VoidRains.Helpers.StateManagement;

namespace VoidRains.Content.Bosses;

/// <summary>
/// This is a specialized NPC that manages much of the structures common to bosses
/// </summary>
public abstract class BossNPC : ModNPC
{
    private bool isFirstFrame = true;
    
    /// <summary>
    /// Common data and management used in all bosses.
    /// </summary>
    public AttackManager AttackManager { get; } = new();
    
    public Player TargetPlayer => Main.player[NPC.target];

    public virtual void OnFirstFrame()
    {
        
    }

    public sealed override void AI()
    {
        if (isFirstFrame)
        {
            OnFirstFrame();
            isFirstFrame = false;
        }
        
        AttackManager.PreAttackAi();
        BossAI();
        if (Main.netMode != NetmodeID.Server)
        {
            ClientBossAI();
        }
        
        AttackManager.PostAttackAi();
    }

    public virtual void BossAI() { }

    public virtual void ClientBossAI() { }

    /// <summary>
    /// Gets a random player within a certain distance from the boss's center
    /// </summary>
    /// <param name="maxDistance">The maximum distance a player can be to be chosen</param>
    /// <returns>A random player</returns>
    public Player RandomTargetablePlayer(float maxDistance)
    {
        var players = Main.player
            .Where(p => p.active && !p.dead && NPC.Distance(p.Center) < maxDistance)
            .ToList();
        var player = Main.rand.NextFromCollection(players);

        return player;
    }
    
    public virtual void SendBossAI(BinaryWriter writer)
    {
        
    }

    public override void SendExtraAI(BinaryWriter writer)
    {
        AttackManager.Serialize(writer);
        
        SendBossAI(writer);
    }

    public virtual void RecieveBossAI(BinaryReader reader)
    {
        
    }

    public sealed override void ReceiveExtraAI(BinaryReader reader)
    {
        AttackManager.Deserialize(reader);
        
        RecieveBossAI(reader);
    }

    protected void NetSync(bool onlySendFromServer = true)
    {
        if (onlySendFromServer && Main.netMode != NetmodeID.Server)
            return;
        
        if (Main.netMode != NetmodeID.SinglePlayer)
            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, NPC.whoAmI);
    }

    protected virtual void LookTowards(Vector2 target, float power)
    {
        NPC.rotation = NPC.rotation.AngleLerp(NPC.AngleTo(target), power);
    }

    protected static bool IsTargetGone(NPC npc)
    {
        var player = Main.player[npc.target];
        return !player.active || player.dead;
    }
}