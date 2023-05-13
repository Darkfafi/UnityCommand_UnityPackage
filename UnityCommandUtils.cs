using System.Collections.Generic;


namespace UnityCommands
{
	public static class UnityCommandUtils
	{
		/// <summary>
		/// Reverts all commands except for the given index command, and then applies the given index command (if able)
		/// </summary>
		public static void SwitchToCommand(this IList<UnityCommand> commands, object data, int commandToApplyIndex, bool force = false)
		{
			for(int i = 0; i < commands.Count; i++)
			{
				if(i == commandToApplyIndex)
				{
					continue;
				}

				commands[i].Revert(data, force);
			}

			if(commandToApplyIndex >= 0 && commandToApplyIndex < commands.Count)
			{
				commands[commandToApplyIndex].Apply(data, force);
			}
		}

		/// <summary>
		/// Applies all commands passed
		/// </summary>
		public static void ApplyCommands(this IList<UnityCommand> commands, object data, bool force = false)
		{
			for(int i = 0; i < commands.Count; i++)
			{
				commands[i].Apply(data, force);
			}
		}

		/// <summary>
		/// Reverts all commands passed
		/// </summary>
		public static void RevertCommands(this IList<UnityCommand> commands, object data, bool force = false)
		{
			for(int i = 0; i < commands.Count; i++)
			{
				commands[i].Revert(data, force);
			}
		}
	}
}