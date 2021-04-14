using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Flags]
	public enum ClientSMimeControlStatus : uint
	{
		None = 0U,
		NotInstalled = 1U,
		MustUpdate = 2U,
		Outdated = 4U,
		OK = 8U,
		ConnectionIsSSL = 16U
	}
}
