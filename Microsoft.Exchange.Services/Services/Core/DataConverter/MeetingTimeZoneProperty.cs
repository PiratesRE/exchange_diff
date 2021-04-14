using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class MeetingTimeZoneProperty : ComplexPropertyBase, IToXmlCommand, IToServiceObjectCommand, ISetCommand, ISetUpdateCommand, IUpdateCommand, IPropertyCommand
	{
		private MeetingTimeZoneProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		public static MeetingTimeZoneProperty CreateCommand(CommandContext commandContext)
		{
			return new MeetingTimeZoneProperty(commandContext);
		}

		public void ToXml()
		{
			throw new InvalidOperationException("MeetingTimeZoneProperty.ToXml should not be called.");
		}

		public void ToServiceObject()
		{
			ToServiceObjectCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectCommandSettings>();
			if (ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2010))
			{
				if (commandSettings.ResponseShape != null && commandSettings.ResponseShape.AdditionalProperties != null)
				{
					foreach (PropertyPath propertyPath in commandSettings.ResponseShape.AdditionalProperties)
					{
						PropertyUri propertyUri = propertyPath as PropertyUri;
						if (propertyUri != null && propertyUri.Uri == PropertyUriEnum.MeetingTimeZone)
						{
							throw new InvalidPropertySetException((CoreResources.IDs)3384523424U, this.commandContext.PropertyInformation.PropertyPath);
						}
					}
				}
				ExTraceGlobals.CalendarDataTracer.TraceError((long)this.GetHashCode(), "Property " + PropertyUriEnum.MeetingTimeZone.ToString() + " is deprecated in this mode and is not returned in AllProperties shape");
				return;
			}
			ServiceObject serviceObject = commandSettings.ServiceObject;
			StoreObject storeObject = commandSettings.StoreObject;
			MeetingRequest meetingRequest = storeObject as MeetingRequest;
			if (meetingRequest != null)
			{
				CalendarItemBase cachedEmbeddedItem = ((MeetingRequest)storeObject).GetCachedEmbeddedItem();
				CalendarItem calendarItem = cachedEmbeddedItem as CalendarItem;
				if (calendarItem != null)
				{
					serviceObject[CalendarItemSchema.MeetingTimeZone] = RecurrenceHelper.MeetingTimeZone.Render(calendarItem);
					return;
				}
			}
			else
			{
				serviceObject[CalendarItemSchema.MeetingTimeZone] = RecurrenceHelper.MeetingTimeZone.Render(storeObject as CalendarItem);
			}
		}

		public void Set()
		{
			if (ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2010))
			{
				throw new InvalidPropertySetException((CoreResources.IDs)3384523424U, this.commandContext.PropertyInformation.PropertyPath);
			}
		}

		public override void SetUpdate(SetPropertyUpdate setPropertyUpdate, UpdateCommandSettings updateCommandSettings)
		{
			if (ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2010))
			{
				throw new InvalidPropertySetException((CoreResources.IDs)3384523424U, this.commandContext.PropertyInformation.PropertyPath);
			}
			CalendarItem calendarItem = updateCommandSettings.StoreObject as CalendarItem;
			if (calendarItem == null)
			{
				return;
			}
			if (calendarItem.Recurrence != null)
			{
				RecurrenceHelper.Recurrence.CreateAndAssignRecurrence(calendarItem.Recurrence.Pattern, calendarItem.Recurrence.Range, calendarItem.Session.ExTimeZone, calendarItem.Recurrence.ReadExTimeZone, calendarItem);
			}
		}
	}
}
