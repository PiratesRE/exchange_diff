using System;

namespace System.Runtime.Versioning
{
	[Flags]
	internal enum SxSRequirements
	{
		None = 0,
		AppDomainID = 1,
		ProcessID = 2,
		CLRInstanceID = 4,
		AssemblyName = 8,
		TypeName = 16
	}
}
