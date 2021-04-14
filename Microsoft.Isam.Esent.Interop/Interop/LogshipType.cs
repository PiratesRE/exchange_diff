using System;

namespace Microsoft.Isam.Esent.Interop
{
	[Flags]
	public enum LogshipType
	{
		Standby = 1,
		Cluster = 2,
		Local = 4
	}
}
