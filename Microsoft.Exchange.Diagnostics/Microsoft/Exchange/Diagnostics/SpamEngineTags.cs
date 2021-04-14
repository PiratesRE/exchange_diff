using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct SpamEngineTags
	{
		public const int RulesEngine = 0;

		public const int Common = 1;

		public const int BackScatter = 2;

		public const int SenderAuthentication = 3;

		public const int UriScan = 4;

		public const int DnsChecks = 5;

		public const int Dkim = 6;

		public const int Dmarc = 7;

		public static Guid guid = new Guid("D47F7E4B-8F27-43fa-9BE6-DDF69C7AE225");
	}
}
