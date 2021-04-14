using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct MessageTrackingTags
	{
		public const int Task = 0;

		public const int ServerStatus = 1;

		public const int LogAnalysis = 2;

		public const int SearchLibrary = 3;

		public const int WebService = 4;

		public static Guid guid = new Guid("0B7BA732-EF67-4e7c-A68F-3D8593D9DC06");
	}
}
