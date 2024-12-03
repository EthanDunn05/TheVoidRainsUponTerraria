using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.Audio;

namespace VoidRains.Common;

public class BulletTypes
{
    public static readonly BulletType Radiant = new("RadBullet", "ShootRadiant", 8);
    public static readonly BulletType Radiant2 = new("RadBullet2", "ShootRadiant", 8);
    public static readonly BulletType Void = new("VoidBullet", "ShootVoid", 13);
    public static readonly BulletType VoidBright = new("VoidBulletBright", "ShootVoid", 13);
    
    // Mainly used for syncing purposes. Order does not matter if used correctly
    // Don't hard code accesses into this array. Use IndexOf instead
    public static readonly ImmutableArray<BulletType> TypeArray =
    [
        Radiant,
        Radiant2,
        Void,
        VoidBright
    ];
}

/// <summary>
/// Represents a type of standard bullet.
/// </summary>
/// <param name="texName">The filename of the texture</param>
/// <param name="shootSound">The filename of the sound to use</param>
/// <param name="frames">How many frames of animation the texture has</param>
public class BulletType(string texName, string shootSound, int frames = 1)
{
    public Asset<Texture2D> Texture => TextureRegistry.Bullet(texName);
    public SoundStyle ShootSound => SoundRegistry.Sound("Bullets/" + shootSound);
    public int Frames { get; } = frames;
}