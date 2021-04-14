using System;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[Flags]
	[ComVisible(true)]
	public enum ExceptionHandlingClauseOptions
	{
		Clause = 0,
		Filter = 1,
		Finally = 2,
		Fault = 4
	}
}
