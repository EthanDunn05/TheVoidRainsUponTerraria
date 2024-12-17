using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace VoidRains.Content.Buffs;

public class OutsideZoneBuff : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = true;
        Main.buffNoSave[Type] = true;
    }

    public override void Update(Player player, ref int buffIndex)
    {
        player.GetModPlayer<OutsideZonePlayer>().OutsideZone = true;
    }

    public class OutsideZonePlayer : ModPlayer
    {
        public bool OutsideZone = false;
        public int DamageTimer = 120;

        public override void ResetEffects()
        {
            OutsideZone = false;
        }

        public override void PostUpdateBuffs()
        {
            if (!OutsideZone)
            {
                DamageTimer++;
                return;
            }

            DamageTimer--;
            DamageTimer = (int) MathHelper.Clamp(DamageTimer, 0, 120);

            if (DamageTimer <= 0)
            {
                Main.chatMonitor.NewText("Hit!");
                DamageTimer = 120;
            }
        }
    }
}