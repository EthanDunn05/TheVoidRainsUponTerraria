using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using VoidRains.Common;

namespace VoidRains.Content.Projectiles;

/// <summary>
/// This projectile is an adaptation of the bullet object in TVRUHH.
/// Most of the bullet features were taken directly from the game with some minor tweaks to work with Terraria.
/// </summary>
public class Bullet : ModProjectile
{
    // The index of the type is synced
    private BulletType BulletType => BulletTypes.TypeArray[(int) Projectile.ai[0]];
    public override string Texture => TextureRegistry.InvisPath;
    
    // Data //
    
    // There are exactly 8 for compactly sending as one byte
    // These are synced early to determine what data to send or receive
    // They MUST NOT be changed after the bullet is created!!!
    private bool syncTurn = false;
    private bool syncSpeedSin = false;
    private bool syncTurnSin = false;
    private bool syncSpeedAccChange = false;
    private bool syncDirChange = false;
    private bool syncTurnChange = false;
    private bool syncSpeedSinChange = false;
    private bool syncTurnSinChange = false;
    
    // Stats
    public float SpeedAcc = 0f;
    public float Turn = 0f;
    public float TurnAcc = 0f;
    public float SpeedSin = 0f;
    public float SpeedSinAcc = 0f;
    public float SpeedSinScale = 0f;
    public float TurnSin = 0f;
    public float TurnSinAcc = 0f;
    public float TurnSinScale = 0f;
    
    // Timers
    public int ChangeSpeedTimer = -1;
    public int ChangeAccTimer = -1;
    public int ChangeDirTimer = -1;
    public int ChangeTurnTimer = -1;
    public int ChangeTurnAccTimer = -1;
    public int ChangeSpeedSinScaleTimer = -1;
    public int ChangeTurnSinScaleTimer = -1;
    
    // Changes from timers
    public float ChangedSpeed = 0f;
    public float ChangedAcc = 0f;
    public float ChangedDir = 0f;
    public float ChangedTurn = 0f;
    public float ChangedTurnAcc = 0f;
    public float ChangedSpeedSinScale = 0f;
    public float ChangedTurnSinScale = 0f;

    public bool LimitTurn = true;

    private Vector2? startVel;

    public override void SetDefaults()
    {
        Projectile.width = 5;
        Projectile.height = 5;
        Projectile.hostile = true;
        Projectile.tileCollide = false;
        Projectile.hide = true;
    }

    public override void SetStaticDefaults()
    {
    }
    
    // Bullet Types //

    public static Projectile SpawnBasic(IEntitySource source, Vector2 position, Vector2 velocity, int damage,
        float knockback, BulletType bulletType)
    {
        var typeIndex = BulletTypes.TypeArray.IndexOf(bulletType);
        var proj = Projectile.NewProjectileDirect(source, position, velocity, ModContent.ProjectileType<Bullet>(), damage,
            knockback, ai0: typeIndex);
        
        if (Main.netMode != NetmodeID.SinglePlayer && proj.owner == Main.myPlayer)
            NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, proj.whoAmI);

