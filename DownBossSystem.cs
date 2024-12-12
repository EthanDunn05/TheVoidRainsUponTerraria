using System.IO;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace VoidRains;

public class DownBossSystem : ModSystem
{
    // I should make this somewhat automatic at some point
    public static bool downedBlueVeyeral = false;
    
    public override void ClearWorld()
    {
        downedBlueVeyeral = false;
    }

    public override void SaveWorldData(TagCompound tag)
    {
        if (downedBlueVeyeral) tag["downedBlueVeyeral"] = true;
    }

    public override void LoadWorldData(TagCompound tag)
    {
        downedBlueVeyeral = tag.ContainsKey("downedBlueVeyeral");
    }

    public override void NetSend(BinaryWriter writer)
    {
        writer.WriteFlags(downedBlueVeyeral);
    }

    public override void NetReceive(BinaryReader reader)
    {
        reader.ReadFlags(out downedBlueVeyeral);
    }
}