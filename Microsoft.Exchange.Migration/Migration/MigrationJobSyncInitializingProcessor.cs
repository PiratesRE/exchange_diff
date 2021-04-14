using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.Migration.Logging;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class MigrationJobSyncInitializingProcessor : JobSyncInitializingProcessor
	{
		protected abstract bool IgnorePostCompleteSubmits { get; }

		protected virtual MigrationUserStatus DetermineInitialStatus()
		{
			if (this.IsValidationSupported && this.IsValidationEnabled)
			{
				return MigrationUserStatus.Validating;
			}
			if (this.IsProvisioningSupported)
			{
				return MigrationUserStatus.Provisioning;
			}
			return MigrationUserStatus.Queued;
		}

		protected void CreateNewJobItem(IMigrationDataRow dataRow, MailboxData mailboxData, MigrationUserStatus status)
		{
			string identifier = dataRow.Identifier;
			MigrationLogger.Log(MigrationEventType.Verbose, "Job {0} creating job item with IDentifier {1}", new object[]
			{
				base.Job,
				identifier
			});
			MigrationJobItem.Create(base.DataProvider, base.Job, status, dataRow, mailboxData);
		}

		protected override MigrationBatchError ProcessExistingJobItem(MigrationJobItem jobItem, IMigrationDataRow dataRow)
		{
			string identifier = dataRow.Identifier;
			if (jobItem.RecipientType != dataRow.RecipientType)
			{
				MigrationLogger.Log(MigrationEventType.Error, "Process Existing JobItem | Job ({0}) | JobItem ({1}) | JobItem.RecipientType ({2}) != DataRow.RecipientType ({3}) | Creating validation error.", new object[]
				{
					base.Job,
					jobItem,
					jobItem.RecipientType,
					dataRow.RecipientType
				});
				LocalizedString locErrorString = ServerStrings.MigrationJobItemRecipientTypeMismatch(identifier, dataRow.RecipientType.ToString(), jobItem.RecipientType.ToString());
				MigrationBatchError validationError = this.GetValidationError(dataRow, locErrorString);
				validationError.RowIndex = dataRow.CursorPosition;
				return validationError;
			}
			MigrationLogger.Log(MigrationEventType.Verbose, "Process Existing JobItem | Job ({0}) | JobItem ({1}) | RecipientType ({2}) | Updating existing JobItem.", new object[]
			{
				base.Job,
				jobItem,
				jobItem.RecipientType
			});
			if (jobItem.UpdateDataRow(base.DataProvider, base.Job, dataRow))
			{
				this.UpdatesEncountered++;
				if (Array.Exists<MigrationUserStatus>(MigrationJobSyncInitializingProcessor.ForcedUpdateStatusJobItemStatusArray, (MigrationUserStatus status) => jobItem.Status == status))
				{
					jobItem.UpdateAndEnableJobItem(base.DataProvider, base.Job, this.DetermineInitialStatus());
				}
			}
			return null;
		}

		protected override LegacyMigrationJobProcessorResponse ResumeJobItems(bool scheduleNewWork)
		{
			LegacyMigrationJobProcessorResponse legacyMigrationJobProcessorResponse = LegacyMigrationJobProcessorResponse.Create(MigrationProcessorResult.Completed, null);
			legacyMigrationJobProcessorResponse.NumItemsOutstanding = new int?(0);
			MigrationUserStatus[] unforcedUpdateStatusJobItemStatusArray = MigrationJobSyncInitializingProcessor.UnforcedUpdateStatusJobItemStatusArray;
			JobItemOperationResult jobItemOperationResult = base.FindAndRunJobItemOperation(unforcedUpdateStatusJobItemStatusArray, MigrationUserStatus.Failed, ConfigBase<MigrationServiceConfigSchema>.GetConfig<int>("SyncMigrationPollingBatchSize"), (MigrationUserStatus status, int itemCount) => MigrationJobItem.GetByStatusAndRestartTime(base.DataProvider, base.Job, status, base.Job.LastRestartTime, new int?(itemCount)), delegate(MigrationJobItem item)
			{
				this.UpdateRunningJobItem(item);
				return true;
			});
			legacyMigrationJobProcessorResponse.NumItemsProcessed = new int?(jobItemOperationResult.NumItemsProcessed);
			legacyMigrationJobProcessorResponse.NumItemsTransitioned = new int?(jobItemOperationResult.NumItemsTransitioned);
			legacyMigrationJobProcessorResponse.NumItemsOutstanding = new int?(base.GetJobItemCount(base.Job.LastRestartTime, unforcedUpdateStatusJobItemStatusArray));
			if (legacyMigrationJobProcessorResponse.NumItemsProcessed > 0 || legacyMigrationJobProcessorResponse.NumItemsOutstanding > 0)
			{
				legacyMigrationJobProcessorResponse.Result = MigrationProcessorResult.Working;
			}
			return legacyMigrationJobProcessorResponse;
		}

		protected void CreateFailedJobItem(IMigrationDataRow dataRow, LocalizedException localizedError)
		{
			MigrationLogger.Log(MigrationEventType.Verbose, "Job {0} creating failed job item with email {1}", new object[]
			{
				base.Job,
				dataRow.Identifier
			});
			MigrationJobItem.CreateFailed(base.DataProvider, base.Job, dataRow, localizedError, null, null);
		}

		protected MailboxData GetMailboxData(string identifier, out LocalizedException error)
		{
			error = null;
			try
			{
				return this.InternalGetMailboxData(identifier);
			}
			catch (MigrationRecipientNotFoundException ex)
			{
				MigrationLogger.Log(MigrationEventType.Warning, ex, "Mailbox not found for {0}", new object[]
				{
					identifier
				});
				error = ex;
			}
			catch (LocalizedException ex2)
			{
				MigrationLogger.Log(MigrationEventType.Warning, ex2, "MailboxData for user '{0}' couldn't be created.", new object[]
				{
					identifier
				});
				error = ex2;
			}
			return null;
		}

		protected virtual MailboxData InternalGetMailboxData(string userEmail)
		{
			return base.DataProvider.ADProvider.GetMailboxDataFromSmtpAddress(userEmail, false, true);
		}

		protected void UpdateRunningJobItem(MigrationJobItem jobItem)
		{
			MigrationUserStatus newStatus = MigrationUserStatus.Queued;
			if (this.IsProvisioningSupported && jobItem.Status == MigrationUserStatus.Failed)
			{
				newStatus = MigrationUserStatus.Provisioning;
			}
			else
			{
				LocalizedException ex;
				MailboxData mailboxData = this.GetMailboxData(jobItem.Identifier, out ex);
				if (mailboxData == null)
				{
					if (ex == null)
					{
						ex = new MigrationUnknownException();
					}
					base.Job.ReportData.Append(Strings.MigrationReportJobItemFailed(jobItem.Identifier, ex.LocalizedString), ex, ReportEntryFlags.Failure | ReportEntryFlags.Fatal | ReportEntryFlags.Target);
					jobItem.SetFailedStatus(base.DataProvider, MigrationUserStatus.Failed, ex, "couldn't get mailbox data", true);
					return;
				}
				jobItem.SetUserMailboxProperties(base.DataProvider, null, mailboxData, null, null);
			}
			if (this.VerifyRequiredProperties(jobItem))
			{
				base.Job.ReportData.Append(Strings.MigrationReportJobItemRetried(jobItem.Identifier));
				jobItem.UpdateAndEnableJobItem(base.DataProvider, base.Job, newStatus);
			}
		}

		protected virtual bool VerifyRequiredProperties(MigrationJobItem jobItem)
		{
			return true;
		}

		protected static readonly MigrationUserStatus[] UnforcedUpdateStatusJobItemStatusArray = new MigrationUserStatus[]
		{
			MigrationUserStatus.Queued,
			MigrationUserStatus.Stopped,
			MigrationUserStatus.IncrementalStopped,
			MigrationUserStatus.Failed,
			MigrationUserStatus.Syncing,
			MigrationUserStatus.IncrementalFailed,
			MigrationUserStatus.CompletionFailed
		};

		protected static readonly MigrationUserStatus[] ForcedUpdateStatusJobItemStatusArray = new MigrationUserStatus[]
		{
			MigrationUserStatus.Queued,
			MigrationUserStatus.Stopped,
			MigrationUserStatus.IncrementalStopped,
			MigrationUserStatus.Failed,
			MigrationUserStatus.Syncing,
			MigrationUserStatus.IncrementalFailed,
			MigrationUserStatus.Synced,
			MigrationUserStatus.IncrementalSyncing,
			MigrationUserStatus.Completing,
			MigrationUserStatus.CompletionSynced,
			MigrationUserStatus.Completed,
			MigrationUserStatus.CompletionFailed,
			MigrationUserStatus.CompletedWithWarnings
		};
	}
}
