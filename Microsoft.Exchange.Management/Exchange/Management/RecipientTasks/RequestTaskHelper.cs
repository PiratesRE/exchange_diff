using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Net;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.EventMessages;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Provisioning.LoadBalancing;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	internal static class RequestTaskHelper
	{
		public static void WriteReportEntries(string request, List<ReportEntry> entries, object target, Task.TaskVerboseLoggingDelegate writeVerbose, Task.TaskWarningLoggingDelegate writeWarning, Task.TaskErrorLoggingDelegate writeError)
		{
			if (entries != null)
			{
				foreach (ReportEntry reportEntry in entries)
				{
					if (reportEntry.Type == ReportEntryType.Error)
					{
						writeError(new InvalidRequestPermanentException(request, reportEntry.Message), ErrorCategory.InvalidOperation, target);
					}
					else if (reportEntry.Type == ReportEntryType.Warning || reportEntry.Type == ReportEntryType.WarningCondition)
					{
						writeWarning(reportEntry.Message);
					}
					else
					{
						writeVerbose(reportEntry.Message);
					}
				}
			}
		}

		public static void TickleMRS(TransactionalRequestJob requestJob, MoveRequestNotification notification, Guid mdbGuid, ITopologyConfigurationSession configSession, List<string> unreachableServers)
		{
			using (MailboxReplicationServiceClient mailboxReplicationServiceClient = requestJob.CreateMRSClient(configSession, mdbGuid, unreachableServers))
			{
				if (notification == MoveRequestNotification.Canceled && mailboxReplicationServiceClient.ServerVersion[3])
				{
					mailboxReplicationServiceClient.RefreshMoveRequest2(requestJob.RequestGuid, mdbGuid, (int)requestJob.Flags, notification);
				}
				else
				{
					mailboxReplicationServiceClient.RefreshMoveRequest(requestJob.RequestGuid, mdbGuid, notification);
				}
			}
		}

		public static bool IsKnownExceptionHandler(Exception exception, Task.TaskVerboseLoggingDelegate writeVerbose)
		{
			if (exception is MapiRetryableException || exception is MapiPermanentException)
			{
				return true;
			}
			if (exception is MailboxReplicationPermanentException || exception is MailboxReplicationTransientException || exception is ConfigurationSettingsException)
			{
				writeVerbose(CommonUtils.FullExceptionMessage(exception));
				return true;
			}
			return false;
		}

		public static bool CheckUserOrgIdIsTenant(OrganizationId userOrgId)
		{
			return !userOrgId.Equals(OrganizationId.ForestWideOrgId);
		}

		public static MailboxDatabase ChooseTargetMDB(IEnumerable<ADObjectId> excludedDatabaseIds, bool checkInitialProvisioningSetting, ADUser adUser, Fqdn domainController, ScopeSet scopeSet, Action<LocalizedString> writeVerbose, Action<LocalizedException, ExchangeErrorCategory, object> writeExchangeError, Action<Exception, ErrorCategory, object> writeError, object identity)
		{
			MailboxProvisioningConstraint mailboxProvisioningConstraint = (adUser == null) ? new MailboxProvisioningConstraint() : adUser.MailboxProvisioningConstraint;
			LoadBalancingReport loadBalancingReport = new LoadBalancingReport();
			MailboxDatabaseWithLocationInfo mailboxDatabaseWithLocationInfo = PhysicalResourceLoadBalancing.FindDatabaseAndLocation(domainController, delegate(string msg)
			{
				writeVerbose(new LocalizedString(msg));
			}, scopeSet, checkInitialProvisioningSetting, false, new int?(Server.E15MinVersion), mailboxProvisioningConstraint, excludedDatabaseIds, ref loadBalancingReport);
			if (mailboxDatabaseWithLocationInfo == null)
			{
				ExManagementApplicationLogger.LogEvent(ManagementEventLogConstants.Tuple_LoadBalancingFailedToFindDatabase, new string[]
				{
					domainController,
					loadBalancingReport.ToString()
				});
				writeExchangeError(new RecipientTaskException(Strings.ErrorAutomaticProvisioningFailedToFindDatabase("TargetDatabase")), ExchangeErrorCategory.ServerOperation, null);
			}
			return mailboxDatabaseWithLocationInfo.MailboxDatabase;
		}

		public static ITopologyConfigurationSession GetConfigSessionForDatabase(ITopologyConfigurationSession originalConfigSession, ADObjectId database)
		{
			ITopologyConfigurationSession result = originalConfigSession;
			if (database != null && ConfigBase<MRSConfigSchema>.GetConfig<bool>("CrossResourceForestEnabled"))
			{
				PartitionId partitionId = database.GetPartitionId();
				if (!partitionId.IsLocalForestPartition())
				{
					ADSessionSettings sessionSettings = ADSessionSettings.FromAccountPartitionRootOrgScopeSet(partitionId);
					result = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(null, true, ConsistencyMode.PartiallyConsistent, null, sessionSettings, 612, "GetConfigSessionForDatabase", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\MailboxReplication\\RequestBase\\RequestTaskHelper.cs");
				}
			}
			return result;
		}

		public static IConfigurationSession CreateOrganizationFindingSession(OrganizationId currentOrgId, OrganizationId executingUserOrgId)
		{
			ADObjectId rootOrgContainerId = ADSystemConfigurationSession.GetRootOrgContainerId(null, null);
			ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(rootOrgContainerId, currentOrgId, executingUserOrgId, true);
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(null, true, ConsistencyMode.PartiallyConsistent, null, sessionSettings, ConfigScopes.TenantSubTree, 643, "CreateOrganizationFindingSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\MailboxReplication\\RequestBase\\RequestTaskHelper.cs");
			tenantOrTopologyConfigurationSession.UseConfigNC = false;
			return tenantOrTopologyConfigurationSession;
		}

		public static ADUser ResolveADUser(IRecipientSession dataSession, IRecipientSession globalCatalogSession, ADServerSettings serverSettings, IIdentityParameter identity, OptionalIdentityData optionalData, string domainController, DataAccessHelper.CategorizedGetDataObjectDelegate getDataObjectHandler, Task.TaskVerboseLoggingDelegate logHandler, Task.ErrorLoggerDelegate errorHandler, bool checkScopes)
		{
			ADUser aduser = (ADUser)RecipientTaskHelper.ResolveDataObject<ADUser>(dataSession, globalCatalogSession, serverSettings, identity, null, optionalData, domainController, getDataObjectHandler, logHandler, errorHandler);
			ADScopeException exception;
			if (checkScopes && !dataSession.TryVerifyIsWithinScopes(aduser, true, out exception))
			{
				errorHandler(exception, (ExchangeErrorCategory)5, identity);
			}
			return aduser;
		}

		public static void ValidateStartAfterTime(DateTime startAfterUtc, Task.TaskErrorLoggingDelegate writeError, DateTime utcNow)
		{
			if (utcNow.AddDays(30.0) < startAfterUtc)
			{
				writeError(new MoveStartAfterDateRangeException(30), ErrorCategory.InvalidArgument, startAfterUtc);
			}
		}

		public static void ValidateCompleteAfterTime(DateTime completeAfterUtc, Task.TaskErrorLoggingDelegate writeError, DateTime utcNow)
		{
			if (utcNow.AddDays(120.0) < completeAfterUtc)
			{
				writeError(new MoveCompleteAfterDateRangeException(120), ErrorCategory.InvalidArgument, completeAfterUtc);
			}
		}

		public static void ValidateStartAfterComesBeforeCompleteAfter(DateTime? startAfterUtc, DateTime? completeAfterUtc, Task.TaskErrorLoggingDelegate writeError)
		{
			if (startAfterUtc != null && completeAfterUtc != null && startAfterUtc.Value > completeAfterUtc.Value)
			{
				writeError(new MoveStartAfterEarlierThanCompleteAfterException(), ErrorCategory.InvalidArgument, startAfterUtc);
			}
		}

		public static void ValidateStartAfterCompleteAfterWithSuspendWhenReadyToComplete(DateTime? startAfter, DateTime? completeAfter, bool suspendWhenReadyToComplete, Task.TaskErrorLoggingDelegate writeError)
		{
			if (suspendWhenReadyToComplete && (startAfter != null || completeAfter != null))
			{
				writeError(new SuspendWhenReadyToCompleteCannotBeSetWithStartAfterOrCompleteAfterException(), ErrorCategory.InvalidArgument, suspendWhenReadyToComplete);
			}
		}

		public static void ValidateIncrementalSyncInterval(TimeSpan incrementalSyncInterval, Task.TaskErrorLoggingDelegate writeError)
		{
			if (incrementalSyncInterval < TimeSpan.Zero || incrementalSyncInterval > TimeSpan.FromDays(120.0))
			{
				writeError(new IncrementalSyncIntervalRangeException(0, 120), ErrorCategory.InvalidArgument, incrementalSyncInterval);
			}
		}

		public static bool CompareUtcTimeWithLocalTime(DateTime? utcTime, DateTime? localTime)
		{
			return (utcTime == null && localTime == null) || (utcTime != null && localTime != null && utcTime.Value == localTime.Value.ToUniversalTime());
		}

		public static void ValidateItemLimits(Unlimited<int> badItemLimit, Unlimited<int> largeItemLimit, SwitchParameter acceptLargeDataLoss, Task.TaskErrorLoggingDelegate writeError, Task.TaskWarningLoggingDelegate writeWarning, string executingUserIdentity)
		{
			Unlimited<int> value = new Unlimited<int>(TestIntegration.Instance.LargeDataLossThreshold);
			PropertyValidationError propertyValidationError = RequestJobSchema.BadItemLimit.ValidateValue(badItemLimit, false);
			if (propertyValidationError != null)
			{
				writeError(new DataValidationException(propertyValidationError), ErrorCategory.InvalidArgument, badItemLimit);
			}
			propertyValidationError = RequestJobSchema.LargeItemLimit.ValidateValue(largeItemLimit, false);
			if (propertyValidationError != null)
			{
				writeError(new DataValidationException(propertyValidationError), ErrorCategory.InvalidArgument, largeItemLimit);
			}
			if (largeItemLimit > value && !acceptLargeDataLoss)
			{
				writeError(new LargeDataLossNotAcceptedPermanentException("LargeItemLimit", largeItemLimit.ToString(), "AcceptLargeDataLoss", executingUserIdentity), ErrorCategory.InvalidArgument, acceptLargeDataLoss);
			}
			if (badItemLimit == RequestTaskHelper.UnlimitedZero && largeItemLimit == RequestTaskHelper.UnlimitedZero && acceptLargeDataLoss)
			{
				writeError(new RecipientTaskException(Strings.ErrorParameterValueNotAllowed("AcceptLargeDataLoss")), ErrorCategory.InvalidArgument, acceptLargeDataLoss);
			}
			if (badItemLimit > RequestTaskHelper.UnlimitedZero)
			{
				writeWarning(Strings.WarningNonZeroItemLimitMove("BadItemLimit"));
			}
			if (largeItemLimit > RequestTaskHelper.UnlimitedZero)
			{
				writeWarning(Strings.WarningNonZeroItemLimitMove("LargeItemLimit"));
			}
		}

		public static void ValidatePrimaryOnlyMoveArchiveDatabase(ADUser user, Action<Exception, ErrorCategory> writeError)
		{
			if (user.HasLocalArchive)
			{
				if (user.ArchiveDatabaseRaw == null)
				{
					writeError(new ArchiveDatabaseNotStampedPermanentException(), ErrorCategory.InvalidArgument);
				}
				if (!ADObjectId.Equals(user.ArchiveDatabase, user.ArchiveDatabaseRaw))
				{
					string archiveDb = (user.ArchiveDatabase != null) ? user.ArchiveDatabase.ToString() : "null";
					string archiveDbRaw = (user.ArchiveDatabaseRaw != null) ? user.ArchiveDatabaseRaw.ToString() : "null";
					writeError(new ArchiveDatabaseDifferentFromRawValuePermanentException(archiveDb, archiveDbRaw), ErrorCategory.InvalidArgument);
				}
			}
		}

		public static void ValidateNotImplicitSplit(RequestFlags moveFlags, ADUser sourceUser, Task.TaskErrorLoggingDelegate writeError, object errorTarget)
		{
			if (CommonUtils.IsImplicitSplit(moveFlags, sourceUser))
			{
				writeError(new ImplicitSplitPermanentException(), ErrorCategory.InvalidArgument, errorTarget);
			}
		}

		public static NetworkCredential GetNetworkCredential(PSCredential psCred, AuthenticationMethod? authMethod)
		{
			return CommonUtils.GetNetworkCredential(psCred, authMethod);
		}

		public static void SetSkipMoving(SkippableMoveComponent[] skipMoving, RequestJobBase moveRequest, Task.TaskErrorLoggingDelegate writeError, bool computeCalculatedDefaultsFromVersions = true)
		{
			moveRequest.SkipFolderACLs = false;
			moveRequest.SkipFolderRules = false;
			moveRequest.SkipFolderPromotedProperties = false;
			moveRequest.SkipFolderViews = false;
			moveRequest.SkipFolderRestrictions = false;
			moveRequest.SkipContentVerification = false;
			moveRequest.BlockFinalization = false;
			moveRequest.FailOnFirstBadItem = false;
			moveRequest.SkipKnownCorruptions = false;
			moveRequest.FailOnCorruptSyncState = false;
			if (computeCalculatedDefaultsFromVersions)
			{
				RequestTaskHelper.SetCalculatedSkipMovingDefaults(moveRequest);
			}
			if (skipMoving == null)
			{
				return;
			}
			int i = 0;
			while (i < skipMoving.Length)
			{
				SkippableMoveComponent skippableMoveComponent = skipMoving[i];
				switch (skippableMoveComponent)
				{
				case SkippableMoveComponent.FolderRules:
					moveRequest.SkipFolderRules = true;
					break;
				case SkippableMoveComponent.FolderACLs:
					moveRequest.SkipFolderACLs = true;
					break;
				case SkippableMoveComponent.FolderPromotedProperties:
					moveRequest.SkipFolderPromotedProperties = true;
					break;
				case SkippableMoveComponent.FolderViews:
					moveRequest.SkipFolderViews = true;
					break;
				case SkippableMoveComponent.FolderRestrictions:
					moveRequest.SkipFolderRestrictions = true;
					break;
				case SkippableMoveComponent.ContentVerification:
					moveRequest.SkipContentVerification = true;
					break;
				case SkippableMoveComponent.BlockFinalization:
					moveRequest.BlockFinalization = true;
					break;
				case SkippableMoveComponent.FailOnFirstBadItem:
					moveRequest.FailOnFirstBadItem = true;
					break;
				case (SkippableMoveComponent)8:
				case (SkippableMoveComponent)9:
				case (SkippableMoveComponent)10:
				case (SkippableMoveComponent)11:
				case (SkippableMoveComponent)13:
					goto IL_100;
				case SkippableMoveComponent.KnownCorruptions:
					moveRequest.SkipKnownCorruptions = true;
					break;
				case SkippableMoveComponent.FailOnCorruptSyncState:
					moveRequest.FailOnCorruptSyncState = true;
					break;
				default:
					goto IL_100;
				}
				IL_11D:
				i++;
				continue;
				IL_100:
				writeError(new ArgumentException(string.Format("Unknown value in SkipMoving parameter: {0}", skippableMoveComponent)), ErrorCategory.InvalidArgument, skipMoving);
				goto IL_11D;
			}
		}

		public static void SetSkipMerging(SkippableMergeComponent[] skipMerging, RequestJobBase dataObject, Task.TaskErrorLoggingDelegate writeError)
		{
			dataObject.SkipFolderACLs = false;
			dataObject.SkipFolderRules = false;
			dataObject.SkipInitialConnectionValidation = false;
			dataObject.FailOnFirstBadItem = false;
			dataObject.SkipContentVerification = false;
			dataObject.SkipKnownCorruptions = false;
			dataObject.FailOnCorruptSyncState = false;
			if (skipMerging == null)
			{
				return;
			}
			int i = 0;
			while (i < skipMerging.Length)
			{
				SkippableMergeComponent skippableMergeComponent = skipMerging[i];
				switch (skippableMergeComponent)
				{
				case SkippableMergeComponent.FolderRules:
					dataObject.SkipFolderRules = true;
					break;
				case SkippableMergeComponent.FolderACLs:
					dataObject.SkipFolderACLs = true;
					break;
				case SkippableMergeComponent.InitialConnectionValidation:
					dataObject.SkipInitialConnectionValidation = true;
					break;
				case (SkippableMergeComponent)3:
					goto IL_AB;
				case SkippableMergeComponent.FailOnFirstBadItem:
					dataObject.FailOnFirstBadItem = true;
					break;
				case SkippableMergeComponent.ContentVerification:
					dataObject.SkipContentVerification = true;
					break;
				case SkippableMergeComponent.KnownCorruptions:
					dataObject.SkipKnownCorruptions = true;
					break;
				case SkippableMergeComponent.FailOnCorruptSyncState:
					dataObject.FailOnCorruptSyncState = true;
					break;
				default:
					goto IL_AB;
				}
				IL_C8:
				i++;
				continue;
				IL_AB:
				writeError(new ArgumentException(string.Format("Unknown value in SkipMerging parameter: {0}", skippableMergeComponent)), ErrorCategory.InvalidArgument, skipMerging);
				goto IL_C8;
			}
		}

		public static void SetInternalFlags(InternalMrsFlag[] flags, RequestJobBase dataObject, Task.TaskErrorLoggingDelegate writeError)
		{
			dataObject.SkipConvertingSourceToMeu = false;
			dataObject.ResolveServer = false;
			dataObject.UseTcp = false;
			dataObject.CrossResourceForest = false;
			dataObject.SkipPreFinalSyncDataProcessing = false;
			dataObject.SkipWordBreaking = false;
			dataObject.SkipStorageProviderForSource = false;
			dataObject.SkipMailboxReleaseCheck = false;
			dataObject.SkipProvisioningCheck = false;
			dataObject.UseCertificateAuthentication = false;
			dataObject.InvalidateContentIndexAnnotations = false;
			if (flags == null)
			{
				return;
			}
			foreach (InternalMrsFlag internalMrsFlag in flags)
			{
				switch (internalMrsFlag)
				{
				case InternalMrsFlag.SkipPreFinalSyncDataProcessing:
					dataObject.SkipPreFinalSyncDataProcessing = true;
					break;
				case InternalMrsFlag.SkipWordBreaking:
					dataObject.SkipWordBreaking = true;
					break;
				case InternalMrsFlag.SkipStorageProviderForSource:
					dataObject.SkipStorageProviderForSource = true;
					break;
				case InternalMrsFlag.SkipMailboxReleaseCheck:
					dataObject.SkipMailboxReleaseCheck = true;
					break;
				case InternalMrsFlag.SkipProvisioningCheck:
					dataObject.SkipProvisioningCheck = true;
					break;
				case InternalMrsFlag.CrossResourceForest:
					dataObject.CrossResourceForest = true;
					break;
				case InternalMrsFlag.DoNotConvertSourceToMeu:
					dataObject.SkipConvertingSourceToMeu = true;
					break;
				case InternalMrsFlag.ResolveServer:
					dataObject.ResolveServer = true;
					break;
				case InternalMrsFlag.UseTcp:
					dataObject.UseTcp = true;
					break;
				case InternalMrsFlag.UseCertificateAuthentication:
					dataObject.UseCertificateAuthentication = true;
					break;
				case InternalMrsFlag.InvalidateContentIndexAnnotations:
					dataObject.InvalidateContentIndexAnnotations = true;
					break;
				default:
					writeError(new ArgumentException(string.Format("Unknown value in InternalFlags parameter: {0}", internalMrsFlag)), ErrorCategory.InvalidArgument, flags);
					break;
				}
			}
		}

		public static void GetUpdatedMRSRequestInfo(RequestStatisticsBase requestJob, bool diagnostic, string diagnosticArgument)
		{
			MoveRequestInfo requestInfo = null;
			CommonUtils.CatchKnownExceptions(delegate
			{
				string mrsServer = MailboxReplicationServiceClient.GetMrsServer(requestJob.WorkItemQueueMdb.ObjectGuid);
				using (MailboxReplicationServiceClient mailboxReplicationServiceClient = MailboxReplicationServiceClient.Create(mrsServer))
				{
					requestInfo = mailboxReplicationServiceClient.GetMoveRequestInfo(requestJob.IdentifyingGuid);
					requestJob.UpdateThroughputFromMoveRequestInfo(requestInfo);
					if (RequestTaskHelper.NeedToUpdateJobPickupMessage())
					{
						requestJob.UpdateMessageFromMoveRequestInfo(requestInfo);
					}
					if (diagnostic)
					{
						string jobPickupFailureMessage = (requestInfo == null) ? string.Empty : requestInfo.Message.ToString();
						requestJob.PopulateDiagnosticInfo(new RequestStatisticsDiagnosticArgument(diagnosticArgument), jobPickupFailureMessage);
					}
				}
			}, null);
		}

		private static bool NeedToUpdateJobPickupMessage()
		{
			return ConfigBase<MRSConfigSchema>.GetConfig<bool>("ShowJobPickupStatusInRequestStatisticsMessage");
		}

		public static void SetStartAfter(DateTime? startAfter, RequestJobBase dataObject, StringBuilder changedValuesTracker = null)
		{
			DateTime? value = null;
			if (startAfter != null)
			{
				value = new DateTime?(startAfter.Value.ToUniversalTime());
			}
			string arg = (value == null) ? "(null)" : value.ToString();
			RequestTaskHelper.TrackerAppendLine(changedValuesTracker, string.Format("TimeTracker.StartAfter: {0} -> {1}", dataObject.TimeTracker.GetTimestamp(RequestJobTimestamp.StartAfter), arg));
			dataObject.TimeTracker.SetTimestamp(RequestJobTimestamp.StartAfter, value);
			DateTime? timestamp = dataObject.TimeTracker.GetTimestamp(RequestJobTimestamp.DoNotPickUntil);
			RequestTaskHelper.TrackerAppendLine(changedValuesTracker, string.Format("TimeTracker.DoNotPickUntilTimeStamp: {0} -> {1}", timestamp, arg));
			dataObject.TimeTracker.SetTimestamp(RequestJobTimestamp.DoNotPickUntil, value);
		}

		public static void SetCompleteAfter(DateTime? completeAfter, RequestJobBase dataObject, StringBuilder changedValuesTracker = null)
		{
			DateTime? value = null;
			if (completeAfter != null)
			{
				value = new DateTime?(completeAfter.Value.ToUniversalTime());
			}
			string arg = (value == null) ? "(null)" : value.ToString();
			RequestTaskHelper.TrackerAppendLine(changedValuesTracker, string.Format("TimeTracker.CompleteAfter: {0} -> {1}", dataObject.TimeTracker.GetTimestamp(RequestJobTimestamp.CompleteAfter), arg));
			dataObject.TimeTracker.SetTimestamp(RequestJobTimestamp.CompleteAfter, value);
			if (dataObject.Status == RequestStatus.Synced)
			{
				DateTime? timestamp = dataObject.TimeTracker.GetTimestamp(RequestJobTimestamp.DoNotPickUntil);
				TimeSpan incrementalSyncInterval = dataObject.IncrementalSyncInterval;
				if (incrementalSyncInterval == TimeSpan.Zero || (value != null && value.Value < timestamp))
				{
					RequestTaskHelper.TrackerAppendLine(changedValuesTracker, string.Format("TimeTracker.DoNotPickUntilTimeStamp: {0} -> {1}", timestamp, arg));
					dataObject.TimeTracker.SetTimestamp(RequestJobTimestamp.DoNotPickUntil, value);
				}
			}
		}

		internal static LocalizedException TranslateExceptionHandler(Exception e)
		{
			if (e is LocalizedException)
			{
				if (!(e is MapiRetryableException))
				{
					if (!(e is MapiPermanentException))
					{
						goto IL_42;
					}
				}
				try
				{
					LocalizedException ex = StorageGlobals.TranslateMapiException(Strings.UnableToCommunicate, (LocalizedException)e, null, null, string.Empty, new object[0]);
					if (ex != null)
					{
						return ex;
					}
				}
				catch (ArgumentException)
				{
				}
			}
			IL_42:
			return null;
		}

		internal static void ApplyOrganization(TransactionalRequestJob dataObject, OrganizationId organizationId)
		{
			dataObject.OrganizationId = organizationId;
			if (organizationId != OrganizationId.ForestWideOrgId)
			{
				dataObject.PartitionHint = TenantPartitionHint.FromOrganizationId(organizationId);
				dataObject.ExternalDirectoryOrganizationId = dataObject.PartitionHint.GetExternalDirectoryOrganizationId();
			}
		}

		private static void SetCalculatedSkipMovingDefaults(RequestJobBase moveRequest)
		{
			bool flag = false;
			bool flag2 = false;
			if (moveRequest.PrimaryIsMoving)
			{
				ServerVersion serverVersion = new ServerVersion(moveRequest.SourceVersion);
				ServerVersion serverVersion2 = new ServerVersion(moveRequest.TargetVersion);
				if (serverVersion.Major != serverVersion2.Major)
				{
					flag = true;
				}
				if (serverVersion2.Major < Server.Exchange2011MajorVersion)
				{
					flag2 = true;
				}
			}
			if (moveRequest.ArchiveIsMoving)
			{
				ServerVersion serverVersion = new ServerVersion(moveRequest.SourceArchiveVersion);
				ServerVersion serverVersion2 = new ServerVersion(moveRequest.TargetArchiveVersion);
				if (serverVersion.Major != serverVersion2.Major)
				{
					flag = true;
				}
				if (serverVersion2.Major < Server.Exchange2011MajorVersion)
				{
					flag2 = true;
				}
			}
			if (flag)
			{
				moveRequest.SkipFolderPromotedProperties = true;
			}
			if (flag2)
			{
				moveRequest.SkipFolderViews = true;
			}
		}

		private static void TrackerAppendLine(StringBuilder changedValuesTracker, string line)
		{
			if (changedValuesTracker != null)
			{
				changedValuesTracker.AppendLine(line);
			}
		}

		public const int MaxSuspendCommentLength = 4096;

		public const int MaxBatchNameLength = 255;

		public const int MaxNameLength = 255;

		public const int StartAfterMaxDaysFromNow = 30;

		public const int CompleteAfterMaxDaysFromNow = 120;

		public const string ParameterSourceStoreMailbox = "SourceStoreMailbox";

		public const string ParameterSourceDatabase = "SourceDatabase";

		public const string ParameterTargetMailbox = "TargetMailbox";

		public const string ParameterSourceRootFolder = "SourceRootFolder";

		public const string ParameterTargetRootFolder = "TargetRootFolder";

		public const string ParameterTargetIsArchive = "TargetIsArchive";

		public const string ParameterTargetDatabase = "TargetDatabase";

		public const string ParameterArchiveTargetDatabase = "ArchiveTargetDatabase";

		public const string ParameterPrimaryOnly = "PrimaryOnly";

		public const string ParameterArchiveOnly = "ArchiveOnly";

		public const string ParameterForcePull = "ForcePull";

		public const string ParameterForcePush = "ForcePush";

		public const string ParameterRemoteTargetDatabase = "RemoteTargetDatabase";

		public const string ParameterRemoteArchiveTargetDatabase = "RemoteArchiveTargetDatabase";

		public const string ParameterRemoteDatabaseGuid = "RemoteDatabaseGuid";

		public const string ParameterRemoteRestoreType = "RemoteRestoreType";

		public const string ParameterRemote = "Remote";

		public const string ParameterOutbound = "Outbound";

		public const string ParameterRemoteLegacy = "RemoteLegacy";

		public const string ParameterRemoteGlobalCatalog = "RemoteGlobalCatalog";

		public const string ParameterBadItemLimit = "BadItemLimit";

		public const string ParameterLargeItemLimit = "LargeItemLimit";

		public const string ParameterAllowLargeItems = "AllowLargeItems";

		public const string ParameterIgnoreTenantMigrationPolicies = "IgnoreTenantMigrationPolicies";

		public const string ParameterAcceptLargeDataLoss = "AcceptLargeDataLoss";

		public const string ParameterCheckInitialProvisioningSetting = "CheckInitialProvisioningSetting";

		public const string ParameterRemoteHostName = "RemoteHostName";

		public const string ParameterBatchName = "BatchName";

		public const string ParameterRemoteOrganizationName = "RemoteOrganizationName";

		public const string ParameterArchiveDomain = "ArchiveDomain";

		public const string ParameterRemoteCredential = "RemoteCredential";

		public const string ParameterProtect = "Protect";

		public const string ParameterIdentity = "Identity";

		public const string ParameterSuspendWhenReadyToComplete = "SuspendWhenReadyToComplete";

		public const string ParameterSuspend = "Suspend";

		public const string ParameterSuspendComment = "SuspendComment";

		public const string ParameterIgnoreRuleLimitErrors = "IgnoreRuleLimitErrors";

		public const string ParameterDoNotPreserveMailboxSignature = "DoNotPreserveMailboxSignature";

		public const string ParameterTargetDeliveryDomain = "TargetDeliveryDomain";

		public const string ParameterPriority = "Priority";

		public const string ParameterWorkloadType = "WorkloadType";

		public const string ParameterCompletedRequestAgeLimit = "CompletedRequestAgeLimit";

		public const string ParameterForceOffline = "ForceOffline";

		public const string ParameterPreventCompletion = "PreventCompletion";

		public const string ParameterSkipMoving = "SkipMoving";

		public const string ParameterInternalFlags = "InternalFlags";

		public const string ParameterStartAfter = "StartAfter";

		public const string ParameterCompleteAfter = "CompleteAfter";

		public const string ParameterIncrementalSyncInterval = "IncrementalSyncInterval";

		public const string ParameterAllowLegacyDNMismatch = "AllowLegacyDNMismatch";

		public const string ParameterName = "Name";

		public const string ParameterContentFilter = "ContentFilter";

		public const string ParameterContentFilterLanguage = "ContentFilterLanguage";

		public const string ParameterIncludeFolders = "IncludeFolders";

		public const string ParameterExcludeFolders = "ExcludeFolders";

		public const string ParameterExcludeDumpster = "ExcludeDumpster";

		public const string ParameterConflictResolutionOption = "ConflictResolutionOption";

		public const string ParameterAssociatedMessagesCopyOption = "AssociatedMessagesCopyOption";

		public const string ParameterSkipMerging = "SkipMerging";

		public const string TaskNoun = "MailboxImportRequest";

		public const string ParameterContentCodePage = "ContentCodePage";

		public const string ParameterMailbox = "Mailbox";

		public const string ParameterFilePath = "FilePath";

		public const string ParameterIsArchive = "IsArchive";

		public const string ParameterRehomeRequest = "RehomeRequest";

		public static readonly Unlimited<int> UnlimitedZero = new Unlimited<int>(0);

		public static readonly Unlimited<EnhancedTimeSpan> DefaultCompletedRequestAgeLimit = TestIntegration.Instance.GetCompletedRequestAgeLimit(TimeSpan.FromDays(30.0));
	}
}
