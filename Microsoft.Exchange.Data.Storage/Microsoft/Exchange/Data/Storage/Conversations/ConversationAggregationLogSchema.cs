using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Conversations
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class ConversationAggregationLogSchema
	{
		internal enum OperationStart
		{
			OperationName
		}

		internal enum Error
		{
			Exception,
			Context
		}

		internal enum MailboxOwnerData
		{
			SideConversationProcessingEnabled,
			SearchDuplicatedMessages,
			IsGroupMailbox
		}

		internal enum ParentMessageData
		{
			ConversationFamilyId,
			ConversationId,
			InternetMessageId,
			ItemClass,
			SupportsSideConversation
		}

		internal enum DeliveredMessageData
		{
			InternetMessageId,
			ItemClass
		}

		internal enum AggregationResult
		{
			ConversationFamilyId,
			ConversationId,
			IsOutOfOrderDelivery,
			NewConversationCreated,
			SupportsSideConversation,
			FixupStage
		}

		internal enum SideConversationProcessingData
		{
			ParentMessageReplyAllParticipantsCount,
			ParentMessageReplyAllDisplayNames,
			DeliveredMessageReplyAllParticipantsCount,
			DeliveredMessageReplyAllDisplayNames,
			RequiredBindToParentMessage,
			DisplayNameCheckResult
		}

		internal enum OperationEnd
		{
			OperationName,
			Elapsed,
			CPU,
			RPCCount,
			RPCLatency,
			DirectoryCount,
			DirectoryLatency,
			StoreTimeInServer,
			StoreTimeInCPU,
			StorePagesRead,
			StorePagesPreRead,
			StoreLogRecords,
			StoreLogBytes
		}
	}
}
