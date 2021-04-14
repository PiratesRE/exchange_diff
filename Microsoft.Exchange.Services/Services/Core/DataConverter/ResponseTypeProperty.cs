using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class ResponseTypeProperty : SimpleProperty, IToXmlCommand, IToXmlForPropertyBagCommand, IToServiceObjectCommand, IToServiceObjectForPropertyBagCommand, IPropertyCommand
	{
		private ResponseTypeProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		public new static ResponseTypeProperty CreateCommand(CommandContext commandContext)
		{
			return new ResponseTypeProperty(commandContext);
		}

		public override void ToServiceObject()
		{
			ToServiceObjectCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectCommandSettings>();
			PropertyInformation propertyInformation = this.commandContext.PropertyInformation;
			ServiceObject serviceObject = commandSettings.ServiceObject;
			StoreObject storeObject = commandSettings.StoreObject;
			ResponseType? responseType = null;
			CalendarItemBase calendarItemBase = commandSettings.StoreObject as CalendarItemBase;
			if (calendarItemBase != null)
			{
				responseType = new ResponseType?(calendarItemBase.ResponseType);
				return;
			}
			MeetingRequest meetingRequest = commandSettings.StoreObject as MeetingRequest;
			if (meetingRequest != null)
			{
				try
				{
					responseType = meetingRequest.GetCalendarItemResponseType();
				}
				catch (CorruptDataException arg)
				{
					ExTraceGlobals.CalendarDataTracer.TraceDebug<StoreObjectId, CorruptDataException>((long)this.GetHashCode(), "[ResponseTypeProperty::ToServiceObject] Failed in correlation for item {0} with corruptdata; Exception: '{1}'", meetingRequest.StoreObjectId, arg);
				}
				catch (CorrelationFailedException arg2)
				{
					ExTraceGlobals.CalendarDataTracer.TraceDebug<StoreObjectId, CorrelationFailedException>((long)this.GetHashCode(), "[ResponseTypeProperty::ToServiceObject] Failed in correlation for item {0} with correlationfailed; Exception: '{1}'", meetingRequest.StoreObjectId, arg2);
				}
				if (responseType != null)
				{
					this.WriteServiceProperty(responseType.Value, serviceObject, propertyInformation);
					return;
				}
			}
			else
			{
				base.ToServiceObject();
			}
		}
	}
}
