using Science;
using Recipes;

namespace AngryGuards {

	[AutoLoadedResearchable]
	public class AngryGuardMatchlockGun : BaseResearchable
	{

		public AngryGuardMatchlockGun()
		{
			key = "angryguards.research.angryguardmatchlockgun";
			icon = "gamedata/mods/adrenalynn/AngryGuards/icons/angryguardmatchlockgun.png";
			iterationCount = 50;
			AddIterationRequirement("leadbullet", 5);
			AddIterationRequirement("matchlockgun");
			AddDependency("pipliz.baseresearch.matchlockgun");
			AddDependency("angryguards.research.angryguardcrossbow");
		}

		public override void OnResearchComplete(ColonyScienceState manager, EResearchCompletionReason reason)
		{
			RecipeColony recipeData = manager.Colony.RecipeData;
			recipeData.UnlockRecipe(new RecipeKey("angryguards.crafter.guardmatchlockgunday"));
			recipeData.UnlockRecipe(new RecipeKey("angryguards.crafter.guardmatchlockgunnight"));
		}

	}

}

