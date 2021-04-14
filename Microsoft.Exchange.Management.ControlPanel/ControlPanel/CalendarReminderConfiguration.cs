using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class CalendarReminderConfiguration : CalendarConfigurationBase
	{
		public CalendarReminderConfiguration(MailboxCalendarConfiguration mailboxCalendarConfiguration) : base(mailboxCalendarConfiguration)
		{
		}

		[DataMember]
		public bool RemindersEnabled
		{
			get
			{
				return base.MailboxCalendarConfiguration.RemindersEnabled;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool ReminderSoundEnabled
		{
			get
			{
				return base.MailboxCalendarConfiguration.ReminderSoundEnabled;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public Identity DefaultReminderTime
		{
			get
			{
				return this.ReminderTime((int)base.MailboxCalendarConfiguration.DefaultReminderTime.TotalMinutes);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		private Identity ReminderTime(int totalMinutes)
		{
			int num = totalMinutes / 10080;
			int num2 = totalMinutes % 10080 / 1440;
			int num3 = totalMinutes % 10080 % 1440 / 60;
			int num4 = totalMinutes % 10080 % 1440 % 60;
			string text = (num > 0) ? string.Format("{0}{1}{0}", RtlUtil.DecodedDirectionMark, string.Format("{0} {1} ", num, (num > 1) ? OwaOptionStrings.Weeks : OwaOptionStrings.Week)) : string.Empty;
			string text2 = (num2 > 0) ? string.Format("{0}{1}{0}", RtlUtil.DecodedDirectionMark, string.Format("{0} {1} ", num2, (num2 > 1) ? OwaOptionStrings.Days : OwaOptionStrings.Day)) : string.Empty;
			string text3 = (num3 > 0) ? string.Format("{0}{1}{0}", RtlUtil.DecodedDirectionMark, string.Format("{0} {1} ", num3, (num3 > 1) ? OwaOptionStrings.Hours : OwaOptionStrings.Hour)) : string.Empty;
			string text4 = (num4 > 0) ? string.Format("{0}{1}{0}", RtlUtil.DecodedDirectionMark, string.Format("{0} {1} ", num4, (num4 > 1) ? OwaOptionStrings.Minutes : OwaOptionStrings.Minute)) : string.Empty;
			return new Identity(totalMinutes.ToString(), string.Format("{0}{1}{2}{3}", new object[]
			{
				text,
				text2,
				text3,
				text4
			}));
		}
	}
}
