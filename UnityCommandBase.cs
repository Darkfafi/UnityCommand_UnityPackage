using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UnityCommands
{
	[Serializable]
	public class UnityCommandBase<TData> : IUnityCommand
	{
		[Serializable]
		public delegate void UnityCommandHandler(TData data, bool apply);

		[Serializable]
		public class UnityCommandEvent : UnityEvent<TData> { }

		[SerializeField]
		private UnityCommandEvent _onApply;

		[SerializeField]
		private UnityCommandEvent _onRevert;

		private List<UnityCommandHandler> _instructions = new List<UnityCommandHandler>();

		public bool IsApplied
		{
			get; private set;
		}

		/// <summary>
		/// Reverts all commands except for the given index command, and then applies the given index command (if able)
		/// <param name="commands">The commands to perform the logics on</param>
		/// <param name="data">Data to pass to the event and instructions</param>
		/// <param name="commandToApplyIndex">The index of the command which to call <see cref="Apply(object, bool)"/> on. An invalid index will simply ignore this step and only revert all the commands</param>
		/// <param name="force">To force the event & instructions which are linked to the methods specified</param>
		/// </summary>
		public static void SwitchToCommand(IList<UnityCommandBase<TData>> commands, TData data, int commandToApplyIndex, bool force = false) => UnityCommandUtils.SwitchToCommand(commands, data, commandToApplyIndex, force);

		/// <summary>
		/// Applies all commands passed
		/// <param name="commands">The commands to perform the logics on</param>
		/// <param name="data">Data to pass to the event and instructions</param>
		/// <param name="force">To force the event and instructions to fire even if <see cref="IsApplied"/> is True</param>
		/// </summary>
		public static void ApplyCommands(IList<UnityCommandBase<TData>> commands, TData data, bool force = false) => UnityCommandUtils.ApplyCommands(commands, data, force);

		/// <summary>
		/// Reverts all commands passed
		/// <param name="commands">The commands to perform the logics on</param>
		/// <param name="data">Data to pass to the event and instructions</param>
		/// <param name="force">To force the event and instructions to fire even if <see cref="IsApplied"/> is False</param>
		/// </summary>
		public static void RevertCommands(IList<UnityCommandBase<TData>> commands, TData data, bool force = false) => UnityCommandUtils.RevertCommands(commands, data, force);

		public bool HasInstruction(UnityCommandHandler instruction) => _instructions.Contains(instruction);

		/// <summary>
		/// Adds an instruction, which is executed on apply or revert after the Editor Event is fired
		/// </summary>
		public bool AddInstruction(UnityCommandHandler instruction)
		{
			if(HasInstruction(instruction))
			{
				return false;
			}

			_instructions.Add(instruction);
			return true;
		}

		/// <summary>
		/// Removes the instruction added through <see cref="AddInstruction(UnityCommandHandler)"/>
		/// </summary>
		public bool RemoveInstruction(UnityCommandHandler instruction)
		{
			return _instructions.Remove(instruction);
		}

		/// <summary>
		/// Removes all instructions added through <see cref="AddInstruction(UnityCommandHandler)"/>
		/// </summary>
		public void ClearInstructions()
		{
			_instructions.Clear();
		}

		/// <summary>
		/// Applies the command, which triggers <see cref="_onApply"/> and the added instructions through <see cref="AddInstruction(UnityCommandHandler)"/>
		/// Note: This only triggers if <see cref="IsApplied"/> is False or when force is set to through
		/// </summary>
		/// <param name="data">Data to pass to the event and instructions</param>
		/// <param name="force">To force the event and instructions to fire even if <see cref="IsApplied"/> is True</param>
		/// <returns>True if the Apply was executed</returns>
		public bool Apply(TData data, bool force = false)
		{
			if(IsApplied && !force)
			{
				return false;
			}

			IsApplied = true;
			if (_onApply != null)
			{
				_onApply.Invoke(data);
			}
			PerformInstructions(data, true);
			return true;
		}

		bool IUnityCommand.Apply(object data, bool force)
		{
			TData castedData;

			try
			{
				castedData = (TData)data;
			}
			catch
			{
				return false;
			}

			return Apply(castedData, force);
		}

		/// <summary>
		/// Reverts the command, which triggers <see cref="_onRevert"/> and the added instructions through <see cref="AddInstruction(UnityCommandHandler)"/>
		/// Note: This only triggers if <see cref="IsApplied"/> is True or when force is set to through
		/// </summary>
		/// <param name="data">Data to pass to the event and instructions</param>
		/// <param name="force">To force the event and instructions to fire even if <see cref="IsApplied"/> is False</param>
		/// <returns>True if the Revert was executed</returns>
		public bool Revert(TData data, bool force = false)
		{
			if(!IsApplied && !force)
			{
				return false;
			}

			IsApplied = false;
			if (_onRevert != null)
			{
				_onRevert.Invoke(data);
			}
			PerformInstructions(data, false);
			return true;
		}

		bool IUnityCommand.Revert(object data, bool force)
		{
			TData castedData;

			try
			{
				castedData = (TData)data;
			}
			catch
			{
				return false;
			}

			return Revert(castedData, force);
		}

		/// <summary>
		/// Executes <see cref="Apply(object, bool)"/> or <see cref="Revert(object, bool)"/> depending on the apply boolean
		/// </summary>
		/// <param name="data">Data to pass to the event and instructions</param>
		/// <param name="apply">If True, tries to execute <see cref="Apply(object, bool)"/> else it tries to execute <see cref="Revert(object, bool)"/></param>
		/// <param name="force">To force the event & instructions which are linked to the method specified</param>
		/// <returns>True if the method specified was executed</returns>
		public bool Execute(TData data, bool apply, bool force = false)
		{
			if(apply)
			{
				return Apply(data, force);
			}
			else
			{
				return Revert(data, force);
			}
		}

		bool IUnityCommand.Execute(object data, bool apply, bool force)
		{
			TData castedData;

			try
			{
				castedData = (TData)data;
			}
			catch
			{
				return false;
			}

			return Execute(castedData, apply, force);
		}

		private void PerformInstructions(TData data, bool apply)
		{
			List<UnityCommandHandler> instructions = new List<UnityCommandHandler>(_instructions);
			for(int i = 0; i < instructions.Count; i++)
			{
				UnityCommandHandler instruction = instructions[i];
				instruction.Invoke(data, apply);
			}
		}

		/// <summary>
		/// Clears all instructions, sets <see cref="IsApplied"/> to false and removes the listeners from <see cref="_onApply"/> & <see cref="_onRevert"/>
		/// </summary>
		public void Dispose()
		{
			IsApplied = false;
			ClearInstructions();
			_onApply.RemoveAllListeners();
			_onRevert.RemoveAllListeners();
		}
	}

	public interface IUnityCommand : IDisposable
	{
		/// <summary>
		/// Returns True if the Apply method was executed without a followed Revert
		/// </summary>
		public bool IsApplied
		{
			get;
		}

		/// <summary>
		/// Removes all instructions added through <see cref="AddInstruction(UnityCommandHandler)"/>
		/// </summary>
		void ClearInstructions();

		/// <summary>
		/// Applies the command, which triggers <see cref="_onApply"/> and the added instructions through <see cref="AddInstruction(UnityCommandHandler)"/>
		/// Note: This only triggers if <see cref="IsApplied"/> is False or when force is set to through
		/// </summary>
		/// <param name="data">Data to pass to the event and instructions</param>
		/// <param name="force">To force the event and instructions to fire even if <see cref="IsApplied"/> is True</param>
		/// <returns>True if the Apply was executed</returns>
		bool Apply(object data, bool force = false);

		/// <summary>
		/// Reverts the command, which triggers <see cref="_onRevert"/> and the added instructions through <see cref="AddInstruction(UnityCommandHandler)"/>
		/// Note: This only triggers if <see cref="IsApplied"/> is True or when force is set to through
		/// </summary>
		/// <param name="data">Data to pass to the event and instructions</param>
		/// <param name="force">To force the event and instructions to fire even if <see cref="IsApplied"/> is False</param>
		/// <returns>True if the Revert was executed</returns>
		bool Revert(object data, bool force = false);

		/// <summary>
		/// Executes <see cref="Apply(object, bool)"/> or <see cref="Revert(object, bool)"/> depending on the apply boolean
		/// </summary>
		/// <param name="data">Data to pass to the event and instructions</param>
		/// <param name="apply">If True, tries to execute <see cref="Apply(object, bool)"/> else it tries to execute <see cref="Revert(object, bool)"/></param>
		/// <param name="force">To force the event & instructions which are linked to the method specified</param>
		/// <returns>True if the method specified was executed</returns>
		bool Execute(object data, bool apply, bool force = false);
	}
}