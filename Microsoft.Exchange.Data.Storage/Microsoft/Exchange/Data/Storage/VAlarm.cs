using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.ContentTypes.iCalendar;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class VAlarm : CalendarComponentBase
	{
		internal VAlarm(CalendarComponentBase root) : base(root)
		{
		}

		internal TimeSpan TimeSpan
		{
			get
			{
				if (this.valueType != CalendarValueType.Duration)
				{
					throw new InvalidOperationException();
				}
				return (TimeSpan)this.value;
			}
		}

		internal ExDateTime ExDateTime
		{
			get
			{
				if (this.valueType != CalendarValueType.DateTime)
				{
					throw new InvalidOperationException();
				}
				return (ExDateTime)((DateTime)this.value);
			}
		}

		internal CalendarValueType ValueType
		{
			get
			{
				return this.valueType;
			}
		}

		internal TriggerRelationship TriggerRelationship
		{
			get
			{
				return this.triggerRelationship;
			}
		}

		internal VAlarmAction Action
		{
			get
			{
				return this.action;
			}
		}

		internal string Message
		{
			get
			{
				return this.message;
			}
		}

		internal static void Demote(CalendarWriter calendarWriter, TimeSpan minutes, string description, string recipientAddress)
		{
			calendarWriter.StartComponent(ComponentId.VAlarm);
			calendarWriter.WriteProperty(PropertyId.Description, description);
			calendarWriter.StartProperty(PropertyId.Trigger);
			calendarWriter.WriteParameter("RELATED", "START");
			calendarWriter.WritePropertyValue(minutes);
			if (recipientAddress == null)
			{
				calendarWriter.WriteProperty(PropertyId.Action, "DISPLAY");
			}
			else
			{
				calendarWriter.WriteProperty(PropertyId.Action, "EMAIL");
				calendarWriter.WriteProperty(PropertyId.Summary, "Reminder");
				calendarWriter.WriteProperty(PropertyId.Attendee, recipientAddress);
			}
			calendarWriter.EndComponent();
		}

		public static int CalculateReminderMinutesBeforeStart(VAlarm valarm, ExDateTime startTime, ExDateTime endTime)
		{
			int result;
			if (valarm.ValueType == CalendarValueType.Duration)
			{
				if (valarm.TriggerRelationship == TriggerRelationship.Start || valarm.TriggerRelationship == TriggerRelationship.None)
				{
					result = ((valarm.TimeSpan != TimeSpan.MinValue) ? ((int)valarm.TimeSpan.Negate().TotalMinutes) : 15);
				}
				else if (valarm.TriggerRelationship == TriggerRelationship.End)
				{
					int num = (int)valarm.TimeSpan.TotalMinutes;
					ExDateTime dt = endTime.AddMinutes((double)num);
					result = (int)(startTime - dt).TotalMinutes;
				}
				else
				{
					result = 15;
				}
			}
			else
			{
				result = (int)(startTime - valarm.ExDateTime).TotalMinutes;
			}
			return result;
		}

		public static void PromoteEmailReminders(Item item, List<VAlarm> emailVAlarms, ExDateTime startTime, ExDateTime endTime, bool isOccurrence)
		{
			if (item != null && emailVAlarms != null && emailVAlarms.Count > 0)
			{
				Reminders<EventTimeBasedInboxReminder> reminders = new Reminders<EventTimeBasedInboxReminder>();
				foreach (VAlarm valarm in emailVAlarms)
				{
					int reminderOffset = VAlarm.CalculateReminderMinutesBeforeStart(valarm, startTime, endTime);
					EventTimeBasedInboxReminder eventTimeBasedInboxReminder = new EventTimeBasedInboxReminder();
					eventTimeBasedInboxReminder.CustomMessage = valarm.Message;
					eventTimeBasedInboxReminder.ReminderOffset = reminderOffset;
					eventTimeBasedInboxReminder.OccurrenceChange = (isOccurrence ? EmailReminderChangeType.Added : EmailReminderChangeType.None);
					reminders.ReminderList.Add(eventTimeBasedInboxReminder);
				}
				Reminders<EventTimeBasedInboxReminder>.Set(item, InternalSchema.EventTimeBasedInboxReminders, reminders);
			}
		}

		protected override void ProcessProperty(CalendarPropertyBase calendarProperty)
		{
			PropertyId propertyId = calendarProperty.CalendarPropertyId.PropertyId;
			if (propertyId != PropertyId.Description)
			{
				switch (propertyId)
				{
				case PropertyId.Action:
					if (string.Compare((string)calendarProperty.Value, "DISPLAY", StringComparison.CurrentCultureIgnoreCase) == 0)
					{
						this.action = VAlarmAction.Display;
						return;
					}
					if (string.Compare((string)calendarProperty.Value, "EMAIL", StringComparison.CurrentCultureIgnoreCase) == 0)
					{
						this.action = VAlarmAction.Email;
						return;
					}
					if (string.Compare((string)calendarProperty.Value, "AUDIO", StringComparison.CurrentCultureIgnoreCase) == 0)
					{
						this.action = VAlarmAction.Display;
						return;
					}
					this.action = VAlarmAction.Unknown;
					return;
				case PropertyId.Repeat:
					break;
				case PropertyId.Trigger:
					this.CheckTriggerArguments(calendarProperty);
					this.value = calendarProperty.Value;
					if (this.value != null && typeof(TimeSpan).GetTypeInfo().IsAssignableFrom(this.value.GetType().GetTypeInfo()))
					{
						this.valueType = CalendarValueType.Duration;
						return;
					}
					if (this.value != null && typeof(DateTime).GetTypeInfo().IsAssignableFrom(this.value.GetType().GetTypeInfo()))
					{
						this.valueType = CalendarValueType.DateTime;
						return;
					}
					this.valueType = calendarProperty.ValueType;
					return;
				default:
					return;
				}
			}
			else
			{
				this.message = (string)calendarProperty.Value;
			}
		}

		protected override bool ValidateProperties()
		{
			return this.value != null && this.action != VAlarmAction.Unknown;
		}

		private void CheckTriggerArguments(CalendarPropertyBase property)
		{
			this.triggerRelationship = TriggerRelationship.None;
			foreach (CalendarParameter calendarParameter in property.Parameters)
			{
				if (calendarParameter.ParameterId == ParameterId.TriggerRelationship)
				{
					if (string.Compare((string)calendarParameter.Value, "START", StringComparison.CurrentCultureIgnoreCase) == 0)
					{
						this.triggerRelationship = TriggerRelationship.Start;
						break;
					}
					if (string.Compare((string)calendarParameter.Value, "END", StringComparison.CurrentCultureIgnoreCase) == 0)
					{
						this.triggerRelationship = TriggerRelationship.End;
						break;
					}
					this.triggerRelationship = TriggerRelationship.Unknown;
					break;
				}
			}
		}

		private CalendarValueType valueType;

		private object value;

		private TriggerRelationship triggerRelationship;

		private VAlarmAction action = VAlarmAction.Unknown;

		private string message;
	}
}
