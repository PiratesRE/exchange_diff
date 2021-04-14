using System;

namespace Microsoft.Isam.Esent.Interop.Unpublished
{
	public enum TestHookOp
	{
		TestInjection = 1,
		SetNegativeTesting = 5,
		ResetNegativeTesting,
		GetBfLowMemoryCallback = 11,
		TraceTestMarker,
		EvictCache = 18,
		Corrupt,
		EnableAutoInc,
		SetErrorTrap,
		GetTablePgnoFDP
	}
}
