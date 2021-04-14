using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Directory;
using Microsoft.Exchange.MailboxLoadBalance.LoadBalance;

namespace Microsoft.Exchange.MailboxLoadBalance.ServiceSupport
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface IClientFactory
	{
		ILoadBalanceService GetLoadBalanceClientForServer(DirectoryServer server, bool allowFallbackToLocal);

		ILoadBalanceService GetLoadBalanceClientForDatabase(DirectoryDatabase database);

		IInjectorService GetInjectorClientForDatabase(DirectoryDatabase database);

		IPhysicalDatabase GetPhysicalDatabaseConnection(DirectoryDatabase database);

		ILoadBalanceService GetLoadBalanceClientForCentralServer();
	}
}
