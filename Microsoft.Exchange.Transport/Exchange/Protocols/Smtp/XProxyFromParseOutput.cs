using System;
using Microsoft.Exchange.Data.Transport.Smtp;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal struct XProxyFromParseOutput
	{
		public ProxyParseCommonOutput ProxyParseCommonOutput;

		public uint SequenceNumber;

		public uint? PermissionsInt;

		public AuthenticationSource? AuthSource;
	}
}
