using System;

namespace Microsoft.Exchange.MessagingPolicies.UnJournalAgent
{
	internal static class MessageConstants
	{
		public static class MimeHeader
		{
			public const string ContentTransferEncodingSevenBit = "7bit";

			public const string ContentTypePlainText = "plaintext";

			public const string E12EnvelopeJournal = "X-MS-Journal-Report";

			public const string ContentIdentifier = "Content-Identifier";

			public const string ContentIdentifierTypo = "Content-Identifer";

			public const string ExchangeJournalReport = "exjournalreport";

			public const string OriginalMessageDate = "X-MS-EHA-MessageDate";

			public const string EHAConfirmBatchSize = "X-MS-EHA-ConfirmBatchSize";

			public const string EHAConfirmTimeout = "X-MS-EHA-ConfirmTimeout";

			public const string EHAMessageRetainUntil = "X-MS-EHA-MessageExpiryDate";

			public const string EHAMessageID = "X-MS-EHAMessageID";

			public const string ProcessedByUnjournal = "X-MS-Exchange-Organization-Unjournal-Processed";

			public const string SenderIsRecipient = "X-MS-Exchange-Organization-Unjournal-SenderIsRecipient";

			public const string SenderAddress = "X-MS-Exchange-Organization-Unjournal-SenderAddress";

			public const string ProcessedByUnjournalForNdr = "X-MS-Exchange-Organization-Unjournal-ProcessedNdr";

			public const string MessageOriginalDate = "X-MS-Exchange-Organization-Unjournal-OriginalReceiveDate";

			public const string MessageExpiryDate = "X-MS-Exchange-Organization-Unjournal-OriginalExpiryDate";

			public const string InternalJournalReport = "X-MS-InternalJournal";
		}

		public static class AddressType
		{
			public const string SMTP = "smtp";
		}

		public static class JournalReportField
		{
			public const string Recipient = "Recipient";

			public const string To = "To";

			public const string Cc = "Cc";

			public const string Bcc = "Bcc";

			public const string OnBehalfOf = "On-Behalf-Of";

			public const string Sender = "Sender:";

			public const string MessageId = "Message-ID:";

			public const string Recipients = "Recipients:";
		}

		public static class ContentType
		{
			public const string MSTnef = "application/ms-tnef";

			public const string TextPlain = "text/plain";

			public const string TextHtml = "text/html";

			public const string MessageRfc822 = "message/rfc822";
		}

		public static class EHAJournalHeaderDefaults
		{
			public const int DefaultConfirmBatchSize = 1000;

			public const int DefaultConfirmTimeout = 3600;

			public static readonly DateTime DefaultRetainUntilDate = DateTime.MaxValue;
		}

		public static class EHAConfirmationMessage
		{
			public const string EHAJournalLogSegmentStatus = ",status=<";

			public const string EHAJournalLogSegmentDefectivePermanentError = ",<permanenterror>";

			public const string EHAJournalLogSegmentDefectiveUnprovisionedUsers = ",<unprovisionedusers>";

			public const string EHAJournalLogSegmentDefectiveNoUserResolved = ",<nousersresolved>";

			public const string EHAJournalLogSegmentDefectiveDistributionGroup = ",<distributiongroup>";

			public const string EHAJournalLogSegmentDefectiveUnprovisionedUsersAndDistributionGroups = ",<unprovisionedanddistributionlistusers>";

			public const string EHAJournalLogSegmentSender = ",SND=<";

			public const string EHAJournalLogSegmentRecipient = ",RCP=<";

			public const string EHAJournalLogSegmentBatchSize = ",RBS=<";

			public const string EHAJournalLogSegmentTimeout = ",RTO=<";

			public const string EHAJournalLogSegmentMessageID = ",RID=<";

			public const string EHAJournalLogSegmentExpirationDate = ",RXD=<";

			public const string EHAJournalLogSegmentExternalOrganizationId = ",ExtOrgId=<";

			public const string EHAJournalLogSegmentClose = ">";

			public const string EHAJournalLogTitle = "EHAJournal";
		}
	}
}
