using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct ConnectionFilteringTags
	{
		public const int Error = 0;

		public const int Factory = 1;

		public const int OnConnect = 2;

		public const int OnMailFrom = 3;

		public const int OnRcptTo = 4;

		public const int OnEOH = 5;

		public const int DNS = 6;

		public const int IPAllowDeny = 7;

		public static Guid guid = new Guid("F0A7EB4B-2EE5-478f-A589-5273CAC4E5EE");
	}
}
