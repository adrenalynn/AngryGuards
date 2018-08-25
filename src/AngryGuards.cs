using Pipliz;
using Pipliz.Mods.APIProvider.Jobs;
using ChatCommands;

namespace AngryGuards {

	[ModLoader.ModManager]
	public static class AngryGuards
	{
		public const string NAMESPACE = "AngryGuards";
		public const string PERMISSION_PREFIX = "mods.angryguards";

		[ModLoader.ModCallback(ModLoader.EModCallbackType.AfterItemTypesDefined, NAMESPACE + ".RegisterJobs")]
		[ModLoader.ModCallbackProvidesFor("pipliz.apiprovider.jobs.resolvetypes")]
		public static void AfterItemTypesDefined()
		{
			Blocks.Setup();
			BlockJobManagerTracker.Register<AngryGuardBowDayJob>("angryguards.guardbowday");
			BlockJobManagerTracker.Register<AngryGuardBowNightJob>("angryguards.guardbownight");
			BlockJobManagerTracker.Register<AngryGuardCrossbowDayJob>("angryguards.guardcrossbowday");
			BlockJobManagerTracker.Register<AngryGuardCrossbowNightJob>("angryguards.guardcrossbownight");
			BlockJobManagerTracker.Register<AngryGuardMatchlockGunDayJob>("angryguards.guardmatchlockgunday");
			BlockJobManagerTracker.Register<AngryGuardMatchlockGunNightJob>("angryguards.guardmatchlockgunnight");
			Log.Write("Angry Guards completed registering jobs");
			CommandManager.RegisterCommand(new FriendlyCommand());
			CommandManager.RegisterCommand(new GlobalFriendlyCommand());
		}

		[ModLoader.ModCallback(ModLoader.EModCallbackType.AfterWorldLoad, NAMESPACE + ".AfterWorldLoad")]
		public static void AfterWorldLoad()
		{
			PlayerTracker.Load();
		}

		[ModLoader.ModCallback(ModLoader.EModCallbackType.OnQuit, NAMESPACE + ".OnQuit")]
		public static void OnQuit()
		{
			PlayerTracker.Save();
		}

		public static class Blocks
		{
			public static ItemTypes.ItemType GuardBowJobDayXN;
			public static ItemTypes.ItemType GuardBowJobDayXP;
			public static ItemTypes.ItemType GuardBowJobDayZN;
			public static ItemTypes.ItemType GuardBowJobDayZP;

			public static ItemTypes.ItemType GuardBowJobNightXN;
			public static ItemTypes.ItemType GuardBowJobNightXP;
			public static ItemTypes.ItemType GuardBowJobNightZN;
			public static ItemTypes.ItemType GuardBowJobNightZP;

			public static ItemTypes.ItemType GuardCrossbowJobDayXN;
			public static ItemTypes.ItemType GuardCrossbowJobDayXP;
			public static ItemTypes.ItemType GuardCrossbowJobDayZN;
			public static ItemTypes.ItemType GuardCrossbowJobDayZP;

			public static ItemTypes.ItemType GuardCrossbowJobNightXN;
			public static ItemTypes.ItemType GuardCrossbowJobNightXP;
			public static ItemTypes.ItemType GuardCrossbowJobNightZN;
			public static ItemTypes.ItemType GuardCrossbowJobNightZP;

			public static ItemTypes.ItemType GuardMatchlockGunJobDayXN;
			public static ItemTypes.ItemType GuardMatchlockGunJobDayXP;
			public static ItemTypes.ItemType GuardMatchlockGunJobDayZN;
			public static ItemTypes.ItemType GuardMatchlockGunJobDayZP;

			public static ItemTypes.ItemType GuardMatchlockGunJobNightXN;
			public static ItemTypes.ItemType GuardMatchlockGunJobNightXP;
			public static ItemTypes.ItemType GuardMatchlockGunJobNightZN;
			public static ItemTypes.ItemType GuardMatchlockGunJobNightZP;

