using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Assistants.Diagnostics
{
	internal class DiagnosticsArgument : DiagnosableArgument
	{
		public DiagnosticsArgument(string argument)
		{
			base.Initialize(argument);
		}

		protected override void InitializeSchema(Dictionary<string, Type> schema)
		{
			schema["summary"] = typeof(bool);
			schema["running"] = typeof(bool);
			schema["queued"] = typeof(bool);
			schema["history"] = typeof(bool);
			schema["assistant"] = typeof(string);
			schema["database"] = typeof(string);
			schema["mailboxprocessor"] = typeof(bool);
			schema["lockedmailboxes"] = typeof(bool);
			schema["mailboxprocessorscantime"] = typeof(bool);
			schema["publicfolder"] = typeof(bool);
			schema["split"] = typeof(bool);
			schema["recent"] = typeof(bool);
			schema["old"] = typeof(bool);
			schema["mailbox"] = typeof(Guid);
		}

		public const string DiagnosticsComponentName = "MailboxAssistants";

		public const string SummaryArgument = "summary";

		public const string RunningArgument = "running";

		public const string QueuedArgument = "queued";

		public const string AssistantArgument = "assistant";

		public const string DatabaseArgument = "database";

		public const string WindowJobHistoryArgument = "history";

		public const string MailboxProcessorAssistantArgument = "mailboxprocessor";

		public const string MailboxProcessorAssistantLockedMailboxArgument = "lockedmailboxes";

		public const string MailboxProcessorAssistantScanTime = "mailboxprocessorscantime";

		public const string MailboxProcessorAssistantXmlRoot = "MailboxProcessorAssistant";

		public const string MailboxProcessorLastScanXmlElem = "MailboxProcessorLastScan";

		public const string MailboxDatabaseXmlElem = "MailboxDatabase";

		public const string MailboxDatabaseLastScanXmlElem = "LastScan";

		public const string MailboxLockedDetectorXmlElem = "MailboxLockedDetector";

		public const string MailboxLockedXmlElem = "LockedMailbox";

		public const string MailboxGuidXmlElem = "MailboxGuid";

		public const string MailboxLockedCounterXmlElem = "LockedDetectionCounter";

		public const string PublicFolderAssistantArgument = "publicfolder";

		public const string PublicFolderSplitArgument = "split";

		public const string PublicFolderRecentSplitArgument = "recent";

		public const string PublicFolderOldSplitArgument = "old";

		public const string PublicFolderSplitMailboxArgument = "mailbox";

		public const string PublicFolderAssistantXmlRoot = "PublicFolderAssistant";

		public const string PublicFolderSplitXmlRoot = "PublicFolderSplit";

		public const string PublicFolderSplitStateXmlElement = "PublicFolderSplitState";

		public const string PublicFolderSplitDateXmlElement = "PublicFolderSplitDate";

		public const string PublicFolderSplitStateValueXmlElement = "SplitState";

		public const string PublicFolderSplitDateValueXmlElement = "SplitDate";

		public const string PublicFolderMailboxGuidXmlElement = "MailboxGuid";
	}
}
