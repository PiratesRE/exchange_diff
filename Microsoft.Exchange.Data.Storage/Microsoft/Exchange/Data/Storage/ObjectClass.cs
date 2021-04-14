using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class ObjectClass
	{
		public static bool IsApprovalMessage(string itemClass)
		{
			return ObjectClass.IsOfClass(itemClass, "IPM.Note.Microsoft.Approval");
		}

		public static bool IsCalendarItemOccurrence(string itemClass)
		{
			return ObjectClass.IsOfClass(itemClass, "IPM.Appointment.Occurrence");
		}

		public static bool IsCalendarItemCalendarItemOccurrenceOrRecurrenceException(string itemClass)
		{
			return ObjectClass.IsCalendarItemOrOccurrence(itemClass) || ObjectClass.IsRecurrenceException(itemClass);
		}

		public static bool IsRecurrenceException(string itemClass)
		{
			return ObjectClass.IsOfClass(itemClass, "IPM.OLE.CLASS.{00061055-0000-0000-C000-000000000046}");
		}

		public static bool IsCalendarItem(string itemClass)
		{
			return ObjectClass.IsOfClass(itemClass, "IPM.Appointment");
		}

		public static bool IsCalendarItemSeries(string itemClass)
		{
			return ObjectClass.IsOfClass(itemClass, "IPM.AppointmentSeries");
		}

		public static bool IsCalendarItemOrOccurrence(string itemClass)
		{
			return ObjectClass.IsCalendarItem(itemClass) || ObjectClass.IsCalendarItemOccurrence(itemClass);
		}

		public static bool IsFolderTreeData(string itemClass)
		{
			return ObjectClass.IsOfClass(itemClass, "IPM.Microsoft.WunderBar.Link", false);
		}

		public static bool IsMessage(string itemClass, bool includeCalendaringMessages = false)
		{
			bool flag = ObjectClass.IsOfClass(itemClass, "IPM.Note") || ObjectClass.IsOfClass(itemClass, "REPORT");
			if (includeCalendaringMessages)
			{
				flag |= (ObjectClass.IsMeetingMessage(itemClass) || ObjectClass.IsMeetingMessageSeries(itemClass));
			}
			return flag;
		}

		public static bool IsPost(string itemClass)
		{
			return ObjectClass.IsOfClass(itemClass, "IPM.Post");
		}

		public static bool IsReport(string itemClass)
		{
			return ObjectClass.IsOfClass(itemClass, "REPORT") || ObjectClass.ReportClasses.IsReportOfSpecialCasedClass(itemClass);
		}

		public static bool IsReport(string itemClass, string reportSuffixOrClass)
		{
			return (ObjectClass.IsOfClass(itemClass, "REPORT") && ObjectClass.HasSuffix(itemClass, reportSuffixOrClass)) || (ObjectClass.ReportClasses.IsReportOfSpecialCasedClass(reportSuffixOrClass) && ObjectClass.IsOfClass(itemClass, reportSuffixOrClass));
		}

		public static bool IsDsn(string itemClass)
		{
			return ObjectClass.IsDsnNegative(itemClass) || ObjectClass.IsDsnPositive(itemClass);
		}

		public static bool IsDsnPositive(string itemClass)
		{
			return ObjectClass.IsReport(itemClass, "DR");
		}

		public static bool IsDsnNegative(string itemClass)
		{
			return ObjectClass.IsReport(itemClass, "NDR");
		}

		public static bool IsMdn(string itemClass)
		{
			return ObjectClass.IsReport(itemClass, "IPNRN") || ObjectClass.IsReport(itemClass, "IPNNRN");
		}

		public static bool IsMeetingMessageSeries(string itemClass)
		{
			return ObjectClass.IsOfClass(itemClass, "IPM.MeetingMessageSeries");
		}

		public static bool IsMeetingMessage(string itemClass)
		{
			return ObjectClass.IsOfClass(itemClass, "IPM.Schedule.Meeting") || ObjectClass.IsOfClass(itemClass, "IPM.Schedule.Meeting.Notification") || ObjectClass.IsOfClass(itemClass, "IPM.Notification.Meeting");
		}

		public static bool IsMeetingRequest(string itemClass)
		{
			return ObjectClass.IsOfClass(itemClass, "IPM.Schedule.Meeting.Request");
		}

		public static bool IsMeetingRequestSeries(string itemClass)
		{
			return ObjectClass.IsOfClass(itemClass, "IPM.MeetingMessageSeries.Request");
		}

		public static bool IsMeetingInquiry(string itemClass)
		{
			return ObjectClass.IsOfClass(itemClass, "IPM.Schedule.Inquiry");
		}

		public static bool IsMiddleTierRulesMessage(string itemClass)
		{
			return ObjectClass.IsOfClass(itemClass, "IPM.Rule.Version2.Message");
		}

		public static bool IsFailedInboundICal(string itemClass)
		{
			return ObjectClass.IsOfClass(itemClass, "IPM.Note.NotSupportedICal");
		}

		public static bool IsMeetingResponse(string itemClass)
		{
			return ObjectClass.IsOfClass(itemClass, "IPM.Schedule.Meeting.Resp");
		}

		public static bool IsMeetingResponseSeries(string itemClass)
		{
			return ObjectClass.IsOfClass(itemClass, "IPM.MeetingMessageSeries.Resp");
		}

		public static bool IsMeetingPositiveResponse(string itemClass)
		{
			return ObjectClass.IsOfClass(itemClass, "IPM.Schedule.Meeting.Resp.Pos") || ObjectClass.IsOfClass(itemClass, "IPM.MeetingMessageSeries.Resp.Pos");
		}

		public static bool IsMeetingNegativeResponse(string itemClass)
		{
			return ObjectClass.IsOfClass(itemClass, "IPM.Schedule.Meeting.Resp.Neg") || ObjectClass.IsOfClass(itemClass, "IPM.MeetingMessageSeries.Resp.Neg");
		}

		public static bool IsMeetingTentativeResponse(string itemClass)
		{
			return ObjectClass.IsOfClass(itemClass, "IPM.Schedule.Meeting.Resp.Tent") || ObjectClass.IsOfClass(itemClass, "IPM.MeetingMessageSeries.Resp.Tent");
		}

		public static bool IsExternalSharingSubscription(string itemClass)
		{
			return ObjectClass.IsOfClass(itemClass, "IPM.ExternalSharingSubscription");
		}

		public static bool IsSharingFolderBindingMessage(string itemClass)
		{
			return ObjectClass.IsOfClass(itemClass, "IPM.Sharing.Binding.In");
		}

		public static bool IsOutlookRecall(string itemClass)
		{
			return ObjectClass.IsOfClass(itemClass, "IPM.Outlook.Recall");
		}

		public static bool IsMeetingCancellation(string itemClass)
		{
			return ObjectClass.IsOfClass(itemClass, "IPM.Schedule.Meeting.Canceled");
		}

		public static bool IsMeetingCancellationSeries(string itemClass)
		{
			return ObjectClass.IsOfClass(itemClass, "IPM.MeetingMessageSeries.Canceled");
		}

		public static bool IsMeetingForwardNotification(string itemClass)
		{
			return ObjectClass.IsOfClass(itemClass, "IPM.Schedule.Meeting.Notification.Forward") || ObjectClass.IsOfClass(itemClass, "IPM.Notification.Meeting.Forward");
		}

		public static bool IsMeetingForwardNotificationSeries(string itemClass)
		{
			return ObjectClass.IsOfClass(itemClass, "IPM.MeetingMessageSeries.Notification.Forward");
		}

		public static bool IsContact(string itemClass)
		{
			return ObjectClass.IsOfClass(itemClass, "IPM.Contact");
		}

		public static bool IsUserPhoto(string itemClass)
		{
			return ObjectClass.IsOfClass(itemClass, "IPM.UserPhoto");
		}

		public static bool IsUserPhotoPreview(string itemClass)
		{
			return ObjectClass.IsOfClass(itemClass, "IPM.UserPhoto.Preview");
		}

		public static bool IsPlace(string itemClass)
		{
			return ObjectClass.IsOfClass(itemClass, "IPM.Contact.Place");
		}

		public static bool IsDistributionList(string itemClass)
		{
			return ObjectClass.IsOfClass(itemClass, "IPM.DistList");
		}

		public static bool IsSmimeClearSigned(string itemClass)
		{
			return ObjectClass.HasSuffix(itemClass, "SMIME.MultipartSigned");
		}

		public static bool IsSmime(string itemClass)
		{
			return ObjectClass.HasSuffix(itemClass, "SMIME") || ObjectClass.HasSuffix(itemClass, "SMIME.MultipartSigned") || ObjectClass.HasSuffix(itemClass, "SMIME.Encrypted") || ObjectClass.HasSuffix(itemClass, "SMIME.SignedEncrypted") || ObjectClass.HasSuffix(itemClass, "SMIME.Signed");
		}

		public static bool IsNotesItem(string itemClass)
		{
			return ObjectClass.IsOfClass(itemClass, "IPM.StickyNote");
		}

		public static bool IsTask(string itemClass)
		{
			return ObjectClass.IsOfClass(itemClass, "IPM.Task");
		}

		public static bool IsTaskRequest(string itemClass)
		{
			return ObjectClass.IsOfClass(itemClass, "IPM.TaskRequest");
		}

		public static bool IsGenericMessage(string itemClass)
		{
			return ObjectClass.IsOfClass(itemClass, "IPM");
		}

		public static bool IsJournalItem(string itemClass)
		{
			return ObjectClass.IsOfClass(itemClass, "IPM.Activity");
		}

		public static bool IsUMMessage(string itemClass)
		{
			return ObjectClass.IsOfClass(itemClass, "IPM.Note.Microsoft.Missed.Voice") || ObjectClass.IsOfClass(itemClass, "IPM.Note.Microsoft.Voicemail.UM") || ObjectClass.IsOfClass(itemClass, "IPM.Note.Microsoft.Fax") || ObjectClass.IsOfClass(itemClass, "IPM.Note.Microsoft.Partner.UM") || ObjectClass.HasSuffix(itemClass, "Microsoft.Voicemail") || itemClass.IndexOf(".Microsoft.Voicemail.", StringComparison.OrdinalIgnoreCase) != -1;
		}

		public static bool IsVoiceMessage(string itemClass)
		{
			return ObjectClass.IsOfClass(itemClass, "IPM.Note.Microsoft.Voicemail.UM.CA") || ObjectClass.IsOfClass(itemClass, "IPM.Note.Microsoft.Voicemail.UM") || ObjectClass.IsOfClass(itemClass, "IPM.Note.rpmsg.Microsoft.Voicemail.UM.CA") || ObjectClass.IsOfClass(itemClass, "IPM.Note.rpmsg.Microsoft.Voicemail.UM") || ObjectClass.IsOfClass(itemClass, "IPM.Note.Microsoft.Exchange.Voice.UM.CA") || ObjectClass.IsOfClass(itemClass, "IPM.Note.Microsoft.Exchange.Voice.UM");
		}

		public static bool IsRightsManagedContentClass(string contentClass)
		{
			return ObjectClass.IsOfClass(contentClass, "rpmsg.message") || ObjectClass.IsOfClass(contentClass, "IPM.Note.rpmsg.Microsoft.Voicemail.UM.CA") || ObjectClass.IsOfClass(contentClass, "IPM.Note.rpmsg.Microsoft.Voicemail.UM");
		}

		public static bool IsUMPartnerMessage(string itemClass)
		{
			return ObjectClass.IsOfClass(itemClass, "IPM.Note.Microsoft.Partner.UM");
		}

		public static bool IsUMTranscriptionRequest(string itemClass)
		{
			return ObjectClass.IsOfClass(itemClass, "IPM.Note.Microsoft.Partner.UM.TranscriptionRequest");
		}

		public static bool IsUMCDRMessage(string itemClass)
		{
			return ObjectClass.IsOfClass(itemClass, "IPM.Note.Microsoft.CDR.UM");
		}

		public static bool IsMissedCall(string itemClass)
		{
			return ObjectClass.IsOfClass(itemClass, "IPM.Note.Microsoft.Missed.Voice");
		}

		public static bool IsVoicemailSearchFolder(string itemClass)
		{
			return ObjectClass.IsOfClass(itemClass, "IPF.Note.Microsoft.Voicemail");
		}

		public static bool IsShortcutMessageEntry(int favLevelMask)
		{
			return 1 == favLevelMask;
		}

		public static bool IsSmsMessage(string itemClass)
		{
			return ObjectClass.IsOfClass(itemClass, "IPM.Note.Mobile.SMS");
		}

		public static bool IsNonSendableWithRecipients(string itemClass)
		{
			return ObjectClass.IsOfClass(itemClass, "IPM.Appointment") || ObjectClass.IsOfClass(itemClass, "IPM.Task") || ObjectClass.IsOfClass(itemClass, "IPM.OLE.CLASS.{00061055-0000-0000-C000-000000000046}");
		}

		public static bool IsToDoMessage(string itemClass)
		{
			return ObjectClass.IsOfClass(itemClass, "IPM.Note.QuickCapture");
		}

		public static bool IsEventReminderMessage(string itemClass)
		{
			return ObjectClass.IsOfClass(itemClass, "IPM.Note.Reminder.Event");
		}

		public static bool IsReminderMessage(string itemClass)
		{
			return ObjectClass.IsOfClass(itemClass, "IPM.Note.Reminder");
		}

		public static bool IsConfigurationItem(string itemClass)
		{
			return ObjectClass.IsOfClass(itemClass, "IPM.Configuration");
		}

		public static bool IsGenericFolder(string containerClass)
		{
			return ObjectClass.IsOfClass(containerClass, "IPF");
		}

		public static bool IsMessageFolder(string containerClass)
		{
			return ObjectClass.IsOfClass(containerClass, "IPF.Note");
		}

		public static bool IsTaskFolder(string containerClass)
		{
			return ObjectClass.IsOfClass(containerClass, "IPF.Task");
		}

		public static bool IsNotesFolder(string containerClass)
		{
			return ObjectClass.IsOfClass(containerClass, "IPF.StickyNote");
		}

		public static bool IsJournalFolder(string containerClass)
		{
			return ObjectClass.IsOfClass(containerClass, "IPF.Journal");
		}

		public static bool IsShortcutFolder(string containerClass)
		{
			return ObjectClass.IsOfClass(containerClass, "IPF.ShortcutFolder");
		}

		public static bool IsCalendarFolder(string containerClass)
		{
			return ObjectClass.IsOfClass(containerClass, "IPF.Appointment");
		}

		public static bool IsInfoPathFormFolder(string containerClass)
		{
			return ObjectClass.IsOfClass(containerClass, "IPF.Note.InfoPathForm");
		}

		public static bool IsBirthdayCalendarFolder(string containerClass)
		{
			return ObjectClass.IsOfClass(containerClass, "IPF.Appointment.Birthday");
		}

		public static bool IsContactsFolder(string containerClass)
		{
			return ObjectClass.IsOfClass(containerClass, "IPF.Contact");
		}

		public static bool IsMailboxDiscoverySearchRequest(string itemClass)
		{
			return ObjectClass.IsOfClass(itemClass, "IPM.Configuration.MailboxDiscoverySearchRequest");
		}

		public static bool IsSubscriptionDataItem(string itemClass)
		{
			return ObjectClass.IsOfClass(itemClass, "Exchange.PushNotification.Subscription");
		}

		public static bool IsOutlookServiceSubscriptionDataItem(string itemClass)
		{
			return ObjectClass.IsOfClass(itemClass, "OutlookService.Notification.Subscription");
		}

		public static bool IsParkedMeetingMessage(string itemClass)
		{
			return ObjectClass.IsOfClass(itemClass, "IPM.Parked.MeetingMessage");
		}

		public static StoreObjectType GetObjectType(string itemClass)
		{
			if (ObjectClass.IsOfClass(itemClass, "IPM.Note.Rules.OofTemplate.Microsoft", false))
			{
				return StoreObjectType.OofMessage;
			}
			if (ObjectClass.IsOfClass(itemClass, "IPM.Note.Rules.ExternalOofTemplate.Microsoft", false))
			{
				return StoreObjectType.ExternalOofMessage;
			}
			if (ObjectClass.IsOfClass(itemClass, "IPM.Note.Reminder"))
			{
				return StoreObjectType.ReminderMessage;
			}
			if (ObjectClass.IsOfClass(itemClass, "IPM.Note"))
			{
				return StoreObjectType.Message;
			}
			if (ObjectClass.IsOfClass(itemClass, "IPM.Post"))
			{
				return StoreObjectType.Post;
			}
			if (ObjectClass.IsReport(itemClass))
			{
				return StoreObjectType.Report;
			}
			if (ObjectClass.IsCalendarItem(itemClass))
			{
				return StoreObjectType.CalendarItem;
			}
			if (ObjectClass.IsCalendarItemSeries(itemClass))
			{
				return StoreObjectType.CalendarItemSeries;
			}
			if (ObjectClass.IsMeetingRequestSeries(itemClass))
			{
				return StoreObjectType.MeetingRequestSeries;
			}
			if (ObjectClass.IsMeetingRequest(itemClass))
			{
				return StoreObjectType.MeetingRequest;
			}
			if (ObjectClass.IsOfClass(itemClass, "IPM.MeetingMessageSeries.Resp"))
			{
				return StoreObjectType.MeetingResponseSeries;
			}
			if (ObjectClass.IsOfClass(itemClass, "IPM.Schedule.Meeting.Resp"))
			{
				return StoreObjectType.MeetingResponse;
			}
			if (ObjectClass.IsMeetingCancellationSeries(itemClass))
			{
				return StoreObjectType.MeetingCancellationSeries;
			}
			if (ObjectClass.IsMeetingCancellation(itemClass))
			{
				return StoreObjectType.MeetingCancellation;
			}
			if (ObjectClass.IsOfClass(itemClass, "IPM.Schedule.Meeting.Notification.Forward"))
			{
				return StoreObjectType.MeetingForwardNotification;
			}
			if (ObjectClass.IsOfClass(itemClass, "IPM.MeetingMessageSeries.Notification.Forward"))
			{
				return StoreObjectType.MeetingForwardNotificationSeries;
			}
			if (ObjectClass.IsOfClass(itemClass, "IPM.Notification.Meeting"))
			{
				return StoreObjectType.MeetingForwardNotification;
			}
			if (ObjectClass.IsOfClass(itemClass, "IPM.Schedule.Inquiry"))
			{
				return StoreObjectType.MeetingInquiryMessage;
			}
			if (ObjectClass.IsOfClass(itemClass, "IPM.Contact.Place"))
			{
				return StoreObjectType.Place;
			}
			if (ObjectClass.IsOfClass(itemClass, "IPM.Contact"))
			{
				return StoreObjectType.Contact;
			}
			if (ObjectClass.IsOfClass(itemClass, "IPM.UserPhoto.Preview"))
			{
				return StoreObjectType.UserPhotoPreview;
			}
			if (ObjectClass.IsOfClass(itemClass, "IPM.UserPhoto"))
			{
				return StoreObjectType.UserPhoto;
			}
			if (ObjectClass.IsOfClass(itemClass, "IPM.DistList"))
			{
				return StoreObjectType.DistributionList;
			}
			if (ObjectClass.IsOfClass(itemClass, "IPM.Task"))
			{
				return StoreObjectType.Task;
			}
			if (ObjectClass.IsOfClass(itemClass, "IPM.TaskRequest"))
			{
				return StoreObjectType.TaskRequest;
			}
			if (ObjectClass.IsOfClass(itemClass, "IPM.ConversationAction"))
			{
				return StoreObjectType.ConversationActionItem;
			}
			if (ObjectClass.IsOfClass(itemClass, "IPM.Note.rpmsg.Microsoft.Voicemail.UM.CA"))
			{
				return StoreObjectType.Message;
			}
			if (ObjectClass.IsOfClass(itemClass, "IPM.Note.rpmsg.Microsoft.Voicemail.UM"))
			{
				return StoreObjectType.Message;
			}
			if (ObjectClass.IsOfClass(itemClass, "IPM.Sharing", false))
			{
				return StoreObjectType.SharingMessage;
			}
			if (ObjectClass.IsOfClass(itemClass, "IPM.GroupMailbox.JoinRequest"))
			{
				return StoreObjectType.GroupMailboxRequestMessage;
			}
			if (ObjectClass.IsOfClass(itemClass, "IPM.MailboxAssociation.Group"))
			{
				return StoreObjectType.MailboxAssociationGroup;
			}
			if (ObjectClass.IsOfClass(itemClass, "IPM.MailboxAssociation.User"))
			{
				return StoreObjectType.MailboxAssociationUser;
			}
			if (ObjectClass.IsOfClass(itemClass, "IPM.HierarchySync.Metadata"))
			{
				return StoreObjectType.HierarchySyncMetadata;
			}
			if (ObjectClass.IsSubscriptionDataItem(itemClass))
			{
				return StoreObjectType.PushNotificationSubscription;
			}
			if (ObjectClass.IsOutlookServiceSubscriptionDataItem(itemClass))
			{
				return StoreObjectType.OutlookServiceSubscription;
			}
			if (ObjectClass.IsConfigurationItem(itemClass))
			{
				return StoreObjectType.Configuration;
			}
			if (ObjectClass.IsParkedMeetingMessage(itemClass))
			{
				return StoreObjectType.ParkedMeetingMessage;
			}
			if (ObjectClass.IsGenericMessage(itemClass))
			{
				return StoreObjectType.Message;
			}
			if (ObjectClass.IsCalendarFolder(itemClass))
			{
				return StoreObjectType.CalendarFolder;
			}
			if (ObjectClass.IsContactsFolder(itemClass))
			{
				return StoreObjectType.ContactsFolder;
			}
			if (ObjectClass.IsTaskFolder(itemClass))
			{
				return StoreObjectType.TasksFolder;
			}
			if (ObjectClass.IsNotesFolder(itemClass))
			{
				return StoreObjectType.NotesFolder;
			}
			if (ObjectClass.IsJournalFolder(itemClass))
			{
				return StoreObjectType.JournalFolder;
			}
			if (ObjectClass.IsShortcutFolder(itemClass))
			{
				return StoreObjectType.ShortcutFolder;
			}
			if (ObjectClass.IsGenericFolder(itemClass))
			{
				return StoreObjectType.Folder;
			}
			return StoreObjectType.Unknown;
		}

		public static Schema GetSchema(StoreObject storeObject)
		{
			return ObjectClass.GetSchema(storeObject.ClassName);
		}

		public static Schema GetSchema(string className)
		{
			if (ObjectClass.IsOfClass(className, "IPM.OLE.CLASS.{00061055-0000-0000-C000-000000000046}") || ObjectClass.IsOfClass(className, "IPM.Appointment.Occurrence"))
			{
				return CalendarItemOccurrenceSchema.Instance;
			}
			StoreObjectType objectType = ObjectClass.GetObjectType(className);
			return ObjectClass.GetSchema(objectType);
		}

		public static bool HasSuffix(string itemClass, string suffix)
		{
			return itemClass != null && itemClass.EndsWith(suffix, StringComparison.OrdinalIgnoreCase) && itemClass.Length > suffix.Length && itemClass[itemClass.Length - suffix.Length - 1] == '.';
		}

		internal static bool IsDerivedClass(string itemClass, string baseClass)
		{
			return itemClass != null && itemClass.StartsWith(baseClass, StringComparison.OrdinalIgnoreCase) && itemClass.Length > baseClass.Length && itemClass[baseClass.Length] == '.';
		}

		internal static bool IsOfClass(string itemClass, string baseClass, bool orDerivedClass)
		{
			return itemClass != null && itemClass.StartsWith(baseClass, StringComparison.OrdinalIgnoreCase) && (itemClass.Length == baseClass.Length || (orDerivedClass && itemClass.Length > baseClass.Length && itemClass[baseClass.Length] == '.'));
		}

		public static bool IsOfClass(string itemClass, string baseClass)
		{
			return ObjectClass.IsOfClass(itemClass, baseClass, true);
		}

		public static string MakeReportClassName(string itemClass, string reportSuffixOrClass)
		{
			if (ObjectClass.ReportClasses.IsReportOfSpecialCasedClass(reportSuffixOrClass))
			{
				return reportSuffixOrClass;
			}
			return string.Join(".", new string[]
			{
				"REPORT",
				itemClass,
				reportSuffixOrClass
			});
		}

		public static string GetContainerMessageClass(StoreObjectType type)
		{
			EnumValidator.ThrowIfInvalid<StoreObjectType>(type, "type");
			string result;
			ObjectClass.tableContainerMessageClass.TryGetValue(type, out result);
			return result;
		}

		private static Schema GetSchema(StoreObjectType objectType)
		{
			switch (objectType)
			{
			case StoreObjectType.Folder:
			case StoreObjectType.CalendarFolder:
			case StoreObjectType.ContactsFolder:
			case StoreObjectType.TasksFolder:
			case StoreObjectType.NotesFolder:
			case StoreObjectType.JournalFolder:
			case StoreObjectType.SearchFolder:
			case StoreObjectType.OutlookSearchFolder:
			case StoreObjectType.ShortcutFolder:
				return FolderSchema.Instance;
			case StoreObjectType.Message:
			case StoreObjectType.ConflictMessage:
			case StoreObjectType.TaskRequest:
			case StoreObjectType.Note:
			case StoreObjectType.OofMessage:
			case StoreObjectType.ExternalOofMessage:
				return MessageItemSchema.Instance;
			case StoreObjectType.MeetingMessage:
			case StoreObjectType.MeetingCancellation:
				return MeetingMessageInstanceSchema.Instance;
			case StoreObjectType.MeetingRequest:
				return MeetingRequestSchema.Instance;
			case StoreObjectType.MeetingResponse:
				return MeetingResponseSchema.Instance;
			case StoreObjectType.CalendarItem:
				return CalendarItemSchema.Instance;
			case StoreObjectType.CalendarItemOccurrence:
				return CalendarItemOccurrenceSchema.Instance;
			case StoreObjectType.Contact:
				return ContactSchema.Instance;
			case StoreObjectType.DistributionList:
				return DistributionListSchema.Instance;
			case StoreObjectType.Task:
				return TaskSchema.Instance;
			case StoreObjectType.Post:
				return PostItemSchema.Instance;
			case StoreObjectType.Report:
				return ReportMessageSchema.Instance;
			case StoreObjectType.MeetingForwardNotification:
				return MeetingForwardNotificationSchema.Instance;
			case StoreObjectType.ConversationActionItem:
				return ConversationActionItemSchema.Instance;
			case StoreObjectType.SharingMessage:
				return SharingMessageItemSchema.Instance;
			case StoreObjectType.MeetingInquiryMessage:
				return MeetingInquiryMessageSchema.Instance;
			case StoreObjectType.MailboxAssociationGroup:
				return MailboxAssociationGroupSchema.Instance;
			case StoreObjectType.MailboxAssociationUser:
				return MailboxAssociationUserSchema.Instance;
			case StoreObjectType.GroupMailboxRequestMessage:
				return GroupMailboxJoinRequestMessageSchema.Instance;
			case StoreObjectType.ReminderMessage:
				return ReminderMessageSchema.Instance;
			case StoreObjectType.Configuration:
				return ConfigurationItemSchema.Instance;
			case StoreObjectType.HierarchySyncMetadata:
				return HierarchySyncMetadataItemSchema.Instance;
			}
			return ItemSchema.Instance;
		}

		private static Dictionary<StoreObjectType, string> BuildContainerMessageClassTable(Dictionary<StoreObjectType, string> tableClass)
		{
			return new Dictionary<StoreObjectType, string>
			{
				{
					StoreObjectType.Folder,
					"IPF.Note"
				},
				{
					StoreObjectType.CalendarFolder,
					"IPF.Appointment"
				},
				{
					StoreObjectType.ContactsFolder,
					"IPF.Contact"
				},
				{
					StoreObjectType.TasksFolder,
					"IPF.Task"
				},
				{
					StoreObjectType.NotesFolder,
					"IPF.StickyNote"
				},
				{
					StoreObjectType.ShortcutFolder,
					"IPF.ShortcutFolder"
				},
				{
					StoreObjectType.SearchFolder,
					"IPF.Note"
				},
				{
					StoreObjectType.OutlookSearchFolder,
					"IPF.Note"
				},
				{
					StoreObjectType.Message,
					"IPM.Note"
				},
				{
					StoreObjectType.MeetingMessage,
					"IPM.Schedule.Meeting"
				},
				{
					StoreObjectType.MeetingRequest,
					"IPM.Schedule.Meeting.Request"
				},
				{
					StoreObjectType.MeetingResponse,
					"IPM.Schedule.Meeting.Resp"
				},
				{
					StoreObjectType.MeetingCancellation,
					"IPM.Schedule.Meeting.Canceled"
				},
				{
					StoreObjectType.MeetingInquiryMessage,
					"IPM.Schedule.Inquiry"
				},
				{
					StoreObjectType.ConflictMessage,
					"IPM.Conflict.Message"
				},
				{
					StoreObjectType.CalendarItem,
					"IPM.Appointment"
				},
				{
					StoreObjectType.CalendarItemOccurrence,
					"IPM.Appointment.Occurrence"
				},
				{
					StoreObjectType.Contact,
					"IPM.Contact"
				},
				{
					StoreObjectType.Place,
					"IPM.Contact.Place"
				},
				{
					StoreObjectType.DistributionList,
					"IPM.DistList"
				},
				{
					StoreObjectType.Task,
					"IPM.Task"
				},
				{
					StoreObjectType.TaskRequest,
					"IPM.TaskRequest"
				},
				{
					StoreObjectType.ReminderMessage,
					"IPM.Note.Reminder"
				},
				{
					StoreObjectType.Note,
					"IPM.StickyNote"
				},
				{
					StoreObjectType.Post,
					"IPM.Post"
				},
				{
					StoreObjectType.Report,
					"REPORT"
				},
				{
					StoreObjectType.SharingMessage,
					"IPM.Sharing"
				},
				{
					StoreObjectType.GroupMailboxRequestMessage,
					"IPM.GroupMailbox.JoinRequest"
				},
				{
					StoreObjectType.OofMessage,
					"IPM.Note.Rules.OofTemplate.Microsoft"
				},
				{
					StoreObjectType.ExternalOofMessage,
					"IPM.Note.Rules.ExternalOofTemplate.Microsoft"
				},
				{
					StoreObjectType.UserPhoto,
					"IPM.UserPhoto"
				},
				{
					StoreObjectType.UserPhotoPreview,
					"IPM.UserPhoto.Preview"
				},
				{
					StoreObjectType.MailboxAssociationGroup,
					"IPM.MailboxAssociation.Group"
				},
				{
					StoreObjectType.MailboxAssociationUser,
					"IPM.MailboxAssociation.User"
				},
				{
					StoreObjectType.HierarchySyncMetadata,
					"IPM.HierarchySync.Metadata"
				}
			};
		}

		public const int PublicFolderFavLevelMask = 1;

		public const string GenericItem = "IPM";

		public const string Message = "IPM.Note";

		public const string MapiSubmitLAMProbe = "IPM.Note.MapiSubmitLAMProbe";

		public const string MapiSubmitSystemProbe = "IPM.Note.MapiSubmitSystemProbe";

		public const string Post = "IPM.Post";

		public const string Report = "REPORT";

		public const string Appointment = "IPM.Appointment";

		public const string CalendarItemSeries = "IPM.AppointmentSeries";

		public const string Contact = "IPM.Contact";

		public const string UserPhoto = "IPM.UserPhoto";

		public const string UserPhotoPreview = "IPM.UserPhoto.Preview";

		public const string UserPhotoDeletedNotification = "IPM.UserPhoto.DeletedNotification";

		public const string Place = "IPM.Contact.Place";

		public const string DistributionList = "IPM.DistList";

		public const string ScheduleMeeting = "IPM.Schedule.Meeting";

		public const string MeetingNotification = "IPM.Schedule.Meeting.Notification";

		public const string E12RTMMeetingNotification = "IPM.Notification.Meeting";

		public const string MeetingMessageSeries = "IPM.MeetingMessageSeries";

		public const string MeetingRequest = "IPM.Schedule.Meeting.Request";

		public const string MeetingRequestSeries = "IPM.MeetingMessageSeries.Request";

		public const string MeetingInquiry = "IPM.Schedule.Inquiry";

		public const string MeetingCancellation = "IPM.Schedule.Meeting.Canceled";

		public const string MeetingCancellationSeries = "IPM.MeetingMessageSeries.Canceled";

		public const string MeetingResponsePrefix = "IPM.Schedule.Meeting.Resp";

		public const string MeetingResponseSeriesPrefix = "IPM.MeetingMessageSeries.Resp";

		public const string MiddleTierRules = "IPM.Rule.Version2.Message";

		public const string PositiveMeetingResponseSuffix = "Pos";

		public const string NegativeMeetingResponseSuffix = "Neg";

		public const string TentativeMeetingResponseSuffix = "Tent";

		public const string PositiveMeetingResponse = "IPM.Schedule.Meeting.Resp.Pos";

		public const string PositiveMeetingResponseSeries = "IPM.MeetingMessageSeries.Resp.Pos";

		public const string NegativeMeetingResponse = "IPM.Schedule.Meeting.Resp.Neg";

		public const string NegativeMeetingResponseSeries = "IPM.MeetingMessageSeries.Resp.Neg";

		public const string TentativeMeetingResponse = "IPM.Schedule.Meeting.Resp.Tent";

		public const string TentativeMeetingResponseSeries = "IPM.MeetingMessageSeries.Resp.Tent";

		public const string MeetingForwardNotification = "IPM.Schedule.Meeting.Notification.Forward";

		public const string MeetingForwardNotificationSeries = "IPM.MeetingMessageSeries.Notification.Forward";

		public const string E12RTMMeetingForwardNotification = "IPM.Notification.Meeting.Forward";

		public const string NotSupportedICal = "IPM.Note.NotSupportedICal";

		public const string ContentsSyncData = "Exchange.ContentsSyncData";

		public const string SecureEncrypted = "IPM.Note.Secure";

		public const string SmimeEncrypted = "IPM.Note.SMIME";

		internal const string SmimeSuffix = "SMIME";

		internal const string SmimeMultipartSignedSuffix = "SMIME.MultipartSigned";

		internal const string SmimeSignedSuffix = "SMIME.Signed";

		internal const string SmimeSignedEncryptedSuffix = "SMIME.SignedEncrypted";

		internal const string SmimeEncryptedSuffix = "SMIME.Encrypted";

		public const string SecureSign = "IPM.Note.Secure.Sign";

		public const string SmimeSigned = "IPM.Note.SMIME.MultipartSigned";

		public const string StickyNote = "IPM.StickyNote";

		public const string Task = "IPM.Task";

		internal const string CustomTask = "IPM.Task.";

		public const string TaskRequest = "IPM.TaskRequest";

		internal const string ConflictMessage = "IPM.Conflict.Message";

		internal const string ConflictMessagePrefix = "IPM.Conflict";

		internal const string ConflictResolution = "IPM.Conflict.Resolution.Message";

		public const string ElcJournalMsg = "IPM.Note.JournalReport.Msg";

		public const string ElcJournalTnef = "IPM.Note.JournalReport.Tnef";

		internal const string RpmsgMessageContentClass = "rpmsg.message";

		internal const string RecurrenceException = "IPM.OLE.CLASS.{00061055-0000-0000-C000-000000000046}";

		internal const string AppointmentOccurrence = "IPM.Appointment.Occurrence";

		internal const string OutlookRecall = "IPM.Outlook.Recall";

		public const string OutlookJournalItem = "IPM.Activity";

		public const string UserActivityItem = "IPM.Activity";

		public const string ExchangeUMBetaPureAudioClass = "IPM.Note.Microsoft.Exchange.Voice.UM";

		public const string ExchangeUMPureAudioClass = "IPM.Note.Microsoft.Voicemail.UM";

		public const string ExchangeUMBetaMessageClass = "IPM.Note.Microsoft.Exchange.Voice.UM.CA";

		public const string ExchangeUMMessageClass = "IPM.Note.Microsoft.Voicemail.UM.CA";

		public const string ExchangeUMPartnerClass = "IPM.Note.Microsoft.Partner.UM";

		public const string ExchangeUMTranscriptionRequestClass = "IPM.Note.Microsoft.Partner.UM.TranscriptionRequest";

		public const string ExchangeUMProtectedMessageClass = "IPM.Note.rpmsg.Microsoft.Voicemail.UM.CA";

		public const string ExchangeUMProtectedPureAudioClass = "IPM.Note.rpmsg.Microsoft.Voicemail.UM";

		public const string ExchangeUMCDRMessageClass = "IPM.Note.Microsoft.CDR.UM";

		public const string ApprovalNotificationMessageClass = "IPM.Note.Microsoft.Approval.Reply";

		public const string ApprovalApproveNotificationMessageClass = "IPM.Note.Microsoft.Approval.Reply.Approve";

		public const string ApprovalRejectNotificationMessageClass = "IPM.Note.Microsoft.Approval.Reply.Reject";

		public const string ApprovalRequestMessageClass = "IPM.Note.Microsoft.Approval.Request";

		public const string ApprovalMessageClassPrefix = "IPM.Note.Microsoft.Approval";

		public const string ApprovalInitiationMessageClass = "IPM.Microsoft.Approval.Initiation";

		internal const string OutlookWunderBarLinkMessageClass = "IPM.Microsoft.WunderBar.Link";

		internal const string FaxMessageClass = "IPM.Note.Microsoft.Fax";

		internal const string FaxCaMessageClass = "IPM.Note.Microsoft.Fax.CA";

		internal const string MissedCallMessageClass = "IPM.Note.Microsoft.Missed.Voice";

		public const string ExchangeUMVoiceUcClass = "IPM.Note.Microsoft.Conversation.Voice";

		internal const string ExchangeUMMessageSuffix = "Microsoft.Voicemail";

		internal const string ExchangeVoicemailMessageTag = ".Microsoft.Voicemail.";

		internal const string CustomMessageClass = "IPM.Note.Custom";

		public const string InfopathMessageClass = "IPM.InfoPathForm";

		internal const string ReplicationMessageClass = "IPM.Replication";

		internal const string ConversationActionItemMessageClass = "IPM.ConversationAction";

		internal const string SharingMessageClass = "IPM.Sharing";

		internal const string GroupMailboxJoinRequestClass = "IPM.GroupMailbox.JoinRequest";

		internal const string GroupMailboxWelcomeMessageClass = "IPM.Note.GroupMailbox.WelcomeEmail";

		internal const string SharingFolderIndexMessageClass = "IPM.Sharing.Index.In";

		internal const string AggregationPopMessageClass = "IPM.Aggregation.Pop";

		internal const string AggregationDavMessageClass = "IPM.Aggregation.Dav";

		internal const string AggregationDeltaSyncMessageClass = "IPM.Aggregation.DeltaSync";

		internal const string AggregationIMAPMessageClass = "IPM.Aggregation.IMAP";

		internal const string AggregationFacebookMessageClass = "IPM.Aggregation.Facebook";

		internal const string AggregationLinkedInMessageClass = "IPM.Aggregation.LinkedIn";

		internal const string SharingFolderBindingMessageClass = "IPM.Sharing.Binding.In";

		internal const string RssServerLockMessageClass = "IPM.Microsoft.RssLock";

		internal const string AggregationCacheSubscriptionsMessageClass = "IPM.Aggregation.Cache.Subscriptions";

		internal const string RssPostMessageClass = "IPM.Post.RSS";

		internal const string OofMessageClass = "IPM.Note.Rules.OofTemplate.Microsoft";

		internal const string ExternalOofMessageClass = "IPM.Note.Rules.ExternalOofTemplate.Microsoft";

		internal const string FreeBusyDataMessageClass = "IPM.Microsoft.ScheduleData.FreeBusy";

		public const string MmsMessageClass = "IPM.Note.Mobile.MMS";

		public const string SmsMessageClass = "IPM.Note.Mobile.SMS";

		public const string SmsAlertMessageClass = "IPM.Note.Mobile.SMS.Alert";

		public const string SmsAlertCalendarMessageClass = "IPM.Note.Mobile.SMS.Alert.Calendar";

		public const string SmsAlertVoicemailMessageClass = "IPM.Note.Mobile.SMS.Alert.Voicemail";

		public const string SmsUndercurrentMessageClass = "IPM.Note.Mobile.SMS.Undercurrent";

		public const string SmsAlertInfoMessageClass = "IPM.Note.Mobile.SMS.Alert.Info";

		internal const string Document = "IPM.Document";

		internal const string LiveMeetingRequest = "IPM.Appointment.Live Meeting Request";

		internal const string MessageRecallReport = "IPM.Recall.Report";

		internal const string MultimediaMessage = "IPM.Note.Mobile.MMS";

		internal const string Remote = "IPM.Remote";

		internal const string Resend = "IPM.Resend";

		internal const string RuleReplyTemplate = "IPM.Note.Rules.ReplyTemplate.Microsoft";

		internal const string SmimeReceipt = "IPM.Note.RECEIPT.SMIME";

		internal const string TaskAccept = "IPM.TaskRequest.Accept";

		internal const string TaskUpdate = "IPM.TaskRequest.Update";

		internal const string TaskDecline = "IPM.TaskRequest.Decline";

		internal const string UserUserOofSettings = "IPM.Microsoft.OOF.UserOofSettings";

		internal const string ExternalSharingSubscription = "IPM.ExternalSharingSubscription";

		internal const string PublishingSubscription = "IPM.PublishingSubscription";

		internal const string RuleMessage = "IPM.Rule.Message";

		internal const string MiddleTierRuleMessage = "IPM.Rule.Version2.Message";

		internal const string ExtendedRuleMessage = "IPM.ExtendedRule.Message";

		internal const string AuditLog = "IPM.AuditLog";

		internal const string PeopleConnectNotificationConnected = "IPM.Note.PeopleConnect.Notification.Connected";

		internal const string PeopleConnectNotificationDisconnected = "IPM.Note.PeopleConnect.Notification.Disconnected";

		internal const string PeopleConnectNotificationNewTokenNeeded = "IPM.Note.PeopleConnect.Notification.NewTokenNeeded";

		internal const string PeopleConnectNotificationInitialSyncCompleted = "IPM.Note.PeopleConnect.Notification.InitialSyncCompleted";

		internal const string MailboxDiscoverySearchConfiguration = "IPM.Configuration.MailboxDiscoverySearch";

		internal const string MailboxDiscoverySearchRequest = "IPM.Configuration.MailboxDiscoverySearchRequest";

		internal const string MailboxAssociationGroup = "IPM.MailboxAssociation.Group";

		internal const string MailboxAssociationUser = "IPM.MailboxAssociation.User";

		internal const string HierarchySyncMetadata = "IPM.HierarchySync.Metadata";

		internal const string PushNotificationSubscriptionDataItem = "Exchange.PushNotification.Subscription";

		internal const string ToDoMessageClass = "IPM.Note.QuickCapture";

		internal const string ReminderMessageClass = "IPM.Note.Reminder";

		internal const string EventReminderMessageClass = "IPM.Note.Reminder.Event";

		internal const string ModernReminderMessageClass = "IPM.Note.Reminder.Modern";

		internal const string OutlookServiceSubscriptionDataItem = "OutlookService.Notification.Subscription";

		internal const string ConfigurationItemClass = "IPM.Configuration";

		internal const string ParkedMeetingMessage = "IPM.Parked.MeetingMessage";

		public const string GenericFolder = "IPF";

		public const string MessageFolder = "IPF.Note";

		public const string CalendarFolder = "IPF.Appointment";

		public const string ContactFolder = "IPF.Contact";

		public const string TaskFolder = "IPF.Task";

		public const string JournalFolder = "IPF.Journal";

		public const string ShortcutFolder = "IPF.ShortcutFolder";

		public const string StickyNoteFolder = "IPF.StickyNote";

		public const string RecipientCacheFolder = "IPF.Contact.RecipientCache";

		public const string GalContactsFolder = "IPF.Contact.GalContacts";

		public const string ConfigurationFolder = "IPF.Configuration";

		public const string BirthdayCalendarFolder = "IPF.Appointment.Birthday";

		public const string SmsAndChatsSyncFolder = "IPF.SmsAndChatsSync";

		internal const string IPFReminder = "Outlook.Reminder";

		internal const string IPFNoteInfoPathFormFolder = "IPF.Note.InfoPathForm";

		internal const string IPFNoteOutlookHomepage = "IPF.Note.OutlookHomepage";

		internal const string ExchangeUMVoiceMailSearchFolderClass = "IPF.Note.Microsoft.Voicemail";

		internal const string ExchangeUMFaxSearchFolderClass = "IPF.Note.Microsoft.Fax";

		internal const string CommunicatorHistoryFolderClass = "IPF.Note.Microsoft.Conversation";

		internal const string OscContactSync = "IPM.Microsoft.OSC.ContactSync";

		internal const string OscSyncLockPrefix = "IPM.Microsoft.OSC.SyncLock.";

		internal const string OutlookContactLinkTimeStamp = "IPM.Microsoft.ContactLink.TimeStamp";

		internal const string QuickContactsFolder = "IPF.Contact.MOC.QuickContacts";

		internal const string ImContactListFolder = "IPF.Contact.MOC.ImContactList";

		internal const string OrganizationalContactsFolder = "IPF.Contact.OrganizationalContacts";

		private static Dictionary<StoreObjectType, string> tableContainerMessageClass = ObjectClass.BuildContainerMessageClassTable(ObjectClass.tableContainerMessageClass);

		internal static class ReportSuffixes
		{
			internal const string DsnFailed = "NDR";

			internal const string DsnDelivered = "DR";

			internal const string DsnDelayed = "Delayed.DR";

			internal const string DsnRelayed = "Relayed.DR";

			internal const string DsnExpanded = "Expanded.DR";

			internal const string MdnRead = "IPNRN";

			internal const string MdnNotRead = "IPNNRN";
		}

		internal static class ReportClasses
		{
			internal static bool IsReportOfSpecialCasedClass(string itemClass)
			{
				return ObjectClass.IsOfClass(itemClass, "IPM.Note.Exchange.ActiveSync.Report");
			}

			internal const string AirsyncBadItem = "IPM.Note.Exchange.ActiveSync.Report";
		}
	}
}
