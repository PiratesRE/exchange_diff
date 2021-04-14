using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ServiceHost.SyncMigrationServicelet;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Migration.Logging;

namespace Microsoft.Exchange.Migration
{
	internal class ReportMigrationJobProcessor : MigrationJobProcessorBase
	{
		public ReportMigrationJobProcessor(MigrationJob migrationObject, IMigrationDataProvider dataProvider) : base(migrationObject, dataProvider)
		{
			this.ReportData = new MigrationJobReportingCursor(ReportingStageEnum.Unknown);
		}

		protected override Func<int?, IEnumerable<StoreObjectId>>[] ProcessableChildObjectQueries
		{
			get
			{
				return new Func<int?, IEnumerable<StoreObjectId>>[]
				{
					(int? maxCount) => MigrationJobItem.GetAllIds(this.DataProvider, this.MigrationObject, maxCount)
				};
			}
		}

		protected override int? MaxChildObjectsToProcessCount
		{
			get
			{
				return null;
			}
		}

		private IMigrationDataProvider ReportProvider { get; set; }

		private ReportMigrationJobProcessor.DisposableMigrationReportItem ReportItem { get; set; }

		private MigrationReportCsvSchema CsvSchema { get; set; }

		private MigrationJobReportingCursor ReportData { get; set; }

		private string LicensingHelpUrl
		{
			get
			{
				if (this.DataProvider.ADProvider.IsLicensingEnforced)
				{
					return string.Format(CultureInfo.InvariantCulture, MigrationJobReportingCursor.MoacHelpUrlFormat, new object[]
					{
						this.MigrationObject.AdminCulture.LCID
					});
				}
				return null;
			}
		}

		internal void SendReportMail()
		{
			MigrationReportData migrationReportData = new MigrationReportData(this.ReportData, this.GetEmailSubject(), "BatchReport.htm", this.LicensingHelpUrl);
			IMigrationEmailHandler migrationEmailHandler = MigrationServiceFactory.Instance.CreateEmailHandler(this.DataProvider);
			using (IMigrationEmailMessageItem migrationEmailMessageItem = migrationEmailHandler.CreateEmailMessage())
			{
				bool includeReportLink = !this.ReportItem.TryCopyAttachment(migrationEmailMessageItem);
				if (!string.IsNullOrEmpty(migrationReportData.LicensingHelpUrl))
				{
					MigrationReportGenerator.ComposeAttachmentFromResource(migrationEmailMessageItem, "Information.gif", "Information");
				}
				string body = migrationReportData.ComposeBodyFromTemplate(this.GetTemplateData(includeReportLink));
				MultiValuedProperty<SmtpAddress> notificationEmails = this.MigrationObject.NotificationEmails;
				string emailSubject = migrationReportData.EmailSubject;
				ExTraceGlobals.FaultInjectionTracer.TraceTest(4209388861U);
				migrationEmailMessageItem.Send(notificationEmails, emailSubject, body);
			}
		}

		protected override MigrationProcessorResponse ProcessChild(MigrationJobItem child)
		{
			if (MigrationJobReportingCursor.FailureStatuses.Contains(child.Status))
			{
				this.ReportData.MigrationErrorCount += child.CountSelf;
			}
			else
			{
				if (child.InitialSyncDuration != null)
				{
					this.ReportData.SyncDuration += child.InitialSyncDuration.Value;
				}
				if (child.ItemsSkipped > 0L)
				{
					this.ReportData.PartialMigrationCounts++;
				}
				this.ReportData.MigrationSuccessCount += child.CountSelf;
			}
			this.CsvSchema.WriteRow(this.ReportItem.AttachmentWriter, child);
			return MigrationJobItemProcessorResponse.Create(MigrationProcessorResult.Completed, null, null, null, null, null, false, null);
		}

