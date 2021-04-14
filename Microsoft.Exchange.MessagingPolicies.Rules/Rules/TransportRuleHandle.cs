using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal struct TransportRuleHandle
	{
		public TransportRule AdRule { get; set; }

		public TransportRule Rule { get; set; }
	}
}
