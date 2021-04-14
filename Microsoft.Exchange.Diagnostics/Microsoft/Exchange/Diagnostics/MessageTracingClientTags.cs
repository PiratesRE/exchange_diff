using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct MessageTracingClientTags
	{
		public const int Parser = 0;

		public const int Writer = 1;

		public const int Reader = 2;

		public const int LogMonitor = 3;

		public const int General = 4;

		public const int TransportQueue = 5;

		public static Guid guid = new Guid("0402AB9A-3D53-4353-AC55-9A9491E5A22A");
	}
}
