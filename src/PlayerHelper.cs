﻿using System;
using System.IO;
using Pipliz;
using System.Text.RegularExpressions;
using Steamworks;

namespace AngryGuards
{

	public static class PlayerHelper
	{

		public static bool TryGetPlayer(string identifier, out Players.Player targetPlayer, out string error, bool includeOffline = false)
		{
			targetPlayer = null;
			if (identifier.StartsWith("'")) {
				if (identifier.EndsWith("'")) {
					identifier = identifier.Substring(1, identifier.Length - 2);
				} else {
					error = "missing ' after playername";
					return false;
				}
			}
			if (identifier.Length < 1) {
				error = "no playername given";
				return false;
			}

			// try to find by steamID first
			ulong steamid;
			if (ulong.TryParse(identifier, out steamid)) {
				Steamworks.CSteamID csteamid = new Steamworks.CSteamID(steamid);
				if (csteamid.IsValid()) {
					error = "";
					if (Players.TryGetPlayer(csteamid, out targetPlayer)) {
						return true;
					} else {
						targetPlayer = null;
					}
				}
			}

			// try to find by hash code
			var m = Regex.Match(identifier, @"#(?<hash>\d{8})");
			int givenHash;
			if (m.Success && int.TryParse(m.Groups["hash"].Value, out givenHash)) {
				foreach (Players.Player player in Players.PlayerDatabase.Values) {
					if (!includeOffline && player.ConnectionState != Players.EConnectionState.Connected) {
						continue;
					}
					if (player.ID.SteamID.GetHashCode() == givenHash) {
						if (targetPlayer == null) {
							targetPlayer = player;
						} else {
							targetPlayer = null;
							error = "duplicate hash code, please use full SteamID";
							return false;
						}
					}
				}

				if (targetPlayer != null) {
					error = "";
					return true;
				}
			}

			// try to find by string closest match
			Players.Player closestMatch = null;
			int closestDist = int.MaxValue;
			foreach (var player in Players.PlayerDatabase.Values) {
				if (!includeOffline && player.ConnectionState != Players.EConnectionState.Connected) {
					continue;
				}
				if (player.Name != null) {
					if (string.Equals(player.Name, identifier, StringComparison.InvariantCultureIgnoreCase)) {
						if (targetPlayer == null) {
							targetPlayer = player;
						} else {
							targetPlayer = null;
							error = "duplicate player name, pls use SteamID";
							return false;
						}
					} else {
						int levDist = LevenshteinDistance.Compute(player.Name.ToLower(), identifier.ToLower());
						if (levDist < closestDist) {
							closestDist = levDist;
							closestMatch = player;
						} else if (levDist == closestDist) {
							closestMatch = null;
						}
					}
				}
			}

			if (targetPlayer != null) {
				error = "";
				return true;
			} else if (closestMatch != null && (closestDist < closestMatch.Name.Length * 0.2)) {
				error = "";
				targetPlayer = closestMatch;
				Log.Write($"Name '{identifier}' did not match, picked closest match '{targetPlayer.Name}' instead");
				return true;
			}
			error = "player not found";
			return false;
		}
	}

	// src: https://www.dotnetperls.com/levenshtein
	static class LevenshteinDistance
	{
		public static int Compute(string s, string t)
		{
			var n = s.Length;
			var m = t.Length;
			int[,] d = new int[n + 1, m + 1];
			if (n == 0) {
				return m;
			}
			if (m == 0) {
				return n;
			}
			for (var i = 0; i <= n; d[i, 0] = i++) {
			}
			for (var j = 0; j <= m; d[0, j] = j++) {
			}
			for (var i = 1; i <= n; i++) {
				for (var j = 1; j <= m; j++) {
					var cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
					d[i, j] = System.Math.Min(
							System.Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
							d[i - 1, j - 1] + cost);
				}
			}
			return d[n, m];
		}
	}
}

