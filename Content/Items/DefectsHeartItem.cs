using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using VoidRains.Content.Bosses.BlueVeyeral;

namespace VoidRains.Content.Items;

public class DefectsHeartItem : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 1;
        ItemID.Sets.SortingPriorityBossSpawns[Type] = 12;
    }
    
    public override void SetDefaults()
    {
        Item.width = 20;
        Item.height = 17;
        Item.maxStack = 999;
        Item.value = Terraria.Item.buyPrice(platinum: 1);
        Item.rare = ItemRarityID.Red;
        Item.useAnimation = 30;
        Item.useTime = 30;
        Item.useStyle = ItemUseStyleID.HoldUp;
        Item.consumable = false;
    }

    public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
    {
        itemGroup = ContentSamples.CreativeHelper.ItemGroup.BossSpawners;
    }

    public override bool CanUseItem(Player player)
    {
        return !NPC.AnyNPCs(ModContent.NPCType<BlueVeyeral>());
    }

    public override bool? UseItem(Player player)
    {
        if (player.whoAmI != Main.myPlayer) return true;
        
        var type = ModContent.NPCType<BlueVeyeral>();
        if (Main.netMode != NetmodeID.MultiplayerClient)
        {
            NPC.SpawnOnPlayer(player.whoAmI, type);
        }
        else
        {
            NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: type);
        }

        return true;
    }

    public override void AddRecipes()
    {
        Recipe.Create(Type, 1)
            .AddTile(TileID.LunarCraftingStation)
            .AddIngredient(ItemID.LunarBar, 10)
            .AddIngredient(ItemID.FragmentNebula, 5)
            .Register();
    }
}