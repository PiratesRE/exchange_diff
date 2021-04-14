using System;
using Microsoft.Exchange.Data.Storage.Optics;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.WorkingSet.Publisher
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IWorkingSetPublisherPerformanceTracker : IMailboxPerformanceTracker, IPerformanceTracker
	{
		string OriginalMessageSender { get; set; }

		string OriginalMessageSenderRecipientType { get; set; }

		string OriginalMessageClass { get; set; }

		string OriginalMessageId { get; set; }

		string OriginalInternetMessageId { get; set; }

		int ParticipantsInOriginalMessage { get; set; }

		string PublishedMessageId { get; set; }

		string PublishedIntnernetMessageId { get; set; }

		bool IsGroupParticipantAddedToParticipants { get; set; }

		long EnsureGroupParticipantAddedMilliseconds { get; set; }

		long DedupeParticipantsMilliseconds { get; set; }

		void IncrementParticipantsAddedToPublishedMessage();

		void IncrementParticipantsSkippedInPublishedMessage();

		bool HasWorkingSetUser { get; set; }

		string Exception { get; set; }
	}
}
