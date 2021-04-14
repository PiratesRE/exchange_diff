using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics.Components.ServiceHost.SyncMigrationServicelet;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Migration
{
	internal class MigrationReportGenerator
	{
		internal MigrationReportGenerator(IMigrationDataProvider dataProvider, MigrationJob migrationJob)
		{
			MigrationUtil.ThrowOnNullArgument(dataProvider, "dataProvider");
			MigrationUtil.ThrowOnNullArgument(migrationJob, "migrationJob");
			this.dataProvider = dataProvider;
			this.migrationJob = migrationJob;
		}

		internal MigrationReportGenerator(IMigrationDataProvider dataProvider, MigrationJob migrationJob, bool isBatchCompletionReport, string successReportFileName, string failureReportFileName) : this(dataProvider, migrationJob)
		{
			MigrationUtil.ThrowOnNullOrEmptyArgument(successReportFileName, "successReportFileName");
			MigrationUtil.ThrowOnNullOrEmptyArgument(failureReportFileName, "failureReportFileName");
			this.supportAttachmentGeneration = true;
			MigrationReportItem migrationReportItem = MigrationReportGenerator.CreateReportItem(dataProvider, migrationJob, isBatchCompletionReport ? MigrationReportType.BatchSuccessReport : MigrationReportType.FinalizationSuccessReport, successReportFileName);
			this.successReportId = migrationReportItem.Identifier;
			MigrationReportItem migrationReportItem2 = MigrationReportGenerator.CreateReportItem(dataProvider, migrationJob, isBatchCompletionReport ? MigrationReportType.BatchFailureReport : MigrationReportType.FinalizationFailureReport, failureReportFileName);
			this.failureReportId = migrationReportItem2.Identifier;
		}

		internal MigrationReportId SuccessReportId
		{
			get
			{
				return this.successReportId;
			}
		}

		internal MigrationReportId FailureReportId
		{
			get
			{
				return this.failureReportId;
			}
		}

		internal bool SupportAttachmentGeneration
		{
			get
			{
				return this.supportAttachmentGeneration;
			}
		}

		internal static MigrationReportGenerator CreateFromCursor(IMigrationDataProvider dataProvider, MigrationJob migrationJob, MigrationJobReportingCursor cursor)
		{
			return new MigrationReportGenerator(dataProvider, migrationJob)
			{
				successReportId = cursor.SuccessReportId,
				failureReportId = cursor.FailureReportId,
				supportAttachmentGeneration = true
			};
		}

		internal static void ComposeAttachmentFromResource(IMigrationEmailMessageItem reportItem, string resourceFileName, string resourceContentId)
		{
			using (Stream resourceStream = MigrationReportGenerator.GetResourceStream(resourceFileName))
			{
				if (resourceStream == null)
				{
					MigrationApplication.NotifyOfCriticalError(new FileNotFoundException(), "Unable to load resource " + resourceFileName);
				}
				else
				{
					using (IMigrationAttachment migrationAttachment = reportItem.CreateAttachment(resourceFileName))
					{
						Util.StreamHandler.CopyStreamData(resourceStream, migrationAttachment.Stream);
						migrationAttachment.Save(resourceContentId);
					}
				}
			}
		}

		internal static string GetTemplate(string templateName)
		{
			string result;
			using (Stream resourceStream = MigrationReportGenerator.GetResourceStream(templateName))
			{
				if (resourceStream == null)
				{
					MigrationApplication.NotifyOfCriticalError(new FileNotFoundException(), "Unable to load resource " + templateName);
					result = string.Empty;
				}
				else
				{
					using (StreamReader streamReader = new StreamReader(resourceStream))
					{
						result = streamReader.ReadToEnd();
					}
				}
			}
			return result;
		}

		internal MigrationJobReportingCursor WriteReportData(MigrationJobReportingCursor initialCursorPosition, MigrationJobReportWriterDelegate reportWriter)
		{
			if (!this.SupportAttachmentGeneration)
			{
				throw new InvalidOperationException("This method cannot be invoked when the generator does not support attachment generation. Has the right constructor been used?");
			}
			MigrationJobReportingCursor reportingCursor = null;
			using (IMigrationDataProvider reportProvider = this.dataProvider.GetProviderForFolder(MigrationFolderName.SyncMigrationReports))
			{
				MigrationReportItem migrationReportItem = MigrationReportItem.Get(reportProvider, this.SuccessReportId);
				MigrationReportItem failureReportItem = MigrationReportItem.Get(reportProvider, this.FailureReportId);
				migrationReportItem.WriteStream(reportProvider, delegate(StreamWriter successWriter)
				{
					failureReportItem.WriteStream(reportProvider, delegate(StreamWriter failureWriter)
					{
						reportingCursor = reportWriter(initialCursorPosition, successWriter, failureWriter);
					});
				});
			}
			return reportingCursor;
		}

		internal void SendReportMail(MigrationReportData reportData)
		{
			IMigrationEmailHandler migrationEmailHandler = MigrationServiceFactory.Instance.CreateEmailHandler(this.dataProvider);
			using (IMigrationEmailMessageItem migrationEmailMessageItem = migrationEmailHandler.CreateEmailMessage())
			{
				if (reportData.IsIncludeFailureReportLink)
				{
					MigrationReportGenerator.ComposeAttachmentFromResource(migrationEmailMessageItem, "ErrorImage.gif", "ErrorImage");
				}
				if (!string.IsNullOrEmpty(reportData.LicensingHelpUrl))
				{
					MigrationReportGenerator.ComposeAttachmentFromResource(migrationEmailMessageItem, "Information.gif", "Information");
				}
				MultiValuedProperty<SmtpAddress> notificationEmails = this.migrationJob.NotificationEmails;
				string emailSubject = reportData.EmailSubject;
				string successReportLink = null;
				string failureReportLink = null;
				if (this.SupportAttachmentGeneration)
				{
					MigrationUtil.AssertOrThrow(reportData.ReportUrls != null, "report urls should be set by the time email is sent.  See GenerateUrls", new object[0]);
					if (!string.IsNullOrEmpty(reportData.ReportUrls.SuccessUrl))
					{
						successReportLink = string.Format(CultureInfo.CurrentCulture, "<a href=\"{0}\">{1}</a>", new object[]
						{
							reportData.ReportUrls.SuccessUrl,
							Strings.StatisticsReportLink
						});
					}
					if (!string.IsNullOrEmpty(reportData.ReportUrls.ErrorUrl))
					{
						failureReportLink = string.Format(CultureInfo.CurrentCulture, "<a href=\"{0}\">{1}</a>", new object[]
						{
							reportData.ReportUrls.ErrorUrl,
							Strings.ErrorReportLink
						});
					}
				}
				IEnumerable<KeyValuePair<string, string>> bodyData = reportData.TemplateDataGenerator(reportData.ReportingCursor, successReportLink, failureReportLink);
				string body = reportData.ComposeBodyFromTemplate(bodyData);
				ExTraceGlobals.FaultInjectionTracer.TraceTest(4209388861U);
				migrationEmailMessageItem.Send(notificationEmails, emailSubject, body);
			}
		}

		internal MigrationReportSet GenerateUrls(MigrationReportData reportData)
		{
			string successUrl = null;
			string errorUrl = null;
			ExDateTime? exDateTime = null;
			using (IMigrationDataProvider providerForFolder = this.dataProvider.GetProviderForFolder(MigrationFolderName.SyncMigrationReports))
			{
				if (reportData.IsIncludeSuccessReportLink)
				{
					MigrationReportItem migrationReportItem = MigrationReportItem.Get(providerForFolder, this.SuccessReportId);
					successUrl = migrationReportItem.GetUrl(providerForFolder);
					exDateTime = new ExDateTime?(migrationReportItem.CreationTime);
				}
				if (reportData.IsIncludeFailureReportLink)
				{
					MigrationReportItem migrationReportItem2 = MigrationReportItem.Get(providerForFolder, this.FailureReportId);
					errorUrl = migrationReportItem2.GetUrl(providerForFolder);
					if (exDateTime == null)
					{
						exDateTime = new ExDateTime?(migrationReportItem2.CreationTime);
					}
				}
			}
			if (exDateTime == null)
			{
				exDateTime = new ExDateTime?(ExDateTime.UtcNow);
			}
			reportData.ReportUrls = new MigrationReportSet((DateTime)exDateTime.Value, successUrl, errorUrl);
			this.migrationJob.SetReportUrls(this.dataProvider, reportData.ReportUrls);
			return reportData.ReportUrls;
		}

		private static MigrationReportItem CreateReportItem(IMigrationDataProvider dataProvider, MigrationJob migrationJob, MigrationReportType reportType, string reportFileName)
		{
			MigrationReportItem result;
			using (IMigrationDataProvider providerForFolder = dataProvider.GetProviderForFolder(MigrationFolderName.SyncMigrationReports))
			{
				result = MigrationReportItem.Create(providerForFolder, new Guid?(migrationJob.JobId), migrationJob.MigrationType, migrationJob.IsStaged, reportType, reportFileName);
			}
			return result;
		}

		private static Stream GetResourceStream(string resourceFile)
		{
			return Assembly.GetAssembly(typeof(JobSyncCompletingProcessor)).GetManifestResourceStream(resourceFile);
		}

		internal const string ErrorImageName = "ErrorImage";

		internal const string CsvExtension = ".csv";

		internal const string ReportLinkUrlFormat = "<a href=\"{0}\">{1}</a>";

		internal const string ErrorImageExtension = ".gif";

		internal const string InformationImageExtension = ".gif";

		internal const string InformationImageName = "Information";

		private readonly IMigrationDataProvider dataProvider;

		private readonly MigrationJob migrationJob;

		private MigrationReportId successReportId;

		private MigrationReportId failureReportId;

		private bool supportAttachmentGeneration;
	}
}
