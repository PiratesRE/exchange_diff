using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct ManagedStore_DirectoryServicesTags
	{
		public const int General = 0;

		public const int ADCalls = 1;

		public const int CallStack = 2;

		public const int FaultInjection = 20;

		public static Guid guid = new Guid("2d756daa-9cee-497d-b5a1-dc2f994b99ca");
	}
}
