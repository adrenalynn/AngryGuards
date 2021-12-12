using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using Pipliz;
using BlockEntities.Implementations;
using Jobs;
using Chatting;

namespace AngryGuards {

	// Active: guards shoot all players (unless friendly)
	// Passive: guards shoot players only after they attack colonists
	public enum GuardModeSetting {
		Active,
		Passive
	};

	// Weapon definition
	public struct Weapon {
		public int Damage, Reload, Range;

		public Weapon(int d, int r, int l)
		{
			Damage = d;
			Range = r;
			Reload = l;
		}
	}

	// Config
	public class ModConfig
	{
		public Weapon Bow { get; set; }
		public Weapon Crossbow { get; set; }
		public Weapon MatchlockGun { get; set; }
		public GuardModeSetting GuardMode { get; set; }
		public bool ShootMountedPlayers { get; set; }
		public int PassiveProtectionRange { get; set; }
	}

	public static class AngryGuards
	{
		public const string NAMESPACE = "AngryGuards";
		public const string PERMISSION_PREFIX = "mods.angryguards";
		public static string MOD_DIRECTORY;
		public static ModInterfaces interfaces = new ModInterfaces();

		public static List<Colony> ColonyWarMode = new List<Colony>();
		public static ModConfig config;
		private const string CONFIG_FILE = "angryguards-config.json";
		private static string ConfigFilePath {
			get {
				return Path.Combine(Path.Combine("gamedata", "savegames"), Path.Combine(ServerManager.WorldName, CONFIG_FILE));
			}
		}

		public static void OnAssemblyLoaded(string path)
		{
			MOD_DIRECTORY = Path.GetDirectoryName(path);
		}

		// initialize blocks and jobs
		public static void AfterItemTypesDefined()
		{
			LoadConfig();

			// Bow Guard
			AngryGuardJobSettings BowGuardDay = new AngryGuardJobSettings(
				"angryguards.guardbowday", "pipliz.guardbowday",
				AngryGuardJobSettings.EGuardSleepType.Night,
				config.Bow.Damage, config.Bow.Range, config.Bow.Reload, "bowShoot",
				new InventoryItem(BlockTypes.BuiltinBlocks.Indices.bronzearrow),
				new InventoryItem(BlockTypes.BuiltinBlocks.Indices.bow)
			);
			AngryGuardJobSettings BowGuardNight = new AngryGuardJobSettings(
				"angryguards.guardbownight", "pipliz.guardbownight",
				AngryGuardJobSettings.EGuardSleepType.Day,
				config.Bow.Damage, config.Bow.Range, config.Bow.Reload, "bowShoot",
				new InventoryItem(BlockTypes.BuiltinBlocks.Indices.bronzearrow),
				new InventoryItem(BlockTypes.BuiltinBlocks.Indices.bow)
			);

			// Crossbow Guard
			AngryGuardJobSettings CrossbowGuardDay = new AngryGuardJobSettings(
				"angryguards.guardcrossbowday", "pipliz.guardcrossbowday",
				AngryGuardJobSettings.EGuardSleepType.Night,
				config.Crossbow.Damage, config.Crossbow.Range, config.Crossbow.Reload, "bowShoot",
				new InventoryItem(BlockTypes.BuiltinBlocks.Indices.crossbowbolt),
				new InventoryItem(BlockTypes.BuiltinBlocks.Indices.crossbow)
			);
			AngryGuardJobSettings CrossbowGuardNight = new AngryGuardJobSettings(
				"angryguards.guardcrossbownight", "pipliz.guardcrossbownight",
				AngryGuardJobSettings.EGuardSleepType.Day,
				config.Crossbow.Damage, config.Crossbow.Range, config.Crossbow.Reload, "bowShoot",
				new InventoryItem(BlockTypes.BuiltinBlocks.Indices.crossbowbolt),
				new InventoryItem(BlockTypes.BuiltinBlocks.Indices.crossbow)
			);

			// Matchlock Gun Guard
			AngryGuardJobSettings MatchlockGunGuardDay = new AngryGuardJobSettings(
				"angryguards.guardmatchlockgunday", "pipliz.guardmatchlockday",
				AngryGuardJobSettings.EGuardSleepType.Night,
				config.MatchlockGun.Damage, config.MatchlockGun.Range, config.MatchlockGun.Reload, "matchlock",
				new InventoryItem(BlockTypes.BuiltinBlocks.Indices.leadbullet),
				new InventoryItem(BlockTypes.BuiltinBlocks.Indices.matchlockgun)
			);
			MatchlockGunGuardDay.ShootItem.Add(new InventoryItem(BlockTypes.BuiltinBlocks.Indices.gunpowderpouch));
			//MatchlockGunGuardDay.OnShootResultItem = new ItemTypes.ItemTypeDrops(BlockTypes.BuiltinBlocks.Indices.linenpouch, 1, 0.9f);

			AngryGuardJobSettings MatchlockGunGuardNight = new AngryGuardJobSettings(
				"angryguards.guardmatchlockgunnight", "pipliz.guardmatchlocknight",
				AngryGuardJobSettings.EGuardSleepType.Day,
				config.MatchlockGun.Damage, config.MatchlockGun.Range, config.MatchlockGun.Reload, "matchlock",
				new InventoryItem(BlockTypes.BuiltinBlocks.Indices.leadbullet),
				new InventoryItem(BlockTypes.BuiltinBlocks.Indices.matchlockgun)
			);
			MatchlockGunGuardNight.ShootItem.Add(new InventoryItem(BlockTypes.BuiltinBlocks.Indices.gunpowderpouch));
			//MatchlockGunGuardNight.OnShootResultItem = new ItemTypes.ItemTypeDrops(BlockTypes.BuiltinBlocks.Indices.linenpouch, 1, 0.9f);

			ServerManager.BlockEntityCallbacks.RegisterEntityManager(new BlockJobManager<AngryGuardJobInstance>(BowGuardDay));
			ServerManager.BlockEntityCallbacks.RegisterEntityManager(new BlockJobManager<AngryGuardJobInstance>(BowGuardNight));
			ServerManager.BlockEntityCallbacks.RegisterEntityManager(new BlockJobManager<AngryGuardJobInstance>(CrossbowGuardDay));
			ServerManager.BlockEntityCallbacks.RegisterEntityManager(new BlockJobManager<AngryGuardJobInstance>(CrossbowGuardNight));
			ServerManager.BlockEntityCallbacks.RegisterEntityManager(new BlockJobManager<AngryGuardJobInstance>(MatchlockGunGuardDay));
			ServerManager.BlockEntityCallbacks.RegisterEntityManager(new BlockJobManager<AngryGuardJobInstance>(MatchlockGunGuardNight));


			CommandManager.RegisterCommand(new FriendlyCommand());
			Log.Write($"Angry Guards started with guard mode: {config.GuardMode}");
		}

