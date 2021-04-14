using System;

namespace Microsoft.Exchange.Server.Storage.PropTags
{
	public static class MessagePropGroups
	{
		public const ulong Group0Mask = 1UL;

		public const ulong Group1Mask = 2UL;

		public const ulong Group2Mask = 4UL;

		public const ulong Group3Mask = 8UL;

		public const ulong Group4Mask = 16UL;

		public const ulong Group5Mask = 32UL;

		public const ulong Group6Mask = 64UL;

		public const ulong Group7Mask = 128UL;

		public const ulong Group8Mask = 256UL;

		public const ulong Group9Mask = 512UL;

		public const ulong Group10Mask = 1024UL;

		public const ulong Group11Mask = 2048UL;

		public const ulong Group12Mask = 4096UL;

		public const ulong Group13Mask = 8192UL;

		public const ulong Group14Mask = 16384UL;

		public const ulong Group15Mask = 32768UL;

		public const ulong Group16Mask = 65536UL;

		public const ulong Group17Mask = 131072UL;

		public const ulong Group18Mask = 262144UL;

		public const ulong Group19Mask = 524288UL;

		public const ulong Group20Mask = 1048576UL;

		public const ulong Group21Mask = 2097152UL;

		public const ulong Group22Mask = 4194304UL;

		public const ulong Group23Mask = 8388608UL;

		public const ulong GroupReminderMask = 4611686018427387904UL;

		public const ulong GroupContentIndexMask = 2305843009213693952UL;

		public const ulong GroupAntivirusMask = 1152921504606846976UL;

		public const ulong GroupRetentionTagMask = 576460752303423488UL;

		public const ulong GroupRetentionMask = 288230376151711744UL;

		public const ulong GroupAppointmentTimeMask = 144115188075855872UL;

		public const ulong GroupAppointmentFreeBusyMask = 72057594037927936UL;

		public const ulong GroupCommonCategoryPropsMask = 36028797018963968UL;

		public const ulong GroupModernRemindersMask = 18014398509481984UL;

		public const ulong GroupTimerEventPropsMask = 9007199254740992UL;

		public const ulong OtherGroupMask = 9223372036854775808UL;

		public const int NumberedGroupCount = 24;

		public const ulong AllNumberedGroupsMask = 16777215UL;

		public static readonly int CurrentGroupMappingId = 33685505;

		public static readonly ulong[] NumberedGroupMasks = new ulong[]
		{
			1UL,
			2UL,
			4UL,
			8UL,
			16UL,
			32UL,
			64UL,
			128UL,
			256UL,
			512UL,
			1024UL,
			2048UL,
			4096UL,
			8192UL,
			16384UL,
			32768UL,
			65536UL,
			131072UL,
			262144UL,
			524288UL,
			1048576UL,
			2097152UL,
			4194304UL,
			8388608UL
		};

