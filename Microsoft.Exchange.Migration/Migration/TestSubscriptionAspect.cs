using System;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Migration.DataAccessLayer;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class TestSubscriptionAspect
	{
		internal TestSubscriptionAspect(MigrationEndpointBase endpoint, MigrationJobItem jobItem, bool update)
		{
			this.Identifier = jobItem.Identifier;
			MRSSubscriptionId mrssubscriptionId = (MRSSubscriptionId)jobItem.SubscriptionId;
			if (mrssubscriptionId != null)
			{
				this.SubscriptionId = mrssubscriptionId.Id;
			}
			if (jobItem.MigrationType == MigrationType.ExchangeOutlookAnywhere)
			{
				ExchangeOutlookAnywhereEndpoint exchangeOutlookAnywhereEndpoint = (ExchangeOutlookAnywhereEndpoint)endpoint;
				ExchangeJobItemSubscriptionSettings settings = MRSMergeRequestAccessor.GetSettings(jobItem);
				this.RemoteServer = exchangeOutlookAnywhereEndpoint.RpcProxyServer;
				this.RemoteServerDN = settings.ExchangeServerDN;
				this.RemoteMailboxDN = settings.MailboxDN;
				this.UserName = exchangeOutlookAnywhereEndpoint.Username;
				this.HasAdminPrivilege = true;
				return;
			}
			if (jobItem.MigrationType == MigrationType.ExchangeRemoteMove || jobItem.MigrationType == MigrationType.ExchangeLocalMove)
			{
				ExchangeRemoteMoveEndpoint exchangeRemoteMoveEndpoint = (ExchangeRemoteMoveEndpoint)endpoint;
				MoveJobSubscriptionSettings moveJobSubscriptionSettings = (MoveJobSubscriptionSettings)jobItem.MigrationJob.SubscriptionSettings;
				if (exchangeRemoteMoveEndpoint != null)
				{
					this.RemoteServer = exchangeRemoteMoveEndpoint.RemoteServer;
					this.UserName = exchangeRemoteMoveEndpoint.Username;
				}
				Unlimited<int>? unlimited;
				Unlimited<int>? unlimited2;
				string targetDatabase;
				string targetArchiveDatabase;
				bool flag;
				bool flag2;
				MrsMoveRequestAccessor.RetrieveDuplicatedSettings(moveJobSubscriptionSettings, (MoveJobItemSubscriptionSettings)jobItem.SubscriptionSettings, !update, out unlimited, out unlimited2, out targetDatabase, out targetArchiveDatabase, out flag, out flag2);
				this.BadItemLimit = ((unlimited == null) ? string.Empty : unlimited.ToString());
				this.LargeItemLimit = ((unlimited2 == null) ? string.Empty : unlimited2.ToString());
				this.StartAfter = (DateTime?)moveJobSubscriptionSettings.StartAfter;
				this.CompleteAfter = (DateTime?)moveJobSubscriptionSettings.CompleteAfter;
				if (!update)
				{
					this.TargetDatabase = targetDatabase;
					this.TargetArchiveDatabase = targetArchiveDatabase;
					return;
				}
			}
			else if (jobItem.MigrationType == MigrationType.PSTImport)
			{
				PSTImportEndpoint pstimportEndpoint = (PSTImportEndpoint)endpoint;
				PSTJobSubscriptionSettings jobSettings = (PSTJobSubscriptionSettings)jobItem.MigrationJob.SubscriptionSettings;
				if (pstimportEndpoint != null)
				{
					this.RemoteServer = pstimportEndpoint.RemoteServer;
					this.UserName = pstimportEndpoint.Username;
				}
				Unlimited<int>? unlimited3;
				Unlimited<int>? unlimited4;
				bool flag3;
				bool flag4;
				PSTImportAccessor.RetrieveDuplicatedSettings(jobSettings, (PSTJobItemSubscriptionSettings)jobItem.SubscriptionSettings, !update, out unlimited3, out unlimited4, out flag3, out flag4);
				this.BadItemLimit = ((unlimited3 == null) ? string.Empty : unlimited3.ToString());
				this.LargeItemLimit = ((unlimited4 == null) ? string.Empty : unlimited4.ToString());
			}
		}

		public string RemoteServer { get; private set; }

		public string UserName { get; private set; }

		public string Identifier { get; private set; }

		public string RemoteServerDN { get; private set; }

		public string RemoteMailboxDN { get; private set; }

		public TimeSpan TimeoutForFailingSubscription { get; private set; }

		public Guid SubscriptionId { get; private set; }

		public bool HasAdminPrivilege { get; private set; }

		public string BadItemLimit { get; private set; }

		public string LargeItemLimit { get; private set; }

		public string TargetDatabase { get; private set; }

		public string TargetArchiveDatabase { get; private set; }

		public DateTime? StartAfter { get; private set; }

		public DateTime? CompleteAfter { get; private set; }

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}: [JobItemIdentifier = {1}, SubscriptionID: {2}]", new object[]
			{
				base.GetType(),
				this.Identifier,
				this.SubscriptionId
			});
		}
	}
}
