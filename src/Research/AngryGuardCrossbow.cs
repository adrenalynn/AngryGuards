using Science;
using Recipes;

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

		public override void OnResearchComplete(ColonyScienceState manager, EResearchCompletionReason reason)
		{
			RecipeColony recipeData = manager.Colony.RecipeData;
			recipeData.UnlockRecipe(new RecipeKey("angryguards.crafter.guardcrossbowday"));
			recipeData.UnlockRecipe(new RecipeKey("angryguards.crafter.guardcrossbownight"));
		}

	}

}

