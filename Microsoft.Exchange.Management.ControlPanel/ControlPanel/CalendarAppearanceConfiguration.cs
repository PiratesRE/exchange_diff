using System;
using System.Globalization;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.ControlPanel.WebControls;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class CalendarAppearanceConfiguration : CalendarConfigurationBase
	{
		public CalendarAppearanceConfiguration(MailboxCalendarConfiguration mailboxCalendarConfiguration) : base(mailboxCalendarConfiguration)
		{
			this.workingHoursTimeZone = base.MailboxCalendarConfiguration.WorkingHoursTimeZone.ExTimeZone;
			this.currentUserTimeZone = RbacPrincipal.Current.UserTimeZone;
		}

		[DataMember]
		public int WorkDays
		{
			get
			{
				return (int)base.MailboxCalendarConfiguration.WorkDays;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public Identity WorkingHoursStartTime
		{
			get
			{
				return this.WorkingHoursTime((int)base.MailboxCalendarConfiguration.WorkingHoursStartTime.TotalMinutes);
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public Identity WorkingHoursEndTime
		{
			get
			{
				return this.WorkingHoursTime((int)base.MailboxCalendarConfiguration.WorkingHoursEndTime.TotalMinutes);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string WorkingHoursTimeZone
		{
			get
			{
				return string.Format(OwaOptionStrings.TimeZoneLabelText, RtlUtil.ConvertToBidiString(this.workingHoursTimeZone.LocalizableDisplayName.ToString(CultureInfo.CurrentCulture), RtlUtil.IsRtl));
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool IsTimeZoneDifferent
		{
			get
			{
				return this.currentUserTimeZone != null && this.workingHoursTimeZone.Id != this.currentUserTimeZone.Id;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string UpdateTimeZoneNoteLink
		{
			get
			{
				ExTimeZone exTimeZone = (this.currentUserTimeZone == null) ? ExTimeZone.CurrentTimeZone : this.currentUserTimeZone;
				string arg = RtlUtil.ConvertToBidiString(exTimeZone.LocalizableDisplayName.ToString(CultureInfo.CurrentCulture), RtlUtil.IsRtl);
				return string.Format(OwaOptionStrings.UpdateTimeZoneNoteLinkText, arg);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public int WeekStartDay
		{
			get
			{
				return (int)base.MailboxCalendarConfiguration.WeekStartDay;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public int FirstWeekOfYear
		{
			get
			{
				if (base.MailboxCalendarConfiguration.FirstWeekOfYear != FirstWeekRules.LegacyNotSet)
				{
					return (int)base.MailboxCalendarConfiguration.FirstWeekOfYear;
				}
				return 1;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool ShowWeekNumbers
		{
			get
			{
				return base.MailboxCalendarConfiguration.ShowWeekNumbers;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string TimeIncrement
		{
			get
			{
				return ((int)base.MailboxCalendarConfiguration.TimeIncrement).ToString();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		private Identity WorkingHoursTime(int minutes)
		{
			DateTime dateTime = DateTime.UtcNow.Date + TimeSpan.FromMinutes((double)minutes);
			return new Identity(minutes.ToString(), dateTime.ToString(RbacPrincipal.Current.TimeFormat));
		}

		private ExTimeZone workingHoursTimeZone;

		private ExTimeZone currentUserTimeZone;
	}
}
