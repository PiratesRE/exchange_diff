using System;

namespace Microsoft.Exchange.Data.Transport.Email
{
	internal static class MessageClasses
	{
		public const string IpmNote = "IPM.Note";

		public const string IpmForm = "IPM.Form";

		public const string IpmNoteSMime = "IPM.Note.SMIME";

		public const string SmimeEncryptedSuffix = ".SMIME";

		public const string IpmNoteSMimeMultipartSigned = "IPM.Note.SMIME.MultipartSigned";

		public const string SmimeSignedSuffix = ".SMIME.MultipartSigned";

		public const string IpmNoteSecureSign = "IPM.Note.Secure.Sign";

		public const string IpmNoteSecure = "IPM.Note.Secure";

		public const string IpmAppointment = "IPM.Appointment";

		public const string IpmTaskRequest = "IPM.TaskRequest";

		public const string IpmVoice = "IPM.Note.Microsoft.Voicemail.UM";

		public const string IpmVoiceCa = "IPM.Note.Microsoft.Voicemail.UM.CA";

		public const string IpmVoiceProtectedCa = "IPM.Note.rpmsg.Microsoft.Voicemail.UM.CA";

		public const string IpmVoiceProtected = "IPM.Note.rpmsg.Microsoft.Voicemail.UM";

		public const string IpmFax = "IPM.Note.Microsoft.Fax";

		public const string IpmFaxCa = "IPM.Note.Microsoft.Fax.CA";

		public const string IpmMissedCall = "IPM.Note.Microsoft.Missed.Voice";

		public const string IpmVoiceUc = "IPM.Note.Microsoft.Conversation.Voice";

		public const string IpmUMPartner = "IPM.Note.Microsoft.Partner.UM";

		public const string InfoPathMessageClass = "IPM.InfoPathForm";

		public const string PrefixIpmNoteMobile = "IPM.Note.Mobile.";

		public const string IpmNoteMobileSms = "IPM.Note.Mobile.SMS";

		public const string IpmNoteMobileMms = "IPM.Note.Mobile.MMS";

		internal const string CustomMessageClass = "IPM.Note.Custom";

		public const string IpmReplication = "IPM.Replication";

		public const string IpmConflictMessage = "IPM.Conflict.Message";

		public const string IpmConflictFolder = "IPM.Conflict.Folder";

		public const string IpmOutlookRecall = "IPM.Outlook.Recall";

		public const string PrefixIpmForm = "IPM.Form.";

		public const string PrefixIpmNoteRulesReplyTemplate = "IPM.Note.Rules.ReplyTemplate.";

		public const string PrefixIpmNoteRulesExternalOofTemplate = "IPM.Note.Rules.ExternalOofTemplate.";

		public const string PrefixIpmNoteRulesOofTemplate = "IPM.Note.Rules.OofTemplate.";

		public const string PrefixReportIpmNote = "Report.IPM.Note.";

		public const string SrvInfoExpiry = "SrvInfo.Expiry";

		public const string RssPost = "IPM.Post.RSS";

		public const string IpmSharing = "IPM.Sharing";

		public const string PrefixIpmDocument = "IPM.Document.";

		public const string PrefixIpmRecallReport = "IPM.Recall.Report.";

		public const string PrefixIpmMailbeatBounce = "IPM.Mailbeat.Bounce.";

		public const string IpmNoteStorageQuotaWarning = "IPM.Note.StorageQuotaWarning";

		public const string PrefixIpmNoteStorageQuotaWarning = "IPM.Note.StorageQuotaWarning.";

		public const string IpmNoteStorageQuotaWarningWarning = "IPM.Note.StorageQuotaWarning.Warning";

		public const string IpmNoteStorageQuotaWarningSend = "IPM.Note.StorageQuotaWarning.Send";

		public const string IpmNoteStorageQuotaWarningSendReceive = "IPM.Note.StorageQuotaWarning.SendReceive";

		public const string PrefixIpmScheduleMeeting = "IPM.Schedule.Meeting.";

		public const string IpmScheduleMeetingRequest = "IPM.Schedule.Meeting.Request";

		public const string IpmScheduleMeetingRespNeg = "IPM.Schedule.Meeting.Resp.Neg";

		public const string IpmScheduleMeetingRespPos = "IPM.Schedule.Meeting.Resp.Pos";

		public const string IpmScheduleMeetingRespTent = "IPM.Schedule.Meeting.Resp.Tent";

		public const string IpmScheduleMeetingCanceled = "IPM.Schedule.Meeting.Canceled";

		public const string ApprovalInitiationMessageClass = "IPM.Microsoft.Approval.Initiation";

		public const string PrefixApprovalMessageClass = "IPM.Note.Microsoft.Approval.";

		public static class MobileMessageSuffix
		{
			public const string ShortMessage = "SMS";

			public const string MultimediaMessage = "MMS";
		}

		public static class ReportSuffix
		{
			public const string DsnFailed = "NDR";

			public const string DsnDelivered = "DR";

			public const string DsnDelayed = "Delayed.DR";

			public const string DsnRelayed = "Relayed.DR";

			public const string DsnExpanded = "Expanded.DR";

			public const string MdnRead = "IPNRN";

			public const string MdnNotRead = "IPNNRN";
		}

		public static class RecallReportSuffix
		{
			public const string Success = "Success";

			public const string Failure = "Failure";
		}

		public static class MailbeatBounceSuffix
		{
			public const string Reply = "Reply";

			public const string Request = "Request";
		}

		public static class StorageQuotaWarningSuffix
		{
			public const string Warning = "Warning";

			public const string Send = "Send";

			public const string SendReceive = "SendReceive";
		}

		public static class ScheduleSuffix
		{
			public const string Request = "Request";

			public const string Canceled = "Canceled";

			public const string RespNeg = "Resp.Neg";

			public const string RespPos = "Resp.Pos";

			public const string RespTent = "Resp.Tent";
		}
	}
}
