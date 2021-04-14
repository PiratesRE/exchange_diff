using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Data.Storage.Conversations
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IConversationAggregationLogger : IExtensibleLogger, IWorkloadLogger
	{
		void LogParentMessageData(IStorePropertyBag parentMessage);

		void LogDeliveredMessageData(ICorePropertyBag deliveredMessage);

		void LogMailboxOwnerData(IMailboxOwner mailboxOwner, bool shouldSearchForDuplicatedMessage);

		void LogAggregationResultData(ConversationAggregationResult aggregationResult);

		void LogSideConversationProcessingData(HashSet<string> parentReplyAllParticipants, HashSet<string> deliveredReplyAllParticipants);

		void LogSideConversationProcessingData(ParticipantSet parentReplyAllParticipants, ParticipantSet deliveredReplyAllParticipants);

		void LogSideConversationProcessingData(string checkResult, bool requiredBindToParentMessage);
	}
}
