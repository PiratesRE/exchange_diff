using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct BackgroundJobManagerTags
	{
		public const int SDK = 0;

		public const int Service = 1;

		public const int FaultInjection = 2;

		public static Guid guid = new Guid("790DC26A-9CC8-400D-84B0-1CAA155054BE");
	}
}
