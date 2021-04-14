using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Transport;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal sealed class RcptParseOutput
	{
		public RcptParseOutput()
		{
			this.Notify = DsnRequestedFlags.Default;
			this.LowAuthenticationLevelTarpitOverride = TarpitAction.None;
			this.ConsumerMailOptionalArguments = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);
		}

		public RoutingAddress RecipientAddress { get; set; }

		public RoutingAddress Orar { get; set; }

		public DsnRequestedFlags Notify { get; set; }

		public string ORcpt { get; set; }

		public string RDst { get; set; }

		public TarpitAction LowAuthenticationLevelTarpitOverride { get; set; }

		public int TooManyRecipientsResponseCount { get; set; }

		public Dictionary<string, string> ConsumerMailOptionalArguments { get; set; }
	}
}
