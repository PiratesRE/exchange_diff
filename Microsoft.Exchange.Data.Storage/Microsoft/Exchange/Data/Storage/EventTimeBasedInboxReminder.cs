using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[DataContract]
	internal class EventTimeBasedInboxReminder : IReminder
	{
		public EventTimeBasedInboxReminder()
		{
			this.Initialize();
		}

		[DataMember]
		public Guid Identifier { get; set; }

		[DataMember]
		public int ReminderOffset
		{
			get
			{
				return this.reminderOffset;
			}
			set
			{
				if (value < -2628000 || value > 2628000)
				{
					throw new InvalidParamException(ServerStrings.InvalidReminderOffset(value, -2628000, 2628000));
				}
				this.reminderOffset = value;
			}
		}

		[DataMember]
		public string CustomMessage
		{
			get
			{
				return this.customMessage;
			}
			set
			{
				this.customMessage = (value ?? string.Empty);
				if (this.customMessage.Length > 1024)
				{
					throw new InvalidParamException(ServerStrings.CustomMessageLengthExceeded);
				}
			}
		}

		[DataMember]
		public bool IsOrganizerReminder { get; set; }

		[DataMember]
		public EmailReminderChangeType OccurrenceChange { get; set; }

		[DataMember]
		public Guid SeriesReminderId { get; set; }

		public bool IsFormerSeriesReminder
		{
			get
			{
				return this.OccurrenceChange == EmailReminderChangeType.Override || this.OccurrenceChange == EmailReminderChangeType.Deleted;
			}
		}

		public bool IsActiveExceptionReminder
		{
			get
			{
				return this.OccurrenceChange == EmailReminderChangeType.Added || this.OccurrenceChange == EmailReminderChangeType.Override;
			}
		}

		public static void UpdateIdentifiersForModifiedReminders(Reminders<EventTimeBasedInboxReminder> reminders)
		{
			if (reminders != null)
			{
				foreach (EventTimeBasedInboxReminder eventTimeBasedInboxReminder in reminders.ReminderList)
				{
					if (eventTimeBasedInboxReminder.IsFormerSeriesReminder && eventTimeBasedInboxReminder.SeriesReminderId == Guid.Empty)
					{
						eventTimeBasedInboxReminder.SeriesReminderId = eventTimeBasedInboxReminder.Identifier;
						eventTimeBasedInboxReminder.Identifier = Guid.NewGuid();
					}
				}
			}
		}

		public static EventTimeBasedInboxReminder GetSeriesReminder(Reminders<EventTimeBasedInboxReminder> reminders, Guid reminderId)
		{
			foreach (EventTimeBasedInboxReminder eventTimeBasedInboxReminder in reminders.ReminderList)
			{
				Guid a = eventTimeBasedInboxReminder.IsFormerSeriesReminder ? eventTimeBasedInboxReminder.SeriesReminderId : eventTimeBasedInboxReminder.Identifier;
				if (a == reminderId)
				{
					return eventTimeBasedInboxReminder;
				}
			}
			return null;
		}

		[OnDeserializing]
		public void OnDeserializing(StreamingContext context)
		{
			this.Initialize();
		}

		public int GetCurrentVersion()
		{
			return 3;
		}

		private void Initialize()
		{
			this.ReminderOffset = 10080;
			this.CustomMessage = string.Empty;
			this.IsOrganizerReminder = false;
			this.OccurrenceChange = EmailReminderChangeType.None;
			this.SeriesReminderId = Guid.Empty;
		}

		private const int DefaultReminderOffset = 10080;

		private const int MinReminderOffset = -2628000;

		private const int MaxReminderOffset = 2628000;

		private const int MaxCustomMessageLength = 1024;

		private const int CurrentVersion = 3;

		private int reminderOffset;

		private string customMessage;
	}
}
