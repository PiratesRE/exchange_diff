using System;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	public enum EventDescription
	{
		[EventDescriptionInformation(IsTerminal = false)]
		Submitted,
		[EventDescriptionInformation(IsTerminal = false)]
		SubmittedCrossSite,
		[EventDescriptionInformation(IsTerminal = false)]
		Resolved,
		[EventDescriptionInformation(IsTerminal = false)]
		Expanded,
		[EventDescriptionInformation(IsTerminal = true, EventPriority = 1)]
		Delivered,
		[EventDescriptionInformation(IsTerminal = true, EventPriority = 0)]
		MovedToFolderByInboxRule,
		[EventDescriptionInformation(IsTerminal = false)]
		RulesCc,
		[EventDescriptionInformation(IsTerminal = true, EventPriority = 4)]
		FailedGeneral,
		[EventDescriptionInformation(IsTerminal = true, EventPriority = 3)]
		FailedModeration,
		[EventDescriptionInformation(IsTerminal = true, EventPriority = 4)]
		FailedTransportRules,
		[EventDescriptionInformation(IsTerminal = false, EventPriority = 5)]
		SmtpSend,
		[EventDescriptionInformation(IsTerminal = false, EventPriority = 5)]
		SmtpSendCrossSite,
		[EventDescriptionInformation(IsTerminal = false, EventPriority = 5)]
		SmtpSendCrossForest,
		[EventDescriptionInformation(IsTerminal = false)]
		SmtpReceive,
		[EventDescriptionInformation(IsTerminal = false)]
		Forwarded,
		[EventDescriptionInformation(IsTerminal = false)]
		Pending,
		[EventDescriptionInformation(IsTerminal = false)]
		PendingModeration,
		[EventDescriptionInformation(IsTerminal = false, EventPriority = 2)]
		ApprovedModeration,
		[EventDescriptionInformation(IsTerminal = false, EventPriority = 4)]
		QueueRetry,
		[EventDescriptionInformation(IsTerminal = false, EventPriority = 4)]
		QueueRetryNoRetryTime,
		[EventDescriptionInformation(IsTerminal = false)]
		MessageDefer,
		[EventDescriptionInformation(IsTerminal = true, EventPriority = 5)]
		TransferredToForeignOrg,
		[EventDescriptionInformation(IsTerminal = true, EventPriority = 5)]
		TransferredToLegacyExchangeServer,
		[EventDescriptionInformation(IsTerminal = false, EventPriority = 5)]
		TransferredToPartnerOrg,
		[EventDescriptionInformation(IsTerminal = false, EventPriority = 5)]
		DelayedAfterTransferToPartnerOrg,
		[EventDescriptionInformation(IsTerminal = true, EventPriority = 0)]
		Read,
		[EventDescriptionInformation(IsTerminal = true, EventPriority = 0)]
		NotRead,
		[EventDescriptionInformation(IsTerminal = true, EventPriority = 0)]
		ForwardedToDelegateAndDeleted,
		[EventDescriptionInformation(IsTerminal = true, EventPriority = 4)]
		ExpiredWithNoModerationDecision
	}
}
