using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Localization;
using Terraria.ModLoader;
using VoidRains.Content.Bosses.BlueVeyeral;
using VoidRains.Content.Items;

namespace VoidRains;

public class ModIntegration : ModSystem
{
    public override void PostSetupContent()
    {
        SetupMusicDisplay();
        SetupBossChecklist();
    }

    private void SetupMusicDisplay()
    {
	    // Music Display
	    if (!ModLoader.TryGetMod("MusicDisplay", out var display)) return;
	    var modName = Language.GetText("Mods.VoidRains.MusicDisplay.ModName");

	    AddSong("Assets/Music/BlueVeyeral", "BlueVeyeral");
	    return;

	    void AddSong(string songPath, string songName)
        {
	        var author = Language.GetText("Mods.VoidRains.MusicDisplay." + songName + ".Author");
	        var displayName = Language.GetText("Mods.VoidRains.MusicDisplay." + songName + ".DisplayName");
	        display.Call("AddMusic", (short)MusicLoader.GetMusicSlot(Mod, songPath), displayName, author, modName);
        }
    }

    private void SetupBossChecklist()
    {
	    // Boss Checklist
	    if (!ModLoader.TryGetMod("BossChecklist", out var bossChecklist)) return;
		AddBossLog(
			bossChecklist,
			nameof(BlueVeyeral),
			18.1f,
			() => DownBossSystem.downedBlueVeyeral,
			[ ModContent.NPCType<BlueVeyeral>() ],
			ModContent.ItemType<DefectsHeartItem>(),
			(sb, rect, col) =>
			{
				var texture = ModContent.Request<Texture2D>("VoidRains/Assets/Textures/Bestiary/BlueVeyeral_Preview").Value;
				var centered = new Vector2(rect.X + (rect.Width / 2f) - (texture.Width / 2f), rect.Y + (rect.Height / 2f) - (texture.Height / 2f));
				sb.Draw(texture, centered, col);
			}
		);
    }

    private void AddBossLog(Mod bossChecklistMod, string internalName, float progression, Func<bool> downedBool, List<int> bossIds,
	    int spawnItem, Action<SpriteBatch, Rectangle, Color> drawCode)
    {
	    bossChecklistMod.Call(
			"LogBoss",
			Mod,
			internalName,
			progression,
			downedBool,
			bossIds,
			new Dictionary<string, object>()
			{
				{ "spawnInfo", Language.GetText($"Mods.VoidRains.NPCs.{internalName}.SpawnInfo").WithFormatArgs(spawnItem) },
				{ "spawnItems", spawnItem },
				{ "customPortrait", drawCode }
			}
	    );
    }
}