		protected override MigrationJobProcessorResponse ProcessObject()
		{
			this.CsvSchema = new MigrationReportCsvSchema(this.MigrationObject.IsProvisioningSupported);
			int count = MigrationReportItem.GetCount(this.ReportProvider, this.MigrationObject.JobId);
			int config = ConfigBase<MigrationServiceConfigSchema>.GetConfig<int>("MaxReportItemsPerJob");
			if (count >= config)
			{
				MigrationLogger.Log(MigrationEventType.Verbose, "Attempting to removing {0} report items for a maximum of {1}", new object[]
				{
					count - config,
					config
				});
				foreach (MigrationReportItem migrationReportItem in MigrationReportItem.GetByJobId(this.ReportProvider, new Guid?(this.MigrationObject.JobId), count - config))
				{
					MigrationLogger.Log(MigrationEventType.Information, "Removing report {0}", new object[]
					{
						migrationReportItem.ReportName
					});
					migrationReportItem.Delete(this.ReportProvider);
				}
			}
			this.ReportItem = new ReportMigrationJobProcessor.DisposableMigrationReportItem(this.MigrationObject);
			this.ReportItem.Initialize(this.ReportProvider, this.CsvSchema);
			return MigrationJobProcessorResponse.Create(MigrationProcessorResult.Completed, null, null, null, null, null);
		}

		protected override MigrationJobProcessorResponse ApplyResponse(MigrationJobProcessorResponse response)
		{
			if (response.Result == MigrationProcessorResult.Completed)
			{
				this.ReportItem.Save();
				MigrationReportSet reportSet = new MigrationReportSet((DateTime)this.ReportItem.CreationTime, this.ReportItem.EcpUrl, null);
				this.MigrationObject.SetReportUrls(this.DataProvider, reportSet);
				if (this.MigrationObject.NotificationEmails != null && this.MigrationObject.NotificationEmails.Count > 0)
				{
					this.SendReportMail();
				}
				this.MigrationObject.SetStatus(this.DataProvider, this.MigrationObject.Status, this.MigrationObject.State, new MigrationFlags?(this.MigrationObject.Flags & ~MigrationFlags.Report), null, null, null, null, null, null, true, null, response.ProcessingDuration);
				return MigrationJobProcessorResponse.Create(MigrationProcessorResult.Working, null, null, null, null, null);
			}
			return base.ApplyResponse(response);
		}

		protected override void SetContext()
		{
			base.SetContext();
			this.ReportProvider = this.DataProvider.GetProviderForFolder(MigrationFolderName.SyncMigrationReports);
		}

		protected override void RestoreContext()
		{
			if (this.ReportItem != null)
			{
				this.ReportItem.Dispose();
				this.ReportItem = null;
			}
			if (this.ReportProvider != null)
			{
				this.ReportProvider.Dispose();
				this.ReportProvider = null;
			}
			base.RestoreContext();
		}

		private string GetLicensingWarningSection()
		{
			return MigrationJobReportingCursor.GetLicensingHtml(this.LicensingHelpUrl);
		}

