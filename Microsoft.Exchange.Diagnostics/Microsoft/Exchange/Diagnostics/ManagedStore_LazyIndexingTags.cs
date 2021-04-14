using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct ManagedStore_LazyIndexingTags
	{
		public const int PseudoIndex = 0;

		public const int CategoryHeaderViewPopulation = 1;

		public const int CategoryHeaderViewMaintenance = 2;

		public const int CategorizedViews = 3;

		public const int FaultInjection = 20;

		public static Guid guid = new Guid("0e12474e-7e64-471f-93f5-901f795c4ae0");
	}
}
