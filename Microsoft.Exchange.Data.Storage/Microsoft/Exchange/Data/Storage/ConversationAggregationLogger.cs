using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.Conversations;
using Microsoft.Exchange.Data.Storage.Optics;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ConversationAggregationLogger : MailboxLoggerBase, IConversationAggregationLogger, IExtensibleLogger, IWorkloadLogger
	{
		internal ConversationAggregationLogger(Guid mailboxGuid, OrganizationId organizationId) : base(mailboxGuid, organizationId, ConversationAggregationLogger.Instance.Value)
		{
		}

		public void LogParentMessageData(IStorePropertyBag parentMessage)
		{
			if (parentMessage == null)
			{
				return;
			}
			this.LogEvent(new SchemaBasedLogEvent<ConversationAggregationLogSchema.ParentMessageData>
			{
				{
					ConversationAggregationLogSchema.ParentMessageData.ConversationFamilyId,
					parentMessage.TryGetProperty(ItemSchema.ConversationFamilyId)
				},
				{
					ConversationAggregationLogSchema.ParentMessageData.ConversationId,
					parentMessage.TryGetProperty(ItemSchema.ConversationId)
				},
				{
					ConversationAggregationLogSchema.ParentMessageData.InternetMessageId,
					parentMessage.TryGetProperty(ItemSchema.InternetMessageId)
				},
				{
					ConversationAggregationLogSchema.ParentMessageData.ItemClass,
					parentMessage.TryGetProperty(StoreObjectSchema.ItemClass)
				},
				{
					ConversationAggregationLogSchema.ParentMessageData.SupportsSideConversation,
					parentMessage.TryGetProperty(ItemSchema.SupportsSideConversation)
				}
			});
		}

		public override void LogEvent(ILogEvent logEvent)
		{
			base.LogEvent(logEvent);
			ConversationAggregationLogger.Tracer.TraceDebug<string>((long)this.GetHashCode(), "{0}", logEvent.ToString());
		}

		public void LogDeliveredMessageData(ICorePropertyBag deliveredMessage)
		{
			this.LogEvent(new SchemaBasedLogEvent<ConversationAggregationLogSchema.DeliveredMessageData>
			{
				{
					ConversationAggregationLogSchema.DeliveredMessageData.InternetMessageId,
					deliveredMessage.TryGetProperty(ItemSchema.InternetMessageId)
				},
				{
					ConversationAggregationLogSchema.DeliveredMessageData.ItemClass,
					deliveredMessage.TryGetProperty(StoreObjectSchema.ItemClass)
				}
			});
		}

		public void LogMailboxOwnerData(IMailboxOwner owner, bool shouldSearchForDuplicatedMessage)
		{
			this.LogEvent(new SchemaBasedLogEvent<ConversationAggregationLogSchema.MailboxOwnerData>
			{
				{
					ConversationAggregationLogSchema.MailboxOwnerData.IsGroupMailbox,
					owner.IsGroupMailbox
				},
				{
					ConversationAggregationLogSchema.MailboxOwnerData.SideConversationProcessingEnabled,
					owner.SideConversationProcessingEnabled
				},
				{
					ConversationAggregationLogSchema.MailboxOwnerData.SearchDuplicatedMessages,
					shouldSearchForDuplicatedMessage
				}
			});
		}

		public void LogAggregationResultData(ConversationAggregationResult aggregationResult)
		{
			this.LogEvent(new SchemaBasedLogEvent<ConversationAggregationLogSchema.AggregationResult>
			{
				{
					ConversationAggregationLogSchema.AggregationResult.ConversationFamilyId,
					aggregationResult.ConversationFamilyId
				},
				{
					ConversationAggregationLogSchema.AggregationResult.ConversationId,
					ConversationId.Create(aggregationResult.ConversationIndex)
				},
				{
					ConversationAggregationLogSchema.AggregationResult.IsOutOfOrderDelivery,
					ConversationIndex.IsFixupAddingOutOfOrderMessageToConversation(aggregationResult.Stage)
				},
				{
					ConversationAggregationLogSchema.AggregationResult.NewConversationCreated,
					ConversationIndex.IsFixUpCreatingNewConversation(aggregationResult.Stage)
				},
				{
					ConversationAggregationLogSchema.AggregationResult.SupportsSideConversation,
					aggregationResult.SupportsSideConversation
				},
				{
					ConversationAggregationLogSchema.AggregationResult.FixupStage,
					aggregationResult.Stage
				}
			});
		}

		public void LogSideConversationProcessingData(HashSet<string> parentReplyAllParticipants, HashSet<string> deliveredReplyAllParticipants)
		{
			SchemaBasedLogEvent<ConversationAggregationLogSchema.SideConversationProcessingData> schemaBasedLogEvent = new SchemaBasedLogEvent<ConversationAggregationLogSchema.SideConversationProcessingData>();
			if (deliveredReplyAllParticipants.Count > 10)
			{
				schemaBasedLogEvent.Add(ConversationAggregationLogSchema.SideConversationProcessingData.ParentMessageReplyAllParticipantsCount, parentReplyAllParticipants.Count);
				schemaBasedLogEvent.Add(ConversationAggregationLogSchema.SideConversationProcessingData.DeliveredMessageReplyAllParticipantsCount, deliveredReplyAllParticipants.Count);
			}
			else
			{
				schemaBasedLogEvent.Add(ConversationAggregationLogSchema.SideConversationProcessingData.ParentMessageReplyAllDisplayNames, ExtensibleLogger.FormatPIIValue(this.ConvertParticipantsToLogString(parentReplyAllParticipants)));
				schemaBasedLogEvent.Add(ConversationAggregationLogSchema.SideConversationProcessingData.ParentMessageReplyAllParticipantsCount, parentReplyAllParticipants.Count);
				schemaBasedLogEvent.Add(ConversationAggregationLogSchema.SideConversationProcessingData.DeliveredMessageReplyAllDisplayNames, ExtensibleLogger.FormatPIIValue(this.ConvertParticipantsToLogString(deliveredReplyAllParticipants)));
				schemaBasedLogEvent.Add(ConversationAggregationLogSchema.SideConversationProcessingData.DeliveredMessageReplyAllParticipantsCount, deliveredReplyAllParticipants.Count);
			}
			this.LogEvent(schemaBasedLogEvent);
		}

		public void LogSideConversationProcessingData(ParticipantSet parentReplyAllParticipants, ParticipantSet deliveredReplyAllParticipants)
		{
			SchemaBasedLogEvent<ConversationAggregationLogSchema.SideConversationProcessingData> schemaBasedLogEvent = new SchemaBasedLogEvent<ConversationAggregationLogSchema.SideConversationProcessingData>();
			if (deliveredReplyAllParticipants.Count > 10)
			{
				schemaBasedLogEvent.Add(ConversationAggregationLogSchema.SideConversationProcessingData.ParentMessageReplyAllParticipantsCount, parentReplyAllParticipants.Count);
				schemaBasedLogEvent.Add(ConversationAggregationLogSchema.SideConversationProcessingData.DeliveredMessageReplyAllParticipantsCount, deliveredReplyAllParticipants.Count);
			}
			else
			{
				schemaBasedLogEvent.Add(ConversationAggregationLogSchema.SideConversationProcessingData.ParentMessageReplyAllDisplayNames, ExtensibleLogger.FormatPIIValue(this.ConvertParticipantsToLogString(parentReplyAllParticipants)));
				schemaBasedLogEvent.Add(ConversationAggregationLogSchema.SideConversationProcessingData.ParentMessageReplyAllParticipantsCount, parentReplyAllParticipants.Count);
				schemaBasedLogEvent.Add(ConversationAggregationLogSchema.SideConversationProcessingData.DeliveredMessageReplyAllDisplayNames, ExtensibleLogger.FormatPIIValue(this.ConvertParticipantsToLogString(deliveredReplyAllParticipants)));
				schemaBasedLogEvent.Add(ConversationAggregationLogSchema.SideConversationProcessingData.DeliveredMessageReplyAllParticipantsCount, deliveredReplyAllParticipants.Count);
			}
			this.LogEvent(schemaBasedLogEvent);
		}

		public void LogSideConversationProcessingData(string checkResult, bool requiredBindToParentMessage)
		{
			this.LogEvent(new SchemaBasedLogEvent<ConversationAggregationLogSchema.SideConversationProcessingData>
			{
				{
					ConversationAggregationLogSchema.SideConversationProcessingData.DisplayNameCheckResult,
					checkResult
				},
				{
					ConversationAggregationLogSchema.SideConversationProcessingData.RequiredBindToParentMessage,
					requiredBindToParentMessage
				}
			});
		}

		public void LogException(string operation, Exception exception)
		{
			this.LogEvent(new SchemaBasedLogEvent<ConversationAggregationLogSchema.Error>
			{
				{
					ConversationAggregationLogSchema.Error.Context,
					operation
				},
				{
					ConversationAggregationLogSchema.Error.Exception,
					exception.ToString()
				}
			});
		}

		private string ConvertParticipantsToLogString(IEnumerable<string> data)
		{
			if (data != null)
			{
				return string.Join("|", data);
			}
			return null;
		}

		private string ConvertParticipantsToLogString(IEnumerable<IParticipant> data)
		{
			if (data != null)
			{
				return string.Join("|", from entry in data
				select entry.DisplayName);
			}
			return null;
		}

		private const int MAX_PARTICIPANTS_COUNT = 10;

		private static readonly Lazy<ExtensibleLogger> Instance = new Lazy<ExtensibleLogger>(() => new ExtensibleLogger(ConversationAggregationLogConfiguration.Default));

		private static readonly Trace Tracer = ExTraceGlobals.ConversationTracer;
	}
}
