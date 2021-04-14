using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Migration.Logging;

namespace Microsoft.Exchange.Migration
{
	internal abstract class JobSyncCompletingProcessor : JobProcessor
	{
		protected abstract bool IsBatchSyncedReport { get; }

		protected virtual string LicensingHelpUrl
		{
			get
			{
				if (base.DataProvider.ADProvider.IsLicensingEnforced)
				{
					return string.Format(CultureInfo.InvariantCulture, MigrationJobReportingCursor.MoacHelpUrlFormat, new object[]
					{
						base.Job.AdminCulture.LCID
					});
				}
				return null;
			}
		}

		protected virtual MigrationJobReportWriterDelegate ReportHeaderWriter
		{
			get
			{
				return delegate(MigrationJobReportingCursor cursor, StreamWriter successWriter, StreamWriter failureWriter)
				{
					MigrationSuccessReportCsvSchema.WriteHeader(successWriter, base.Job.MigrationType, this.IsBatchSyncedReport, base.Job.IsStaged);
					MigrationFailureReportCsvSchema.WriteHeader(failureWriter, base.Job.MigrationType, this.IsBatchSyncedReport);
					return null;
				};
			}
		}

		protected virtual MigrationJobReportWriterDelegate ReportWriter
		{
			get
			{
				return delegate(MigrationJobReportingCursor currentCursor, StreamWriter successWriter, StreamWriter failureWriter)
				{
					switch (currentCursor.ReportingStage)
					{
					case ReportingStageEnum.ProcessingJobItems:
						return this.ProcessJobItemsForReporting(currentCursor, successWriter, failureWriter);
					case ReportingStageEnum.ProcessingValidationErrors:
						return this.ProcessValidationErrorsForReporting(currentCursor, failureWriter);
					case ReportingStageEnum.Completed:
						return currentCursor;
					default:
						throw new InvalidOperationException("This method should not be called with invalid reporting cursor stage " + currentCursor);
					}
				};
			}
		}

		protected virtual string SuccessReportFileName
		{
			get
			{
				return "MigrationStatistics.csv";
			}
		}

		protected virtual string FailureReportFileName
		{
			get
			{
				return "MigrationErrors.csv";
			}
		}

		protected abstract MigrationUserStatus[] JobItemStatesForSuccess { get; }

		protected abstract MigrationUserStatus[] JobItemStatesForFailure { get; }

		public static string GetStatisticsSummaryMessage(MigrationJobReportingCursor cursor)
		{
			if (!cursor.AreSuccessfulMigrationsPresent)
			{
				return string.Empty;
			}
			int partialMigrationCounts = cursor.PartialMigrationCounts;
			if (partialMigrationCounts == 0)
			{
				return Strings.NoPartialMigrationSummaryMessage;
			}
			if (partialMigrationCounts == 1)
			{
				return Strings.PartialMigrationSummaryMessageSingular(partialMigrationCounts);
			}
			return Strings.PartialMigrationSummaryMessagePlural(partialMigrationCounts);
		}

		protected abstract string GetTemplateName(bool areErrorsPresent);

		protected abstract string GetEmailSubject(bool areErrorsPresent);

		protected abstract IDictionary<string, string> GetTemplateData(MigrationJobReportingCursor migrationReportData, string successReportLink, string failureReportLink);

		protected abstract IEnumerable<MigrationJobItem> GetJobItemsToProcess(string startingIndex, int maxCount);

		protected LegacyMigrationJobProcessorResponse Process(out MigrationJobReportingCursor currentCursor)
		{
			LegacyMigrationJobProcessorResponse legacyMigrationJobProcessorResponse = LegacyMigrationJobProcessorResponse.Create(MigrationProcessorResult.Working, null);
			currentCursor = MigrationJobReportingCursor.Deserialize(base.Job.LastCursorPosition);
			MigrationReportGenerator migrationReportGenerator;
			if (currentCursor == null)
			{
				migrationReportGenerator = new MigrationReportGenerator(base.DataProvider, base.Job, this.IsBatchSyncedReport, this.SuccessReportFileName, this.FailureReportFileName);
				if (this.ReportHeaderWriter != null)
				{
					migrationReportGenerator.WriteReportData(null, this.ReportHeaderWriter);
				}
				currentCursor = new MigrationJobReportingCursor(ReportingStageEnum.ProcessingJobItems, string.Empty, migrationReportGenerator.SuccessReportId, migrationReportGenerator.FailureReportId);
				currentCursor.SyncDuration = new TimeSpan?(default(TimeSpan));
			}
			else
			{
				migrationReportGenerator = MigrationReportGenerator.CreateFromCursor(base.DataProvider, base.Job, currentCursor);
			}
			currentCursor = migrationReportGenerator.WriteReportData(currentCursor, this.ReportWriter);
			if (currentCursor.ReportingStage == ReportingStageEnum.Completed)
			{
				MigrationReportData reportData = new MigrationReportData(currentCursor, new MigrationJobTemplateDataGeneratorDelegate(this.GetTemplateData), this.GetEmailSubject(currentCursor.HasErrors), this.GetTemplateName(currentCursor.HasErrors), this.LicensingHelpUrl);
				migrationReportGenerator.GenerateUrls(reportData);
				if (base.Job.NotificationEmails != null && base.Job.NotificationEmails.Count > 0)
				{
					migrationReportGenerator.SendReportMail(reportData);
				}
				legacyMigrationJobProcessorResponse.Result = MigrationProcessorResult.Completed;
				base.Job.UpdateInitialSyncProperties(base.DataProvider, currentCursor.SyncDuration.Value);
			}
			else
			{
				base.Job.UpdateLastProcessedRow(base.DataProvider, currentCursor.Serialize(), null, 0);
			}
			return legacyMigrationJobProcessorResponse;
		}

