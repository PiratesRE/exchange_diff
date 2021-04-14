using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class InboxRemindersProperty : ComplexPropertyBase, IToXmlCommand, IToXmlForPropertyBagCommand, IToServiceObjectCommand, ISetCommand, ISetUpdateCommand, IDeleteUpdateCommand, IUpdateCommand, IPropertyCommand
	{
		private InboxRemindersProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		public static InboxRemindersProperty CreateCommand(CommandContext commandContext)
		{
			return new InboxRemindersProperty(commandContext);
		}

		public void ToXml()
		{
			throw new InvalidOperationException("InboxRemindersProperty.ToXml should not be called.");
		}

		public void ToXmlForPropertyBag()
		{
			throw new InvalidOperationException("InboxRemindersProperty.ToXmlForPropertyBag should not be called.");
		}

		public void ToServiceObject()
		{
			ToServiceObjectCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectCommandSettings>();
			ServiceObject serviceObject = commandSettings.ServiceObject;
			StoreObject storeObject = commandSettings.StoreObject;
			InboxReminderType[] array = null;
			if (ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2012))
			{
				using (CalendarItemBase calendarItemBase = storeObject as CalendarItemBase)
				{
					if (calendarItemBase == null)
					{
						return;
					}
					calendarItemBase.Load(new PropertyDefinition[]
					{
						CalendarItemBaseSchema.EventTimeBasedInboxReminders
					});
					Reminders<EventTimeBasedInboxReminder> eventTimeBasedInboxReminders = calendarItemBase.EventTimeBasedInboxReminders;
					if (eventTimeBasedInboxReminders != null && eventTimeBasedInboxReminders.ReminderList.Count > 0)
					{
						array = new InboxReminderType[eventTimeBasedInboxReminders.ReminderList.Count];
						for (int i = 0; i < eventTimeBasedInboxReminders.ReminderList.Count; i++)
						{
							EventTimeBasedInboxReminder eventTimeBasedInboxReminder = eventTimeBasedInboxReminders.ReminderList[i];
							array[i] = new InboxReminderType
							{
								Id = eventTimeBasedInboxReminder.Identifier,
								ReminderOffset = eventTimeBasedInboxReminder.ReminderOffset,
								Message = eventTimeBasedInboxReminder.CustomMessage,
								IsOrganizerReminder = eventTimeBasedInboxReminder.IsOrganizerReminder,
								OccurrenceChange = (EmailReminderChangeType)eventTimeBasedInboxReminder.OccurrenceChange
							};
						}
					}
				}
			}
			serviceObject[CalendarItemSchema.InboxReminders] = array;
		}

		public void Set()
		{
			SetCommandSettings commandSettings = base.GetCommandSettings<SetCommandSettings>();
			CalendarItemBase calendarItem = (CalendarItemBase)commandSettings.StoreObject;
			InboxRemindersProperty.SetProperty(commandSettings.ServiceObject, calendarItem);
		}

		public override void SetUpdate(SetPropertyUpdate setPropertyUpdate, UpdateCommandSettings updateCommandSettings)
		{
			CalendarItemBase calendarItem = (CalendarItemBase)updateCommandSettings.StoreObject;
			ServiceObject serviceObject = setPropertyUpdate.ServiceObject;
			InboxRemindersProperty.SetProperty(serviceObject, calendarItem);
		}

		public override void DeleteUpdate(DeletePropertyUpdate deletePropertyUpdate, UpdateCommandSettings updateCommandSettings)
		{
			CalendarItemBase calendarItemBase = updateCommandSettings.StoreObject as CalendarItemBase;
			if (calendarItemBase == null)
			{
				throw new InvalidPropertyDeleteException(deletePropertyUpdate.PropertyPath);
			}
			InboxRemindersProperty.DeleteProperty(calendarItemBase);
		}

		internal static void SetProperty(ServiceObject serviceObject, CalendarItemBase calendarItem)
		{
			InboxReminderType[] array = (InboxReminderType[])serviceObject.PropertyBag[CalendarItemSchema.InboxReminders];
			if (array != null)
			{
				calendarItem.Load(new PropertyDefinition[]
				{
					CalendarItemBaseSchema.EventTimeBasedInboxReminders
				});
				Reminders<EventTimeBasedInboxReminder> eventTimeBasedInboxReminders = calendarItem.EventTimeBasedInboxReminders;
				Reminders<EventTimeBasedInboxReminder> reminders = new Reminders<EventTimeBasedInboxReminder>();
				foreach (InboxReminderType inboxReminderType in array)
				{
					Guid seriesReminderId = Guid.Empty;
					if (eventTimeBasedInboxReminders != null)
					{
						EventTimeBasedInboxReminder eventTimeBasedInboxReminder = (EventTimeBasedInboxReminder)eventTimeBasedInboxReminders.GetReminder(inboxReminderType.Id);
						if (eventTimeBasedInboxReminder != null)
						{
							seriesReminderId = eventTimeBasedInboxReminder.SeriesReminderId;
						}
					}
					EventTimeBasedInboxReminder item = new EventTimeBasedInboxReminder
					{
						Identifier = inboxReminderType.Id,
						ReminderOffset = inboxReminderType.ReminderOffset,
						CustomMessage = inboxReminderType.Message,
						IsOrganizerReminder = inboxReminderType.IsOrganizerReminder,
						OccurrenceChange = (EmailReminderChangeType)inboxReminderType.OccurrenceChange,
						SeriesReminderId = seriesReminderId
					};
					reminders.ReminderList.Add(item);
				}
				calendarItem.EventTimeBasedInboxReminders = reminders;
			}
		}

		internal static void DeleteProperty(CalendarItemBase calendarItem)
		{
			calendarItem.Load(new PropertyDefinition[]
			{
				CalendarItemBaseSchema.EventTimeBasedInboxReminders
			});
			calendarItem.EventTimeBasedInboxReminders = null;
		}
	}
}
