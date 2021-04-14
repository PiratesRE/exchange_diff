using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Transport.Sync.Common.Exceptions;
using Microsoft.Exchange.Transport.Sync.Common.Rpc.Notification;

namespace Microsoft.Exchange.Transport.Sync.Common.SyncHealthLog
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SyncHealthLog : DisposeTrackableBase, ISyncHealthLog
	{
		public SyncHealthLog(SyncHealthLogConfiguration syncHealthLogConfiguration)
		{
			this.Configure(syncHealthLogConfiguration);
		}

		public static LogSchema SyncHealthSchema
		{
			get
			{
				return SyncHealthLog.syncHealthSchema;
			}
		}

		public void Configure(SyncHealthLogConfiguration syncHealthLogConfiguration)
		{
			base.CheckDisposed();
			SyncUtilities.ThrowIfArgumentNull("syncHealthLogConfiguration", syncHealthLogConfiguration);
			this.enabled = syncHealthLogConfiguration.SyncHealthLogEnabled;
			this.logger.Configure(syncHealthLogConfiguration.SyncHealthLogPath, syncHealthLogConfiguration.SyncHealthLogMaxAge, syncHealthLogConfiguration.SyncHealthLogMaxDirectorySize, syncHealthLogConfiguration.SyncHealthLogMaxFileSize);
		}

		public void Close()
		{
			base.CheckDisposed();
			lock (this.syncRoot)
			{
				if (this.logger != null)
				{
					this.logger.Close();
					this.logger = null;
				}
			}
		}

		public void LogSync(string hubServerName, string databaseGuid, string userMailboxGuid, string subscriptionGuid, string tenantGuid, string incomingServerName, string domain, string subscriptionType, string aggregationType, TimeSpan syncDuration, string aggregationStatus, string detailedAggregationStatus, List<Exception> exceptions, Exception syncEngineException, int totalItemAddsEnumeratedFromRemoteServer, int totalItemAddsAppliedToLocalServer, int totalItemChangesEnumeratedFromRemoteServer, int totalItemChangesAppliedToLocalServer, int totalItemDeletesEnumeratedFromRemoteServer, int totalItemDeletesAppliedToLocalServer, int totalFolderAddsEnumeratedFromRemoteServer, int totalFolderAddsAppliedToLocalServer, int totalFolderChangesEnumeratedFromRemoteServer, int totalFolderChangesAppliedToLocalServer, int totalFolderDeletesEnumeratedFromRemoteServer, int totalFolderDeletesAppliedToLocalServer, ByteQuantifiedSize bytesFromRemoteServer, bool isPermanentSyncError, bool isTransientSyncError, int oversizeItemErrorsCount, int poisonItemErrorsCount, int unresolveableFolderNameErrorsCount, int objectNotFoundErrorsCount, int otherItemLevelErrorsCount, int permanentItemErrorsCount, int transientItemErrorsCount, int permanentFolderErrorsCount, int transientFolderErrorsCount, int totalItemAddsPermanentExceptions, int totalItemAddsTransientExceptions, int totalItemDeletesPermanentExceptions, int totalItemDeletesTransientExceptions, int totalItemChangesPermanentExceptions, int totalItemChangesTransientExceptions, int totalFolderAddsPermanentExceptions, int totalFolderAddsTransientExceptions, int totalFolderDeletesPermanentExceptions, int totalFolderDeletesTransientExceptions, int totalFolderChangesPermanentExceptions, int totalFolderChangesTransientExceptions, int totalSuccessfulRemoteRoundtrips, TimeSpan averageSuccessfulRemoteRoundtripTime, int totalUnsuccessfulRemoteRoundtrips, TimeSpan averageUnsuccessfulRemoteRoundtripTime, int totalSuccessfulNativeRoundtrips, TimeSpan averageSuccessfulNativeRoundtripTime, int totalUnsuccessfulNativeRoundtrips, TimeSpan averageUnsuccessfulNativeRoundtripTime, TimeSpan averageNativeBackoffTime, int totalSuccessfulEngineRoundtrips, TimeSpan averageSuccessfulEngineRoundtripTime, int totalUnsuccessfulEngineRoundtrips, TimeSpan averageUnsuccessfulEngineRoundtripTime, TimeSpan averageEngineBackoffTime, long remoteMailboxItemCount, long remoteMailboxFolderCount, long totalSizeOfSourceMailbox, TimeSpan workItemLifetime, string syncPhaseAfterSync, string syncPhaseBeforeSync, int retryCount, bool wasRecoverySync, bool userMailboxOpened, TimeSpan mailboxCpuUnhealthyBackOffTime, TimeSpan mailboxCpuFairBackOffTime, TimeSpan mailboxCpuUnknownBackOffTime, TimeSpan dbReplicationLogUnhealthyBackOffTime, TimeSpan dbReplicationLogFairBackOffTime, TimeSpan dbReplicationLogUnknownBackOffTime, TimeSpan dbRpcLatenyUnhealthyBackOffTime, TimeSpan dbRpcLatenyFairBackOffTime, TimeSpan dbRpcLatenyUnknownBackOffTime, TimeSpan unknownResourceUnhealthyBackOffTime, TimeSpan unknownResourceFairBackOffTime, TimeSpan unknownResourceUnknownBackOffTime, TimeSpan totalBackOffTime)
		{
			base.CheckDisposed();
			SyncUtilities.ThrowIfArgumentNull("hubServerName", hubServerName);
			SyncUtilities.ThrowIfArgumentNull("databaseGuid", databaseGuid);
			SyncUtilities.ThrowIfArgumentNull("userMailboxGuid", userMailboxGuid);
			SyncUtilities.ThrowIfArgumentNull("subscriptionGuid", subscriptionGuid);
			SyncUtilities.ThrowIfArgumentNull("tenantGuid", tenantGuid);
			SyncUtilities.ThrowIfArgumentNull("incomingServerName", incomingServerName);
			SyncUtilities.ThrowIfArgumentNull("domain", domain);
			SyncUtilities.ThrowIfArgumentNull("subscriptionType", subscriptionType);
			SyncUtilities.ThrowIfArgumentNull("aggregationType", aggregationType);
			SyncUtilities.ThrowIfArgumentNull("aggregationStatus", aggregationStatus);
			SyncUtilities.ThrowIfArgumentNull("detailedAggregationStatus", detailedAggregationStatus);
			SyncUtilities.ThrowIfArgumentNull("syncPhaseAfterSync", syncPhaseAfterSync);
			SyncUtilities.ThrowIfArgumentNull("syncPhaseBeforeSync", syncPhaseBeforeSync);
			string value = this.ConvertExceptionListToString(exceptions);
			string value2 = string.Empty;
			if (syncEngineException != null)
			{
				string name = syncEngineException.GetType().Name;
				string arg = (syncEngineException.InnerException != null) ? syncEngineException.InnerException.GetType().Name : "NoInnerException";
				value2 = string.Format("{0}_{1}", name, arg);
			}
			lock (this.syncRoot)
			{
				this.LogEvent(SyncHealthEventsStrings.SyncHealthEvents.Sync, new KeyValuePair<string, object>[]
				{
					new KeyValuePair<string, object>("HSN", hubServerName),
					new KeyValuePair<string, object>("UMG", userMailboxGuid),
					new KeyValuePair<string, object>("SG", subscriptionGuid),
					new KeyValuePair<string, object>("ISN", incomingServerName),
					new KeyValuePair<string, object>("ST", subscriptionType),
					new KeyValuePair<string, object>("AT", aggregationType),
					new KeyValuePair<string, object>("SD", syncDuration.TotalMilliseconds.ToString()),
					new KeyValuePair<string, object>("AS", aggregationStatus),
					new KeyValuePair<string, object>("DAS", detailedAggregationStatus),
					new KeyValuePair<string, object>("TIAEFRS", totalItemAddsEnumeratedFromRemoteServer),
					new KeyValuePair<string, object>("TIAATRS", totalItemAddsAppliedToLocalServer),
					new KeyValuePair<string, object>("TICEFRS", totalItemChangesEnumeratedFromRemoteServer),
					new KeyValuePair<string, object>("TICATLS", totalItemChangesAppliedToLocalServer),
					new KeyValuePair<string, object>("TIDEFRS", totalItemDeletesEnumeratedFromRemoteServer),
					new KeyValuePair<string, object>("TIDATLS", totalItemDeletesAppliedToLocalServer),
					new KeyValuePair<string, object>("TFAEFRS", totalFolderAddsEnumeratedFromRemoteServer),
					new KeyValuePair<string, object>("TFAATLS", totalFolderAddsAppliedToLocalServer),
					new KeyValuePair<string, object>("TFCEFRS", totalFolderChangesEnumeratedFromRemoteServer),
					new KeyValuePair<string, object>("TFCATLS", totalFolderChangesAppliedToLocalServer),
					new KeyValuePair<string, object>("TFDEFRS", totalFolderDeletesEnumeratedFromRemoteServer),
					new KeyValuePair<string, object>("TFDATLS", totalFolderDeletesAppliedToLocalServer),
					new KeyValuePair<string, object>("KBFRS", bytesFromRemoteServer.ToKB().ToString()),
					new KeyValuePair<string, object>("PSE", isPermanentSyncError.ToString()),
					new KeyValuePair<string, object>("TSE", isTransientSyncError.ToString()),
					new KeyValuePair<string, object>("OIEC", oversizeItemErrorsCount),
					new KeyValuePair<string, object>("PoIEC", poisonItemErrorsCount),
					new KeyValuePair<string, object>("OILEC", otherItemLevelErrorsCount),
					new KeyValuePair<string, object>("PIEC", permanentItemErrorsCount),
					new KeyValuePair<string, object>("TIEC", transientItemErrorsCount),
					new KeyValuePair<string, object>("TIAPE", totalItemAddsPermanentExceptions),
					new KeyValuePair<string, object>("TIATE", totalItemAddsTransientExceptions),
					new KeyValuePair<string, object>("TIDPE", totalItemDeletesPermanentExceptions),
					new KeyValuePair<string, object>("TIDTE", totalItemDeletesTransientExceptions),
					new KeyValuePair<string, object>("TICPE", totalItemChangesPermanentExceptions),
					new KeyValuePair<string, object>("TICTE", totalItemChangesTransientExceptions),
					new KeyValuePair<string, object>("TFAPE", totalFolderAddsPermanentExceptions),
					new KeyValuePair<string, object>("TFATE", totalFolderAddsTransientExceptions),
					new KeyValuePair<string, object>("TFDPE", totalFolderDeletesPermanentExceptions),
					new KeyValuePair<string, object>("TFDTE", totalFolderDeletesTransientExceptions),
					new KeyValuePair<string, object>("TFCPE", totalFolderChangesPermanentExceptions),
					new KeyValuePair<string, object>("TFCTE", totalFolderChangesTransientExceptions),
					new KeyValuePair<string, object>("TSRR", totalSuccessfulRemoteRoundtrips),
					new KeyValuePair<string, object>("ASRRT", averageSuccessfulRemoteRoundtripTime.TotalMilliseconds.ToString()),
					new KeyValuePair<string, object>("TURR", totalUnsuccessfulRemoteRoundtrips),
					new KeyValuePair<string, object>("AURRT", averageUnsuccessfulRemoteRoundtripTime.TotalMilliseconds.ToString()),
					new KeyValuePair<string, object>("TSNR", totalSuccessfulNativeRoundtrips),
					new KeyValuePair<string, object>("ASNRT", averageSuccessfulNativeRoundtripTime.TotalMilliseconds.ToString()),
					new KeyValuePair<string, object>("TUNR", totalUnsuccessfulNativeRoundtrips),
					new KeyValuePair<string, object>("AUNRT", averageUnsuccessfulNativeRoundtripTime.TotalMilliseconds.ToString()),
					new KeyValuePair<string, object>("ANBT", averageNativeBackoffTime.TotalMilliseconds.ToString()),
					new KeyValuePair<string, object>("TSER", totalSuccessfulEngineRoundtrips),
					new KeyValuePair<string, object>("ASERT", averageSuccessfulEngineRoundtripTime.TotalMilliseconds.ToString()),
					new KeyValuePair<string, object>("TUER", totalUnsuccessfulEngineRoundtrips),
					new KeyValuePair<string, object>("AUERT", averageUnsuccessfulEngineRoundtripTime.TotalMilliseconds.ToString()),
					new KeyValuePair<string, object>("AEBT", averageEngineBackoffTime.TotalMilliseconds.ToString()),
					new KeyValuePair<string, object>("RMIC", remoteMailboxItemCount.ToString()),
					new KeyValuePair<string, object>("RMFC", remoteMailboxFolderCount.ToString()),
					new KeyValuePair<string, object>("WILT", workItemLifetime.TotalMilliseconds.ToString()),
					new KeyValuePair<string, object>("SPAS", syncPhaseAfterSync),
					new KeyValuePair<string, object>("DG", databaseGuid),
					new KeyValuePair<string, object>("UFNEC", unresolveableFolderNameErrorsCount),
					new KeyValuePair<string, object>("ONFEC", objectNotFoundErrorsCount),
					new KeyValuePair<string, object>("DM", domain),
					new KeyValuePair<string, object>("TG", tenantGuid),
					new KeyValuePair<string, object>("RC", retryCount),
					new KeyValuePair<string, object>("WRS", wasRecoverySync.ToString()),
					new KeyValuePair<string, object>("SPBS", syncPhaseBeforeSync),
					new KeyValuePair<string, object>("UMO", userMailboxOpened.ToString()),
					new KeyValuePair<string, object>("INE", value),
					new KeyValuePair<string, object>("CUBOT", mailboxCpuUnhealthyBackOffTime.TotalMilliseconds.ToString()),
					new KeyValuePair<string, object>("CFBOT", mailboxCpuFairBackOffTime.TotalMilliseconds.ToString()),
					new KeyValuePair<string, object>("CKBOT", mailboxCpuUnknownBackOffTime.TotalMilliseconds.ToString()),
					new KeyValuePair<string, object>("RUBOT", dbReplicationLogUnhealthyBackOffTime.TotalMilliseconds.ToString()),
					new KeyValuePair<string, object>("RFBOT", dbReplicationLogFairBackOffTime.TotalMilliseconds.ToString()),
					new KeyValuePair<string, object>("RKBOT", dbReplicationLogUnknownBackOffTime.TotalMilliseconds.ToString()),
					new KeyValuePair<string, object>("LUBOT", dbRpcLatenyUnhealthyBackOffTime.TotalMilliseconds.ToString()),
					new KeyValuePair<string, object>("LFBOT", dbRpcLatenyFairBackOffTime.TotalMilliseconds.ToString()),
					new KeyValuePair<string, object>("LNBOT", dbRpcLatenyUnknownBackOffTime.TotalMilliseconds.ToString()),
					new KeyValuePair<string, object>("UUBOT", unknownResourceUnhealthyBackOffTime.TotalMilliseconds.ToString()),
					new KeyValuePair<string, object>("UFBOT", unknownResourceFairBackOffTime.TotalMilliseconds.ToString()),
					new KeyValuePair<string, object>("UKBOT", unknownResourceUnknownBackOffTime.TotalMilliseconds.ToString()),
					new KeyValuePair<string, object>("TBOT", totalBackOffTime.TotalMilliseconds.ToString()),
					new KeyValuePair<string, object>("TSSM", totalSizeOfSourceMailbox.ToString()),
					new KeyValuePair<string, object>("SEE", value2)
				});
			}
		}

		public void LogPolicyInducedSubscriptionDeletion(string hubServerName, string tenantGuid, string databaseGuid, string userMailboxGuid, string subscriptionGuid, string subscriptionType, int retryCount, bool wasRecoverySync, bool isPermanentSyncError, bool isTransientSyncError, List<Exception> exceptions)
		{
			base.CheckDisposed();
			SyncUtilities.ThrowIfArgumentNull("hubServerName", hubServerName);
			SyncUtilities.ThrowIfArgumentNull("tenantGuid", tenantGuid);
			SyncUtilities.ThrowIfArgumentNull("databaseGuid", databaseGuid);
			SyncUtilities.ThrowIfArgumentNull("userMailboxGuid", userMailboxGuid);
			SyncUtilities.ThrowIfArgumentNull("subscriptionGuid", subscriptionGuid);
			SyncUtilities.ThrowIfArgumentNull("subscriptionType", subscriptionType);
			string value = this.ConvertExceptionListToString(exceptions);
			lock (this.syncRoot)
			{
				this.LogEvent(SyncHealthEventsStrings.SyncHealthEvents.PolicyInducedSubscriptionDeletion, new KeyValuePair<string, object>[]
				{
					new KeyValuePair<string, object>("HSN", hubServerName),
					new KeyValuePair<string, object>("TG", tenantGuid),
					new KeyValuePair<string, object>("DG", databaseGuid),
					new KeyValuePair<string, object>("UMG", userMailboxGuid),
					new KeyValuePair<string, object>("SG", subscriptionGuid),
					new KeyValuePair<string, object>("ST", subscriptionType),
					new KeyValuePair<string, object>("RC", retryCount),
					new KeyValuePair<string, object>("WRS", wasRecoverySync.ToString()),
					new KeyValuePair<string, object>("PSE", isPermanentSyncError.ToString()),
					new KeyValuePair<string, object>("TSE", isTransientSyncError.ToString()),
					new KeyValuePair<string, object>("INE", value)
				});
			}
		}

		public void LogMailboxNotification(Guid mailboxGuid, Guid mdbGuid, SubscriptionNotificationRpcMethodCode notificationType)
		{
			base.CheckDisposed();
			SyncUtilities.ThrowIfGuidEmpty("mailboxGuid", mailboxGuid);
			SyncUtilities.ThrowIfGuidEmpty("mdbGuid", mdbGuid);
			lock (this.syncRoot)
			{
				this.LogEvent(SyncHealthEventsStrings.SyncHealthEvents.MailboxNotification, new KeyValuePair<string, object>[]
				{
					new KeyValuePair<string, object>("UMG", mailboxGuid.ToString()),
					new KeyValuePair<string, object>("DG", mdbGuid.ToString()),
					new KeyValuePair<string, object>("NT", notificationType.ToString())
				});
			}
		}

		public void LogRemoteServerHealth(string hubServerName, string remoteServerName, string state, int backOffCount, DateTime lastBackOffStartTime, DateTime lastUpdateTime)
		{
			base.CheckDisposed();
			SyncUtilities.ThrowIfArgumentNullOrEmpty("hubServerName", hubServerName);
			SyncUtilities.ThrowIfArgumentNullOrEmpty("remoteServerName", remoteServerName);
			SyncUtilities.ThrowIfArgumentNullOrEmpty("state", state);
			lock (this.syncRoot)
			{
				this.LogEvent(SyncHealthEventsStrings.SyncHealthEvents.RemoteServerHealth, new KeyValuePair<string, object>[]
				{
					new KeyValuePair<string, object>("HSN", hubServerName),
					new KeyValuePair<string, object>("RSN", remoteServerName),
					new KeyValuePair<string, object>("ST", state),
					new KeyValuePair<string, object>("BC", backOffCount),
					new KeyValuePair<string, object>("LBT", lastBackOffStartTime),
					new KeyValuePair<string, object>("LUT", lastUpdateTime)
				});
			}
		}

		public void LogSubscriptionDispatch(string mailboxServerName, string tenantGuid, string userMailboxGuid, string subscriptionGuid, string incomingServerName, string subscriptionType, string aggregationType, string dispatchedTo, bool isSuccessful, bool isPermanentError, bool isTransientError, string dispatchError, bool isBeyondSyncPollingFrequency, int secondsBeyondPollingFrequency, string workType, string dispatchTrigger, string databaseGuid)
		{
			base.CheckDisposed();
			SyncUtilities.ThrowIfArgumentNull("mailboxServerName", mailboxServerName);
			SyncUtilities.ThrowIfArgumentNull("tenantGuid", tenantGuid);
			SyncUtilities.ThrowIfArgumentNull("userMailboxGuid", userMailboxGuid);
			SyncUtilities.ThrowIfArgumentNull("subscriptionGuid", subscriptionGuid);
			SyncUtilities.ThrowIfArgumentNull("incomingServerName", incomingServerName);
			SyncUtilities.ThrowIfArgumentNull("subscriptionType", subscriptionType);
			SyncUtilities.ThrowIfArgumentNull("aggregationType", aggregationType);
			SyncUtilities.ThrowIfArgumentNull("dispatchedTo", dispatchedTo);
			SyncUtilities.ThrowIfArgumentNull("dispatchError", dispatchError);
			SyncUtilities.ThrowIfArgumentNull("workType", workType);
			SyncUtilities.ThrowIfArgumentNull("dispatchTrigger", dispatchTrigger);
			SyncUtilities.ThrowIfArgumentNull("databaseGuid", databaseGuid);
			lock (this.syncRoot)
			{
				this.LogEvent(SyncHealthEventsStrings.SyncHealthEvents.SubscriptionDispatch, new KeyValuePair<string, object>[]
				{
					new KeyValuePair<string, object>("MSN", mailboxServerName),
					new KeyValuePair<string, object>("TG", tenantGuid),
					new KeyValuePair<string, object>("UMG", userMailboxGuid),
					new KeyValuePair<string, object>("SG", subscriptionGuid),
					new KeyValuePair<string, object>("ISN", incomingServerName),
					new KeyValuePair<string, object>("ST", subscriptionType),
					new KeyValuePair<string, object>("AT", aggregationType),
					new KeyValuePair<string, object>("DT", dispatchedTo),
					new KeyValuePair<string, object>("IS", isSuccessful.ToString()),
					new KeyValuePair<string, object>("PE", isPermanentError.ToString()),
					new KeyValuePair<string, object>("TE", isTransientError.ToString()),
					new KeyValuePair<string, object>("DE", dispatchError),
					new KeyValuePair<string, object>("BSPF", isBeyondSyncPollingFrequency.ToString()),
					new KeyValuePair<string, object>("SBPF", secondsBeyondPollingFrequency),
					new KeyValuePair<string, object>("WT", workType),
					new KeyValuePair<string, object>("DTR", dispatchTrigger),
					new KeyValuePair<string, object>("DG", databaseGuid)
				});
			}
		}

		public void LogSubscriptionCompletion(string mailboxServerName, string tenantGuid, string userMailboxGuid, string subscriptionGuid, string incomingServerName, string subscriptionType, string aggregationType, string processedBy, TimeSpan syncDuration, bool moreDataAvailable)
		{
			base.CheckDisposed();
			SyncUtilities.ThrowIfArgumentNull("mailboxServerName", mailboxServerName);
			SyncUtilities.ThrowIfArgumentNull("tenantGuid", tenantGuid);
			SyncUtilities.ThrowIfArgumentNull("userMailboxGuid", userMailboxGuid);
			SyncUtilities.ThrowIfArgumentNull("subscriptionGuid", subscriptionGuid);
			SyncUtilities.ThrowIfArgumentNull("incomingServerName", incomingServerName);
			SyncUtilities.ThrowIfArgumentNull("subscriptionType", subscriptionType);
			SyncUtilities.ThrowIfArgumentNull("aggregationType", aggregationType);
			SyncUtilities.ThrowIfArgumentNull("processedBy", processedBy);
			lock (this.syncRoot)
			{
				this.LogEvent(SyncHealthEventsStrings.SyncHealthEvents.SubscriptionCompletion, new KeyValuePair<string, object>[]
				{
					new KeyValuePair<string, object>("MSN", mailboxServerName),
					new KeyValuePair<string, object>("TG", tenantGuid),
					new KeyValuePair<string, object>("UMG", userMailboxGuid),
					new KeyValuePair<string, object>("SG", subscriptionGuid),
					new KeyValuePair<string, object>("ISN", incomingServerName),
					new KeyValuePair<string, object>("ST", subscriptionType),
					new KeyValuePair<string, object>("AT", aggregationType),
					new KeyValuePair<string, object>("PB", processedBy),
					new KeyValuePair<string, object>("SD", syncDuration.TotalMilliseconds.ToString()),
					new KeyValuePair<string, object>("MDA", moreDataAvailable.ToString())
				});
			}
		}

		public void LogSubscriptionCreation(string mailboxServerName, string tenantGuid, string userMailboxGuid, string subscriptionGuid, string subscriptionType, string creationType, string emailDomain, string incomingServerName, int port, string authenticationType, string encryptionType, DateTime creationTime, string aggregationType)
		{
			base.CheckDisposed();
			SyncUtilities.ThrowIfArgumentNull("mailboxServerName", mailboxServerName);
			SyncUtilities.ThrowIfArgumentNull("tenantGuid", tenantGuid);
			SyncUtilities.ThrowIfArgumentNull("userMailboxGuid", userMailboxGuid);
			SyncUtilities.ThrowIfArgumentNull("subscriptionGuid", subscriptionGuid);
			SyncUtilities.ThrowIfArgumentNull("subscriptionType", subscriptionType);
			SyncUtilities.ThrowIfArgumentNull("creationType", creationType);
			SyncUtilities.ThrowIfArgumentNull("emailDomain", emailDomain);
			SyncUtilities.ThrowIfArgumentNull("incomingServerName", incomingServerName);
			SyncUtilities.ThrowIfArgumentNull("authenticationType", authenticationType);
			SyncUtilities.ThrowIfArgumentNull("encryptionType", encryptionType);
			SyncUtilities.ThrowIfArgumentNull("aggregationType", aggregationType);
			lock (this.syncRoot)
			{
				this.LogEvent(SyncHealthEventsStrings.SyncHealthEvents.SubscriptionCreation, new KeyValuePair<string, object>[]
				{
					new KeyValuePair<string, object>("MSN", mailboxServerName),
					new KeyValuePair<string, object>("TG", tenantGuid),
					new KeyValuePair<string, object>("UMG", userMailboxGuid),
					new KeyValuePair<string, object>("SG", subscriptionGuid),
					new KeyValuePair<string, object>("ISN", incomingServerName),
					new KeyValuePair<string, object>("ST", subscriptionType),
					new KeyValuePair<string, object>("CT", creationType),
					new KeyValuePair<string, object>("ED", emailDomain),
					new KeyValuePair<string, object>("P", port),
					new KeyValuePair<string, object>("AT", authenticationType),
					new KeyValuePair<string, object>("ET", encryptionType),
					new KeyValuePair<string, object>("CTM", creationTime),
					new KeyValuePair<string, object>("AGT", aggregationType)
				});
			}
		}

		public void LogSubscriptionDeletion(string mailboxServerName, string tenantGuid, string userMailboxGuid, string subscriptionGuid, string subscriptionType, string incomingServerName, bool wasSubscriptionDeleted, string aggregationType)
		{
			base.CheckDisposed();
			SyncUtilities.ThrowIfArgumentNull("mailboxServerName", mailboxServerName);
			SyncUtilities.ThrowIfArgumentNull("tenantGuid", tenantGuid);
			SyncUtilities.ThrowIfArgumentNull("userMailboxGuid", userMailboxGuid);
			SyncUtilities.ThrowIfArgumentNull("subscriptionGuid", subscriptionGuid);
			SyncUtilities.ThrowIfArgumentNull("subscriptionType", subscriptionType);
			SyncUtilities.ThrowIfArgumentNull("incomingServerName", incomingServerName);
			SyncUtilities.ThrowIfArgumentNull("aggregationType", aggregationType);
			lock (this.syncRoot)
			{
				this.LogEvent(SyncHealthEventsStrings.SyncHealthEvents.SubscriptionDeletion, new KeyValuePair<string, object>[]
				{
					new KeyValuePair<string, object>("MSN", mailboxServerName),
					new KeyValuePair<string, object>("TG", tenantGuid),
					new KeyValuePair<string, object>("UMG", userMailboxGuid),
					new KeyValuePair<string, object>("SG", subscriptionGuid),
					new KeyValuePair<string, object>("ST", subscriptionType),
					new KeyValuePair<string, object>("ISN", incomingServerName),
					new KeyValuePair<string, object>("DEL", wasSubscriptionDeleted.ToString()),
					new KeyValuePair<string, object>("AGT", aggregationType)
				});
			}
		}

		public void LogDatabaseDiscovery(ExDateTime dbPollingTimeStamp, string dbPollingSource, int totalDBCount, int enabledDBCount, string databaseId, string databaseEvent, string currentDatabaseState)
		{
			base.CheckDisposed();
			SyncUtilities.ThrowIfArgumentNull("dbPollingTimeStamp", dbPollingTimeStamp);
			SyncUtilities.ThrowIfArgumentNull("dbPollingSource", dbPollingSource);
			SyncUtilities.ThrowIfArgumentNull("databaseId", databaseId);
			SyncUtilities.ThrowIfArgumentNull("databaseEvent", databaseEvent);
			SyncUtilities.ThrowIfArgumentNull("currentDatabaseState", currentDatabaseState);
			lock (this.syncRoot)
			{
				this.LogEvent(SyncHealthEventsStrings.SyncHealthEvents.DatabaseDiscovery, new KeyValuePair<string, object>[]
				{
					new KeyValuePair<string, object>("PT", dbPollingTimeStamp.ToString()),
					new KeyValuePair<string, object>("PS", dbPollingSource),
					new KeyValuePair<string, object>("TC", totalDBCount),
					new KeyValuePair<string, object>("EC", enabledDBCount),
					new KeyValuePair<string, object>("ID", databaseId),
					new KeyValuePair<string, object>("EVT", databaseEvent),
					new KeyValuePair<string, object>("CS", currentDatabaseState)
				});
			}
		}

		public void LogWorkTypeBudgets(KeyValuePair<string, object>[] eventData)
		{
			base.CheckDisposed();
			lock (this.syncRoot)
			{
				this.LogEvent(SyncHealthEventsStrings.SyncHealthEvents.WorkTypeBudgets, eventData);
			}
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				this.Close();
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<SyncHealthLog>(this);
		}

		private void LogEvent(SyncHealthEventsStrings.SyncHealthEvents eventId, params KeyValuePair<string, object>[] eventData)
		{
			if (this.enabled)
			{
				LogRowFormatter logRowFormatter = new LogRowFormatter(SyncHealthLog.SyncHealthSchema);
				logRowFormatter[1] = SyncHealthEventsStrings.StringMap[eventId];
				logRowFormatter[2] = eventData;
				this.logger.Append(logRowFormatter, 0);
			}
		}

		private string ConvertExceptionListToString(List<Exception> exceptions)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (exceptions != null && exceptions.Count != 0)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>();
				foreach (Exception ex in exceptions)
				{
					SyncPermanentException ex2 = ex as SyncPermanentException;
					string name = ex.GetType().Name;
					string arg = (ex.InnerException != null) ? ex.InnerException.GetType().Name : "NoInnerException";
					bool flag = ex2 != null && ex2.IsPromotedFromTransientException;
					string key = string.Format("{0}_{1}_{2}", name, arg, flag);
					if (dictionary.ContainsKey(key))
					{
						int num = dictionary[key];
						dictionary[key] = num + 1;
					}
					else
					{
						dictionary.Add(key, 1);
					}
				}
				if (dictionary.Count != 0)
				{
					int num = 0;
					foreach (KeyValuePair<string, int> keyValuePair in dictionary)
					{
						if (num == 0)
						{
							stringBuilder.Append('{');
						}
						stringBuilder.AppendFormat("{0}_{1}", keyValuePair.Key, keyValuePair.Value.ToString());
						num++;
						if (num != dictionary.Count)
						{
							stringBuilder.Append('|');
						}
						else
						{
							stringBuilder.Append('}');
						}
					}
				}
			}
			return stringBuilder.ToString();
		}

		private const int NumberOfBytesPerKB = 1024;

		private static LogSchema syncHealthSchema = new LogSchema("M E Transport Sync", Assembly.GetExecutingAssembly().GetName().Version.ToString(), "Sync Health Log", new string[]
		{
			"TimeStamp",
			"EventId",
			"EventData"
		});

		private Log logger = new Log("SyncHealthLog", new LogHeaderFormatter(SyncHealthLog.SyncHealthSchema), "SyncHealthLogs");

		private object syncRoot = new object();

		private bool enabled;
	}
}
