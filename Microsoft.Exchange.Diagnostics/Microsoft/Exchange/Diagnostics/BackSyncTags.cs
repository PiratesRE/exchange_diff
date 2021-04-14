using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct BackSyncTags
	{
		public const int BackSync = 0;

		public const int ActiveDirectory = 1;

		public const int TenantFullSync = 2;

		public const int Merge = 3;

		public static Guid guid = new Guid("3C237538-546C-4659-AED9-F445236DFB91");
	}
}
