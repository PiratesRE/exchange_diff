using System;

namespace Microsoft.Isam.Esent.Interop.Unpublished
{
	[Flags]
	public enum TestInjectionGrbit : uint
	{
		ProbabilityPct = 1U,
		ProbabilityCount = 2U,
		ProbabilityPermanent = 4U,
		ProbabilityFailUntil = 8U,
		ProbabilitySuppress = 1073741824U,
		ProbabilityCleanup = 2147483648U
	}
}
