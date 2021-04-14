using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Migration
{
	internal class MigrationReportData
	{
		public MigrationReportData(MigrationJobReportingCursor reportingContext, string emailSubject, string templateName, string licensingHelpUrl)
		{
			this.ReportingCursor = reportingContext;
			this.EmailSubject = emailSubject;
			this.TemplateName = templateName;
			this.LicensingHelpUrl = licensingHelpUrl;
			this.ReportUrls = null;
		}

		public MigrationReportData(MigrationJobReportingCursor reportingContext, MigrationJobTemplateDataGeneratorDelegate templateDataGenerator, string emailSubject, string templateName, string licensingHelpUrl)
		{
			this.ReportingCursor = reportingContext;
			this.TemplateDataGenerator = templateDataGenerator;
			this.EmailSubject = emailSubject;
			this.TemplateName = templateName;
			this.LicensingHelpUrl = licensingHelpUrl;
			this.ReportUrls = null;
		}

		public MigrationJobReportingCursor ReportingCursor { get; private set; }

		public string EmailSubject { get; private set; }

		public string TemplateName { get; private set; }

		public string LicensingHelpUrl { get; private set; }

		public MigrationReportSet ReportUrls { get; internal set; }

		public bool IsIncludeFailureReportLink
		{
			get
			{
				return this.ReportingCursor.MigrationErrorCount.GetTotal() > 0;
			}
		}

		public bool IsIncludeSuccessReportLink
		{
			get
			{
				return this.ReportingCursor.MigrationSuccessCount.GetTotal() > 0;
			}
		}

		public int PartialMigrationCount
		{
			get
			{
				return this.ReportingCursor.PartialMigrationCounts;
			}
		}

		public MigrationJobTemplateDataGeneratorDelegate TemplateDataGenerator { get; set; }

		public string ComposeBodyFromTemplate(IEnumerable<KeyValuePair<string, string>> bodyData)
		{
			return MigrationReportData.ComposeBodyFromTemplate(this.TemplateName, bodyData);
		}

		private static string ComposeBodyFromTemplate(string templateName, IEnumerable<KeyValuePair<string, string>> bodyData)
		{
			StringBuilder stringBuilder = new StringBuilder(MigrationReportGenerator.GetTemplate(templateName));
			foreach (KeyValuePair<string, string> keyValuePair in bodyData)
			{
				stringBuilder.Replace(keyValuePair.Key, keyValuePair.Value);
			}
			string text = stringBuilder.ToString();
			if (MigrationReportData.TemplateMarkerRegex.IsMatch(text))
			{
				string text2 = string.Format(CultureInfo.InvariantCulture, "The body data specified did not have enough information to fill out the template. Template was: {0}. The body looks like this: {1}", new object[]
				{
					templateName,
					text
				});
				MigrationApplication.NotifyOfCriticalError(new InvalidOperationException(), text2);
				throw new MigrationDataCorruptionException(text2);
			}
			return text;
		}

		private static readonly Regex TemplateMarkerRegex = new Regex("{.*}", RegexOptions.Compiled);
	}
}
