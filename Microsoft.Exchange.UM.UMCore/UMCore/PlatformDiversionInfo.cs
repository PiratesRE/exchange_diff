using System;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class PlatformDiversionInfo
	{
		public PlatformDiversionInfo(string header, string calledParty, string userAtHost, RedirectReason reason, DiversionSource source)
		{
			this.DiversionHeader = header;
			this.OriginalCalledParty = calledParty;
			this.UserAtHost = userAtHost;
			this.RedirectReason = reason;
			this.DiversionSource = source;
		}

		public string DiversionHeader { get; private set; }

		public string OriginalCalledParty { get; private set; }

		public string UserAtHost { get; private set; }

		public RedirectReason RedirectReason { get; private set; }

		public DiversionSource DiversionSource { get; private set; }
	}
}
