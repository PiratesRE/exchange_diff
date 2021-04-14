using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal enum ProxyScenarios
	{
		None,
		LocalMdbAndProxy,
		RemoteMdbAndProxy = 16,
		LocalProxyRemoteMdb = 32
	}
}
