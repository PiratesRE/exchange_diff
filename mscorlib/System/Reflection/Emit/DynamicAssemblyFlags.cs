using System;

namespace System.Reflection.Emit
{
	[Flags]
	internal enum DynamicAssemblyFlags
	{
		None = 0,
		AllCritical = 1,
		Aptca = 2,
		Critical = 4,
		Transparent = 8,
		TreatAsSafe = 16
	}
}
