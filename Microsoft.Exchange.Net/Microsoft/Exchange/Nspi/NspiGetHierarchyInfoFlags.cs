using System;

namespace Microsoft.Exchange.Nspi
{
	[Flags]
	public enum NspiGetHierarchyInfoFlags
	{
		None = 0,
		Dos = 1,
		OneOff = 2,
		Unicode = 4,
		Admin = 8,
		Paged = 16
	}
}
