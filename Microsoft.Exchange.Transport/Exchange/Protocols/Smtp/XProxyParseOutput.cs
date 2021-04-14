using System;
using System.Security.Principal;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal struct XProxyParseOutput
	{
		public ProxyParseCommonOutput ProxyParseCommonOutput;

		public SecurityIdentifier SecurityId;

		public SmtpAddress ClientProxyAddress;

		public byte[] RedactedBuffer;

		public int? CapabilitiesInt;
	}
}
