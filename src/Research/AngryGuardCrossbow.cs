using Pipliz.Mods.APIProvider.Science;
using Server.Science;

namespace AngryGuards {

	[AutoLoadedResearchable]
	public class AngryGuardCrossbow : BaseResearchable
	{

		public AngryGuardCrossbow()
		{
			key = "angryguards.research.angryguardcrossbow";
			icon = "gamedata/mods/adrenalynn/AngryGuards/icons/angryguardcrossbow.png";
			iterationCount = 50;
			AddIterationRequirement("crossbowbolt", 5);
			AddIterationRequirement("crossbow");
			AddDependency("pipliz.baseresearch.crossbow");
			AddDependency("angryguards.research.angryguardbow");
		}

		public override void OnResearchComplete(ScienceManagerPlayer manager, EResearchCompletionReason reason)
		{
			RecipeStorage.GetPlayerStorage(manager.Player).SetRecipeAvailability("angryguards.crafter.guardcrossbowday", true, "pipliz.crafter");
			RecipeStorage.GetPlayerStorage(manager.Player).SetRecipeAvailability("angryguards.crafter.guardcrossbownight", true, "pipliz.crafter");
		}

	}

}

