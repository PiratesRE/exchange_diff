using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public sealed class ItemClassType
	{
		public static bool IsReportType(string itemType)
		{
			if (itemType == null)
			{
				throw new ArgumentNullException("itemType");
			}
			return itemType.IndexOf("REPORT", StringComparison.OrdinalIgnoreCase) != -1;
		}

		public static bool IsMeetingType(string itemType)
		{
			if (itemType == null)
			{
				throw new ArgumentNullException("itemType");
			}
			return itemType.IndexOf("IPM.Schedule.Meeting", StringComparison.OrdinalIgnoreCase) != -1;
		}

		public static bool IsSmsType(string itemType)
		{
			if (itemType == null)
			{
				throw new ArgumentNullException("itemType");
			}
			return itemType.IndexOf("IPM.Note.Mobile.SMS", StringComparison.OrdinalIgnoreCase) != -1;
		}

		public static string GetDisplayString(string itemType)
		{
			switch (itemType)
			{
			case "IPM.Appointment":
				return LocalizedStrings.GetNonEncoded(-1218353654);
			case "IPM.Note.Microsoft.Approval.Request":
				return LocalizedStrings.GetNonEncoded(-1921998649);
			case "IPM.Note.Microsoft.Approval.Reply.Approve":
				return LocalizedStrings.GetNonEncoded(2134275567);
			case "IPM.Note.Microsoft.Approval.Reply.Reject":
				return LocalizedStrings.GetNonEncoded(617284623);
			case "IPM.Conflict.Folder":
				return LocalizedStrings.GetNonEncoded(-949566239);
			case "IPM.Conflict.Message":
				return LocalizedStrings.GetNonEncoded(-1125004178);
			case "IPM.Contact":
				return LocalizedStrings.GetNonEncoded(1212144717);
			case "REPORT.IPM.Note.DR":
				return LocalizedStrings.GetNonEncoded(-673778217);
			case "REPORT.REPORT.IPM.Note.DR.NDR":
				return LocalizedStrings.GetNonEncoded(-1214411457);
			case "IPM.NOTE.SECURE.SIGN":
				return LocalizedStrings.GetNonEncoded(-1744898725);
			case "IPM.Note.Secure.Sign.Reply":
				return LocalizedStrings.GetNonEncoded(-1638959343);
			case "IPM.DistList":
				return LocalizedStrings.GetNonEncoded(-257188171);
			case "IPM.Document":
				return LocalizedStrings.GetNonEncoded(1894440736);
			case "IPM.Document.Outlook.Template":
				return LocalizedStrings.GetNonEncoded(936058579);
			case "IPM.Note.Exchange.Security.Enrollment":
				return LocalizedStrings.GetNonEncoded(-1728547674);
			case "IPM.Note.Microsoft.Fax.CA":
				return LocalizedStrings.GetNonEncoded(441553720);
			case "IPM.Schedule.Meeting.Resp.Pos":
				return LocalizedStrings.GetNonEncoded(-1480422595);
			case "REPORT.IPM.Schedule.Meeting.Resp.Pos.NDR":
				return LocalizedStrings.GetNonEncoded(1578126677);
			case "IPM.Schedule.Meeting.Canceled":
				return LocalizedStrings.GetNonEncoded(-1395325573);
			case "REPORT.IPM.Schedule.Meeting.Canceled.NDR":
				return LocalizedStrings.GetNonEncoded(2117886819);
			case "IPM.Schedule.Meeting.Resp.Neg":
				return LocalizedStrings.GetNonEncoded(1577758192);
			case "REPORT.IPM.Schedule.Meeting.Resp.Neg.NDR":
				return LocalizedStrings.GetNonEncoded(501159136);
			case "IPM.Schedule.Meeting.Request":
				return LocalizedStrings.GetNonEncoded(715990345);
			case "REPORT.IPM.Schedule.Meeting.Request.DR":
				return LocalizedStrings.GetNonEncoded(1722709255);
			case "REPORT.IPM.Schedule.Meeting.Request.NDR":
				return LocalizedStrings.GetNonEncoded(723650401);
			case "REPORT.IPM.Schedule.Meeting.Request.IPNRN":
				return LocalizedStrings.GetNonEncoded(247246403);
			case "IPM.Schedule.Meeting.Resp.Tent":
				return LocalizedStrings.GetNonEncoded(418650720);
			case "REPORT.IPM.Schedule.Meeting.Resp.Tent.NDR":
				return LocalizedStrings.GetNonEncoded(1657585888);
			case "IPM.Note":
				return LocalizedStrings.GetNonEncoded(375540844);
			case "IPM.Microsoft.Answer":
				return LocalizedStrings.GetNonEncoded(1418057561);
			case "REPORT.IPM.Microsoft.Answer.NDR":
				return LocalizedStrings.GetNonEncoded(-2092364623);
			case "IPM.Document.Microsoft Internet Mail Message":
				return LocalizedStrings.GetNonEncoded(569182364);
			case "REPORT.IPM.Note.NDR":
				return LocalizedStrings.GetNonEncoded(-240308911);
			case "IPM.OCTEL.VOICE":
				return LocalizedStrings.GetNonEncoded(1324666722);
			case "REPORT.IPM.OCTEL.VOICE.NDR":
				return LocalizedStrings.GetNonEncoded(1751917506);
			case "IPM.Note.Rules.OofTemplate.Microsoft":
				return LocalizedStrings.GetNonEncoded(-445260260);
			case "IPM.Outlook.Recall":
				return LocalizedStrings.GetNonEncoded(-1730664879);
			case "IPM.Post":
				return LocalizedStrings.GetNonEncoded(1671058613);
			case "REPORT.IPM.Note.IPNNRN":
				return LocalizedStrings.GetNonEncoded(1317782997);
			case "REPORT.REPORT.IPM.Note.IPNNRN.NDR":
				return LocalizedStrings.GetNonEncoded(-383020123);
			case "REPORT.IPM.Note.IPNRN":
				return LocalizedStrings.GetNonEncoded(-44453782);
			case "IPM.Recall":
				return LocalizedStrings.GetNonEncoded(-1875477896);
			case "IPM.Recall.Report.Failure":
				return LocalizedStrings.GetNonEncoded(-2103299596);
			case "IPM.Recall.Report.Success":
				return LocalizedStrings.GetNonEncoded(-1247003399);
			case "IPM.Sharing":
				return LocalizedStrings.GetNonEncoded(-958660555);
			case "IPM.Note.SMIME":
				return LocalizedStrings.GetNonEncoded(-1021771534);
			case "IPM.Note.SMIME.MultipartSigned":
				return LocalizedStrings.GetNonEncoded(-246092698);
			case "IPM.Note.Mobile.SMS":
				return LocalizedStrings.GetNonEncoded(629771022);
			case "IPM.Task":
				return LocalizedStrings.GetNonEncoded(-2113219524);
			case "IPM.TaskRequest":
				return LocalizedStrings.GetNonEncoded(2041022245);
			case "IPM.TaskRequest.Accept":
				return LocalizedStrings.GetNonEncoded(-791766735);
			case "REPORT.IPM.TaskRequest.Accept.NDR":
				return LocalizedStrings.GetNonEncoded(-1588693551);
			case "IPM.TaskRequest.Decline":
				return LocalizedStrings.GetNonEncoded(1476481161);
			case "REPORT.IPM.TaskRequest.Decline.NDR":
				return LocalizedStrings.GetNonEncoded(61479553);
			case "REPORT.IPM.TaskRequest.NDR":
				return LocalizedStrings.GetNonEncoded(561961253);
			case "IPM.TaskRequest.Update":
				return LocalizedStrings.GetNonEncoded(332739048);
			case "REPORT.IPM.TaskRequest.Update.NDR":
				return LocalizedStrings.GetNonEncoded(-707557640);
			case "IPM.Note.Microsoft.Voicemail.UM":
			case "IPM.Note.Microsoft.Voicemail.UM.CA":
			case "IPM.Note.Microsoft.Exchange.Voice.UM":
			case "IPM.Note.Microsoft.Exchange.Voice.UM.CA":
			case "IPM.Note.rpmsg.Microsoft.Voicemail.UM":
			case "IPM.Note.rpmsg.Microsoft.Voicemail.UM.CA":
				return LocalizedStrings.GetNonEncoded(-1757037630);
			}
			return LocalizedStrings.GetNonEncoded(-1718015515);
		}

		public const string ActiveSyncNote = "IPM.Note.Exchange.ActiveSync";

		public const string Activity = "IPM.Activity";

		public const string Appointment = "IPM.Appointment";

		public const string ADUser = "AD.RecipientType.User";

		public const string ADMailUser = "AD.RecipientType.MailboxUser";

		public const string ADMailEnabledUser = "AD.RecipientType.MailEnabledUser";

		public const string ADContact = "AD.RecipientType.Contact";

		public const string ADMailEnabledContact = "AD.RecipientType.MailEnabledContact";

		public const string ADPublicFolder = "AD.RecipientType.PublicFolder";

		public const string ADGroup = "AD.RecipientType.Group";

		public const string ADDynamicDL = "AD.RecipientType.DynamicDL";

		public const string ADMailEnabledUniversalDistributionGroup = "AD.RecipientType.MailEnabledUniversalDistributionGroup";

		public const string ADMailEnabledUniversalSecurityGroup = "AD.RecipientType.MailEnabledUniversalSecurityGroup";

		public const string ADMailEnabledNonUniversalGroup = "AD.RecipientType.MailEnabledNonUniversalGroup";

		public const string ADPublicDatabase = "AD.RecipientType.PublicDatabase";

		public const string ADAttendantMailbox = "AD.RecipientType.SystemAttendantMailbox";

		public const string ADRoom = "AD.ResourceType.Room";

		public const string ADInvalidUser = "AD.RecipientType.Invalid";

		public const string ApprovalRequest = "IPM.Note.Microsoft.Approval.Request";

		public const string ApprovalReplyApprove = "IPM.Note.Microsoft.Approval.Reply.Approve";

		public const string ApprovalReplyReject = "IPM.Note.Microsoft.Approval.Reply.Reject";

		public const string Contact = "IPM.Contact";

		public const string ConflictFolder = "IPM.Conflict.Folder";

		public const string ConflictMessage = "IPM.Conflict.Message";

		public const string ContentClassDefinition = "IPM.ContentClassDef";

		public const string DeliveryReport = "REPORT.IPM.Note.DR";

		public const string DeliveryReportNDR = "REPORT.REPORT.IPM.Note.DR.NDR";

		public const string EncryptedUnsignedMessage = "IPM.NOTE.SECURE";

		public const string DigitallySignedMessage = "IPM.NOTE.SECURE.SIGN";

		public const string DigitallySignedMessageReply = "IPM.Note.Secure.Sign.Reply";

		public const string DistributionList = "IPM.DistList";

		public const string Document = "IPM.Document";

		public const string DocumentOutlookTemplate = "IPM.Document.Outlook.Template";

		public const string ExchangeSecurityEnrollment = "IPM.Note.Exchange.Security.Enrollment";

		public const string Fax = "IPM.Note.Microsoft.Fax.CA";

		public const string FreeBusyData = "IPM.Microsoft.ScheduleData.FreeBusy";

		public const string E12Beta1Fax = "IPM.Note.Microsoft.Exchange.Fax.CA";

		public const string InfoPathForm = "IPM.InfoPathForm";

		public const string InkNodes = "IPM.InkNodes";

		public const string MeetingAcceptance = "IPM.Schedule.Meeting.Resp.Pos";

		public const string MeetingAcceptanceNDR = "REPORT.IPM.Schedule.Meeting.Resp.Pos.NDR";

		public const string MeetingCancelled = "IPM.Schedule.Meeting.Canceled";

		public const string MeetingCancelledNDR = "REPORT.IPM.Schedule.Meeting.Canceled.NDR";

		public const string MeetingDecline = "IPM.Schedule.Meeting.Resp.Neg";

		public const string MeetingDeclineNDR = "REPORT.IPM.Schedule.Meeting.Resp.Neg.NDR";

		public const string MeetingRequest = "IPM.Schedule.Meeting.Request";

		public const string MeetingRequestDR = "REPORT.IPM.Schedule.Meeting.Request.DR";

		public const string MeetingRequestNDR = "REPORT.IPM.Schedule.Meeting.Request.NDR";

		public const string MeetingRequestReadReceipt = "REPORT.IPM.Schedule.Meeting.Request.IPNRN";

		public const string MeetingTentative = "IPM.Schedule.Meeting.Resp.Tent";

		public const string MeetingTentativeNDR = "REPORT.IPM.Schedule.Meeting.Resp.Tent.NDR";

		public const string Message = "IPM.Note";

		public const string MicrosoftAnswer = "IPM.Microsoft.Answer";

		public const string MicrosoftAnswerNDR = "REPORT.IPM.Microsoft.Answer.NDR";

		public const string MicrosoftInternetMailMessage = "IPM.Document.Microsoft Internet Mail Message";

		public const string NoteNDR = "REPORT.IPM.Note.NDR";

		public const string OctelVoice = "IPM.OCTEL.VOICE";

		public const string OctelVoiceNDR = "REPORT.IPM.OCTEL.VOICE.NDR";

		public const string OofAutoReply = "IPM.Note.Rules.OofTemplate.Microsoft";

		public const string OutlookRecall = "IPM.Outlook.Recall";

		public const string Post = "IPM.Post";

		public const string ReadReceiptFail = "REPORT.IPM.Note.IPNNRN";

		public const string ReadReceiptFailNDR = "REPORT.REPORT.IPM.Note.IPNNRN.NDR";

		public const string ReadReceiptSuccess = "REPORT.IPM.Note.IPNRN";

		public const string Recall = "IPM.Recall";

		public const string RecallFailureReport = "IPM.Recall.Report.Failure";

		public const string RecallSuccessReport = "IPM.Recall.Report.Success";

		public const string RulesNote = "IPM.Note.Rules";

		public const string Sharing = "IPM.Sharing";

		public const string SMIME = "IPM.Note.SMIME";

		public const string SMIMESigned = "IPM.Note.SMIME.MultipartSigned";

		public const string SMS = "IPM.Note.Mobile.SMS";

		public const string Task = "IPM.Task";

		public const string TaskRequest = "IPM.TaskRequest";

		public const string TaskRequestNDR = "REPORT.IPM.TaskRequest.NDR";

		public const string TaskRequestAccept = "IPM.TaskRequest.Accept";

		public const string TaskRequestAcceptNDR = "REPORT.IPM.TaskRequest.Accept.NDR";

		public const string TaskRequestDecline = "IPM.TaskRequest.Decline";

		public const string TaskRequestDeclineNDR = "REPORT.IPM.TaskRequest.Decline.NDR";

		public const string TaskRequestUpdate = "IPM.TaskRequest.Update";

		public const string TaskRequestUpdateNDR = "REPORT.IPM.TaskRequest.Update.NDR";

		public const string VoiceMail = "IPM.Note.Microsoft.Voicemail.UM";

		public const string VoiceMailCA = "IPM.Note.Microsoft.Voicemail.UM.CA";

		public const string E12Beta1VoiceMail = "IPM.Note.Microsoft.Exchange.Voice.UM";

		public const string E12Beta1VoiceMailCA = "IPM.Note.Microsoft.Exchange.Voice.UM.CA";

		public const string ProtectedVoiceMail = "IPM.Note.rpmsg.Microsoft.Voicemail.UM";

		public const string ProtectedVoiceMailCA = "IPM.Note.rpmsg.Microsoft.Voicemail.UM.CA";
	}
}
