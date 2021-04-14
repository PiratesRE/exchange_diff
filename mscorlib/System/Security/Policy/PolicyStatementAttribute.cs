using System;
using System.Runtime.InteropServices;

namespace System.Security.Policy
{
	[Flags]
	[ComVisible(true)]
	[Serializable]
	public enum PolicyStatementAttribute
	{
		Nothing = 0,
		Exclusive = 1,
		LevelFinal = 2,
		All = 3
	}
}