		private IDictionary<string, string> GetTemplateData(bool includeReportLink)
		{
			MigrationObjectsCount migrationErrorCount = this.ReportData.MigrationErrorCount;
			MigrationObjectsCount migrationSuccessCount = this.ReportData.MigrationSuccessCount;
			bool hasErrors = this.ReportData.HasErrors;
			Dictionary<string, string> dictionary = new Dictionary<string, string>(15);
			string text = JobSyncCompletingProcessor.GetStatisticsSummaryMessage(this.ReportData);
			if (includeReportLink)
			{
				dictionary.Add("{StatisticsReportLink}", this.ReportItem.EcpUrl);
			}
			else
			{
				dictionary.Add("{StatisticsReportLink}", string.Empty);
			}
			if (!string.IsNullOrEmpty(text))
			{
				text = string.Format(CultureInfo.InvariantCulture, "<br />{0}", new object[]
				{
					text
				});
			}
			dictionary.Add("{StatisticsSummaryMessage}", text);
			dictionary.Add("{MoacWarningSection}", this.GetLicensingWarningSection());
			dictionary.Add("{ReportHeader}", this.GetEmailSubject());
			if (!hasErrors)
			{
				dictionary.Add("{imghtml}", string.Empty);
				dictionary.Add("{LabelCouldntMigrate}", string.Empty);
				dictionary.Add("{ErrorSummaryMessage}", string.Empty);
			}
			else
			{
				dictionary.Add("{imghtml}", "<img width=\"16\" height=\"16\" src=\"cid:ErrorImage\" />");
				dictionary.Add("{LabelCouldntMigrate}", Strings.LabelCouldntMigrate);
				dictionary.Add("{ErrorSummaryMessage}", migrationErrorCount.ToString());
			}
			dictionary.Add("{ExtraNotes}", string.Empty);
			dictionary.Add("{LabelStartedByUser}", Strings.LabelSubmittedByUser);
			dictionary.Add("{StartedByUser}", this.MigrationObject.SubmittedByUser);
			dictionary.Add("{LabelStartDateTime}", Strings.LabelStartDateTime);
			dictionary.Add("{LabelRunTime}", Strings.LabelRunTime);
			ExTimeZone userTimeZone = this.MigrationObject.UserTimeZone;
			if (this.MigrationObject.StartTime != null)
			{
				dictionary.Add("{StartDateTime}", userTimeZone.ConvertDateTime(this.MigrationObject.StartTime.Value).ToString("D", CultureInfo.CurrentCulture));
				TimeSpan timeSpan = ExDateTime.UtcNow - this.MigrationObject.StartTime.Value;
				dictionary.Add("{RunTime}", ItemStateTransitionHelper.LocalizeTimeSpan(timeSpan));
			}
			else
			{
				dictionary.Add("{StartDateTime}", string.Empty);
				dictionary.Add("{RunTime}", string.Empty);
			}
			MigrationObjectsCount migrationObjectsCount = migrationSuccessCount;
			dictionary.Add("{LabelSynced}", Strings.LabelSynced);
			dictionary.Add("{CompletedData}", migrationObjectsCount.ToString());
			MigrationObjectsCount migrationObjectsCount2 = migrationErrorCount + migrationObjectsCount;
			dictionary.Add("{LabelTotalRows}", Strings.LabelTotalRows);
			dictionary.Add("{TotalRows}", migrationObjectsCount2.ToString());
			dictionary.Add("{LabelLogMailFooter}", Strings.LabelLogMailFooter);
			return dictionary;
		}

		private string GetEmailSubject()
		{
			if (this.MigrationObject.IsCancelled)
			{
				switch (this.MigrationObject.JobCancellationStatus)
				{
				case JobCancellationStatus.NotCancelled:
					break;
				case JobCancellationStatus.CancelledByUserRequest:
					return Strings.MigrationBatchCancelledByUser(this.MigrationObject.JobName);
				case JobCancellationStatus.CancelledDueToHighFailureCount:
					return Strings.MigrationBatchCancelledBySystem(this.MigrationObject.JobName);
				default:
					throw new InvalidOperationException("Unsupported job cancellation status " + this.MigrationObject.JobCancellationStatus);
				}
			}
			if (this.ReportData.HasErrors)
			{
				return Strings.MigrationBatchReportMailErrorHeader(this.MigrationObject.JobName);
			}
			return Strings.MigrationBatchReportMailHeader(this.MigrationObject.JobName);
		}

		private const string ReportFileName = "MigrationReport.csv";

		private const string ReportTemplate = "BatchReport.htm";

		private class DisposableMigrationReportItem : MigrationReportItem, IDisposeTrackable, IDisposable
		{
			public DisposableMigrationReportItem(MigrationJob job) : base("MigrationReport.csv", new Guid?(job.JobId), job.MigrationType, MigrationReportType.BatchReport, job.IsStaged)
			{
				base.Version = 3L;
				this.disposed = false;
				this.disposeTracker = this.GetDisposeTracker();
			}

			public StreamWriter AttachmentWriter { get; private set; }

			public string EcpUrl { get; private set; }