			public static void Setup()
			{
				GuardBowJobDayXN = ItemTypes.GetType(ItemTypes.IndexLookup.GetIndex("angryguards.guardbowdayx-"));
				GuardBowJobDayXP = ItemTypes.GetType(ItemTypes.IndexLookup.GetIndex("angryguards.guardbowdayx+"));
				GuardBowJobDayZN = ItemTypes.GetType(ItemTypes.IndexLookup.GetIndex("angryguards.guardbowdayz-"));
				GuardBowJobDayZP = ItemTypes.GetType(ItemTypes.IndexLookup.GetIndex("angryguards.guardbowdayz+"));

				GuardBowJobNightXN = ItemTypes.GetType(ItemTypes.IndexLookup.GetIndex("angryguards.guardbownightx-"));
				GuardBowJobNightXP = ItemTypes.GetType(ItemTypes.IndexLookup.GetIndex("angryguards.guardbownightx+"));
				GuardBowJobNightZN = ItemTypes.GetType(ItemTypes.IndexLookup.GetIndex("angryguards.guardbownightz-"));
				GuardBowJobNightZP = ItemTypes.GetType(ItemTypes.IndexLookup.GetIndex("angryguards.guardbownightz+"));

				GuardCrossbowJobDayXN = ItemTypes.GetType(ItemTypes.IndexLookup.GetIndex("angryguards.guardcrossbowdayx-"));
				GuardCrossbowJobDayXP = ItemTypes.GetType(ItemTypes.IndexLookup.GetIndex("angryguards.guardcrossbowdayx+"));
				GuardCrossbowJobDayZN = ItemTypes.GetType(ItemTypes.IndexLookup.GetIndex("angryguards.guardcrossbowdayz-"));
				GuardCrossbowJobDayZP = ItemTypes.GetType(ItemTypes.IndexLookup.GetIndex("angryguards.guardcrossbowdayz+"));

				GuardCrossbowJobNightXN = ItemTypes.GetType(ItemTypes.IndexLookup.GetIndex("angryguards.guardcrossbownightx-"));
				GuardCrossbowJobNightXP = ItemTypes.GetType(ItemTypes.IndexLookup.GetIndex("angryguards.guardcrossbownightx+"));
				GuardCrossbowJobNightZN = ItemTypes.GetType(ItemTypes.IndexLookup.GetIndex("angryguards.guardcrossbownightz-"));
				GuardCrossbowJobNightZP = ItemTypes.GetType(ItemTypes.IndexLookup.GetIndex("angryguards.guardcrossbownightz+"));

				GuardMatchlockGunJobDayXN = ItemTypes.GetType(ItemTypes.IndexLookup.GetIndex("angryguards.guardmatchlockgundayx-"));
				GuardMatchlockGunJobDayXP = ItemTypes.GetType(ItemTypes.IndexLookup.GetIndex("angryguards.guardmatchlockgundayx+"));
				GuardMatchlockGunJobDayZN = ItemTypes.GetType(ItemTypes.IndexLookup.GetIndex("angryguards.guardmatchlockgundayz-"));
				GuardMatchlockGunJobDayZP = ItemTypes.GetType(ItemTypes.IndexLookup.GetIndex("angryguards.guardmatchlockgundayz+"));

				GuardMatchlockGunJobNightXN = ItemTypes.GetType(ItemTypes.IndexLookup.GetIndex("angryguards.guardmatchlockgunnightx-"));
				GuardMatchlockGunJobNightXP = ItemTypes.GetType(ItemTypes.IndexLookup.GetIndex("angryguards.guardmatchlockgunnightx+"));
				GuardMatchlockGunJobNightZN = ItemTypes.GetType(ItemTypes.IndexLookup.GetIndex("angryguards.guardmatchlockgunnightz-"));
				GuardMatchlockGunJobNightZP = ItemTypes.GetType(ItemTypes.IndexLookup.GetIndex("angryguards.guardmatchlockgunnightz+"));
			}
		}

	} // class

} // namespace