        return proj;
    }
    
    public static Projectile SpawnTurning(IEntitySource source, Vector2 position, Vector2 velocity, int damage,
        float knockback, float turnStart, int turnChangeTime, float turnEnd, BulletType bulletType)
    {
        var typeIndex = BulletTypes.TypeArray.IndexOf(bulletType);
        var proj =  Projectile.NewProjectileDirect(source, position, velocity, ModContent.ProjectileType<Bullet>(), damage,
            knockback, ai0: typeIndex);

        var modProj = (Bullet) proj.ModProjectile;
        modProj.syncTurn = true;
        modProj.syncTurnChange = true;
        modProj.Turn = turnStart;
        modProj.ChangeTurnTimer = turnChangeTime;
        modProj.ChangedTurn = turnEnd;
        
        if (Main.netMode != NetmodeID.SinglePlayer && proj.owner == Main.myPlayer)
            NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, proj.whoAmI);

        return proj;
    }
    
    public static Projectile SpawnWaveTurn(IEntitySource source, Vector2 position, Vector2 velocity, int damage,
        float knockback, float turnSinScale, float turnSinAcc, BulletType bulletType)
    {
        var typeIndex = BulletTypes.TypeArray.IndexOf(bulletType);
        var proj =  Projectile.NewProjectileDirect(source, position, velocity, ModContent.ProjectileType<Bullet>(), damage,
            knockback, ai0: typeIndex);

        var modProj = (Bullet) proj.ModProjectile;
        modProj.syncTurnSin = true;
        modProj.TurnSinScale = turnSinScale;
        modProj.TurnSinAcc = turnSinAcc;
        
        if (Main.netMode != NetmodeID.SinglePlayer && proj.owner == Main.myPlayer)
            NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, proj.whoAmI);

        return proj;
    }
    
    // AI //

    public override void AI()
    {
        startVel ??= Projectile.velocity;
        
        // Extract direction and speed
        var dir = Projectile.velocity.ToRotation();
        var speed = Projectile.velocity.Length();
        
        // Manage the timers
        ManageTimer(ref ChangeSpeedTimer, ref speed, ChangedSpeed);
        ManageTimer(ref ChangeAccTimer, ref SpeedAcc, ChangedAcc);
        ManageTimer(ref ChangeDirTimer, ref dir, ChangedDir);
        ManageTimer(ref ChangeTurnTimer, ref Turn, ChangedTurn);
        ManageTimer(ref ChangeTurnAccTimer, ref TurnAcc, ChangedTurnAcc);
        ManageTimer(ref ChangeSpeedSinScaleTimer, ref SpeedSinScale, ChangedSpeedSinScale);
        ManageTimer(ref ChangeTurnSinScaleTimer, ref TurnSinScale, ChangedTurnSinScale);

        // Here's where the magic happens :)
        speed += SpeedAcc;

        if (SpeedSinScale != 0f)
        {
            SpeedSin += SpeedSinAcc;
            speed += SpeedSinScale * MathF.Sin(SpeedSin);
        }

        dir += Turn;
        Turn += TurnAcc;

        if (TurnSinScale != 0f)
        {
            TurnSin += TurnSinAcc;
            dir += TurnSinScale * MathF.Sin(TurnSin);
        }

        // Convert direction and speed back into the velocity vector
        Projectile.velocity = dir.ToRotationVector2() * speed;
        
        // Limit the turn for turn and sin
        if (LimitTurn)
        {
            if (Vector2.Dot(Projectile.velocity, (Vector2) startVel) < 0) Projectile.active = false;
        }
        
        // Visuals
        Projectile.rotation = Projectile.velocity.ToRotation();
        
        Projectile.frameCounter++;
        if (Projectile.frameCounter >= 2)
        {
            Projectile.frame = (Projectile.frame + 1) % BulletType.Frames;
            Projectile.frameCounter = 0;
        }
    }

    private void ManageTimer(ref int timer, ref float value, float changedValue)
    {
        // Timer is disabled
        if (timer == -1) return;
        
        timer--;
        if (timer != 0) return;

        value = changedValue;
        timer = -1; // Disable timer when done
    }
    
    // Drawing //

    public override bool PreDraw(ref Color lightColor)
    {
        // Eventually this will need to be adapted for fire bullets since those aren't centered
        
        var drawPos = Projectile.Center - Main.screenPosition;
        var texture = BulletType.Texture.Value;
        var frame = texture.Frame(verticalFrames: BulletType.Frames, frameY: Projectile.frame);
        var origin = texture.Size() / new Vector2(1, BulletType.Frames) * 0.5f;

        Main.spriteBatch.Draw(
            texture, drawPos,
            frame, Color.White,
            Projectile.rotation, origin, Projectile.scale,
            SpriteEffects.None, 0f);

        return false;
    }
    
    public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs,
        List<int> behindProjectiles, List<int> overPlayers,
        List<int> overWiresUI)
    {
        behindNPCs.Add(index);
    }
    
    // Syncing //
    // I'm using one byte to communicate what actually needs to be synced.
    // This should save a ton of bandwidth on not sending default values
    
    public override void SendExtraAI(BinaryWriter writer)
    {
        // Write what data to sync
        writer.WriteFlags(
            syncTurn,
            syncSpeedSin,
            syncTurnSin,
            syncSpeedAccChange,
            syncDirChange,
            syncTurnChange,
            syncSpeedSinChange,
            syncTurnSinChange
        );
        
        writer.Write(SpeedAcc);
        
        if (syncTurn) writer.Write(Turn);
        
        if (syncSpeedSin)
        {
            writer.Write(SpeedSin);
            writer.Write(SpeedSinScale);
            writer.Write(SpeedSinAcc);
        }

        if (syncTurnSin)
        {
            writer.Write(TurnSin);
            writer.Write(TurnSinScale);
            writer.Write(TurnSinAcc);
        }

        if (syncSpeedAccChange)
        {
            writer.Write7BitEncodedInt(ChangeSpeedTimer);
            writer.Write7BitEncodedInt(ChangeAccTimer);
            writer.Write(ChangedSpeed);
            writer.Write(ChangedAcc);
        }

        if (syncDirChange)
        {
            writer.Write7BitEncodedInt(ChangeDirTimer);
            writer.Write(ChangedDir);
        }
        
        if (syncTurnChange)
        {
            writer.Write7BitEncodedInt(ChangeTurnTimer);
            writer.Write(ChangedTurn);
        }

        if (syncSpeedSinChange)
        {
            writer.Write7BitEncodedInt(ChangeSpeedSinScaleTimer);
            writer.Write(ChangedSpeedSinScale);
        }
        
        if (syncTurnSinChange)
        {
            writer.Write7BitEncodedInt(ChangeTurnSinScaleTimer);
            writer.Write(ChangedTurnSinScale);
        }
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        // Read what data to sync
        reader.ReadFlags(
            out syncTurn,
            out syncSpeedSin,
            out syncTurnSin,
            out syncSpeedAccChange,
            out syncDirChange,
            out syncTurnChange,
            out syncSpeedSinChange,
            out syncTurnSinChange
        );
        
        SpeedAcc = reader.ReadSingle();
        
        if (syncTurn) Turn = reader.ReadSingle();
        
        if (syncSpeedSin)
        {
            SpeedSin = reader.ReadSingle();
            SpeedSinScale = reader.ReadSingle();
            SpeedSinAcc = reader.ReadSingle();
        }

        if (syncTurnSin)
        {
            TurnSin = reader.ReadSingle();
            TurnSinScale = reader.ReadSingle();
            TurnSinAcc = reader.ReadSingle();
        }

        if (syncSpeedAccChange)
        {
            ChangeSpeedTimer = reader.Read7BitEncodedInt();
            ChangeAccTimer = reader.Read7BitEncodedInt();
            ChangedSpeed = reader.ReadSingle();
            ChangedAcc = reader.ReadSingle();
        }

        if (syncDirChange)
        {
            ChangeDirTimer = reader.Read7BitEncodedInt();
            ChangedDir = reader.ReadSingle();
        }
        
        if (syncTurnChange)
        {
            ChangeTurnTimer = reader.Read7BitEncodedInt();
            ChangedTurn = reader.ReadSingle();
        }

        if (syncSpeedSinChange)
        {
            ChangeSpeedSinScaleTimer = reader.Read7BitEncodedInt();
            ChangedSpeedSinScale = reader.ReadSingle();
        }
        
        if (syncTurnSinChange)
        {
            ChangeTurnSinScaleTimer = reader.Read7BitEncodedInt();
            ChangedTurnSinScale = reader.ReadSingle();
        }
    }
}