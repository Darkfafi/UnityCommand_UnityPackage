using System.Collections.Generic;


namespace UnityCommands
{
	public static class UnityCommandUtils
	{
		/// <summary>
		/// Reverts all commands except for the given index command, and then applies the given index command (if able)
		/// </summary>
		public static void SwitchToCommandRaw(this IList<IUnityCommand> commands, object data, int commandToApplyIndex, bool force = false)
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
		/// Reverts all commands except for the given index command, and then applies the given index command (if able)
		/// </summary>
		public static void SwitchToCommand<TData>(this IList<UnityCommandBase<TData>> commands, TData data, int commandToApplyIndex, bool force = false)
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
		public static void ApplyCommandsRaw(this IList<IUnityCommand> commands, object data, bool force = false)
		{
			for(int i = 0; i < commands.Count; i++)
			{
				commands[i].Apply(data, force);
			}
		}

		/// <summary>
		/// Applies all commands passed
		/// </summary>
		public static void ApplyCommands<TData>(this IList<UnityCommandBase<TData>> commands, TData data, bool force = false)
		{
			for(int i = 0; i < commands.Count; i++)
			{
				commands[i].Apply(data, force);
			}
		}

		/// <summary>
		/// Reverts all commands passed
		/// </summary>
		public static void RevertCommandsRaw(this IList<IUnityCommand> commands, object data, bool force = false)
		{
			for(int i = 0; i < commands.Count; i++)
			{
				commands[i].Revert(data, force);
			}
		}

		/// <summary>
		/// Reverts all commands passed
		/// </summary>
		public static void RevertCommands<TData>(this IList<UnityCommandBase<TData>> commands, TData data, bool force = false)
		{
			for(int i = 0; i < commands.Count; i++)
			{
				commands[i].Revert(data, force);
			}
		}
	}
}