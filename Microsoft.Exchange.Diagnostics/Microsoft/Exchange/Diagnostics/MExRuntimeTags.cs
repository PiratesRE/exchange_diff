using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct MExRuntimeTags
	{
		public const int Initialize = 0;

		public const int Dispatch = 1;

		public const int Shutdown = 2;

		public static Guid guid = new Guid("b7916055-456d-46f6-bdd2-42ac88ccb655");
	}
}
