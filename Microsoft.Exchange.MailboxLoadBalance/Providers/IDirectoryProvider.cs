using System;
using System.Collections.Generic;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Directory;
using Microsoft.Exchange.MailboxLoadBalance.QueueProcessing;

namespace Microsoft.Exchange.MailboxLoadBalance.Providers
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IDirectoryProvider
	{
		IEnumerable<DirectoryDatabaseAvailabilityGroup> GetDatabaseAvailabilityGroups();

		DirectoryServer GetServer(Guid serverGuid);

		DirectoryDatabase GetDatabase(Guid guid);

		DirectoryForest GetLocalForest();

		IEnumerable<DirectoryServer> GetServers(DirectoryIdentity dagIdentity);

		IEnumerable<DirectoryServer> GetServers();

		IEnumerable<DirectoryDatabase> GetDatabasesOwnedByServer(DirectoryServer server);

		DirectoryServer GetLocalServer();

		IEnumerable<DirectoryServer> GetActivationPreferenceForDatabase(DirectoryDatabase database);

		DirectoryDatabase GetDatabaseForMailbox(DirectoryIdentity identity);

		IEnumerable<DirectoryMailbox> GetMailboxesForDatabase(DirectoryDatabase database);

		IEnumerable<NonConnectedMailbox> GetDisconnectedMailboxesForDatabase(DirectoryDatabase database);

		DirectoryObject GetDirectoryObject(DirectoryIdentity directoryObjectIdentity);

		IRequest CreateRequestToMove(DirectoryMailbox directoryMailbox, DirectoryIdentity targetIdentity, string batchName, ILogger logger);

		DirectoryServer GetServerByFqdn(Fqdn fqdn);

		IEnumerable<DirectoryDatabase> GetCachedDatabasesForProvisioning();
	}
}
