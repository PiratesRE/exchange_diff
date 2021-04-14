using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class ReminderDueByProperty : ComplexPropertyBase, IToXmlCommand, IToXmlForPropertyBagCommand, IToServiceObjectCommand, IToServiceObjectForPropertyBagCommand, ISetCommand, ISetUpdateCommand, IUpdateCommand, IPropertyCommand
	{
		private ReminderDueByProperty(CommandContext commandContext) : base(commandContext)
		{
			this.propertyDefinition = ItemSchema.ReminderDueBy;
		}

		public static ReminderDueByProperty CreateCommand(CommandContext commandContext)
		{
			return new ReminderDueByProperty(commandContext);
		}

		public void Set()
		{
		}

		void ISetCommand.SetPhase3()
		{
			this.SetPhase3();
			SetCommandSettings commandSettings = base.GetCommandSettings<SetCommandSettings>();
			string valueOrDefault = commandSettings.ServiceObject.GetValueOrDefault<string>(this.commandContext.PropertyInformation);
			Item item = (Item)commandSettings.StoreObject;
			this.SetProperty(item, valueOrDefault);
		}

		public override void SetUpdate(SetPropertyUpdate setPropertyUpdate, UpdateCommandSettings updateCommandSettings)
		{
			Item item = (Item)updateCommandSettings.StoreObject;
			string valueOrDefault = setPropertyUpdate.ServiceObject.GetValueOrDefault<string>(this.commandContext.PropertyInformation);
			this.SetProperty(item, valueOrDefault);
		}

		public void ToServiceObject()
		{
			ToServiceObjectCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectCommandSettings>();
			StoreObject storeObject = commandSettings.StoreObject;
			if (PropertyCommand.StorePropertyExists(storeObject, this.propertyDefinition))
			{
				ExDateTime systemDateTime = (ExDateTime)storeObject[this.propertyDefinition];
				commandSettings.ServiceObject[this.commandContext.PropertyInformation] = ExDateTimeConverter.ToSoapHeaderTimeZoneRelatedXsdDateTime(systemDateTime);
			}
		}

		public void ToServiceObjectForPropertyBag()
		{
			ToServiceObjectForPropertyBagCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectForPropertyBagCommandSettings>();
			ExDateTime systemDateTime;
			if (PropertyCommand.TryGetValueFromPropertyBag<ExDateTime>(commandSettings.PropertyBag, this.propertyDefinition, out systemDateTime))
			{
				commandSettings.ServiceObject[this.commandContext.PropertyInformation] = ExDateTimeConverter.ToSoapHeaderTimeZoneRelatedXsdDateTime(systemDateTime);
			}
		}

		private void SetProperty(Item item, string value)
		{
			CalendarItemBase calendarItemBase = item as CalendarItemBase;
			ExTimeZone timeZone;
			if (ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2010))
			{
				timeZone = ((calendarItemBase == null) ? item.Session.ExTimeZone : calendarItemBase.StartTimeZone);
			}
			else
			{
				timeZone = ExTimeZone.UtcTimeZone;
			}
			item[this.propertyDefinition] = ExDateTimeConverter.ParseTimeZoneRelated(value, timeZone);
		}

		public void ToXml()
		{
			ToXmlCommandSettings commandSettings = base.GetCommandSettings<ToXmlCommandSettings>();
			StoreObject storeObject = commandSettings.StoreObject;
			if (PropertyCommand.StorePropertyExists(storeObject, this.propertyDefinition))
			{
				ExDateTime systemDateTime = (ExDateTime)storeObject[this.propertyDefinition];
				base.CreateXmlTextElement(commandSettings.ServiceItem, this.xmlLocalName, ExDateTimeConverter.ToSoapHeaderTimeZoneRelatedXsdDateTime(systemDateTime));
			}
		}

		public void ToXmlForPropertyBag()
		{
			ToXmlForPropertyBagCommandSettings commandSettings = base.GetCommandSettings<ToXmlForPropertyBagCommandSettings>();
			ExDateTime systemDateTime;
			if (PropertyCommand.TryGetValueFromPropertyBag<ExDateTime>(commandSettings.PropertyBag, this.propertyDefinition, out systemDateTime))
			{
				base.CreateXmlTextElement(commandSettings.ServiceItem, this.xmlLocalName, ExDateTimeConverter.ToSoapHeaderTimeZoneRelatedXsdDateTime(systemDateTime));
			}
		}

		private PropertyDefinition propertyDefinition;
	}
}
