using System;
using Microsoft.Exchange.Data.Storage.Optics;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.GroupMailbox
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IGroupEscalateItemPerformanceTracker : IMailboxPerformanceTracker, IPerformanceTracker
	{
		string OriginalMessageSender { get; set; }

		string OriginalMessageSenderRecipientType { get; set; }

		string OriginalMessageClass { get; set; }

		string OriginalMessageId { get; set; }

		string OriginalInternetMessageId { get; set; }

		int ParticipantsInOriginalMessage { get; set; }

		bool IsGroupParticipantAddedToReplyTo { get; set; }

		bool IsGroupParticipantAddedToParticipants { get; set; }

		bool IsGroupParticipantReplyToSkipped { get; set; }

		long EnsureGroupParticipantAddedMilliseconds { get; set; }

		long DedupeParticipantsMilliseconds { get; set; }

		bool EscalateToYammer { get; set; }

		long SendToYammerMilliseconds { get; set; }

		void IncrementParticipantsAddedToEscalatedMessage();

		void IncrementParticipantsSkippedInEscalatedMessage();

		bool HasEscalatedUser { get; set; }

		bool UnsubscribeUrlInserted { get; set; }

		long BuildUnsubscribeUrlMilliseconds { get; set; }

		long LinkBodySize { get; set; }

		long LinkOnBodyDetectionMilliseconds { get; set; }

		long LinkInsertOnBodyMilliseconds { get; set; }
	}
}