		public static readonly StorePropInfo[][] NumberedGroupLists = new StorePropInfo[][]
		{
			new StorePropInfo[]
			{
				PropTag.Message.AlternateRecipientAllowed.PropInfo,
				PropTag.Message.ChangeKey.PropInfo,
				PropTag.Message.ClientSubmitTime.PropInfo,
				PropTag.Message.CodePageId.PropInfo,
				PropTag.Message.CreationTime.PropInfo,
				PropTag.Message.DeliveryOrRenewTime.PropInfo,
				PropTag.Message.DisableFullFidelity.PropInfo,
				PropTag.Message.ELCAutoCopyTag.PropInfo,
				PropTag.Message.ELCMoveDate.PropInfo,
				PropTag.Message.ExtendedRuleActions.PropInfo,
				PropTag.Message.ImapCachedMsgSize.PropInfo,
				PropTag.Message.IMAPId.PropInfo,
				PropTag.Message.InternetArticleNumber.PropInfo,
				PropTag.Message.InternetCPID.PropInfo,
				PropTag.Message.LastModificationTime.PropInfo,
				PropTag.Message.LocalCommitTime.PropInfo,
				PropTag.Message.MessageCCMe.PropInfo,
				PropTag.Message.MessageCodePage.PropInfo,
				PropTag.Message.MessageDeliveryTime.PropInfo,
				PropTag.Message.MessageLocaleId.PropInfo,
				PropTag.Message.MessageRecipMe.PropInfo,
				PropTag.Message.MessageSize32.PropInfo,
				PropTag.Message.MessageToMe.PropInfo,
				PropTag.Message.PredecessorChangeList.PropInfo,
				PropTag.Message.RenewTime.PropInfo,
				PropTag.Message.SearchKey.PropInfo,
				PropTag.Message.SearchKeySvrEid.PropInfo,
				PropTag.Message.SecureSubmitFlags.PropInfo,
				PropTag.Message.TrustSender.PropInfo,
				PropTag.Message.VID.PropInfo,
				PropTag.Message.VirusScannerStamp.PropInfo
			},
			new StorePropInfo[]
			{
				PropTag.Message.BlockStatus.PropInfo,
				PropTag.Message.FlagStatus.PropInfo,
				PropTag.Message.FollowupIcon.PropInfo,
				PropTag.Message.HasAttach.PropInfo,
				PropTag.Message.IconIndex.PropInfo,
				PropTag.Message.IsIRMMessage.PropInfo,
				PropTag.Message.IsReadColumn.PropInfo,
				PropTag.Message.LastVerbExecuted.PropInfo,
				PropTag.Message.LastVerbExecutionTime.PropInfo,
				PropTag.Message.MessageFlags.PropInfo,
				PropTag.Message.NonReceiptNotificationRequested.PropInfo,
				PropTag.Message.Preview.PropInfo,
				PropTag.Message.Read.PropInfo,
				PropTag.Message.ReadCnNewExport.PropInfo,
				PropTag.Message.ReadReceiptRequested.PropInfo,
				PropTag.Message.ReplyTime.PropInfo,
				PropTag.Message.SubmitFlags.PropInfo,
				PropTag.Message.SubmitResponsibility.PropInfo,
				NamedPropInfo.Common.CurrentVersion,
				NamedPropInfo.Common.CurrentVersionName
			},
			new StorePropInfo[]
			{
				PropTag.Message.DeliveryReportRequested.PropInfo,
				PropTag.Message.Importance.PropInfo,
				PropTag.Message.Priority.PropInfo,
				PropTag.Message.Sensitivity.PropInfo,
				NamedPropInfo.Common.LastAuthorClass,
				NamedPropInfo.Common.SideEffects
			},
			new StorePropInfo[]
			{
				PropTag.Message.BodyHtml.PropInfo,
				PropTag.Message.BodyHtmlUnicode.PropInfo,
				PropTag.Message.BodyUnicode.PropInfo,
				PropTag.Message.NativeBodyInfo.PropInfo,
				PropTag.Message.RtfCompressed.PropInfo,
				PropTag.Message.RTFInSync.PropInfo
			},
			new StorePropInfo[]
			{
				PropTag.Message.DisplayBcc.PropInfo,
				PropTag.Message.DisplayCc.PropInfo,
				PropTag.Message.DisplayTo.PropInfo,
				PropTag.Message.MessageRecipients.PropInfo,
				PropTag.Message.ReplyRecipientEntries.PropInfo,
				NamedPropInfo.Appointment.AllAttendeesString,
				NamedPropInfo.Appointment.ApptUnsendableRecips,
				NamedPropInfo.Appointment.CCAttendeesString,
				NamedPropInfo.Appointment.ToAttendeesString
			},
			new StorePropInfo[]
			{
				PropTag.Message.MessageAttachList.PropInfo,
				PropTag.Message.MessageAttachments.PropInfo
			},
			new StorePropInfo[]
			{
				NamedPropInfo.Common.ReminderNextTime,
				NamedPropInfo.Common.ReminderSet,
				NamedPropInfo.Common.ReminderTime,
				NamedPropInfo.Task.TaskDateCompleted,
				NamedPropInfo.Task.TaskDueDate
			},
			new StorePropInfo[]
			{
				PropTag.Message.ConversationFamilyId.PropInfo,
				PropTag.Message.ConversationId.PropInfo,
				PropTag.Message.ConversationIndex.PropInfo,
				PropTag.Message.ConversationIndexTracking.PropInfo,
				PropTag.Message.ConversationItemConversationId.PropInfo,
				PropTag.Message.ConversationTopic.PropInfo,
				PropTag.Message.DisplayName.PropInfo,
				PropTag.Message.MessageClass.PropInfo,
				PropTag.Message.NormalizedSubject.PropInfo,
				PropTag.Message.SenderEmailAddress.PropInfo,
				PropTag.Message.SenderName.PropInfo,
				PropTag.Message.SenderTelephoneNumber.PropInfo,
				PropTag.Message.SentRepresentingEmailAddress.PropInfo,
				PropTag.Message.SentRepresentingName.PropInfo,
				PropTag.Message.Subject.PropInfo,
				PropTag.Message.SubjectPrefix.PropInfo,
				NamedPropInfo.InternetHeaders.ContentClass,
				NamedPropInfo.Messaging.MessageBccMe,
				NamedPropInfo.UnifiedMessaging.PstnCallbackTelephoneNumber,
				NamedPropInfo.Sharing.SharingInstanceGuid
			},
			new StorePropInfo[]
			{
				PropTag.Message.Account.PropInfo,
				PropTag.Message.Assistant.PropInfo,
				PropTag.Message.AssistantTelephoneNumber.PropInfo,
				PropTag.Message.Birthday.PropInfo,
				PropTag.Message.Business2TelephoneNumber.PropInfo,
				PropTag.Message.BusinessFaxNumber.PropInfo,
				PropTag.Message.BusinessHomePage.PropInfo,
				PropTag.Message.BusinessTelephoneNumber.PropInfo,
				PropTag.Message.CallbackTelephoneNumber.PropInfo,
				PropTag.Message.CarTelephoneNumber.PropInfo,
				PropTag.Message.ChildrensNames.PropInfo,
				PropTag.Message.CompanyMainPhoneNumber.PropInfo,
				PropTag.Message.CompanyName.PropInfo,
				PropTag.Message.Country.PropInfo,
				PropTag.Message.CustomerId.PropInfo,
				PropTag.Message.DepartmentName.PropInfo,
				PropTag.Message.DiscoveryAnnotation.PropInfo,
				PropTag.Message.DisplayNamePrefix.PropInfo,
				PropTag.Message.EmailAddress.PropInfo,
				PropTag.Message.Generation.PropInfo,
				PropTag.Message.GivenName.PropInfo,
				PropTag.Message.GovernmentIDNumber.PropInfo,
				PropTag.Message.Home2TelephoneNumber.PropInfo,
				PropTag.Message.HomeAddressCity.PropInfo,
				PropTag.Message.HomeAddressCountry.PropInfo,
				PropTag.Message.HomeAddressPostalCode.PropInfo,
				PropTag.Message.HomeAddressStateOrProvince.PropInfo,
				PropTag.Message.HomeAddressStreet.PropInfo,
				PropTag.Message.HomeFaxNumber.PropInfo,
				PropTag.Message.HomeTelephoneNumber.PropInfo,
				PropTag.Message.Locality.PropInfo,
				PropTag.Message.ManagerName.PropInfo,
				PropTag.Message.MiddleName.PropInfo,
				PropTag.Message.MobileTelephoneNumber.PropInfo,
				PropTag.Message.NickName.PropInfo,
				PropTag.Message.OfficeLocation.PropInfo,
				PropTag.Message.OtherAddressCity.PropInfo,
				PropTag.Message.OtherAddressCountry.PropInfo,
				PropTag.Message.OtherAddressPostalCode.PropInfo,
				PropTag.Message.OtherAddressStateOrProvince.PropInfo,
				PropTag.Message.OtherAddressStreet.PropInfo,
				PropTag.Message.PagerTelephoneNumber.PropInfo,
				PropTag.Message.PostalAddress.PropInfo,
				PropTag.Message.PostalCode.PropInfo,
				PropTag.Message.PostOfficeBox.PropInfo,
				PropTag.Message.PrimaryTelephoneNumber.PropInfo,
				PropTag.Message.RadioTelephoneNumber.PropInfo,
				PropTag.Message.SpouseName.PropInfo,
				PropTag.Message.StateOrProvince.PropInfo,
				PropTag.Message.StreetAddress.PropInfo,
				PropTag.Message.SurName.PropInfo,
				PropTag.Message.Title.PropInfo,
				PropTag.Message.WeddingAnniversary.PropInfo,
				NamedPropInfo.AirSync.ASIMAddress2,
				NamedPropInfo.AirSync.ASIMAddress3,
				NamedPropInfo.AirSync.ASMMS,
				NamedPropInfo.Common.Billing,
				NamedPropInfo.Common.Companies,
				NamedPropInfo.Common.Contacts,
				NamedPropInfo.Address.Email2DisplayName,
				NamedPropInfo.Address.Email2EmailAddress,
				NamedPropInfo.Address.Email2OriginalDisplayName,
				NamedPropInfo.Address.Email3DisplayName,
				NamedPropInfo.Address.Email3EmailAddress,
				NamedPropInfo.Address.Email3OriginalDisplayName,
				NamedPropInfo.Address.EmailDisplayName,
				NamedPropInfo.Address.EmailEmailAddress,
				NamedPropInfo.Address.EmailOriginalDisplayName,
				NamedPropInfo.PublicStrings.FileAs,
				NamedPropInfo.Address.FileUnder,
				NamedPropInfo.Address.HomeAddress,
				NamedPropInfo.Address.HTML,
				NamedPropInfo.Address.InstMsg,
				NamedPropInfo.PublicStrings.Keywords,
				NamedPropInfo.Log.LogType,
				NamedPropInfo.Address.OtherAddress,
				NamedPropInfo.Sharing.SharingBrowseUrl,
				NamedPropInfo.Sharing.SharingInitiatorName,
				NamedPropInfo.Sharing.SharingLocalName,
				NamedPropInfo.Sharing.SharingRemoteComment,
				NamedPropInfo.Sharing.SharingRemoteName,
				NamedPropInfo.Sharing.SharingRemotePath,
				NamedPropInfo.Task.TaskOwner,
				NamedPropInfo.Common.ToDoTitle,
				NamedPropInfo.UnifiedMessaging.UMAudioNotes,
				NamedPropInfo.Address.WorkAddress,
				NamedPropInfo.Address.WorkAddressCity,
				NamedPropInfo.Address.WorkAddressCountry,
				NamedPropInfo.Address.WorkAddressPostalCode,
				NamedPropInfo.Address.WorkAddressState,
				NamedPropInfo.Address.WorkAddressStreet,
				NamedPropInfo.Address.YomiCompanyName,
				NamedPropInfo.Address.YomiFirstName,
				NamedPropInfo.Address.YomiLastName
			},
			new StorePropInfo[]
			{
				NamedPropInfo.Common.ContactLinkName,
				NamedPropInfo.Common.Request,
				NamedPropInfo.Common.UberGroup
			},
			new StorePropInfo[]
			{
				PropTag.Message.InternetContent.PropInfo,
				NamedPropInfo.PublicStrings.DRMServerLicenseCompressed
			},
			new StorePropInfo[]
			{
				NamedPropInfo.Appointment.ApptLastSequence,
				NamedPropInfo.Appointment.ApptSeqTime,
				NamedPropInfo.Appointment.ApptSequence,
				NamedPropInfo.Appointment.ChangeHighlight,
				NamedPropInfo.Appointment.FInvited,
				NamedPropInfo.Meeting.OwnerCriticalChange
			},
			new StorePropInfo[]
			{
				PropTag.Message.ReplyRequested.PropInfo,
				PropTag.Message.ResponseRequested.PropInfo
			},
			new StorePropInfo[]
			{
				NamedPropInfo.Appointment.ApptReplyName,
				NamedPropInfo.Appointment.ApptReplyTime,
				NamedPropInfo.Meeting.AttendeeCriticalChange,
				NamedPropInfo.Appointment.ResponseStatus
			},
			new StorePropInfo[]
			{
				NamedPropInfo.Appointment.ApptDuration,
				NamedPropInfo.Appointment.ApptEndWhole,
				NamedPropInfo.Appointment.ApptNotAllowPropose,
				NamedPropInfo.Appointment.ApptStartWhole,
				NamedPropInfo.Appointment.ApptSubType,
				NamedPropInfo.Appointment.ClipEnd,
				NamedPropInfo.Appointment.ClipStart,
				NamedPropInfo.Meeting.GlobalObjId,
				NamedPropInfo.PublicStrings.OnlineMeetingConfLink,
				NamedPropInfo.PublicStrings.OnlineMeetingExternalLink,
				NamedPropInfo.Appointment.RecurPattern,
				NamedPropInfo.Appointment.Recurring,
				NamedPropInfo.Appointment.RecurType,
				NamedPropInfo.Meeting.WhenProp
			},
			new StorePropInfo[]
			{
				NamedPropInfo.Common.ApptExtractTime,
				NamedPropInfo.Common.ApptExtractVersion
			},
			new StorePropInfo[]
			{
				NamedPropInfo.Appointment.ApptRecur
			},
			new StorePropInfo[]
			{
				NamedPropInfo.Appointment.Location,
				NamedPropInfo.Meeting.OldLocation,
				NamedPropInfo.Meeting.Where
			},
			new StorePropInfo[]
			{
				NamedPropInfo.Appointment.ApptAuxFlags,
				NamedPropInfo.Appointment.ApptStateFlags,
				NamedPropInfo.Appointment.BusyStatus,
				NamedPropInfo.Appointment.IntendedBusyStatus,
				NamedPropInfo.Appointment.OwnerName
			},
			new StorePropInfo[]
			{
				NamedPropInfo.Meeting.TimeZone,
				NamedPropInfo.Appointment.TimeZoneDesc,
				NamedPropInfo.Appointment.TimeZoneStruct
			},
			new StorePropInfo[]
			{
				PropTag.Message.ArchiveTag.PropInfo,
				PropTag.Message.PolicyTag.PropInfo
			},
			new StorePropInfo[]
			{
				PropTag.Message.ArchiveDate.PropInfo,
				PropTag.Message.ArchivePeriod.PropInfo,
				PropTag.Message.RetentionDate.PropInfo,
				PropTag.Message.RetentionFlags.PropInfo,
				PropTag.Message.RetentionPeriod.PropInfo,
				PropTag.Message.StartDateEtc.PropInfo
			},
			new StorePropInfo[]
			{
				PropTag.Message.BodyTag.PropInfo
			},
			new StorePropInfo[]
			{
				NamedPropInfo.CalendarAssistant.CalendarLogTriggerAction,
				NamedPropInfo.CalendarAssistant.ChangeList,
				NamedPropInfo.CalendarAssistant.ClientBuildVersion,
				NamedPropInfo.CalendarAssistant.ClientInfoString,
				NamedPropInfo.CalendarAssistant.ClientIntent,
				NamedPropInfo.CalendarAssistant.ClientMachineName,
				NamedPropInfo.CalendarAssistant.ClientProcessName,
				NamedPropInfo.CalendarAssistant.ItemVersion,
				NamedPropInfo.CalendarAssistant.MailboxDatabaseName,
				NamedPropInfo.CalendarAssistant.MiddleTierProcessName,
				NamedPropInfo.CalendarAssistant.MiddleTierServerBuildVersion,
				NamedPropInfo.CalendarAssistant.MiddleTierServerName,
				NamedPropInfo.CalendarAssistant.OriginalCreationTime,
				NamedPropInfo.CalendarAssistant.OriginalEntryId,
				NamedPropInfo.CalendarAssistant.OriginalFolderId,
				NamedPropInfo.CalendarAssistant.OriginalLastModifiedTime,
				NamedPropInfo.CalendarAssistant.ResponsibleUserName,
				NamedPropInfo.CalendarAssistant.ServerBuildVersion,
				NamedPropInfo.CalendarAssistant.ServerName
			}
		};
	}
}
