using System;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Flags]
	internal enum ServiceAccessFlags
	{
		QueryConfig = 1,
		ChangeConfig = 2,
		QueryStatus = 4,
		EnumerateDependents = 8,
		Start = 16,
		Stop = 32,
		PauseContinue = 64,
		Interrogate = 128,
		UserDefinedControl = 256,
		ReadControl = 131072,
		WriteDac = 262144,
		AllAccess = 393727
	}
}