		// Load config
		public static void LoadConfig()
		{
			if (!File.Exists(ConfigFilePath)) {
				// copy default config from mod directory to world savegame directory
				File.Copy(Path.Combine(MOD_DIRECTORY, CONFIG_FILE), ConfigFilePath);
				Log.Write($"Creating default configuration {ConfigFilePath}");
			} else {
				Log.Write($"Loading config from {ConfigFilePath}");
			}
			try {
				JsonSerializer js = new JsonSerializer();
				JsonTextReader jtr = new JsonTextReader(new StreamReader(ConfigFilePath));
				config = js.Deserialize<ModConfig>(jtr);
			} catch (Exception e) {
				Log.Write($"Could not parse {CONFIG_FILE}: {e.Message}");
			}
		}

		// Load
		public static void AfterWorldLoad()
		{
			PlayerTracker.Load();
		}

		// Save
		public static void OnQuit()
		{
			PlayerTracker.Save();
		}

		// track NPC hits/kills for passive mode
		public static void OnNPCHit(NPC.NPCBase npc, ModLoader.OnHitData data)
		{
			if (!(data.HitSourceObject is Players.Player)) {
				return;
			}
			Players.Player killer = (Players.Player)data.HitSourceObject;
			for (int i = 0; i < npc.Colony.Owners.Count; ++i) {
				if (npc.Colony.Owners[i] == killer || PlayerTracker.IsFriendly(npc.Colony.Owners[i], killer)) {
					return;
				}
			}

			PlayerTracker.AddEnemy(npc.Colony, killer);
		}

		// track block changes within banner range for passive mode
		public static void OnTryChangeBlock(ModLoader.OnTryChangeBlockData userData)
		{
			Players.Player causedBy = null;
			if (userData.RequestOrigin.Type == BlockChangeRequestOrigin.EType.Player) {
				causedBy = userData.RequestOrigin.AsPlayer;
			}
			if (causedBy == null || config.GuardMode != GuardModeSetting.Passive || PermissionsManager.HasPermission(causedBy, AngryGuards.PERMISSION_PREFIX + ".peacekeeper")) {
				return;
			}

			// check if the block change is within range of a banner(colony)
			Pipliz.Collections.Hashmap<int, Colony>.ValueEnumerator colonyEnumerator = ServerManager.ColonyTracker.ColoniesByID.GetValueEnumerator();
			while (colonyEnumerator.MoveNext()) {
				Colony checkColony = colonyEnumerator.Current;
				if (IsOwnerOrFriendly(checkColony, causedBy)) {
					continue;
				}
				for (int i = 0; i < checkColony.Banners.Count; ++i) {
					int distanceX = (int)System.Math.Abs(causedBy.Position.x - checkColony.Banners[i].Position.x);
					int distanceZ = (int)System.Math.Abs(causedBy.Position.z - checkColony.Banners[i].Position.z);
					if (distanceX < config.PassiveProtectionRange && distanceZ < config.PassiveProtectionRange) {
						PlayerTracker.AddEnemy(checkColony, causedBy);
						return;
					}
				}
			}
			return;
		}

		public static bool IsOwnerOrFriendly(Colony colony, Players.Player candidate)
		{
			if (colony.Owners.ContainsByReference(candidate)) {
				return true;
			}
			for (int i = 0; i < colony.Owners.Count; ++i) {
				if (PlayerTracker.IsFriendly(colony.Owners[i], candidate)) {
					return true;
				}
			}
			return false;
		}

		// This method will be called by other mods to start colony wars. Used by the ColonyCommands mod
		public static void ColonySetWarMode(Colony colony, bool mode)
		{
			if (mode == true) {
				Log.Write($"AngryGuards: received war call for colony {colony.Name}");
				if (!ColonyWarMode.Contains(colony)) {
					ColonyWarMode.Add(colony);
				}
			} else {
				Log.Write($"AngryGuards: received end of war for colony {colony.Name}");
				if (ColonyWarMode.Contains(colony)) {
					ColonyWarMode.Remove(colony);
				}
			}
		}

	} // class

} // namespace

