using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery
{
	internal class MeetingMessageProcessingTrackingInfo
	{
		public int MeetingMessageProcessingAttempts { get; set; }

		public int CalendarUpdateXsoCodeAttempts { get; set; }

		public MeetingMessageProcessStages Stage { get; set; }

		public bool ProcessingSucceeded { get; set; }

		public string OrgId { get; set; }

		public string Goid { get; set; }

		public MeetingMessageProcessingTrackingInfo(string legacyDN, Guid mbxGuid)
		{
			this.legacyDN = legacyDN;
			this.mbxGuid = mbxGuid;
		}

		public void AddLogMessage(string logMessage)
		{
			if (string.IsNullOrEmpty(logMessage))
			{
				return;
			}
			this.additionalLogMessages.Append("[");
			this.additionalLogMessages.Append(logMessage);
			this.additionalLogMessages.Append("]");
		}

		public void SaveExceptionInfo(Exception ex)
		{
			if (ex == null)
			{
				return;
			}
			this.exceptionsInfo.Append("[");
			this.exceptionsInfo.Append(string.Format("CurrentRetryValues- MeetingMessageProcessingAttempts-{0} CalendarUpdateXSOCodeAttempts-{1} ", this.MeetingMessageProcessingAttempts, this.CalendarUpdateXsoCodeAttempts));
			this.exceptionsInfo.Append(this.GetExceptionInfo(ex));
			if (ex.InnerException != null)
			{
				this.exceptionsInfo.Append(" InnerException - ");
				this.exceptionsInfo.Append(this.GetExceptionInfo(ex.InnerException));
			}
			this.exceptionsInfo.Append("]");
		}

		public List<KeyValuePair<string, string>> GetExtraEventData()
		{
			List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
			list.Add(new KeyValuePair<string, string>("LegacyDN", this.legacyDN));
			if (this.OrgId != null)
			{
				list.Add(new KeyValuePair<string, string>("OrganizationId", this.OrgId));
			}
			if (this.Goid != null)
			{
				list.Add(new KeyValuePair<string, string>("GlobalObjectId", this.Goid));
			}
			list.Add(new KeyValuePair<string, string>("MbxGuid", this.mbxGuid.ToString()));
			list.Add(new KeyValuePair<string, string>("ProcessingSucceeded", this.ProcessingSucceeded.ToString()));
			list.Add(new KeyValuePair<string, string>("ProcessingStage", this.Stage.ToString()));
			list.Add(new KeyValuePair<string, string>("MeetingMessageProcessingAttempts", this.MeetingMessageProcessingAttempts.ToString(CultureInfo.InvariantCulture)));
			list.Add(new KeyValuePair<string, string>("CalendarUpdateXSOCodeAttempts", this.CalendarUpdateXsoCodeAttempts.ToString(CultureInfo.InvariantCulture)));
			this.AddNewEventData("AdditionalInfo", this.additionalLogMessages, list);
			this.AddNewEventData("ExceptionInfo", this.exceptionsInfo, list);
			return list;
		}

		private void AddNewEventData(string key, StringBuilder sbValue, List<KeyValuePair<string, string>> extraEventData)
		{
			string value = sbValue.ToString();
			if (!string.IsNullOrEmpty(value))
			{
				extraEventData.Add(new KeyValuePair<string, string>(key, value));
			}
		}

		private string GetExceptionInfo(Exception ex)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(ex.GetType().Name + " ExceptionMessage " + SpecialCharacters.SanitizeForLogging(ex.Message));
			string stackTrace = ex.StackTrace;
			if (!string.IsNullOrEmpty(stackTrace))
			{
				stringBuilder.Append(" StackTrace - ");
				stringBuilder.Append(SpecialCharacters.SanitizeForLogging(stackTrace));
			}
			return stringBuilder.ToString();
		}

		private const string LegacyDnKey = "LegacyDN";

		private const string MbxGuidKey = "MbxGuid";

		private const string ProcessingSucceededKey = "ProcessingSucceeded";

		private const string ProcessingStageKey = "ProcessingStage";

		private const string MeetingMessageProcessingAttemptsKey = "MeetingMessageProcessingAttempts";

		private const string CalendarUpdateXsoCodeAttemptsKey = "CalendarUpdateXSOCodeAttempts";

		private const string ExceptionsInfoKey = "ExceptionInfo";

		private const string AdditionalInfoKey = "AdditionalInfo";

		private const string OrganizationId = "OrganizationId";

		private const string GlobalObjectId = "GlobalObjectId";

		private StringBuilder exceptionsInfo = new StringBuilder();

		private StringBuilder additionalLogMessages = new StringBuilder();

		private readonly string legacyDN;

		private readonly Guid mbxGuid;
	}
}
