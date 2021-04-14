using System;

namespace System
{
	[Flags]
	[Serializable]
	public enum ConsoleModifiers
	{
		Alt = 1,
		Shift = 2,
		Control = 4
	}
}
