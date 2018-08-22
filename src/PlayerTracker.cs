using System.Collections.Generic;
using UnityEngine;

namespace AngryGuards {

	public static class PlayerTracker
	{
		private static Dictionary<Players.Player, List<Players.Player>> friendlyPlayers =
			new Dictionary<Players.Player, List<Players.Player>>();

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
				friendlyPlayers[owner] = ownerFriends;
			}

			for (int i = 0; i < Players.CountConnected; i++) {
				Players.Player candidate = Players.GetConnectedByIndex(i);
				if (candidate == owner || ownerFriends.Contains(candidate)) {
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
	}

} // namespace

