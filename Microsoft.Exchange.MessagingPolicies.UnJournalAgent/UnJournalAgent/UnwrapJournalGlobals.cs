using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MessagingPolicies.UnJournalAgent
{
	internal sealed class UnwrapJournalGlobals
	{
		public static TimeSpan RetryIntervalOnError
		{
			get
			{
				return UnwrapJournalGlobals.retryInterval;
			}
		}

		public static ExEventLog Logger
		{
			get
			{
				return UnwrapJournalGlobals.logger;
			}
		}

		public const string RegistryKeyPath = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Exchange_Test\\v15\\BCM";

		public const string DisableUnJournalingInDCKeyName = "DisableUnJournalAgent";

		public const string EhaMigrationMailboxName = "ehamigrationmailbox";

		public const string OfficialAgentName = "Unwrap Journal Agent";

		public const string EhaDeliveryPriorityReason = "eha legacy archive journaling";

		public const string LiveArchiveJournalDeliveryPriorityReason = "Live archive journaling";

		public const string ProcessedOnSubmitted = "Microsoft.Exchange.MessagingPolicies.UnJournalAgent.ProcessedOnSubmitted";

		public const string ProcessedInternalJournalReport = "Microsoft.Exchange.Journaling.ProcessedOnRoutedInternalJournalReport";

		public const string ProcessedOnSubmittedForJournalNdr = "Microsoft.Exchange.MessagingPolicies.UnJournalAgent.ProcessedOnSubmittedForJournalNdr";

		private static TimeSpan retryInterval = new TimeSpan(0, 20, 0);

		private static ExEventLog logger = new ExEventLog(new Guid("7D2A0005-2C75-42ac-B495-8FE62F3B4FCF"), "MSExchange Messaging Policies");
	}
}
