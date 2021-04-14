using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct ProtocolAnalysisBgTags
	{
		public const int Factory = 0;

		public const int Agent = 1;

		public const int Database = 2;

		public const int OnOpenProxyDetection = 3;

		public const int OnDnsQuery = 4;

		public const int OnSenderBlocking = 5;

		public static Guid guid = new Guid("E30C077B-EBAD-4AC8-B2BF-197C3329952F");
	}
}
