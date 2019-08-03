using System;
using System.IO;
using Pipliz;
using Pipliz.JSON;
using Jobs;
using Chatting;

namespace AngryGuards {

	// Active: guards shoot all players (unless friendly)
	// Passive: guards shoot players only after they attack colonists
	public enum GuardMode {
		Active,
		Passive
	};

	[ModLoader.ModManager]
	public static class AngryGuards
	{
		public const string NAMESPACE = "AngryGuards";
		public const string PERMISSION_PREFIX = "mods.angryguards";

		// config definitions
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

		public static GuardMode ModeSetting = GuardMode.Active;

		private const string CONFIG_FILE = "angryguards-config.json";
		private static string ConfigFilePath {
			get {
				return Path.Combine(Path.Combine("gamedata", "savegames"), Path.Combine(ServerManager.WorldName, CONFIG_FILE));
			}
		}

		// initialize blocks and jobs
		[ModLoader.ModCallback(ModLoader.EModCallbackType.AfterItemTypesDefined, NAMESPACE + ".RegisterJobs")]
		[ModLoader.ModCallbackDependsOn("create_servermanager_trackers")]
		[ModLoader.ModCallbackDependsOn("pipliz.server.loadnpctypes")]
		[ModLoader.ModCallbackProvidesFor("create_savemanager")]
		public static void AfterItemTypesDefined()
		{
			LoadConfig();

			// Bow Guard
			AngryGuardJobSettings BowGuardDay = new AngryGuardJobSettings(
				"angryguards.guardbowday", "pipliz.guardbowday",
				AngryGuardJobSettings.EGuardSleepType.Night,
				Bow.Damage, Bow.Range, Bow.Reload, "bowShoot",
				new InventoryItem(BlockTypes.BuiltinBlocks.Indices.bronzearrow),
				new InventoryItem(BlockTypes.BuiltinBlocks.Indices.bow)
			);
			AngryGuardJobSettings BowGuardNight = new AngryGuardJobSettings(
				"angryguards.guardbownight", "pipliz.guardbownight",
				AngryGuardJobSettings.EGuardSleepType.Day,
				Bow.Damage, Bow.Range, Bow.Reload, "bowShoot",
				new InventoryItem(BlockTypes.BuiltinBlocks.Indices.bronzearrow),
				new InventoryItem(BlockTypes.BuiltinBlocks.Indices.bow)
			);

			// Crossbow Guard
			AngryGuardJobSettings CrossbowGuardDay = new AngryGuardJobSettings(
				"angryguards.guardcrossbowday", "pipliz.guardcrossbowday",
				AngryGuardJobSettings.EGuardSleepType.Night,
				Crossbow.Damage, Crossbow.Range, Crossbow.Reload, "bowShoot",
				new InventoryItem(BlockTypes.BuiltinBlocks.Indices.crossbowbolt),
				new InventoryItem(BlockTypes.BuiltinBlocks.Indices.crossbow)
			);
			AngryGuardJobSettings CrossbowGuardNight = new AngryGuardJobSettings(
				"angryguards.guardcrossbownight", "pipliz.guardcrossbownight",
				AngryGuardJobSettings.EGuardSleepType.Day,
				Crossbow.Damage, Crossbow.Range, Crossbow.Reload, "bowShoot",
				new InventoryItem(BlockTypes.BuiltinBlocks.Indices.crossbowbolt),
				new InventoryItem(BlockTypes.BuiltinBlocks.Indices.crossbow)
			);

			// Matchlock Gun Guard
			AngryGuardJobSettings MatchlockGunGuardDay = new AngryGuardJobSettings(
				"angryguards.guardmatchlockgunday", "pipliz.guardmatchlockday",
				AngryGuardJobSettings.EGuardSleepType.Night,
				MatchlockGun.Damage, MatchlockGun.Range, MatchlockGun.Reload, "matchlock",
				new InventoryItem(BlockTypes.BuiltinBlocks.Indices.leadbullet),
				new InventoryItem(BlockTypes.BuiltinBlocks.Indices.matchlockgun)
			);
			MatchlockGunGuardDay.ShootItem.Add(new InventoryItem(BlockTypes.BuiltinBlocks.Indices.gunpowderpouch));
			MatchlockGunGuardDay.OnShootResultItem = new ItemTypes.ItemTypeDrops(BlockTypes.BuiltinBlocks.Indices.linenpouch, 1, 0.9f);

			AngryGuardJobSettings MatchlockGunGuardNight = new AngryGuardJobSettings(
				"angryguards.guardmatchlockgunnight", "pipliz.guardmatchlocknight",
				AngryGuardJobSettings.EGuardSleepType.Day,
				MatchlockGun.Damage, MatchlockGun.Range, MatchlockGun.Reload, "matchlock",
				new InventoryItem(BlockTypes.BuiltinBlocks.Indices.leadbullet),
				new InventoryItem(BlockTypes.BuiltinBlocks.Indices.matchlockgun)
			);
			MatchlockGunGuardNight.ShootItem.Add(new InventoryItem(BlockTypes.BuiltinBlocks.Indices.gunpowderpouch));
			MatchlockGunGuardNight.OnShootResultItem = new ItemTypes.ItemTypeDrops(BlockTypes.BuiltinBlocks.Indices.linenpouch, 1, 0.9f);

			ServerManager.BlockEntityCallbacks.RegisterEntityManager(new BlockJobManager<AngryGuardJobInstance>(BowGuardDay));
			ServerManager.BlockEntityCallbacks.RegisterEntityManager(new BlockJobManager<AngryGuardJobInstance>(BowGuardNight));
			ServerManager.BlockEntityCallbacks.RegisterEntityManager(new BlockJobManager<AngryGuardJobInstance>(CrossbowGuardDay));
			ServerManager.BlockEntityCallbacks.RegisterEntityManager(new BlockJobManager<AngryGuardJobInstance>(CrossbowGuardNight));
			ServerManager.BlockEntityCallbacks.RegisterEntityManager(new BlockJobManager<AngryGuardJobInstance>(MatchlockGunGuardDay));
			ServerManager.BlockEntityCallbacks.RegisterEntityManager(new BlockJobManager<AngryGuardJobInstance>(MatchlockGunGuardNight));


			CommandManager.RegisterCommand(new FriendlyCommand());
			CommandManager.RegisterCommand(new GlobalFriendlyCommand());
			Log.Write($"Angry Guards started with guard mode: {ModeSetting}");
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
				string setting;
				if (configJson.TryGetAs("guardmode", out setting)) {
					if (setting.Equals("active") || setting.Equals("Active")) {
						ModeSetting = GuardMode.Active;
					} else if (setting.Equals("passive") || setting.Equals("Passive")) {
						ModeSetting = GuardMode.Passive;
					} else {
						Log.Write($"ERROR: invalid guardmode setting '{setting}'. Using defaults");
					}
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

		// track NPC hits/kills for passive mode
		[ModLoader.ModCallback(ModLoader.EModCallbackType.OnNPCHit, NAMESPACE + ".OnNPCHit")]
		public static void OnNPCHit(NPC.NPCBase npc, ModLoader.OnHitData data)
		{
			if (!(data.HitSourceObject is Players.Player)) {
				return;
			}
			Players.Player killer = (Players.Player)data.HitSourceObject;
			foreach (Players.Player owner in npc.Colony.Owners) {
				if (owner == killer || PlayerTracker.IsFriendly(owner, killer)) {
					return;
				}
			}

			PlayerTracker.AddEnemy(npc.Colony, killer);
		}

	} // class

} // namespace

