using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal sealed class HeloEhloParseOutput
	{
		public HeloEhloParseOutput(string heloDomain, SmtpReceiveCapabilities tlsDomainCapabilities)
		{
			this.HeloDomain = heloDomain;
			this.TlsDomainCapabilities = tlsDomainCapabilities;
		}

		public string HeloDomain { get; private set; }

		public SmtpReceiveCapabilities TlsDomainCapabilities { get; private set; }
	}
}