			public IMigrationMessageItem MessageItem { get; set; }

			public IMigrationAttachment Attachment { get; set; }

			public void Initialize(IMigrationDataProvider dataProvider, MigrationReportCsvSchema schema)
			{
				MigrationUtil.ThrowOnNullArgument(dataProvider, "dataProvider");
				MigrationUtil.ThrowOnNullArgument(schema, "schema");
				this.MessageItem = dataProvider.CreateMessage();
				base.WriteToMessageItem(this.MessageItem, true);
				this.MessageItem.Save(SaveMode.NoConflictResolution);
				this.MessageItem.Load(MigrationHelper.ItemIdProperties);
				base.MessageId = this.MessageItem.Id;
				this.EcpUrl = base.GetUrl(dataProvider);
				this.Attachment = this.MessageItem.CreateAttachment(base.ReportName);
				this.AttachmentWriter = new StreamWriter(this.Attachment.Stream);
				schema.WriteHeader(this.AttachmentWriter);
			}

			public bool TryCopyAttachment(IMigrationEmailMessageItem message)
			{
				bool flag = MigrationJob.MigrationTypeSupportsProvisioning(base.MigrationType);
				if (flag)
				{
					return false;
				}
				using (IMigrationAttachment attachment = this.MessageItem.GetAttachment(base.ReportName, PropertyOpenMode.ReadOnly))
				{
					if (attachment.Size < ConfigBase<MigrationServiceConfigSchema>.GetConfig<long>("ReportMaxAttachmentSize"))
					{
						using (IMigrationAttachment migrationAttachment = message.CreateAttachment(base.ReportName))
						{
							attachment.Stream.Seek(0L, SeekOrigin.Begin);
							CsvSchema csvSchema = new MigrationReportCsvSchema(flag);
							using (StreamWriter streamWriter = new StreamWriter(migrationAttachment.Stream))
							{
								csvSchema.Copy(attachment.Stream, streamWriter);
							}
							migrationAttachment.Save(null);
						}
						return true;
					}
				}
				return false;
			}

			public void Save()
			{
				this.AttachmentWriter.Flush();
				this.AttachmentWriter.Dispose();
				this.AttachmentWriter = null;
				this.Attachment.Save(null);
				base.ReportedTime = new ExDateTime?(ExDateTime.UtcNow);
				this.MessageItem[MigrationBatchMessageSchema.MigrationJobItemStateLastUpdated] = base.ReportedTime;
				base.WriteToMessageItem(this.MessageItem, true);
				this.MessageItem.Save(SaveMode.NoConflictResolution);
				this.MessageItem.Load(MigrationHelper.ItemIdProperties);
				base.MessageId = this.MessageItem.Id;
				base.CreationTime = this.MessageItem.CreationTime;
				this.AttachmentWriter = new StreamWriter(this.Attachment.Stream);
			}

			public virtual DisposeTracker GetDisposeTracker()
			{
				return DisposeTracker.Get<ReportMigrationJobProcessor.DisposableMigrationReportItem>(this);
			}

			public void SuppressDisposeTracker()
			{
				if (this.disposeTracker != null)
				{
					this.disposeTracker.Suppress();
				}
			}

			public void Dispose()
			{
				this.Dispose(true);
				GC.SuppressFinalize(this);
			}

			private void Dispose(bool disposing)
			{
				if (!this.disposed && disposing)
				{
					if (this.AttachmentWriter != null)
					{
						this.AttachmentWriter.Dispose();
						this.AttachmentWriter = null;
					}
					if (this.Attachment != null)
					{
						this.Attachment.Dispose();
						this.Attachment = null;
					}
					if (this.MessageItem != null)
					{
						this.MessageItem.Dispose();
						this.MessageItem = null;
					}
					if (this.disposeTracker != null)
					{
						this.disposeTracker.Dispose();
						this.disposeTracker = null;
					}
				}
				this.disposed = true;
			}

			private bool disposed;

			private DisposeTracker disposeTracker;
		}
	}
}
