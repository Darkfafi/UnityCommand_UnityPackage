using System;
using UnityEngine.Events;

namespace UnityCommands
{
	// Allow for a custom object to be passed to the Apply / Revert Commands
	[Serializable]
	public class UnityCommandEvent : UnityEvent<object>
	{

	}
}