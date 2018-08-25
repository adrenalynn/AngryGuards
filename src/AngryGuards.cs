using System;
using System.IO;
using Pipliz;
using Pipliz.JSON;
using Pipliz.Mods.APIProvider.Jobs;
using ChatCommands;

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
		[ModLoader.ModCallbackProvidesFor("pipliz.apiprovider.jobs.resolvetypes")]
		public static void AfterItemTypesDefined()
		{
			LoadConfig();
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

