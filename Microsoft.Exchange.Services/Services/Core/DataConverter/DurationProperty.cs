using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class DurationProperty : ComplexPropertyBase, IToXmlCommand, IToXmlForPropertyBagCommand, IToServiceObjectCommand, IToServiceObjectForPropertyBagCommand, IPropertyCommand
	{
		private DurationProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		public static DurationProperty CreateCommand(CommandContext commandContext)
		{
			return new DurationProperty(commandContext);
		}

		public void ToXml()
		{
			throw new InvalidOperationException("DurationProperty.ToXml should not be called.");
		}

		public void ToServiceObject()
		{
			ToServiceObjectCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectCommandSettings>();
			ServiceObject serviceObject = commandSettings.ServiceObject;
			StoreObject storeObject = commandSettings.StoreObject;
			if (PropertyCommand.StorePropertyExists(storeObject, CalendarItemInstanceSchema.StartTime) && PropertyCommand.StorePropertyExists(storeObject, CalendarItemInstanceSchema.EndTime))
			{
				string value = DurationProperty.ToString((ExDateTime)storeObject[CalendarItemInstanceSchema.StartTime], (ExDateTime)storeObject[CalendarItemInstanceSchema.EndTime]);
				serviceObject[this.commandContext.PropertyInformation] = value;
			}
		}

		public void ToXmlForPropertyBag()
		{
			throw new InvalidOperationException("DurationProperty.ToXmlForPropertyBag should not be called.");
		}

		public void ToServiceObjectForPropertyBag()
		{
			ToServiceObjectForPropertyBagCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectForPropertyBagCommandSettings>();
			IDictionary<PropertyDefinition, object> propertyBag = commandSettings.PropertyBag;
			ServiceObject serviceObject = commandSettings.ServiceObject;
			PropertyInformation propertyInformation = this.commandContext.PropertyInformation;
			ExDateTime end;
			ExDateTime start;
			if (PropertyCommand.TryGetValueFromPropertyBag<ExDateTime>(propertyBag, CalendarItemInstanceSchema.EndTime, out end) && PropertyCommand.TryGetValueFromPropertyBag<ExDateTime>(propertyBag, CalendarItemInstanceSchema.StartTime, out start))
			{
				serviceObject[this.commandContext.PropertyInformation] = DurationProperty.ToString(start, end);
			}
		}

		private static string ToString(ExDateTime start, ExDateTime end)
		{
			return DurationProperty.ToString(end - start);
		}

		private static string ToString(TimeSpan timeSpan)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (timeSpan.Ticks < 0L)
			{
				stringBuilder.Append("-P");
				if (timeSpan == TimeSpan.MinValue)
				{
					timeSpan = TimeSpan.MaxValue;
				}
				else
				{
					timeSpan = timeSpan.Negate();
				}
			}
			else
			{
				stringBuilder.Append("P");
			}
			bool flag = timeSpan.Hours != 0 || timeSpan.Minutes != 0 || timeSpan.Seconds != 0;
			if (timeSpan.Days != 0 || !flag)
			{
				stringBuilder.Append(timeSpan.Days);
				stringBuilder.Append("D");
			}
			if (flag)
			{
				stringBuilder.Append("T");
				if (timeSpan.Hours != 0)
				{
					stringBuilder.Append(timeSpan.Hours);
					stringBuilder.Append("H");
				}
				if (timeSpan.Minutes != 0)
				{
					stringBuilder.Append(timeSpan.Minutes);
					stringBuilder.Append("M");
				}
				if (timeSpan.Seconds != 0)
				{
					stringBuilder.Append(timeSpan.Seconds);
					stringBuilder.Append("S");
				}
			}
			return stringBuilder.ToString();
		}
	}
}
