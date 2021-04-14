using System;
using System.Text;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.Powershell.Support
{
	[Serializable]
	public class ResultsPerAttendee
	{
		public ObjectId Identity
		{
			get
			{
				return this.identity;
			}
			set
			{
				if (value != null)
				{
					this.identity = value;
				}
			}
		}

		public SmtpAddress PrimarySMTPAddress { get; set; }

		public bool HasErrors()
		{
			bool flag = false;
			flag |= !string.IsNullOrEmpty(this.ErrorDescription);
			flag |= !string.IsNullOrEmpty(this.WrongStartTime);
			flag |= !string.IsNullOrEmpty(this.WrongEndTime);
			flag |= !string.IsNullOrEmpty(this.WrongLocation);
			flag |= !string.IsNullOrEmpty(this.IntentionalWrongTrackingInfo);
			flag |= !string.IsNullOrEmpty(this.WrongTrackingInfo);
			flag |= !string.IsNullOrEmpty(this.WrongTimeZone);
			flag |= !string.IsNullOrEmpty(this.CantOpen);
			flag |= !string.IsNullOrEmpty(this.Duplicates);
			flag |= !string.IsNullOrEmpty(this.IntentionalMissingMeetings);
			flag |= !string.IsNullOrEmpty(this.MissingMeetings);
			flag |= !string.IsNullOrEmpty(this.RecurrenceProblems);
			flag |= !string.IsNullOrEmpty(this.DelayedUpdatesWrongVersion);
			flag |= !string.IsNullOrEmpty(this.WrongOverlap);
			return flag | !string.IsNullOrEmpty(this.MailboxUnavailable);
		}

		public string ErrorDescription { get; set; }

		public string WrongStartTime { get; set; }

		public string WrongEndTime { get; set; }

		public string WrongLocation { get; set; }

		public string WrongTrackingInfo { get; set; }

		public string IntentionalWrongTrackingInfo { get; set; }

		public string WrongTimeZone { get; set; }

		public string CantOpen { get; set; }

		public string Duplicates { get; set; }

		public string MissingMeetings { get; set; }

		public string IntentionalMissingMeetings { get; set; }

		public string RecurrenceProblems { get; set; }

		public string DelayedUpdatesWrongVersion { get; set; }

		public string WrongOverlap { get; set; }

		public string MailboxUnavailable { get; set; }

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("{0} ", this.PrimarySMTPAddress);
			if (!string.IsNullOrEmpty(this.ErrorDescription))
			{
				stringBuilder.AppendFormat("Error:{0} ", this.ErrorDescription);
			}
			if (!string.IsNullOrEmpty(this.WrongStartTime))
			{
				stringBuilder.AppendFormat("WrongStart:{0} ", this.WrongStartTime);
			}
			if (!string.IsNullOrEmpty(this.WrongEndTime))
			{
				stringBuilder.AppendFormat("WrongEnd:{0} ", this.WrongEndTime);
			}
			if (!string.IsNullOrEmpty(this.WrongLocation))
			{
				stringBuilder.AppendFormat("WrongLocation:{0} ", this.WrongLocation);
			}
			if (!string.IsNullOrEmpty(this.WrongOverlap))
			{
				stringBuilder.AppendFormat("WrongOverlap:{0} ", this.WrongOverlap);
			}
			if (!string.IsNullOrEmpty(this.WrongTimeZone))
			{
				stringBuilder.AppendFormat("WrongTimeZone:{0} ", this.WrongTimeZone);
			}
			return stringBuilder.ToString();
		}

		private ObjectId identity;
	}
}
