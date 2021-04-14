using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal static class ConversationLoaderHelper
	{
		public static PropertyDefinition[] CalculateInferenceEnabledPropertiesToLoad(PropertyDefinition[] baseSetOfPropertiesToLoad)
		{
			List<PropertyDefinition> list = new List<PropertyDefinition>(baseSetOfPropertiesToLoad);
			list.AddRange(ConversationLoaderHelper.inferencePropertiesToLoad);
			return list.ToArray();
		}

		[Conditional("DEBUG")]
		public static void CheckRequestedExtendedProperties(PropertyPath[] additionalProperties)
		{
			if (additionalProperties != null)
			{
				foreach (ExtendedPropertyUri extendedPropertyUri in additionalProperties.OfType<ExtendedPropertyUri>())
				{
					bool flag = false;
					foreach (ExtendedPropertyUri second in ConversationLoaderHelper.specialExtendedPropUris)
					{
						if (ExtendedPropertyUri.AreEqual(extendedPropertyUri, second))
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						throw new InvalidExtendedPropertyException(extendedPropertyUri);
					}
				}
			}
		}

		private const string MessagingNamespaceGuidString = "41F28F13-83F4-4114-A584-EEDB5A6B0BFF";

		private static readonly ExtendedPropertyUri nativeBodyInfoPropertyDefinition = new ExtendedPropertyUri
		{
			PropertyTag = "0x1016",
			PropertyType = MapiPropertyType.Integer
		};

		private static readonly ExtendedPropertyUri normalizedSubjectPropertyDefinition = new ExtendedPropertyUri
		{
			PropertyTag = "0xe1d",
			PropertyType = MapiPropertyType.String
		};

		public static PropertyDefinition[] MandatoryConversationPropertiesToLoad = new PropertyDefinition[]
		{
			StoreObjectSchema.ItemClass,
			ItemSchema.Id,
			StoreObjectSchema.ChangeKey,
			StoreObjectSchema.ParentItemId,
			StoreObjectSchema.LastModifiedTime,
			ItemSchema.ReceivedTime,
			ItemSchema.InternetMessageId,
			MessageItemSchema.IsRead,
			MessageItemSchema.HasBeenSubmitted,
			ItemSchema.Categories,
			ItemSchema.ItemColor,
			ItemSchema.IsToDoItem,
			ItemSchema.FlagStatus,
			ItemSchema.FlagRequest,
			TaskSchema.StartDate,
			TaskSchema.DueDate,
			ItemSchema.CompleteDate,
			MessageItemSchema.LastVerbExecuted,
			MessageItemSchema.LastVerbExecutionTime,
			ItemSchema.Size,
			ItemSchema.IconIndex,
			ItemSchema.DocumentId,
			MessageItemSchema.IsDraft
		};

		public static PropertyDefinition[] InReplyToPropertiesToLoad = new PropertyDefinition[]
		{
			ItemSchema.Preview,
			ItemSchema.From,
			ItemSchema.InternetMessageId
		};

		public static PropertyDefinition[] ModernConversationMandatoryPropertiesToLoad = new PropertyDefinition[]
		{
			MessageItemSchema.ReplyToBlobExists,
			MessageItemSchema.ReplyToNamesExists,
			InternalSchema.EffectiveRights,
			StoreObjectSchema.ItemClass
		};

		public static PropertyInformation[] ModernConversationOptionalPropertiesToLoad = new PropertyInformation[]
		{
			MessageSchema.LikeCount,
			MessageSchema.Likers,
			ItemSchema.SupportsSideConversation
		};

		public static PropertyDefinition[] NonMandatoryPropertiesToLoad = new PropertyDefinition[]
		{
			ItemSchema.Subject,
			MessageItemSchema.MessageInConflict,
			ItemSchema.From,
			ItemSchema.Sender,
			ItemSchema.HasAttachment,
			MessageItemSchema.IsDraft,
			ItemSchema.Importance,
			ItemSchema.Sensitivity,
			ItemSchema.IsClassified,
			MessageItemSchema.IsReadReceiptPending,
			ItemSchema.BlockStatus,
			ItemSchema.EdgePcl,
			ItemSchema.LinkEnabled,
			ItemSchema.IsResponseRequested,
			MessageItemSchema.VoiceMessageAttachmentOrder,
			MessageItemSchema.RequireProtectedPlayOnPhone,
			StoreObjectSchema.IsRestricted,
			StoreObjectSchema.PolicyTag,
			ItemSchema.RetentionDate,
			ConversationLoaderHelper.nativeBodyInfoPropertyDefinition.ToPropertyDefinition(),
			ConversationLoaderHelper.normalizedSubjectPropertyDefinition.ToPropertyDefinition(),
			StoreObjectSchema.ArchiveTag,
			StoreObjectSchema.ArchivePeriod,
			StoreObjectSchema.PolicyTag,
			StoreObjectSchema.RetentionPeriod,
			StoreObjectSchema.RetentionFlags,
			StoreObjectSchema.LastModifiedTime,
			MessageItemSchema.IsReadReceiptRequested,
			MessageItemSchema.IsDeliveryReceiptRequested,
			ItemSchema.InstanceKey,
			ItemSchema.Fid,
			MessageItemSchema.MID,
			StoreObjectSchema.CreationTime,
			ItemSchema.SentTime,
			ItemSchema.ReceivedOrRenewTime,
			ItemSchema.RetentionDate,
			MessageItemSchema.TextMessageDeliveryStatus,
			MessageItemSchema.SharingInstanceGuid,
			ItemSchema.DisplayTo,
			ItemSchema.DisplayCc,
			MessageItemSchema.ReplyToBlobExists,
			MessageItemSchema.ReplyToNamesExists,
			MessageItemSchema.ReplyToNames,
			MessageItemSchema.MessageBccMe,
			MessageItemSchema.VotingBlob,
			MessageItemSchema.VotingResponse,
			ItemSchema.RichContent,
			MessageItemSchema.IsGroupEscalationMessage
		};

		private static PropertyDefinition[] inferencePropertiesToLoad = new PropertyDefinition[]
		{
			ItemSchema.IsClutter
		};

		public static PropertyDefinition[] ComplianceProperties = new PropertyDefinition[]
		{
			ItemSchema.IsClassified,
			ItemSchema.ClassificationGuid,
			ItemSchema.ClassificationDescription,
			ItemSchema.Classification,
			ItemSchema.ClassificationKeep
		};

		public static PropertyDefinition[] VoiceMailProperties = new PropertyDefinition[]
		{
			MessageItemSchema.MessageAudioNotes,
			MessageItemSchema.VoiceMessageDuration,
			MessageItemSchema.VoiceMessageAttachmentOrder,
			MessageItemSchema.PstnCallbackTelephoneNumber
		};

		public static PropertyDefinition[] ApprovalRequestProperties = new PropertyDefinition[]
		{
			MessageItemSchema.ApprovalDecision,
			MessageItemSchema.ApprovalDecisionMaker,
			MessageItemSchema.ApprovalDecisionTime
		};

		public static PropertyDefinition[] ReminderMessageProperties = new PropertyDefinition[]
		{
			ReminderMessageSchema.ReminderText,
			CalendarItemBaseSchema.Location,
			ReminderMessageSchema.ReminderStartTime,
			ReminderMessageSchema.ReminderEndTime
		};

		public static PropertyDefinition[] SingleRecipientProperties = new PropertyDefinition[]
		{
			ItemSchema.From,
			ItemSchema.Sender,
			MessageItemSchema.ReceivedRepresenting,
			MessageItemSchema.ReceivedBy
		};

		public static PropertyDefinition[] ChangeHighlightingProperties = new PropertyDefinition[]
		{
			CalendarItemBaseSchema.ChangeHighlight,
			CalendarItemBaseSchema.OldLocation,
			MeetingRequestSchema.OldStartWhole,
			MeetingRequestSchema.OldEndWhole
		};

		public static PropertyDefinition[] EntityExtractionPropeties = new PropertyDefinition[]
		{
			ItemSchema.ExtractedAddresses,
			ItemSchema.ExtractedContacts,
			ItemSchema.ExtractedEmails,
			ItemSchema.ExtractedMeetings,
			ItemSchema.ExtractedPhones,
			ItemSchema.ExtractedTasks,
			ItemSchema.ExtractedUrls
		};

		public static PropertyDefinition[] MeetingMessageProperties = new PropertyDefinition[]
		{
			CalendarItemBaseSchema.IsOrganizer,
			MeetingMessageSchema.IsOutOfDate
		};

		public static PropertyDefinition[] InferenceReasonsProperties = new PropertyDefinition[]
		{
			ItemSchema.InferencePredictedReplyForwardReasons,
			ItemSchema.InferencePredictedDeleteReasons,
			ItemSchema.InferencePredictedIgnoreReasons
		};

		private static ExtendedPropertyUri voiceMessageAttachmentOrder = new ExtendedPropertyUri
		{
			PropertyTag = "0x6805",
			PropertyType = MapiPropertyType.String
		};

		private static ExtendedPropertyUri pstnCallbackTelephoneNumber = new ExtendedPropertyUri
		{
			PropertyName = "PstnCallbackTelephoneNumber",
			DistinguishedPropertySetId = DistinguishedPropertySet.UnifiedMessaging,
			PropertyType = MapiPropertyType.String
		};

		private static ExtendedPropertyUri voiceMessageDuration = new ExtendedPropertyUri
		{
			PropertyTag = "0x6801",
			PropertyType = MapiPropertyType.Integer
		};

		private static ExtendedPropertyUri isClassified = new ExtendedPropertyUri
		{
			PropertyId = 34229,
			DistinguishedPropertySetId = DistinguishedPropertySet.Common,
			PropertyType = MapiPropertyType.Boolean
		};

		private static ExtendedPropertyUri classificationGuid = new ExtendedPropertyUri
		{
			PropertyId = 34232,
			DistinguishedPropertySetId = DistinguishedPropertySet.Common,
			PropertyType = MapiPropertyType.String
		};

		private static ExtendedPropertyUri classification = new ExtendedPropertyUri
		{
			PropertyId = 34230,
			DistinguishedPropertySetId = DistinguishedPropertySet.Common,
			PropertyType = MapiPropertyType.String
		};

		private static ExtendedPropertyUri classificationDescription = new ExtendedPropertyUri
		{
			PropertyId = 34231,
			DistinguishedPropertySetId = DistinguishedPropertySet.Common,
			PropertyType = MapiPropertyType.String
		};

		private static ExtendedPropertyUri classificationKeep = new ExtendedPropertyUri
		{
			PropertyId = 34234,
			DistinguishedPropertySetId = DistinguishedPropertySet.Common,
			PropertyType = MapiPropertyType.Boolean
		};

		private static ExtendedPropertyUri sharingInstanceGuid = new ExtendedPropertyUri
		{
			PropertyId = 35356,
			DistinguishedPropertySetId = DistinguishedPropertySet.Sharing,
			PropertyType = MapiPropertyType.CLSID
		};

		private static ExtendedPropertyUri messageBccMe = new ExtendedPropertyUri
		{
			PropertyName = "MessageBccMe",
			PropertySetId = "41F28F13-83F4-4114-A584-EEDB5A6B0BFF",
			PropertyType = MapiPropertyType.Boolean
		};

		private static ExtendedPropertyUri retentionFlags = new ExtendedPropertyUri
		{
			PropertyTag = "0x301d",
			PropertyType = MapiPropertyType.Integer
		};

		private static ExtendedPropertyUri retentionPeriod = new ExtendedPropertyUri
		{
			PropertyTag = "0x301a",
			PropertyType = MapiPropertyType.Integer
		};

		private static ExtendedPropertyUri archivePeriod = new ExtendedPropertyUri
		{
			PropertyTag = "0x301e",
			PropertyType = MapiPropertyType.Integer
		};

		private static ExtendedPropertyUri lastVerbExecuted = new ExtendedPropertyUri
		{
			PropertyTag = "0x1081",
			PropertyType = MapiPropertyType.Integer
		};

		private static ExtendedPropertyUri lastVerbExecutionTime = new ExtendedPropertyUri
		{
			PropertyTag = "0x1082",
			PropertyType = MapiPropertyType.SystemTime
		};

		private static ExtendedPropertyUri documentId = new ExtendedPropertyUri
		{
			PropertyTag = "0x6815",
			PropertyType = MapiPropertyType.Integer
		};

		private static List<ExtendedPropertyUri> specialExtendedPropUris = new List<ExtendedPropertyUri>
		{
			ConversationLoaderHelper.normalizedSubjectPropertyDefinition,
			ConversationLoaderHelper.nativeBodyInfoPropertyDefinition,
			ConversationLoaderHelper.voiceMessageAttachmentOrder,
			ConversationLoaderHelper.pstnCallbackTelephoneNumber,
			ConversationLoaderHelper.voiceMessageDuration,
			ConversationLoaderHelper.isClassified,
			ConversationLoaderHelper.classificationGuid,
			ConversationLoaderHelper.classification,
			ConversationLoaderHelper.classificationDescription,
			ConversationLoaderHelper.classificationKeep,
			ConversationLoaderHelper.sharingInstanceGuid,
			ConversationLoaderHelper.messageBccMe,
			ConversationLoaderHelper.retentionFlags,
			ConversationLoaderHelper.retentionPeriod,
			ConversationLoaderHelper.archivePeriod,
			ConversationLoaderHelper.lastVerbExecuted,
			ConversationLoaderHelper.lastVerbExecutionTime,
			ConversationLoaderHelper.documentId
		};
	}
}
