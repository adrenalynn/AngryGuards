using System.Text.RegularExpressions;
using Chatting;
using Chatting.Commands;

namespace AngryGuards
{

	public class GlobalFriendlyCommand : IChatCommand
	{

		public bool IsCommand(string chat)
		{
			return (chat.Equals("/globalfriendly") || chat.StartsWith("/globalfriendly "));
		}

		public bool SyntaxError(Players.Player causedBy)
		{
			Chat.Send(causedBy, "Syntax: /globalfriendly {add|list|remove} {playername}");
			return true;
		}

		public bool TryDoCommand(Players.Player causedBy, string chattext)
		{
			if (!PermissionsManager.CheckAndWarnPermission(causedBy, AngryGuards.PERMISSION_PREFIX + ".global"))  {
				return true;
			}

			var m = Regex.Match(chattext, @"/globalfriendly (?<action>[^ ]+) ?(?<player>[^ ]+)?$");
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
				if (PlayerTracker.GetGlobalFriendlyList(out playerNames)) {
					Chat.Send(causedBy, $"Global friendly list: {playerNames}");
				} else {
					Chat.Send(causedBy, "No global friendly players defined.");
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
				if (!PlayerTracker.AddGlobalFriendly(target)) {
					Chat.Send(causedBy, $"Player {target.Name} is already on the global friendly list");
					return true;
				} else {
					Chat.Send(causedBy, $"Added {target.Name} to the gobal friendly list");
				}

			} else if (action.Equals("remove")) {
				if (!PlayerTracker.RemoveGlobalFriendly(target)) {
					Chat.Send(causedBy, $"Player {target.Name} is not on the global friendly list");
				} else {
					Chat.Send(causedBy, $"Removed {target.Name} from the gobal friendly list");
				}
			}

			return true;
		}

	} // class

} // namespace

