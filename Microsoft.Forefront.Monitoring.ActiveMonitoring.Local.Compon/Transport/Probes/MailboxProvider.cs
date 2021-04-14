using System;
using System.Collections.Generic;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Transport.Probes
{
	internal class MailboxProvider : IMailboxProvider
	{
		private MailboxProvider()
		{
		}

		public static MailboxProvider GetInstance()
		{
			return MailboxProvider.instance;
		}

		public MailboxSelectionResult TryGetMailboxToUse(out Guid mbxGuid, out Guid mdbGuid, out string emailAddress)
		{
			emailAddress = null;
			mbxGuid = Guid.Empty;
			mdbGuid = Guid.Empty;
			MailboxSelectionResult result;
			MailboxDatabaseInfo mailboxDatabaseInfo = this.PickAnActiveMailbox(out result);
			if (mailboxDatabaseInfo != null)
			{
				mbxGuid = mailboxDatabaseInfo.MonitoringAccountMailboxGuid;
				mdbGuid = mailboxDatabaseInfo.MailboxDatabaseGuid;
				emailAddress = string.Format("{0}@{1}", mailboxDatabaseInfo.MonitoringAccount, mailboxDatabaseInfo.MonitoringAccountDomain);
			}
			return result;
		}

		public MailboxDatabaseSelectionResult GetAllMailboxDatabaseInfo(out ICollection<MailboxDatabaseInfo> mailboxDatabases)
		{
			MailboxDatabaseSelectionResult result = MailboxDatabaseSelectionResult.Success;
			mailboxDatabases = null;
			LocalEndpointManager localEndpointManager = LocalEndpointManager.Instance;
			if (localEndpointManager == null || localEndpointManager.MailboxDatabaseEndpoint == null)
			{
				return MailboxDatabaseSelectionResult.NoLocalEndpointManager;
			}
			mailboxDatabases = localEndpointManager.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend;
			if (mailboxDatabases == null || mailboxDatabases.Count == 0)
			{
				return MailboxDatabaseSelectionResult.NoMonitoringMDBs;
			}
			return result;
		}

		private MailboxDatabaseInfo PickAnActiveMailbox(out MailboxSelectionResult mailboxSelectionResult)
		{
			mailboxSelectionResult = MailboxSelectionResult.Success;
			LocalEndpointManager localEndpointManager = LocalEndpointManager.Instance;
			ICollection<MailboxDatabaseInfo> mailboxDatabaseInfoCollectionForBackend = localEndpointManager.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend;
			if (mailboxDatabaseInfoCollectionForBackend == null || mailboxDatabaseInfoCollectionForBackend.Count == 0)
			{
				mailboxSelectionResult = MailboxSelectionResult.NoMonitoringMDBs;
				return null;
			}
			foreach (MailboxDatabaseInfo mailboxDatabaseInfo in mailboxDatabaseInfoCollectionForBackend)
			{
				if (DirectoryAccessor.Instance.IsDatabaseCopyActiveOnLocalServer(mailboxDatabaseInfo.MailboxDatabaseGuid))
				{
					return mailboxDatabaseInfo;
				}
			}
			mailboxSelectionResult = MailboxSelectionResult.NoMonitoringMDBsAreActive;
			return null;
		}

		private static readonly MailboxProvider instance = new MailboxProvider();
	}
}
