using System;
using System.IO;
using Pipliz;
using Pipliz.JSON;
using Pipliz.APIProvider.Jobs;
using Chatting;

namespace AngryGuards {

	[ModLoader.ModManager]
	public static class AngryGuards
	{
		public const string NAMESPACE = "AngryGuards";
		public const string PERMISSION_PREFIX = "mods.angryguards";

		// weapon definition
		public struct Weapon {
			public int Damage, Reload, Range;

			public Weapon(int d, int r, int l)
			{
				Damage = d;
				Range = r;
				Reload = l;
			}
		}
		public static Weapon Bow = new Weapon(45, 20, 5);
		public static Weapon Crossbow = new Weapon(85, 25, 8);
		public static Weapon MatchlockGun = new Weapon(500, 30, 12);

		private const string CONFIG_FILE = "angryguards-config.json";
		private static string ConfigFilePath {
			get {
				return Path.Combine(Path.Combine("gamedata", "savegames"), Path.Combine(ServerManager.WorldName, CONFIG_FILE));
			}
		}

		// initialize blocks and jobs
		[ModLoader.ModCallback(ModLoader.EModCallbackType.AfterItemTypesDefined, NAMESPACE + ".RegisterJobs")]
		[ModLoader.ModCallbackDependsOn("create_servermanager_trackers")]
		[ModLoader.ModCallbackProvidesFor("create_savemanager")]
		public static void AfterItemTypesDefined()
		{
			LoadConfig();

			// Bow Guard
			AngryGuardJobSettings BowGuardDay = new AngryGuardJobSettings(
				"angryguards.guardbowday", "angryguards.guardbowday",
				AngryGuardJobSettings.EGuardSleepType.Night,
				Bow.Damage, Bow.Range, Bow.Reload, "bowShoot",
				new InventoryItem(BlockTypes.BuiltinBlocks.BronzeArrow, 1),
				new InventoryItem(BlockTypes.BuiltinBlocks.Bow, 1)
			);
			AngryGuardJobSettings BowGuardNight = new AngryGuardJobSettings(
				"angryguards.guardbownight", "angryguards.guardbownight",
				AngryGuardJobSettings.EGuardSleepType.Day,
				Bow.Damage, Bow.Range, Bow.Reload, "bowShoot",
				new InventoryItem(BlockTypes.BuiltinBlocks.BronzeArrow, 1),
				new InventoryItem(BlockTypes.BuiltinBlocks.Bow, 1)
			);

			// Crossbow Guard
			AngryGuardJobSettings CrossbowGuardDay = new AngryGuardJobSettings(
				"angryguards.guardcrossbowday", "angryguards.guardcrossbowday",
				AngryGuardJobSettings.EGuardSleepType.Night,
				Crossbow.Damage, Crossbow.Range, Crossbow.Reload, "bowShoot",
				new InventoryItem(BlockTypes.BuiltinBlocks.CrossbowBolt, 1),
				new InventoryItem(BlockTypes.BuiltinBlocks.Crossbow, 1)
			);
			AngryGuardJobSettings CrossbowGuardNight = new AngryGuardJobSettings(
				"angryguards.guardcrossbownight", "angryguards.guardcrossbownight",
				AngryGuardJobSettings.EGuardSleepType.Day,
				Crossbow.Damage, Crossbow.Range, Crossbow.Reload, "bowShoot",
				new InventoryItem(BlockTypes.BuiltinBlocks.CrossbowBolt, 1),
				new InventoryItem(BlockTypes.BuiltinBlocks.Crossbow, 1)
			);

			// Matchlock Gun Guard
			AngryGuardJobSettings MatchlockGunGuardDay = new AngryGuardJobSettings(
				"angryguards.guardmatchlockgunday", "angryguards.guardmatchlockgunday",
				AngryGuardJobSettings.EGuardSleepType.Night,
				MatchlockGun.Damage, MatchlockGun.Range, MatchlockGun.Reload, "matchlock",
				new InventoryItem(BlockTypes.BuiltinBlocks.LeadBullet, 1),
				new InventoryItem(BlockTypes.BuiltinBlocks.MatchlockGun, 1)
			);
			MatchlockGunGuardDay.ShootItem.Add(new InventoryItem(BlockTypes.BuiltinBlocks.GunpowderPouch, 1));
			MatchlockGunGuardDay.OnShootResultItem = new ItemTypes.ItemTypeDrops(BlockTypes.BuiltinBlocks.LinenPouch, 1, 0.9);

			AngryGuardJobSettings MatchlockGunGuardNight = new AngryGuardJobSettings(
				"angryguards.guardmatchlockgunnight", "angryguards.guardmatchlockgunnight",
				AngryGuardJobSettings.EGuardSleepType.Day,
				MatchlockGun.Damage, MatchlockGun.Range, MatchlockGun.Reload, "matchlock",
				new InventoryItem(BlockTypes.BuiltinBlocks.LeadBullet, 1),
				new InventoryItem(BlockTypes.BuiltinBlocks.MatchlockGun, 1)
			);
			MatchlockGunGuardNight.ShootItem.Add(new InventoryItem(BlockTypes.BuiltinBlocks.GunpowderPouch, 1));
			MatchlockGunGuardNight.OnShootResultItem = new ItemTypes.ItemTypeDrops(BlockTypes.BuiltinBlocks.LinenPouch, 1, 0.9);

			ServerManager.BlockEntityCallbacks.RegisterEntityManager(new BlockJobManager<AngryGuardJobInstance>(BowGuardDay));
			ServerManager.BlockEntityCallbacks.RegisterEntityManager(new BlockJobManager<AngryGuardJobInstance>(BowGuardNight));
			ServerManager.BlockEntityCallbacks.RegisterEntityManager(new BlockJobManager<AngryGuardJobInstance>(CrossbowGuardDay));
			ServerManager.BlockEntityCallbacks.RegisterEntityManager(new BlockJobManager<AngryGuardJobInstance>(CrossbowGuardNight));
			ServerManager.BlockEntityCallbacks.RegisterEntityManager(new BlockJobManager<AngryGuardJobInstance>(MatchlockGunGuardDay));
			ServerManager.BlockEntityCallbacks.RegisterEntityManager(new BlockJobManager<AngryGuardJobInstance>(MatchlockGunGuardNight));


			Log.Write("Angry Guards completed registering jobs");
			CommandManager.RegisterCommand(new FriendlyCommand());
			CommandManager.RegisterCommand(new GlobalFriendlyCommand());
		}

