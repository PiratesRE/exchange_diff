using System;
using System.Runtime.InteropServices;

namespace System.Reflection.Emit
{
	[ComVisible(true)]
	[Flags]
	[Serializable]
	public enum AssemblyBuilderAccess
	{
		Run = 1,
		Save = 2,
		RunAndSave = 3,
		ReflectionOnly = 6,
		RunAndCollect = 9
	}
}
