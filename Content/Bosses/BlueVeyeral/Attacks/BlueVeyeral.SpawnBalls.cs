using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VoidRains.Helpers;

namespace VoidRains.Content.Bosses.BlueVeyeral;

public partial class BlueVeyeral
{
    private AcidAnimation? ballsAnimation;

    public AcidAnimation CreateBallsAnimation()
    {
        var anim = new AcidAnimation();

        // Super simple animation. Just spawn balls side to side
        const float heightVariance = 300f;
        const float distanceFromPlayer = 1000f;
        const int ballDelay = 60;

        anim.AddInstantEvent(0, () =>
        {
            // Spawn balls :)
            if (Main.netMode == NetmodeID.MultiplayerClient) return;

            var side = Main.rand.NextBool() ? -1 : 1;
            var pos = TargetPlayer.Center + new Vector2(
                distanceFromPlayer * -side,
                Main.rand.NextFloat(-heightVariance, heightVariance)
            );

            NPC.NewNPCDirect(NPC.GetSource_FromAI(), pos, ModContent.NPCType<BlueBall>(), NPC.whoAmI, side);
        });
        
        anim.AddInstantEvent(ballDelay, () => { });

        return anim;
    }
}