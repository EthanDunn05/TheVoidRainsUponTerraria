﻿using Terraria.Audio;

namespace VoidRains.Common;

public class SoundRegistry
{
    public static SoundStyle VoidRainsHit = Sound("VoidRainsHit");

    public static SoundStyle VoidRainsDeath = Sound("VoidRainsDeath");

    public static SoundStyle Sound(string name) => new SoundStyle("VoidRains/Assets/Sounds/" + name);
}