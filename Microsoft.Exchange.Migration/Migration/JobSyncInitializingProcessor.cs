using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Migration.Logging;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class JobSyncInitializingProcessor : JobProcessor
	{
		protected virtual bool IsProvisioningSupported
		{
			get
			{
				return base.Job.IsProvisioningSupported;
			}
		}

		protected virtual bool IsValidationSupported
		{
			get
			{
				return base.SubscriptionHandler.SupportsAdvancedValidation;
			}
		}

		protected virtual bool IsValidationEnabled
		{
			get
			{
				return base.Job.UseAdvancedValidation;
			}
		}

		protected virtual int UpdatesEncountered { get; set; }

		internal static JobSyncInitializingProcessor CreateProcessor(MigrationType type)
		{
			if (type <= MigrationType.ExchangeOutlookAnywhere)
			{
				if (type == MigrationType.IMAP)
				{
					return new IMAPJobSyncInitializingProcessor();
				}
				if (type == MigrationType.ExchangeOutlookAnywhere)
				{
					return new ExchangeJobSyncInitializingProcessor();
				}
			}
			else
			{
				if (type == MigrationType.ExchangeRemoteMove || type == MigrationType.ExchangeLocalMove)
				{
					return new MoveJobSyncInitializingProcessor();
				}
				if (type == MigrationType.PublicFolder)
				{
					return new PublicFolderJobSyncInitializingProcessor();
				}
			}
			throw new ArgumentException("Invalid MigrationType " + type);
		}

		internal override bool Validate()
		{
			return true;
		}

		internal override MigrationJobStatus GetNextStageStatus()
		{
			if (this.IsValidationSupported)
			{
				return MigrationJobStatus.Validating;
			}
			if (this.IsProvisioningSupported)
			{
				return MigrationJobStatus.ProvisionStarting;
			}
			return MigrationJobStatus.SyncStarting;
		}

		internal override void OnComplete()
		{
			if (base.Job.DisallowExistingUsers)
			{
				base.Job.DisallowExistingUsers = false;
				base.Job.SaveBatchFlagsAndNotificationId(base.DataProvider);
			}
		}

		protected sealed override LegacyMigrationJobProcessorResponse Process(bool scheduleNewWork)
		{
			this.AutoCancelIfTooManyErrors();
			return base.ProcessActions(scheduleNewWork, new Func<bool, LegacyMigrationJobProcessorResponse>[]
			{
				new Func<bool, LegacyMigrationJobProcessorResponse>(this.MoveReports),
				new Func<bool, LegacyMigrationJobProcessorResponse>(this.CreatePendingJobItems),
				new Func<bool, LegacyMigrationJobProcessorResponse>(this.ResumeJobItems),
				new Func<bool, LegacyMigrationJobProcessorResponse>(this.UpdateJobItems)
			});
		}

		protected abstract IMigrationDataRowProvider GetMigrationDataRowProvider();

		protected abstract void CreateNewJobItem(IMigrationDataRow dataRow);

		protected abstract MigrationBatchError ProcessExistingJobItem(MigrationJobItem jobItem, IMigrationDataRow dataRow);

		protected virtual MigrationBatchError HandleDuplicateMigrationDataRow(MigrationJobItem jobItem, IMigrationDataRow dataRow)
		{
			if (base.Job.BatchInputId == jobItem.BatchInputId && jobItem.CursorPosition == dataRow.CursorPosition)
			{
				MigrationLogger.Log(MigrationEventType.Information, "somehow we're reprocessing the same item {0}", new object[]
				{
					jobItem
				});
				return null;
			}
			LocalizedString locErrorString;
			if (string.Equals(jobItem.Identifier, dataRow.Identifier, StringComparison.OrdinalIgnoreCase))
			{
				if (jobItem.MigrationJobId != base.Job.JobId)
				{
					string text;
					try
					{
						text = base.Session.GetJobName(jobItem.MigrationJobId);
					}
					catch (MigrationJobNotFoundException)
					{
						text = null;
					}
					if (!string.IsNullOrEmpty(text))
					{
						locErrorString = Strings.UserDuplicateInOtherBatch(dataRow.Identifier, text);
					}
					else
					{
						locErrorString = Strings.UserDuplicateOrphanedFromBatch(dataRow.Identifier);
					}
				}
				else
				{
					locErrorString = Strings.UserDuplicateInCSV(dataRow.Identifier);
				}
			}
			else
			{
				locErrorString = Strings.UserAlreadyMigratedWithAlternateEmail(jobItem.Identifier);
			}
			return this.GetValidationError(dataRow, locErrorString);
		}

		protected virtual void HandleProvisioningCompletedEvent(ProvisionedObject provisionedObj, MigrationJobItem jobItem)
		{
			if (provisionedObj.Succeeded)
			{
				MigrationUserStatus value = MigrationUserStatus.Queued;
				jobItem.SetUserMailboxProperties(base.DataProvider, new MigrationUserStatus?(value), (MailboxData)provisionedObj.MailboxData, null, new ExDateTime?(ExDateTime.UtcNow));
				return;
			}
			jobItem.SetUserMailboxProperties(base.DataProvider, new MigrationUserStatus?(MigrationUserStatus.Failed), (MailboxData)provisionedObj.MailboxData, new ProvisioningFailedException(new LocalizedString(provisionedObj.Error)), null);
		}

		protected virtual MigrationBatchError GetValidationError(IMigrationDataRow dataRow, LocalizedString locErrorString)
		{
			return new MigrationBatchError
			{
				RowIndex = -1,
				EmailAddress = dataRow.Identifier,
				LocalizedErrorMessage = locErrorString
			};
		}

		protected virtual MigrationBatchError ValidateDataRow(IMigrationDataRow row)
		{
			InvalidDataRow invalidDataRow = row as InvalidDataRow;
			if (invalidDataRow != null)
			{
				return invalidDataRow.Error;
			}
			return null;
		}

		protected virtual LegacyMigrationJobProcessorResponse ResumeJobItems(bool scheduleNewWork)
		{
			return LegacyMigrationJobProcessorResponse.Create(MigrationProcessorResult.Completed, null);
		}

		protected virtual LegacyMigrationJobProcessorResponse UpdateJobItems(bool scheduleNewWork)
		{
			return LegacyMigrationJobProcessorResponse.Create(MigrationProcessorResult.Completed, null);
		}

		private LegacyMigrationJobProcessorResponse MoveReports(bool scheduleNewWork)
		{
			LegacyMigrationJobProcessorResponse legacyMigrationJobProcessorResponse = LegacyMigrationJobProcessorResponse.Create(MigrationProcessorResult.Completed, null);
			if (!base.Session.Config.IsSupported(MigrationFeature.MultiBatch))
			{
				MigrationLogger.Log(MigrationEventType.Verbose, "single-batch behavior.  do not move reports to new job", new object[0]);
				return legacyMigrationJobProcessorResponse;
			}
			if (base.Job.OriginalJobId == null || base.Job.JobId == base.Job.OriginalJobId.Value)
			{
				MigrationLogger.Log(MigrationEventType.Verbose, "batch ids match up, no need to move reports", new object[0]);
				return legacyMigrationJobProcessorResponse;
			}
			using (IMigrationDataProvider providerForFolder = base.DataProvider.GetProviderForFolder(MigrationFolderName.SyncMigrationReports))
			{
				foreach (MigrationReportItem migrationReportItem in MigrationReportItem.GetByJobId(providerForFolder, new Guid?(base.Job.OriginalJobId.Value), ConfigBase<MigrationServiceConfigSchema>.GetConfig<int>("SyncMigrationCancellationBatchSize")))
				{
					if (legacyMigrationJobProcessorResponse.Result == MigrationProcessorResult.Completed)
					{
						legacyMigrationJobProcessorResponse.Result = MigrationProcessorResult.Working;
					}
					MigrationLogger.Log(MigrationEventType.Information, "updating report {0} to point from {1} to {2}", new object[]
					{
						migrationReportItem.ReportName,
						migrationReportItem.JobId,
						base.Job.JobId
					});
					migrationReportItem.UpdateReportItem(providerForFolder, base.Job.JobId);
					legacyMigrationJobProcessorResponse.NumItemsProcessed++;
				}
			}
			return legacyMigrationJobProcessorResponse;
		}

		private LegacyMigrationJobProcessorResponse CreatePendingJobItems(bool scheduleNewWork)
		{
			LegacyMigrationJobProcessorResponse legacyMigrationJobProcessorResponse = LegacyMigrationJobProcessorResponse.Create(MigrationProcessorResult.Completed, null);
			legacyMigrationJobProcessorResponse.NumItemsProcessed = new int?(0);
			legacyMigrationJobProcessorResponse.NumItemsOutstanding = new int?(0);
			if (!base.Job.ShouldProcessDataRows || base.Job.IsDataRowProcessingDone())
			{
				MigrationLogger.Log(MigrationEventType.Verbose, "Job {0} no more data to read, so creation is finished", new object[]
				{
					base.Job
				});
				return legacyMigrationJobProcessorResponse;
			}
			this.UpdatesEncountered = 0;
			legacyMigrationJobProcessorResponse.Result = MigrationProcessorResult.Working;
			List<MigrationBatchError> list = new List<MigrationBatchError>();
			IMigrationDataRow migrationDataRow = null;
			try
			{
				int config = ConfigBase<MigrationServiceConfigSchema>.GetConfig<int>("MaxRowsToProcessInOnePass");
				ExDateTime t = ExDateTime.UtcNow + ConfigBase<MigrationServiceConfigSchema>.GetConfig<TimeSpan>("MaxTimeToProcessInOnePass");
				IMigrationDataRowProvider migrationDataRowProvider = this.GetMigrationDataRowProvider();
				foreach (IMigrationDataRow migrationDataRow2 in migrationDataRowProvider.GetNextBatchItem(base.Job.LastCursorPosition, config))
				{
					MigrationBatchError migrationBatchError = this.ProcessDataRow(migrationDataRow2);
					if (migrationBatchError != null)
					{
						list.Add(migrationBatchError);
					}
					migrationDataRow = migrationDataRow2;
					legacyMigrationJobProcessorResponse.NumItemsProcessed++;
					if (legacyMigrationJobProcessorResponse.NumItemsProcessed.Value >= config || ExDateTime.UtcNow >= t)
					{
						break;
					}
				}
				if (migrationDataRow == null)
				{
					base.Job.SetDataRowProcessingDone(base.DataProvider, list, this.UpdatesEncountered);
				}
			}
			finally
			{
				string text = (migrationDataRow == null) ? null : migrationDataRow.CursorPosition.ToString(CultureInfo.InvariantCulture);
				if (text != null || list.Count > 0)
				{
					base.Job.UpdateLastProcessedRow(base.DataProvider, text, list, this.UpdatesEncountered);
				}
				MigrationLogger.Log(MigrationEventType.Verbose, "JobInitiatedProcessor.Process: Job {0} processed {1} rows in this pass", new object[]
				{
					base.Job,
					legacyMigrationJobProcessorResponse.NumItemsProcessed.Value
				});
			}
			return legacyMigrationJobProcessorResponse;
		}

		private MigrationBatchError ProcessDataRow(IMigrationDataRow row)
		{
			MigrationBatchError migrationBatchError = this.ValidateDataRow(row);
			if (migrationBatchError != null)
			{
				return migrationBatchError;
			}
			MigrationJobItem jobItemByEmailAddress = MigrationServiceHelper.GetJobItemByEmailAddress(base.DataProvider, row.Identifier, true);
			if (jobItemByEmailAddress == null)
			{
				this.CreateNewJobItem(row);
				return null;
			}
			if (jobItemByEmailAddress.MigrationType != base.Job.MigrationType)
			{
				return this.GetValidationError(row, Strings.MigrationUserAlreadyExistsInDifferentType(jobItemByEmailAddress.JobName, jobItemByEmailAddress.MigrationType.ToString()));
			}
			if (jobItemByEmailAddress.MigrationJobId != base.Job.JobId && (base.Job.BatchFlags & MigrationBatchFlags.DisallowExistingUsers) == MigrationBatchFlags.DisallowExistingUsers)
			{
				return this.HandleDuplicateMigrationDataRow(jobItemByEmailAddress, row);
			}
			if (!string.IsNullOrEmpty(base.Job.BatchInputId) && base.Job.BatchInputId.Equals(jobItemByEmailAddress.BatchInputId))
			{
				return this.HandleDuplicateMigrationDataRow(jobItemByEmailAddress, row);
			}
			return this.ProcessExistingJobItem(jobItemByEmailAddress, row);
		}
	}
}
