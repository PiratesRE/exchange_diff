using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class TimeZoneProperty : ComplexPropertyBase, IToXmlCommand, IToXmlForPropertyBagCommand, IToServiceObjectCommand, IToServiceObjectForPropertyBagCommand, ISetCommand, ISetUpdateCommand, IUpdateCommand, IPropertyCommand
	{
		private TimeZoneProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		private TimeZoneProperty(CommandContext commandContext, bool startProperty) : base(commandContext)
		{
			this.propertyIsStartTimeZone = startProperty;
			this.propertyInfo = (startProperty ? CalendarItemSchema.OrganizerSpecific.StartTimeZone : CalendarItemSchema.OrganizerSpecific.EndTimeZone);
		}

		public static TimeZoneProperty CreateCommandForStart(CommandContext commandContext)
		{
			return new TimeZoneProperty(commandContext, true);
		}

		public static TimeZoneProperty CreateCommandForEnd(CommandContext commandContext)
		{
			return new TimeZoneProperty(commandContext, false);
		}

		public void ToXml()
		{
			throw new InvalidOperationException("TimeZoneProperty.ToXml should not be called.");
		}

		public void ToServiceObject()
		{
			ToServiceObjectCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectCommandSettings>();
			StoreObject storeObject = commandSettings.StoreObject;
			MeetingRequest meetingRequest = storeObject as MeetingRequest;
			ExTimeZone timeZoneToRender;
			if (meetingRequest != null)
			{
				CalendarItemBase cachedEmbeddedItem = ((MeetingRequest)storeObject).GetCachedEmbeddedItem();
				timeZoneToRender = this.GetTimeZoneToRender(cachedEmbeddedItem);
			}
			else
			{
				CalendarItemBase calendarItemBase = storeObject as CalendarItemBase;
				timeZoneToRender = this.GetTimeZoneToRender(calendarItemBase);
			}
			if (timeZoneToRender != null)
			{
				TimeZoneDefinitionType timeZoneDefinitionType = new TimeZoneDefinitionType(timeZoneToRender);
				timeZoneDefinitionType.Render(true, EWSSettings.ClientCulture);
				commandSettings.ServiceObject.PropertyBag[this.propertyInfo] = timeZoneDefinitionType;
			}
		}

		public void ToXmlForPropertyBag()
		{
			throw new InvalidOperationException("TimeZoneProperty.ToXmlForPropertyBag should not be called.");
		}

		public void ToServiceObjectForPropertyBag()
		{
			ToServiceObjectForPropertyBagCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectForPropertyBagCommandSettings>();
			IDictionary<PropertyDefinition, object> propertyBag = commandSettings.PropertyBag;
			bool flag;
			if (this.propertyIsStartTimeZone && PropertyCommand.TryGetValueFromPropertyBag<bool>(propertyBag, CalendarItemBaseSchema.IsRecurring, out flag) && flag)
			{
				return;
			}
			byte[] bytes;
			if (PropertyCommand.TryGetValueFromPropertyBag<byte[]>(propertyBag, this.propertyIsStartTimeZone ? TimeZoneProperty.StartTimeZonePropertyDefinition : TimeZoneProperty.EndTimeZonePropertyDefinition, out bytes))
			{
				ExTimeZone utcTimeZone;
				if (!O12TimeZoneFormatter.TryParseTimeZoneBlob(bytes, string.Empty, out utcTimeZone))
				{
					utcTimeZone = ExTimeZone.UtcTimeZone;
				}
				TimeZoneDefinitionType timeZoneDefinitionType = new TimeZoneDefinitionType(utcTimeZone);
				timeZoneDefinitionType.Render(true, EWSSettings.ClientCulture);
				commandSettings.ServiceObject.PropertyBag[this.propertyInfo] = timeZoneDefinitionType;
			}
		}

		public void Set()
		{
			SetCommandSettings commandSettings = base.GetCommandSettings<SetCommandSettings>();
			TimeZoneDefinitionType timeZone = (TimeZoneDefinitionType)commandSettings.ServiceObject.PropertyBag[this.propertyInfo];
			CalendarItemBase calendarItemBase = (CalendarItemBase)commandSettings.StoreObject;
			this.SetProperty(calendarItemBase, timeZone);
		}

		public override void SetUpdate(SetPropertyUpdate setPropertyUpdate, UpdateCommandSettings updateCommandSettings)
		{
			TimeZoneDefinitionType timeZone = (TimeZoneDefinitionType)setPropertyUpdate.ServiceObject.PropertyBag[this.propertyInfo];
			CalendarItemBase calendarItemBase = (CalendarItemBase)updateCommandSettings.StoreObject;
			this.SetProperty(calendarItemBase, timeZone);
			CalendarItem calendarItem = updateCommandSettings.StoreObject as CalendarItem;
			if (calendarItem != null && this.propertyIsStartTimeZone && calendarItem.Recurrence != null)
			{
				RecurrenceHelper.Recurrence.CreateAndAssignRecurrence(calendarItem.Recurrence.Pattern, calendarItem.Recurrence.Range, calendarItem.StartTimeZone, calendarItem.Recurrence.ReadExTimeZone, calendarItem);
			}
		}

		private void SetProperty(CalendarItemBase calendarItemBase, TimeZoneDefinitionType timeZone)
		{
			if (this.propertyIsStartTimeZone)
			{
				if (PropertyCommand.StorePropertyExists(calendarItemBase, CalendarItemInstanceSchema.StartTime))
				{
					ExDateTime exDateTime = calendarItemBase.StartTimeZone.ConvertDateTime(calendarItemBase.StartTime);
					calendarItemBase.StartTime = timeZone.ExTimeZone.Assign(exDateTime);
				}
				else
				{
					calendarItemBase.StartTimeZone = timeZone.ExTimeZone;
				}
				calendarItemBase.Session.ExTimeZone = calendarItemBase.StartTimeZone;
				return;
			}
			if (PropertyCommand.StorePropertyExists(calendarItemBase, CalendarItemInstanceSchema.EndTime))
			{
				ExDateTime exDateTime = calendarItemBase.EndTimeZone.ConvertDateTime(calendarItemBase.EndTime);
				calendarItemBase.EndTime = timeZone.ExTimeZone.Assign(exDateTime);
				return;
			}
			calendarItemBase.EndTimeZone = timeZone.ExTimeZone;
		}

		private ExTimeZone GetTimeZoneToRender(CalendarItemBase calendarItemBase)
		{
			if (calendarItemBase == null)
			{
				return null;
			}
			if (!this.propertyIsStartTimeZone)
			{
				return calendarItemBase.EndTimeZone;
			}
			CalendarItem calendarItem = calendarItemBase as CalendarItem;
			if (calendarItem != null && calendarItem.Recurrence != null && calendarItem.Recurrence.HasTimeZone)
			{
				return calendarItem.Recurrence.CreatedExTimeZone;
			}
			return calendarItemBase.StartTimeZone;
		}

		public static readonly PropertyDefinition StartTimeZonePropertyDefinition = ItemSchema.TimeZoneDefinitionStart;

		public static readonly PropertyDefinition EndTimeZonePropertyDefinition = CalendarItemBaseSchema.TimeZoneDefinitionEnd;

		private bool propertyIsStartTimeZone;

		private PropertyInformation propertyInfo;
	}
}
