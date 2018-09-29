using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Pipliz.JSON;
using Pipliz;

namespace AngryGuards {

	public static class PlayerTracker
	{
		// friendly list per player (guards will not shoot at)
		private static Dictionary<Players.Player, List<Players.Player>> friendlyPlayers =
			new Dictionary<Players.Player, List<Players.Player>>();

		// global friendly list (admin, staff, ...)
		private static List<Players.Player> globalFriendly = new List<Players.Player>();

		// enemy list per player (for passive guards)
		private static Dictionary<Players.Player, List<Players.Player>> enemyPlayers =
			new Dictionary<Players.Player, List<Players.Player>>();

		private const string CONFIG_FILE = "angryguards-friendly.json";
		private static string ConfigFilePath {
			get {
				return Path.Combine(Path.Combine("gamedata", "savegames"), Path.Combine(ServerManager.WorldName, CONFIG_FILE));
			}
		}

		// find non friendly targets within given range
		public static Players.Player FindTarget(Players.Player owner, Vector3 position, int range)
		{
			Players.Player target = null;
			float shortestDistance = range + 1.0f;

			List<Players.Player> ownerFriends;
			if (friendlyPlayers.ContainsKey(owner)) {
				ownerFriends = friendlyPlayers[owner];
			} else {
				ownerFriends = new List<Players.Player>();
			}

			List<Players.Player> ownerEnemies;
			if (enemyPlayers.ContainsKey(owner)) {
				ownerEnemies = enemyPlayers[owner];
			} else {
				ownerEnemies = new List<Players.Player>();
			}

			for (int i = 0; i < Players.CountConnected; i++) {
				Players.Player candidate = Players.GetConnectedByIndex(i);
				if (candidate == owner || ownerFriends.Contains(candidate) || globalFriendly.Contains(candidate)) {
					continue;
				}

				if (AngryGuards.ModeSetting == GuardMode.Passive && !ownerEnemies.Contains(candidate)) {
					continue;
				}

				Vector3 candidateEyePosition = candidate.Position;
				candidateEyePosition[1] += 1;
				float distance = Vector3.Distance(position, candidate.Position);
				if (distance < shortestDistance && (General.Physics.Physics.CanSee(position, candidate.Position) || General.Physics.Physics.CanSee(position, candidateEyePosition))) {
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
		public static bool AddEnemy(Players.Player owner, Players.Player target)
		{
			List<Players.Player> enemies;
			if (!enemyPlayers.ContainsKey(owner)) {
				enemies = new List<Players.Player>();
				enemyPlayers[owner] = enemies;
			} else {
				enemies = enemyPlayers[owner];
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

		// global friendly list as csv
		public static bool GetGlobalFriendlyList(out string names)
		{
			names = string.Join(", ", globalFriendly.Select(x => x.Name).ToArray());
			return true;
		}

		// check if friendly for a player
		public static bool IsFriendly(Players.Player owner, Players.Player candidate)
		{
			if (friendlyPlayers.ContainsKey(owner)) {
				List<Players.Player> ownerFriends = friendlyPlayers[owner];
				if (ownerFriends.Contains(candidate)) {
					return true;
				}
			}
			if (globalFriendly.Contains(candidate)) {
				return true;
			}
			return false;
		}

		// add global friendly
		public static bool AddGlobalFriendly(Players.Player target)
		{
			if (globalFriendly.Contains(target)) {
				return false;
			}
			globalFriendly.Add(target);
			Save();			
			return true;
		}

		// remove global friendly
		public static bool RemoveGlobalFriendly(Players.Player target)
		{
			if (!globalFriendly.Contains(target)) {
				return false;
			}
			globalFriendly.Remove(target);
			Save();			
			return true;
		}

		// Save config to JSON file
		public static void Save()
		{
			Log.Write($"Saving {CONFIG_FILE}");
			JSONNode configJson = new JSONNode();

			JSONNode globalJson = new JSONNode(NodeType.Array);
			foreach (Players.Player target in globalFriendly) {
				JSONNode recordJson = new JSONNode();
				recordJson.SetAs(target.ID.steamID);
				globalJson.AddToArray(recordJson);
			}
			configJson.SetAs("global", globalJson);

			JSONNode playersJson = new JSONNode(NodeType.Array);
			foreach (KeyValuePair<Players.Player, List<Players.Player>> kvp in friendlyPlayers) {
				JSONNode playerJson = new JSONNode();
				Players.Player owner = kvp.Key;
				playerJson.SetAs("owner", owner.ID.steamID);
				JSONNode recordsJson = new JSONNode(NodeType.Array);
				foreach (Players.Player friend in kvp.Value) {
					JSONNode friendJson = new JSONNode();
					friendJson.SetAs(friend.ID.steamID);
					recordsJson.AddToArray(friendJson);
				}
				playerJson.SetAs("friendly", recordsJson);
				playersJson.AddToArray(playerJson);
			}
			configJson.SetAs("players", playersJson);

			try {
				JSON.Serialize(ConfigFilePath, configJson);
			} catch (Exception e) {
				Log.Write($"Error saving {CONFIG_FILE}: {e.Message}");
			}
			return;
		}

		// Load config from JSON file
		public static void Load()
		{
			JSONNode configJson;
			if (!JSON.Deserialize(ConfigFilePath, out configJson, false)) {
				Log.Write($"{CONFIG_FILE} not found, no friendly lists defined");
				return;
			}

			Log.Write($"Loading friendly list from {CONFIG_FILE}");
			try {
				JSONNode globalJson;
				configJson.TryGetAs("global", out globalJson);
				foreach (JSONNode record in globalJson.LoopArray()) {
					Players.Player target;
					string error;
					if (PlayerHelper.TryGetPlayer(record.GetAs<string>(), out target, out error, true)) {
						globalFriendly.Add(target);
					}
				}

				JSONNode playersJson;
				configJson.TryGetAs("players", out playersJson);
				foreach (JSONNode record in playersJson.LoopArray()) {
					Players.Player owner;
					string error;
					if (!PlayerHelper.TryGetPlayer(record.GetAs<string>("owner"), out owner, out error, true)) {
						continue;
					}
					List<Players.Player> friends = new List<Players.Player>();
					JSONNode friendsJson;
					record.TryGetAs("friendly", out friendsJson);
					foreach (JSONNode friendJson in friendsJson.LoopArray()) {
						Players.Player target;
						if (PlayerHelper.TryGetPlayer(friendJson.GetAs<string>(), out target, out error, true)) {
							friends.Add(target);
						}
					}
					friendlyPlayers[owner] = friends;
				}
			} catch (Exception e) {
				Log.Write($"Error parsing {CONFIG_FILE}: {e.Message}");
			}
			return;
		}
	}

} // namespace

