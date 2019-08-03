using System.Text.RegularExpressions;
using System.Collections.Generic;
using Chatting;

namespace AngryGuards
{

	public class FriendlyCommand : IChatCommand
	{

		public bool SyntaxError(Players.Player causedBy)
		{
			Chat.Send(causedBy, "Syntax: /friendly {add|list|remove} {playername}");
			return true;
		}

		public bool TryDoCommand(Players.Player causedBy, string chattext, List<string> splits)
		{
			if (!splits[0].Equals("/friendly")) {
				return false;
			}

			var m = Regex.Match(chattext, @"/friendly (?<action>[^ ]+) ?(?<player>[^ ]+)?$");
			if (!m.Success) {
				return SyntaxError(causedBy);
			}
			string action = m.Groups["action"].Value;
			if ( !(action.Equals("add") || action.Equals("list") || action.Equals("remove")) ) {
				return SyntaxError(causedBy);
			}

			// list does not require a target player
			if (action.Equals("list")) {
				string playerNames;
				if (PlayerTracker.GetFriendlyList(causedBy, out playerNames)) {
					Chat.Send(causedBy, $"Friendly players: {playerNames}");
				} else {
					Chat.Send(causedBy, "No friendly players defined. Guards shoot everyone.");
				}
				return true;
			}

			string playerName = m.Groups["player"].Value;
			string error;
			Players.Player target;
			if (!PlayerHelper.TryGetPlayer(playerName, out target, out error, true)) {
				Chat.Send(causedBy, $"Could not find player {playerName}: {error}");
				return true;
			}

			if (action.Equals("add")) {
				if (!PlayerTracker.AddFriendly(causedBy, target)) {
					Chat.Send(causedBy, $"Player {target.Name} is already on the friendly list");
					return true;
				} else {
					Chat.Send(causedBy, $"Added {target.Name} as friendly");
				}

			} else if (action.Equals("remove")) {
				if (!PlayerTracker.RemoveFriendly(causedBy, target)) {
					Chat.Send(causedBy, $"Player {target.Name} was not on the friendly list");
				} else {
					Chat.Send(causedBy, $"Removed {target.Name} from friendly list");
				}
			}

			return true;
		}

	} // class

} // namespace

