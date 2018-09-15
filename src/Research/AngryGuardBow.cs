using Science;
using Recipes;

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

		public override void OnResearchComplete(ColonyScienceState manager, EResearchCompletionReason reason)
		{
			RecipeColony recipeData = manager.Colony.RecipeData;
			recipeData.UnlockRecipe(new RecipeKey("angryguards.crafter.guardbowday"));
			recipeData.UnlockRecipe(new RecipeKey("angryguards.crafter.guardbownight"));
		}

	}

}

