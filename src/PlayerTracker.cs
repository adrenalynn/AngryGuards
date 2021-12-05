using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using Pipliz;
using MeshedObjects;

namespace AngryGuards {

	public static class PlayerTracker
	{
		// friendly list per player (guards will not shoot at)
		private static Dictionary<Players.Player, List<Players.Player>> friendlyPlayers =
			new Dictionary<Players.Player, List<Players.Player>>();

		// enemy list per colony (for passive guards)
		private static Dictionary<Colony, List<Players.Player>> colonyEnemies =
			new Dictionary<Colony, List<Players.Player>>();

		private const string CONFIG_FILE = "angryguards-friendly.json";
		private static string ConfigFilePath {
			get {
				return Path.Combine(Path.Combine("gamedata", "savegames"), Path.Combine(ServerManager.WorldName, CONFIG_FILE));
			}
		}

		// find non friendly targets within given range
		public static Players.Player FindTarget(Colony owner, Vector3 position, int range)
		{
			Players.Player target = null;
			float shortestDistance = range + 1.0f;

			List<Players.Player> friendlies = new List<Players.Player>();
			for (int i = 0; i < owner.Owners.Count; ++i) {
				if (friendlyPlayers.ContainsKey(owner.Owners[i])) {
					friendlies.AddRange(friendlyPlayers[owner.Owners[i]]);
				}
				friendlies.Add(owner.Owners[i]);
			}

			bool atWar = false;
			if (AngryGuards.ColonyWarMode.Contains(owner)) {
				atWar = true;
			}

			foreach (Players.Player candidate in Players.ConnectedPlayers) {
				if (friendlies.Contains(candidate)) {
					continue;
				}
				if (PermissionsManager.HasPermission(candidate, AngryGuards.PERMISSION_PREFIX + ".peacekeeper")) {
					continue;
				}
				if (candidate.Health <= 0.0f) {
					continue;
				}

				// colonies at war are always active mode and also shoot mounted players
				if (!atWar) {
					if (AngryGuards.config.GuardMode == GuardModeSetting.Passive) {
						if (!colonyEnemies.ContainsKey(owner) || !colonyEnemies[owner].Contains(candidate)) {
							continue;
						}
					}

					// avoid shooting players on gliders. but still shoot them when they kill NPCs
					if (!AngryGuards.config.ShootMountedPlayers && MeshedObjectManager.HasVehicle(candidate)) {
						if (!colonyEnemies.ContainsKey(owner) || !colonyEnemies[owner].Contains(candidate)) {
							continue;
						}
					}
				}

				Vector3 candidateEyePosition = candidate.Position;
				candidateEyePosition[1] += 1;
				float distance = Vector3.Distance(position, candidate.Position);
				if (distance < shortestDistance && (VoxelPhysics.CanSee(position, candidate.Position) || VoxelPhysics.CanSee(position, candidateEyePosition))) {
					shortestDistance = distance;
					target = candidate;
				}
			}

			return target;
		}

		// get the per-player friendly list as comma separated string
		public static bool GetFriendlyList(Players.Player owner, out string names)
		{
			names = "";
			if (!friendlyPlayers.ContainsKey(owner)) {
				return false;
			}
			List<Players.Player> friends = friendlyPlayers[owner];

			names = string.Join(", ", friends.Select(x => x.Name).ToArray());
			return true;
		}

		// add a player to the per-player friendly list
		public static bool AddFriendly(Players.Player owner, Players.Player target)
		{
			List<Players.Player> friends;
			if (!friendlyPlayers.ContainsKey(owner)) {
				friends = new List<Players.Player>();
				friendlyPlayers[owner] = friends;
			} else {
				friends = friendlyPlayers[owner];
			}
			if (friends.Contains(target)) {
				return false;
			}
			friends.Add(target);
			Save();			
			return true;
		}

		// add a player to the enemy list (for passive mode)
		public static bool AddEnemy(Colony owner, Players.Player target)
		{
			List<Players.Player> enemies;
			if (!colonyEnemies.ContainsKey(owner)) {
				enemies = new List<Players.Player>();
				colonyEnemies[owner] = enemies;
			} else {
				enemies = colonyEnemies[owner];
			}
			if (enemies.Contains(target)) {
				return false;
			}
			enemies.Add(target);
			return true;
		}

		// remove a player from the per-player friendly list
		public static bool RemoveFriendly(Players.Player owner, Players.Player target)
		{
			if (!friendlyPlayers.ContainsKey(owner)) {
				return false;
			}
			List<Players.Player> friends = friendlyPlayers[owner];
			if (!friends.Contains(target)) {
				return false;
			}
			friends.Remove(target);
			Save();			

			return true;
		}

		// check if friendly for a player
		public static bool IsFriendly(Players.Player owner, Players.Player candidate)
		{
			if (friendlyPlayers.ContainsKey(owner) && friendlyPlayers[owner].Contains(candidate)) {
				return true;
			}
			return false;
		}

		// Save config to JSON file
		public static void Save()
		{
			Log.Write($"Saving {CONFIG_FILE}");
			try {
				JsonSerializer json = new JsonSerializer();
				JsonTextWriter jsonWriter = new JsonTextWriter(new StreamWriter(ConfigFilePath));
				json.Serialize(jsonWriter, friendlyPlayers);
				jsonWriter.Flush();
			} catch (Exception e) {
				Log.Write($"Error saving {CONFIG_FILE}: {e.Message}");
			}
		}

		// Load config from JSON file
		public static void Load()
		{
			if (!File.Exists(ConfigFilePath)) {
				return;
			}
			Log.Write($"Loading friendly list from {CONFIG_FILE}");
			try {
				JsonSerializer json = new JsonSerializer();
				JsonTextReader jsonReader = new JsonTextReader(new StreamReader(ConfigFilePath));
				friendlyPlayers = json.Deserialize<Dictionary<Players.Player, List<Players.Player>>>(jsonReader);
			} catch (Exception e) {
				Log.Write($"Error parsing {CONFIG_FILE}: {e.Message}");
			}
		}
	}

} // namespace

