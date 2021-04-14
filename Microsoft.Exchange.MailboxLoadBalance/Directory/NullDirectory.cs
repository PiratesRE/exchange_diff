using System;
using System.Collections.Generic;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Providers;
using Microsoft.Exchange.MailboxLoadBalance.QueueProcessing;

namespace Microsoft.Exchange.MailboxLoadBalance.Directory
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class NullDirectory : IDirectoryProvider
	{
		private NullDirectory()
		{
		}

		public IEnumerable<DirectoryDatabaseAvailabilityGroup> GetDatabaseAvailabilityGroups()
		{
			yield break;
		}

		public DirectoryServer GetServer(Guid serverGuid)
		{
			return null;
		}

		public DirectoryDatabase GetDatabase(Guid guid)
		{
			return null;
		}

		public DirectoryForest GetLocalForest()
		{
			return null;
		}

		public IEnumerable<DirectoryServer> GetServers(DirectoryIdentity dagIdentity)
		{
			yield break;
		}

		public IEnumerable<DirectoryServer> GetServers()
		{
			yield break;
		}

		public IEnumerable<DirectoryDatabase> GetDatabasesOwnedByServer(DirectoryServer server)
		{
			yield break;
		}

		public DirectoryServer GetLocalServer()
		{
			return null;
		}

		public IEnumerable<DirectoryServer> GetActivationPreferenceForDatabase(DirectoryDatabase database)
		{
			yield break;
		}

		public DirectoryDatabase GetDatabaseForMailbox(DirectoryIdentity identity)
		{
			return null;
		}

		public IEnumerable<DirectoryMailbox> GetMailboxesForDatabase(DirectoryDatabase database)
		{
			yield break;
		}

		public IEnumerable<NonConnectedMailbox> GetDisconnectedMailboxesForDatabase(DirectoryDatabase database)
		{
			yield break;
		}

		public DirectoryObject GetDirectoryObject(DirectoryIdentity directoryObjectIdentity)
		{
			return null;
		}

		public IRequest CreateRequestToMove(DirectoryMailbox directoryMailbox, DirectoryIdentity targetIdentity, string batchName, ILogger logger)
		{
			return new NullDirectory.NullRequest();
		}

		public DirectoryServer GetServerByFqdn(Fqdn fqdn)
		{
			return null;
		}

		public IEnumerable<DirectoryDatabase> GetCachedDatabasesForProvisioning()
		{
			yield break;
		}

		public static readonly IDirectoryProvider Instance = new NullDirectory();

		private class NullRequest : BaseRequest
		{
			protected override void ProcessRequest()
			{
			}
		}
	}
}
