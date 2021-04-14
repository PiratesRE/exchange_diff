using System;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery
{
	internal class CalendarSettings
	{
		public CalendarSettings(CalendarFlags flags, int reminderTime)
		{
			this.settingsFlags = flags;
			this.defaultReminderTime = reminderTime;
		}

		public CalendarFlags Flags
		{
			get
			{
				return this.settingsFlags;
			}
			set
			{
				this.settingsFlags = value;
			}
		}

		public bool AutomaticBooking
		{
			get
			{
				return (this.settingsFlags & CalendarFlags.AutoBooking) == CalendarFlags.AutoBooking;
			}
		}

		public bool CalendarAssistantActive
		{
			get
			{
				return (this.settingsFlags & CalendarFlags.CalendarAssistantActive) == CalendarFlags.CalendarAssistantActive;
			}
		}

		public bool CalendarAssistantNoiseReduction
		{
			get
			{
				return (this.settingsFlags & CalendarFlags.CalendarAssistantNoiseReduction) == CalendarFlags.CalendarAssistantNoiseReduction;
			}
		}

		public bool CalendarAssistantAddNewItems
		{
			get
			{
				return (this.settingsFlags & CalendarFlags.CalendarAssistantAddNewItems) == CalendarFlags.CalendarAssistantAddNewItems;
			}
		}

		public bool CalendarAssistantProcessExternal
		{
			get
			{
				return (this.settingsFlags & CalendarFlags.CalendarAssistantProcessExternal) == CalendarFlags.CalendarAssistantProcessExternal;
			}
		}

		public bool SkipProcessing
		{
			get
			{
				return (this.settingsFlags & CalendarFlags.SkipProcessing) == CalendarFlags.SkipProcessing;
			}
		}

		public int DefaultReminderTime
		{
			get
			{
				return this.defaultReminderTime;
			}
			set
			{
				this.defaultReminderTime = value;
			}
		}

		private CalendarFlags settingsFlags;

		private int defaultReminderTime;
	}
}
