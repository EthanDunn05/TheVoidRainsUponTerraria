using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;

namespace VoidRains.Common;

public class TextureRegistry
{
    public static string InvisPath = "VoidRains/Assets/Textures/Invisible";
    
    // A simple function that loads a texture
    public static Asset<Texture2D> Tex(string name) => ModContent.Request<Texture2D>($"VoidRains/Assets/Textures/{name}", AssetRequestMode.ImmediateLoad);
    public static Asset<Texture2D> Boss(string name) => Tex("Boss/" + name);
    public static Asset<Texture2D> Bullet(string name) => Tex("Bullets/" + name);
    
    
    // Vanilla textures
    public static string TerrariaProjectile(int projID) => $"Terraria/Images/Projectile_{projID}";
    public static string TerrariaItem(int itemID) => $"Terraria/Images/Item_{itemID}";
    public static string TerrariaNPC(int npcID) => $"Terraria/Images/NPC_{npcID}";
}