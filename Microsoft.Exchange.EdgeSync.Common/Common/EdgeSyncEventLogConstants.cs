using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.EdgeSync.Common
{
	public static class EdgeSyncEventLogConstants
	{
		public const string EventSource = "MSExchange EdgeSync";

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_EdgeSyncStarted = new ExEventLog.EventTuple(1074005027U, 4, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_EdgeSyncStopping = new ExEventLog.EventTuple(1074005028U, 4, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_EdgeSyncStopped = new ExEventLog.EventTuple(1074005029U, 4, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_Failure = new ExEventLog.EventTuple(3221488686U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ConfigurationFailureException = new ExEventLog.EventTuple(3221488687U, 4, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_EdgeLeaseException = new ExEventLog.EventTuple(3221488640U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FailedToReadEntriesFromAD = new ExEventLog.EventTuple(2147746805U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_InitializationFailureException = new ExEventLog.EventTuple(2147746837U, 4, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ProbeFailed = new ExEventLog.EventTuple(3221488620U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FailedDirectTrustMatch = new ExEventLog.EventTuple(3221497720U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_EdgeTopologyException = new ExEventLog.EventTuple(3221488641U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_NoCredentialsFound = new ExEventLog.EventTuple(2147746824U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CredentialDecryptionException = new ExEventLog.EventTuple(3221488649U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MservEntrySyncFailure = new ExEventLog.EventTuple(3221488650U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_CAPICertificateRequired = new ExEventLog.EventTuple(3221488651U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_EhfServiceContractViolation = new ExEventLog.EventTuple(3221488717U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_EhfOperationTimedOut = new ExEventLog.EventTuple(3221488718U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_EhfCommunicationFailure = new ExEventLog.EventTuple(3221488719U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_EhfEntrySyncFailure = new ExEventLog.EventTuple(3221488720U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_EhfFailedUpdateSyncErrors = new ExEventLog.EventTuple(3221488721U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_EhfPerEntryFailuresInBatch = new ExEventLog.EventTuple(3221488722U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_EhfTransientFailure = new ExEventLog.EventTuple(3221488723U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_EhfInvalidCredentials = new ExEventLog.EventTuple(3221488724U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_EhfPerfCountersLoadFailure = new ExEventLog.EventTuple(3221488725U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_EhfAdminSyncFailedToConnectToConfigNamingContext = new ExEventLog.EventTuple(3221488726U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_EhfWebServiceVersionIsNotSupported = new ExEventLog.EventTuple(3221488727U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_EhfFailedToClearForceDomainSyncFlagFromDomainSync = new ExEventLog.EventTuple(3221488728U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_EhfFailedToClearForceDomainSyncFlagFromCompanySync = new ExEventLog.EventTuple(3221488729U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_EhfAdminSyncTransientFailure = new ExEventLog.EventTuple(3221488730U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_EhfAdminSyncPermanentFailure = new ExEventLog.EventTuple(3221488731U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_EhfAdminSyncTransientFailureRetryThresholdReached = new ExEventLog.EventTuple(3221488732U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		private enum Category : short
		{
			Synchronization = 1,
			Topology,
			SyncNow,
			Initialization
		}

		internal enum Message : uint
		{
			EdgeSyncStarted = 1074005027U,
			EdgeSyncStopping,
			EdgeSyncStopped,
			Failure = 3221488686U,
			ConfigurationFailureException,
			EdgeLeaseException = 3221488640U,
			FailedToReadEntriesFromAD = 2147746805U,
			InitializationFailureException = 2147746837U,
			ProbeFailed = 3221488620U,
			FailedDirectTrustMatch = 3221497720U,
			EdgeTopologyException = 3221488641U,
			NoCredentialsFound = 2147746824U,
			CredentialDecryptionException = 3221488649U,
			MservEntrySyncFailure,
			CAPICertificateRequired,
			EhfServiceContractViolation = 3221488717U,
			EhfOperationTimedOut,
			EhfCommunicationFailure,
			EhfEntrySyncFailure,
			EhfFailedUpdateSyncErrors,
			EhfPerEntryFailuresInBatch,
			EhfTransientFailure,
			EhfInvalidCredentials,
			EhfPerfCountersLoadFailure,
			EhfAdminSyncFailedToConnectToConfigNamingContext,
			EhfWebServiceVersionIsNotSupported,
			EhfFailedToClearForceDomainSyncFlagFromDomainSync,
			EhfFailedToClearForceDomainSyncFlagFromCompanySync,
			EhfAdminSyncTransientFailure,
			EhfAdminSyncPermanentFailure,
			EhfAdminSyncTransientFailureRetryThresholdReached
		}
	}
}
