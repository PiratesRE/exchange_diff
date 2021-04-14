using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Transport;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal sealed class Rcpt2ParseOutput
	{
		public Dictionary<string, string> ConsumerMailOptionalArguments { get; set; }

		public RoutingAddress RecipientAddress { get; set; }
	}
}
