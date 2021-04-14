using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class DateTimeProperty : SimpleProperty
	{
		protected DateTimeProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		public new static DateTimeProperty CreateCommand(CommandContext commandContext)
		{
			return new DateTimeProperty(commandContext);
		}

		protected override string ToString(object propertyValue)
		{
			return ExDateTimeConverter.ToSoapHeaderTimeZoneRelatedXsdDateTime((ExDateTime)propertyValue);
		}

		protected override object Parse(string propertyString)
		{
			ExTimeZone exTimeZone = null;
			SetCommandSettings setCommandSettings = this.commandContext.CommandSettings as SetCommandSettings;
			if (setCommandSettings != null)
			{
				exTimeZone = setCommandSettings.StoreObject.Session.ExTimeZone;
			}
			else
			{
				UpdateCommandSettings updateCommandSettings = this.commandContext.CommandSettings as UpdateCommandSettings;
				if (updateCommandSettings != null)
				{
					exTimeZone = updateCommandSettings.StoreObject.Session.ExTimeZone;
				}
			}
			if (ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2010))
			{
				ExDateTime exDateTime = ExDateTimeConverter.ParseTimeZoneRelated(propertyString, EWSSettings.RequestTimeZone);
				return exDateTime;
			}
			ExDateTime exDateTime2 = ExDateTimeConverter.Parse(propertyString);
			if (exTimeZone == null)
			{
				return exDateTime2;
			}
			return exTimeZone.ConvertDateTime(exDateTime2);
		}

		protected override void SetProperty(ServiceObject serviceObject, StoreObject storeObject)
		{
			string propertyString = serviceObject[this.commandContext.PropertyInformation] as string;
			object value = this.Parse(propertyString);
			base.SetPropertyValueOnStoreObject(storeObject, this.propertyDefinition, value);
		}

		protected override void WriteServiceProperty(object propertyValue, ServiceObject serviceObject, PropertyInformation propInfo)
		{
			string propertyValue2 = this.ToString(propertyValue);
			base.WriteServiceProperty(propertyValue2, serviceObject, propInfo);
		}
	}
}
