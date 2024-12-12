using System.Collections.Immutable;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

namespace VoidRains.Common;

public class BeamTypes
{
    public static readonly BeamType Void = new BeamType("Void", 5, 25);
    
    public static readonly ImmutableArray<BeamType> TypeArray =
    [
        Void
    ];
}

public class BeamType(string beamType, int beamFrames, int chargeFrames)
{
    public Asset<Texture2D> ChargeTexture => TextureRegistry.Beam(beamType + "Charge");
    public Asset<Texture2D> StartTexture => TextureRegistry.Beam(beamType + "Start");
    public Asset<Texture2D> MiddleTexture => TextureRegistry.Beam(beamType + "Middle");
    public Asset<Texture2D> EndTexture => TextureRegistry.Beam(beamType + "End");
    public Asset<Texture2D> ThinTexture => TextureRegistry.Beam(beamType + "Thin");

    public int BeamFrames = beamFrames;
    public int ChargeFrames = chargeFrames;
}