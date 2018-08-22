using Pipliz.Mods.APIProvider.Science;
using Server.Science;

namespace AngryGuards {

	[AutoLoadedResearchable]
	public class AngryGuardBow : BaseResearchable
	{

		public AngryGuardBow()
		{
			key = "angryguards.research.angryguardbow";
			icon = "gamedata/mods/adrenalynn/AngryGuards/icons/angryguardbow.png";
			iterationCount = 10;
			AddIterationRequirement("bronzearrow", 3);
			AddIterationRequirement("bow");
			AddDependency("pipliz.baseresearch.archery");
		}

		public override void OnResearchComplete(ScienceManagerPlayer manager, EResearchCompletionReason reason)
		{
			RecipeStorage.GetPlayerStorage(manager.Player).SetRecipeAvailability("angryguards.crafter.guardbowday", true, "pipliz.crafter");
			RecipeStorage.GetPlayerStorage(manager.Player).SetRecipeAvailability("angryguards.crafter.guardbownight", true, "pipliz.crafter");
		}

	}

}

