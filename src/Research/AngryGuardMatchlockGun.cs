using Science;

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

		public override void OnResearchComplete(ScienceManagerPlayer manager, EResearchCompletionReason reason)
		{
			RecipeStorage.GetPlayerStorage(manager.Player).SetRecipeAvailability("angryguards.crafter.guardmatchlockgunday", true, "pipliz.crafter");
			RecipeStorage.GetPlayerStorage(manager.Player).SetRecipeAvailability("angryguards.crafter.guardmatchlockgunnight", true, "pipliz.crafter");
		}

	}

}