		protected string GetLicensingWarningSection()
		{
			return MigrationJobReportingCursor.GetLicensingHtml(this.LicensingHelpUrl);
		}

		private MigrationJobReportingCursor ProcessJobItemsForReporting(MigrationJobReportingCursor currentCursor, StreamWriter successWriter, StreamWriter failureWriter)
		{
			int config = ConfigBase<MigrationServiceConfigSchema>.GetConfig<int>("SyncMigrationMaxJobItemsToProcessForReportGeneration");
			HashSet<MigrationUserStatus> hashSet = new HashSet<MigrationUserStatus>(this.JobItemStatesForSuccess);
			HashSet<MigrationUserStatus> hashSet2 = new HashSet<MigrationUserStatus>(this.JobItemStatesForFailure);
			List<MigrationJobItem> list = new List<MigrationJobItem>(config);
			List<MigrationJobItem> list2 = new List<MigrationJobItem>(config);
			MigrationJobItem migrationJobItem = null;
			IEnumerable<MigrationJobItem> jobItemsToProcess = this.GetJobItemsToProcess(currentCursor.CurrentCursorPosition, config);
			foreach (MigrationJobItem migrationJobItem2 in jobItemsToProcess)
			{
				migrationJobItem = migrationJobItem2;
				if (hashSet.Contains(migrationJobItem2.Status))
				{
					list.Add(migrationJobItem2);
					currentCursor.MigrationSuccessCount += migrationJobItem2.CountSelf;
					if (migrationJobItem2.InitialSyncDuration != null)
					{
						currentCursor.SyncDuration += migrationJobItem2.InitialSyncDuration.Value;
					}
					if (migrationJobItem2.ItemsSkipped > 0L)
					{
						currentCursor.PartialMigrationCounts++;
					}
				}
				else if (hashSet2.Contains(migrationJobItem2.Status))
				{
					list2.Add(migrationJobItem2);
					currentCursor.MigrationErrorCount += migrationJobItem2.CountSelf;
				}
				else
				{
					MigrationLogger.Log(MigrationEventType.Error, "ProcessJobItemsForReporting: found a job item that we're not processing {0}", new object[]
					{
						migrationJobItem2
					});
				}
			}
			MigrationSuccessReportCsvSchema.Write(successWriter, base.Job.MigrationType, list, this.IsBatchSyncedReport, base.Job.IsStaged);
			MigrationFailureReportCsvSchema.Write(failureWriter, base.Job.MigrationType, list2, this.IsBatchSyncedReport);
			MigrationJobReportingCursor nextCursor;
			if (migrationJobItem == null || string.Equals(migrationJobItem.Identifier, currentCursor.CurrentCursorPosition, StringComparison.OrdinalIgnoreCase))
			{
				if (this.IsBatchSyncedReport)
				{
					nextCursor = currentCursor.GetNextCursor(ReportingStageEnum.ProcessingValidationErrors, null);
				}
				else
				{
					nextCursor = currentCursor.GetNextCursor(ReportingStageEnum.Completed, null);
				}
			}
			else
			{
				nextCursor = currentCursor.GetNextCursor(ReportingStageEnum.ProcessingJobItems, migrationJobItem.Identifier);
			}
			return nextCursor;
		}

		private MigrationJobReportingCursor ProcessValidationErrorsForReporting(MigrationJobReportingCursor currentCursor, StreamWriter failureWriter)
		{
			IEnumerable<MigrationBatchError> validationWarnings = base.Job.GetValidationWarnings(base.DataProvider);
			MigrationFailureReportCsvSchema.Write(base.Job.MigrationType, failureWriter, validationWarnings, true);
			currentCursor.MigrationErrorCount += new MigrationObjectsCount(new int?(base.Job.ValidationWarningCount));
			return currentCursor.GetNextCursor(ReportingStageEnum.Completed, null);
		}

		private const string SuccessReportFileNameString = "MigrationStatistics.csv";

		private const string FailureReportFileNameString = "MigrationErrors.csv";
	}
}
