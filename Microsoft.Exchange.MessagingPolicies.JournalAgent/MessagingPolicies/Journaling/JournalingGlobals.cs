using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MessagingPolicies.Journaling
{
	internal sealed class JournalingGlobals
	{
		public static ExEventLog Logger
		{
			get
			{
				return JournalingGlobals.logger;
			}
		}

		public static TimeSpan RetryIntervalOnError
		{
			get
			{
				return JournalingGlobals.retryInterval;
			}
		}

		public const string OfficialAgentName = "Journaling";

		public const string RecipientInfoProperty = "Microsoft.Exchange.Journaling.OriginalRecipientInfo";

		public const string ExternalFlagProperty = "Microsoft.Exchange.Journaling.External";

		public const string InternalFlagProperty = "Microsoft.Exchange.Journaling.Internal";

		public const string ProcessedOnSubmitted = "Microsoft.Exchange.Journaling.ProcessedOnSubmitted";

		public const string ProcessedOnRouted = "Microsoft.Exchange.Journaling.ProcessedOnRouted";

		public const string ProcessedInternalJournalReport = "Microsoft.Exchange.Journaling.ProcessedOnRoutedInternalJournalReport";

		public const string ProcessedOnSubmittedByUnJournalAgent = "Microsoft.Exchange.MessagingPolicies.UnJournalAgent.ProcessedOnSubmitted";

		public const string ProcessedMessage = "X-MS-Exchange-Organization-Processed-By-Journaling";

		public const string ProcessedMessageByGcc = "X-MS-Exchange-Organization-Processed-By-Gcc-Journaling";

		public const string AuthMechanism = "X-MS-Exchange-Organization-AuthMechanism";

		public const int MapiSubmitMechanism = 3;

		public const int SecureMapiSubmitMechanism = 4;

		public const int SecureInternalSubmit = 5;

		public const string FqdnForMessageId = "journal.report.generator";

		public const string ProcessedByUnjournalHeader = "X-MS-Exchange-Organization-Unjournal-Processed";

		public const string ProcessedByUnjournalForNdrHeader = "X-MS-Exchange-Organization-Unjournal-ProcessedNdr";

		public const string InternalJournalReportHeader = "X-MS-InternalJournal";

		public const string OriginalMessageId = "Microsoft.Exchange.Journaling.OriginalMessageId";

		public const string IsGccFlag = "Microsoft.Exchange.Journaling.IsGccFlag";

		public const string RuleIds = "Microsoft.Exchange.Journaling.RuleIds";

		public const string TransportSettingsContainer = "Transport Settings";

		private static ExEventLog logger = new ExEventLog(new Guid("7D2A0005-2C75-42ac-B495-8FE62F3B4FCF"), "MSExchange Messaging Policies");

		private static TimeSpan retryInterval = new TimeSpan(0, 20, 0);
	}
}
