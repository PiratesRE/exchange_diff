using System;
using System.IO;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.TextConverters;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class CalendarItemRecurrenceProperty : ComplexPropertyBase, IToXmlCommand, IToServiceObjectCommand, ISetCommand, ISetUpdateCommand, IDeleteUpdateCommand, IUpdateCommand, IPropertyCommand
	{
		private CalendarItemRecurrenceProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		public static CalendarItemRecurrenceProperty CreateCommand(CommandContext commandContext)
		{
			return new CalendarItemRecurrenceProperty(commandContext);
		}

		public void ToXml()
		{
			throw new InvalidOperationException("CalendarItemRecurrenceProperty.ToXml should not be called.");
		}

		public void ToServiceObject()
		{
			ToServiceObjectCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectCommandSettings>();
			ServiceObject serviceObject = commandSettings.ServiceObject;
			StoreObject storeObject = commandSettings.StoreObject;
			MeetingMessage meetingMessage = storeObject as MeetingMessage;
			if (meetingMessage != null)
			{
				CalendarItem calendarItem = ((MeetingMessage)storeObject).GetCachedEmbeddedItem() as CalendarItem;
				serviceObject.PropertyBag[CalendarItemSchema.OrganizerSpecific.Recurrence] = RecurrenceHelper.Recurrence.Render(calendarItem.Recurrence);
				return;
			}
			CalendarItem calendarItem2 = storeObject as CalendarItem;
			if (calendarItem2 != null)
			{
				serviceObject.PropertyBag[CalendarItemSchema.OrganizerSpecific.Recurrence] = RecurrenceHelper.Recurrence.Render(calendarItem2.Recurrence);
			}
		}

		public void Set()
		{
			SetCommandSettings commandSettings = base.GetCommandSettings<SetCommandSettings>();
			CalendarItem calendarItem = commandSettings.StoreObject as CalendarItem;
			if (calendarItem != null && calendarItem.Body.Format == BodyFormat.TextHtml)
			{
				string value;
				using (TextReader textReader = calendarItem.Body.OpenTextReader(calendarItem.Body.Format))
				{
					value = textReader.ReadToEnd();
				}
				BodyWriteConfiguration bodyWriteConfiguration = new BodyWriteConfiguration(calendarItem.Body.Format);
				bodyWriteConfiguration.SetTargetFormat(BodyFormat.ApplicationRtf, calendarItem.Body.Charset);
				try
				{
					using (TextWriter textWriter = calendarItem.Body.OpenTextWriter(bodyWriteConfiguration))
					{
						textWriter.Write(value);
					}
				}
				catch (InvalidCharsetException)
				{
					throw new CalendarExceptionInvalidRecurrence();
				}
				catch (TextConvertersException)
				{
					throw new CalendarExceptionInvalidRecurrence();
				}
			}
		}

		void ISetCommand.SetPhase2()
		{
			this.SetPhase2();
			SetCommandSettings commandSettings = base.GetCommandSettings<SetCommandSettings>();
			CalendarItem calendarItem = commandSettings.StoreObject as CalendarItem;
			if (calendarItem == null)
			{
				throw new InvalidPropertySetException(this.commandContext.PropertyInformation.PropertyPath);
			}
			CalendarItemRecurrenceProperty.SetProperty(calendarItem, (RecurrenceType)commandSettings.ServiceObject[CalendarItemSchema.OrganizerSpecific.Recurrence], false);
		}

		public override void SetUpdate(SetPropertyUpdate setPropertyUpdate, UpdateCommandSettings updateCommandSettings)
		{
			CalendarItem calendarItem = updateCommandSettings.StoreObject as CalendarItem;
			if (calendarItem == null)
			{
				throw new InvalidPropertySetException(setPropertyUpdate.PropertyPath);
			}
			CalendarItemRecurrenceProperty.SetProperty(calendarItem, (RecurrenceType)setPropertyUpdate.ServiceObject.PropertyBag[CalendarItemSchema.OrganizerSpecific.Recurrence], true);
		}

		public override void DeleteUpdate(DeletePropertyUpdate deletePropertyUpdate, UpdateCommandSettings updateCommandSettings)
		{
			CalendarItem calendarItem = updateCommandSettings.StoreObject as CalendarItem;
			if (calendarItem == null)
			{
				throw new InvalidPropertyDeleteException(deletePropertyUpdate.PropertyPath);
			}
			calendarItem.Recurrence = null;
		}

		private static ExTimeZone GetSafeTimeZoneForRecurrence(ExTimeZone timeZone)
		{
			if (timeZone != ExTimeZone.UtcTimeZone)
			{
				return timeZone;
			}
			return EWSSettings.DefaultGmtTimeZone;
		}

		private static void SetProperty(CalendarItem calendarItem, RecurrenceType recurrenceType, bool performUpdate)
		{
			calendarItem.Session.ExTimeZone = CalendarItemRecurrenceProperty.GetSafeTimeZoneForRecurrence(calendarItem.Session.ExTimeZone);
			calendarItem.ExTimeZone = calendarItem.Session.ExTimeZone;
			ExTimeZone exTimeZone;
			if (ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2007SP1))
			{
				if (ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2010) && performUpdate)
				{
					if (calendarItem.Recurrence == null)
					{
						exTimeZone = calendarItem.StartTimeZone;
					}
					else
					{
						exTimeZone = calendarItem.Recurrence.CreatedExTimeZone;
					}
					exTimeZone = CalendarItemRecurrenceProperty.GetSafeTimeZoneForRecurrence(exTimeZone);
				}
				else
				{
					exTimeZone = calendarItem.Session.ExTimeZone;
				}
			}
			else
			{
				exTimeZone = null;
			}
			Recurrence recurrence;
			if (RecurrenceHelper.Recurrence.Parse(exTimeZone, recurrenceType, out recurrence))
			{
				try
				{
					calendarItem.Recurrence = recurrence;
				}
				catch (InvalidOperationException)
				{
					throw new CalendarExceptionInvalidRecurrence();
				}
				calendarItem[CalendarItemBaseSchema.IsRecurring] = true;
			}
		}
	}
}
