using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Assistants.Diagnostics
{
	internal static class DiagnosticsFormatter
	{
		public static XElement FormatRootElement()
		{
			return new XElement("MailboxAssistants");
		}

		public static XElement FormatAssistantRoot(string assistantRootName)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("assistantRootName", assistantRootName);
			return new XElement("MailboxAssistant", new XAttribute("Type", assistantRootName));
		}

		public static XElement FormatDatabasesRoot()
		{
			return new XElement("MailboxDatabases");
		}

		public static XElement FormatWorkcycleInfoElement(TimeSpan workcycle)
		{
			return new XElement("WorkcycleInterval", workcycle.ToString());
		}

		public static XElement FormatWorkcycleCheckpointInfoElement(TimeSpan workcycleCheckpoint)
		{
			return new XElement("WorkcycleCheckpointInterval", workcycleCheckpoint.ToString());
		}

		public static XElement FormatTimeBasedJobDatabaseStats(string dbName, Guid dbGuid, DiagnosticsSummaryDatabase dbSummary)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("dbName", dbName);
			ArgumentValidator.ThrowIfEmpty("dbGuid", dbGuid);
			ArgumentValidator.ThrowIfNull("dbSummary", dbSummary);
			XElement xelement = DiagnosticsFormatter.FormatTimeBasedJobDatabaseStatsCommon(dbName, dbGuid, dbSummary);
			XElement content = DiagnosticsFormatter.FormatTimeBasedJobMailboxStatsWindow("WindowJob", dbSummary.WindowJobStatistics);
			XElement content2 = DiagnosticsFormatter.FormatTimeBasedJobMailboxStats("OnDemandJobs", dbSummary.OnDemandJobsStatistics);
			xelement.Add(content);
			xelement.Add(content2);
			return xelement;
		}

		public static XElement FormatTimeBasedJobDatabaseStatsCommon(string dbName, Guid dbGuid, DiagnosticsSummaryDatabase dbSummary)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("dbName", dbName);
			ArgumentValidator.ThrowIfEmpty("dbGuid", dbGuid);
			ArgumentValidator.ThrowIfNull("dbSummary", dbSummary);
			XElement xelement = new XElement("MailboxDatabase", new XAttribute("Name", dbName));
			xelement.Add(new XElement("Guid", dbGuid));
			xelement.Add(new XElement("IsAssistantEnabled", dbSummary.IsAssistantEnabled.ToString().ToLower(CultureInfo.InvariantCulture)));
			xelement.Add(new XElement("StartTime", dbSummary.StartTime.ToString("O", CultureInfo.InvariantCulture)));
			return xelement;
		}

		public static XElement FormatTimeBasedMailboxes(bool formatActive, string dbName, Guid dbGuid, DiagnosticsSummaryDatabase dbSummary, IEnumerable<Guid> mailboxCollection)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("dbName", dbName);
			ArgumentValidator.ThrowIfEmpty("dbGuid", dbGuid);
			ArgumentValidator.ThrowIfNull("dbSummary", dbSummary);
			ArgumentValidator.ThrowIfNull("mailboxCollection", mailboxCollection);
			string expandedName = formatActive ? "Running" : "Queued";
			XElement xelement = DiagnosticsFormatter.FormatTimeBasedJobDatabaseStatsCommon(dbName, dbGuid, dbSummary);
			XElement xelement2 = new XElement(expandedName);
			foreach (Guid guid in mailboxCollection)
			{
				XElement content = new XElement("MailboxGuid", guid);
				xelement2.Add(content);
			}
			xelement.Add(xelement2);
			return xelement;
		}

		public static XElement FormatWindowJobHistory(string dbName, Guid dbGuid, DiagnosticsSummaryDatabase dbSummary, DiagnosticsSummaryJobWindow[] windowJobHistory)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("dbName", dbName);
			ArgumentValidator.ThrowIfEmpty("dbGuid", dbGuid);
			ArgumentValidator.ThrowIfNull("dbSummary", dbSummary);
			ArgumentValidator.ThrowIfNull("windowJobHistory", windowJobHistory);
			XElement xelement = DiagnosticsFormatter.FormatTimeBasedJobDatabaseStatsCommon(dbName, dbGuid, dbSummary);
			XElement xelement2 = new XElement("WindowJobs");
			foreach (XElement content in windowJobHistory.Select(new Func<DiagnosticsSummaryJobWindow, XElement>(DiagnosticsFormatter.FormatTimeBasedJobWindowHistoryEntry)))
			{
				xelement2.Add(content);
			}
			xelement.Add(xelement2);
			return xelement;
		}

		public static XElement FormatErrorElement(Exception exception)
		{
			ArgumentValidator.ThrowIfNull("exception", exception);
			return DiagnosticsFormatter.FormatErrorElement(exception.Message);
		}

		public static XElement FormatErrorElement(string message)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("message", message);
			XElement xelement = DiagnosticsFormatter.FormatRootElement();
			xelement.Add(new XElement("Error", message));
			return xelement;
		}

		public static XElement FormatHelpElement(DiagnosticsArgument arguments)
		{
			ArgumentValidator.ThrowIfNull("arguments", arguments);
			XElement xelement = DiagnosticsFormatter.FormatRootElement();
			xelement.Add(new XElement("Help", "Supported arguments: " + arguments.GetSupportedArguments()));
			return xelement;
		}

		private static XElement FormatTimeBasedJobMailboxStats(string jobElementName, DiagnosticsSummaryJob mbxSummary)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("jobElementName", jobElementName);
			ArgumentValidator.ThrowIfNull("mbxSummary", mbxSummary);
			XElement xelement = new XElement(jobElementName);
			xelement.Add(new XElement("ProcessingMailboxCount", mbxSummary.ProcessingCount));
			xelement.Add(new XElement("CompletedMailboxCount", mbxSummary.ProcessedSuccessfullyCount));
			xelement.Add(new XElement("FailedMailboxCount", mbxSummary.ProcessedFailureCount));
			xelement.Add(new XElement("FailedToOpenStoreSessionCount", mbxSummary.FailedToOpenStoreSessionCount));
			xelement.Add(new XElement("RetriedMailboxCount", mbxSummary.RetriedCount));
			xelement.Add(new XElement("QueuedMailboxCount", mbxSummary.QueuedCount));
			return xelement;
		}

		private static XElement FormatTimeBasedJobMailboxStatsWindow(string jobElementName, DiagnosticsSummaryJobWindow mbxSummary)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("jobElementName", jobElementName);
			ArgumentValidator.ThrowIfNull("mbxSummary", mbxSummary);
			XElement xelement = DiagnosticsFormatter.FormatTimeBasedJobMailboxStats(jobElementName, mbxSummary.DiagnosticsSummaryJob);
			xelement.AddFirst(new XElement("FailedFilteringMailboxCount", mbxSummary.FailedFilteringCount));
			xelement.AddFirst(new XElement("FilteredMailboxCount", mbxSummary.FilteredMailboxCount));
			xelement.AddFirst(new XElement("NotInterestingMailboxCount", mbxSummary.NotInterestingCount));
			xelement.AddFirst(new XElement("InterestingMailboxCount", mbxSummary.InterestingCount));
			xelement.AddFirst(new XElement("TotalOnDatabaseMailboxCount", mbxSummary.TotalOnDatabaseCount));
			xelement.AddFirst(new XElement("StartTime", mbxSummary.StartTime.ToString("O", CultureInfo.InvariantCulture)));
			xelement.Add(new XElement("MovedToOnDemandMailboxCount", mbxSummary.ProcessedSeparatelyCount));
			return xelement;
		}

		private static XElement FormatTimeBasedJobWindowHistoryEntry(DiagnosticsSummaryJobWindow windowJob)
		{
			ArgumentValidator.ThrowIfNull("windowJob", windowJob);
			XElement xelement = new XElement("WindowJob");
			xelement.Add(new XElement("StartTime", windowJob.StartTime.ToString("O", CultureInfo.InvariantCulture)));
			xelement.Add(new XElement("EndTime", windowJob.EndTime.ToString("O", CultureInfo.InvariantCulture)));
			xelement.Add(new XElement("TotalOnDatabaseMailboxCount", windowJob.TotalOnDatabaseCount));
			xelement.Add(new XElement("InterestingMailboxCount", windowJob.InterestingCount));
			xelement.Add(new XElement("NotInterestingMailboxCount", windowJob.NotInterestingCount));
			xelement.Add(new XElement("FilteredMailboxCount", windowJob.FilteredMailboxCount));
			xelement.Add(new XElement("FailedFilteringMailboxCount", windowJob.FailedFilteringCount));
			xelement.Add(new XElement("CompletedMailboxCount", windowJob.DiagnosticsSummaryJob.ProcessedSuccessfullyCount));
			xelement.Add(new XElement("MovedToOnDemandMailboxCount", windowJob.ProcessedSeparatelyCount));
			xelement.Add(new XElement("FailedMailboxCount", windowJob.DiagnosticsSummaryJob.ProcessedFailureCount));
			xelement.Add(new XElement("FailedToOpenStoreSessionCount", windowJob.DiagnosticsSummaryJob.FailedToOpenStoreSessionCount));
			xelement.Add(new XElement("RetriedMailboxCount", windowJob.DiagnosticsSummaryJob.RetriedCount));
			return xelement;
		}

		private const string MailboxAssistant = "MailboxAssistant";

		private const string Databases = "MailboxDatabases";

		private const string Database = "MailboxDatabase";

		private const string WindowJob = "WindowJob";

		private const string WindowJobs = "WindowJobs";

		private const string OnDemandJobs = "OnDemandJobs";

		private const string Running = "Running";

		private const string Queued = "Queued";

		private const string NameAttr = "Name";

		private const string TypeAttr = "Type";

		private const string WorkcycleInterval = "WorkcycleInterval";

		private const string WorkcycleCheckpointInterval = "WorkcycleCheckpointInterval";

		private const string TotalOnDatabaseCount = "TotalOnDatabaseMailboxCount";

		private const string InterestingCount = "InterestingMailboxCount";

		private const string NotInterestingCount = "NotInterestingMailboxCount";

		private const string FilteredMailboxCount = "FilteredMailboxCount";

		private const string FailedFilteringMailboxCount = "FailedFilteringMailboxCount";

		private const string ProcessingCount = "ProcessingMailboxCount";

		private const string CompletedMailboxCount = "CompletedMailboxCount";

		private const string FailedMailboxCount = "FailedMailboxCount";

		private const string FailedToOpenStoreSessionCount = "FailedToOpenStoreSessionCount";

		private const string MovedToOnDemandMailboxCount = "MovedToOnDemandMailboxCount";

		private const string RetriedMailboxCount = "RetriedMailboxCount";

		private const string QueuedCount = "QueuedMailboxCount";

		private const string MailboxGuid = "MailboxGuid";

		private const string DatabaseGuid = "Guid";

		private const string IsAssistantEnabled = "IsAssistantEnabled";

		private const string StartTime = "StartTime";

		private const string EndTime = "EndTime";

		private const string Error = "Error";

		private const string Help = "Help";

		private const string SupportedAgrs = "Supported arguments: ";

		private const string DateTimeFormat = "O";
	}
}