		// Load config
		public static void LoadConfig()
		{
			JSONNode configJson;
			if (!JSON.Deserialize(ConfigFilePath, out configJson, false)) {
				Log.Write($"{CONFIG_FILE} not found, using default values");
				return;
			}

			Log.Write($"Loading config from {CONFIG_FILE}");
			try {
				JSONNode weaponJson;
				if (configJson.TryGetAs("bow", out weaponJson)) {
					Bow.Damage = weaponJson.GetAs<int>("damage");
					Bow.Reload = weaponJson.GetAs<int>("reload");
					Bow.Range = weaponJson.GetAs<int>("range");
				}
				if (configJson.TryGetAs("crossbow", out weaponJson)) {
					Crossbow.Damage = weaponJson.GetAs<int>("damage");
					Crossbow.Reload = weaponJson.GetAs<int>("reload");
					Crossbow.Range = weaponJson.GetAs<int>("range");
				}
				if (configJson.TryGetAs("matchlockgun", out weaponJson)) {
					MatchlockGun.Damage = weaponJson.GetAs<int>("damage");
					MatchlockGun.Reload = weaponJson.GetAs<int>("reload");
					MatchlockGun.Range = weaponJson.GetAs<int>("range");
				}
			} catch (Exception e) {
				Log.Write($"Could not parse {CONFIG_FILE}: {e.Message}");
			}

			return;
		}

		// Load
		[ModLoader.ModCallback(ModLoader.EModCallbackType.AfterWorldLoad, NAMESPACE + ".AfterWorldLoad")]
		public static void AfterWorldLoad()
		{
			PlayerTracker.Load();
		}

		// Save
		[ModLoader.ModCallback(ModLoader.EModCallbackType.OnQuit, NAMESPACE + ".OnQuit")]
		public static void OnQuit()
		{
			PlayerTracker.Save();
		}

	} // class

} // namespace

