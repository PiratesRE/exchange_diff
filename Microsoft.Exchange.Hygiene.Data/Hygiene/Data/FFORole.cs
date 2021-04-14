using System;

namespace Microsoft.Exchange.Hygiene.Data
{
	[Flags]
	internal enum FFORole
	{
		None = 0,
		HubTransport = 1,
		FrontendTransport = 2,
		Background = 4,
		Database = 8,
		WebService = 16,
		DomainNameServer = 32
	}
}
