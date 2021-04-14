using System;

namespace Microsoft.Exchange.InfoWorker.Common.Search
{
	internal static class Globals
	{
		public const string LogItemDelimiter = ", ";

		public const string LogLineDelimiter = ";";

		public const string LogNameValueSeperator = ":";

		public const string LogMessageClass = "IPM.Note.Microsoft.Exchange.Search.Log";

		public const string LogAttachmentExtension = ".csv";

		public const string LogDefaultSubject = "Search Results";

		public const string LogMailTemplate = "LogMailTemplate.htm";

		public const string SimpleLogMailTemplate = "SimpleLogMailTemplate.htm";

		public const string StatusMailTemplateName = "StatusMailTemplate.htm";

		public const string RecycleFolderPrefix = "MailboxSearchRecycleFolder";

		public const string ClientInfoString = "Client=EDiscoverySearch;Action=Search;Interactive=False";

		public enum LogFields
		{
			LogMailHeader,
			LogMailHeaderInstructions,
			LogMailSeeAttachment,
			LogMailFooter,
			[LocDescription(Strings.IDs.LogFieldsLastStartTime)]
			LastStartTime,
			[LocDescription(Strings.IDs.LogFieldsLastEndTime)]
			LastEndTime,
			[LocDescription(Strings.IDs.LogFieldsCreatedBy)]
			CreatedBy,
			[LocDescription(Strings.IDs.LogFieldsName)]
			Name,
			[LocDescription(Strings.IDs.LogFieldsSearchQuery)]
			SearchQuery,
			[LocDescription(Strings.IDs.LogFieldsSenders)]
			Senders,
			[LocDescription(Strings.IDs.LogFieldsRecipients)]
			Recipients,
			[LocDescription(Strings.IDs.LogFieldsStartDate)]
			StartDate,
			[LocDescription(Strings.IDs.LogFieldsEndDate)]
			EndDate,
			[LocDescription(Strings.IDs.LogFieldsMessageTypes)]
			MessageTypes,
			[LocDescription(Strings.IDs.LogFieldsSourceRecipients)]
			SourceRecipients,
			[LocDescription(Strings.IDs.LogFieldsTargetMailbox)]
			TargetMailbox,
			[LocDescription(Strings.IDs.LogFieldsNumberSuccessfulMailboxes)]
			NumberSuccessfulMailboxes,
			[LocDescription(Strings.IDs.LogFieldsSuccessfulMailboxes)]
			SuccessfulMailboxes,
			[LocDescription(Strings.IDs.LogFieldsNumberUnsuccessfulMailboxes)]
			NumberUnsuccessfulMailboxes,
			[LocDescription(Strings.IDs.LogFieldsUnsuccessfulMailboxes)]
			UnsuccessfulMailboxes,
			[LocDescription(Strings.IDs.LogFieldsResume)]
			Resume,
			[LocDescription(Strings.IDs.LogFieldsIncludeKeywordStatistics)]
			IncludeKeywordStatistics,
			[LocDescription(Strings.IDs.LogFieldsSearchDumpster)]
			SearchDumpster,
			[LocDescription(Strings.IDs.LogFieldsSearchOperation)]
			SearchOperation,
			[LocDescription(Strings.IDs.LogFieldsLogLevel)]
			LogLevel,
			[LocDescription(Strings.IDs.LogFieldsNumberMailboxesToSearch)]
			NumberMailboxesToSearch,
			[LocDescription(Strings.IDs.LogFieldsStatusMailRecipients)]
			StatusMailRecipients,
			[LocDescription(Strings.IDs.LogFieldsManagedBy)]
			ManagedBy,
			[LocDescription(Strings.IDs.LogFieldsLastRunBy)]
			LastRunBy,
			[LocDescription(Strings.IDs.LogFieldsIdentity)]
			Identity,
			[LocDescription(Strings.IDs.LogFieldsErrors)]
			Errors,
			[LocDescription(Strings.IDs.LogFieldsKeywordHits)]
			KeywordHits,
			[LocDescription(Strings.IDs.LogFieldsStatus)]
			Status,
			[LocDescription(Strings.IDs.LogFieldsStoppedBy)]
			StoppedBy,
			[LocDescription(Strings.IDs.LogFieldsPercentComplete)]
			PercentComplete,
			[LocDescription(Strings.IDs.LogFieldsResultNumberEstimate)]
			ResultNumberEstimate,
			[LocDescription(Strings.IDs.LogFieldsResultNumber)]
			ResultNumber,
			[LocDescription(Strings.IDs.LogFieldsResultSizeEstimate)]
			ResultSizeEstimate,
			[LocDescription(Strings.IDs.LogFieldsResultSize)]
			ResultSize,
			[LocDescription(Strings.IDs.LogFieldsResultSizeCopied)]
			ResultSizeCopied,
			[LocDescription(Strings.IDs.LogFieldsResultsLink)]
			ResultsLink,
			[LocDescription(Strings.IDs.LogFieldsEstimateNotExcludeDuplicates)]
			EstimateNotExcludeDuplicates,
			[LocDescription(Strings.IDs.LogFieldsExcludeDuplicateMessages)]
			ExcludeDuplicateMessages
		}
	}
}
