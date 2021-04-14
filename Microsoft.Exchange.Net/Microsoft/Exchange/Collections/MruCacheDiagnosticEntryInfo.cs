using System;

namespace Microsoft.Exchange.Collections
{
	internal class MruCacheDiagnosticEntryInfo
	{
		public MruCacheDiagnosticEntryInfo(string identifier, TimeSpan timeToLive)
		{
			this.Identifier = identifier;
			this.TimeToLive = timeToLive;
		}

		public string Identifier { get; private set; }

		public TimeSpan TimeToLive { get; private set; }
	}
}
