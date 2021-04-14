using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class PersonaIdProperty : ComplexPropertyBase, IToXmlCommand, IToXmlForPropertyBagCommand, IToServiceObjectCommand, IToServiceObjectForPropertyBagCommand, IPropertyCommand
	{
		private PersonaIdProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		public static PersonaIdProperty CreateCommand(CommandContext commandContext)
		{
			return new PersonaIdProperty(commandContext);
		}

		private BaseItemId CreateServiceObjectId(string id, string changeKey)
		{
			return new ItemId(id, changeKey);
		}

		public override bool ToServiceObjectRequiresMailboxAccess
		{
			get
			{
				return true;
			}
		}

		public void ToServiceObject()
		{
			ToServiceObjectCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectCommandSettings>();
			StoreObject storeObject = commandSettings.StoreObject;
			ServiceObject serviceObject = commandSettings.ServiceObject;
			PropertyInformation propertyInformation = this.commandContext.PropertyInformation;
			PersonId personId = PropertyCommand.GetPropertyValueFromStoreObject(storeObject, PersonSchema.Id) as PersonId;
			string id = IdConverter.PersonIdToEwsId(commandSettings.IdAndSession.Session.MailboxGuid, personId);
			serviceObject.PropertyBag[propertyInformation] = this.CreateServiceObjectId(id, null);
		}

		public void ToServiceObjectForPropertyBag()
		{
			ToServiceObjectForPropertyBagCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectForPropertyBagCommandSettings>();
			IDictionary<PropertyDefinition, object> propertyBag = commandSettings.PropertyBag;
			ServiceObject serviceObject = commandSettings.ServiceObject;
			IdAndSession idAndSession = commandSettings.IdAndSession;
			PropertyInformation propertyInformation = this.commandContext.PropertyInformation;
			PersonId personId = null;
			if (PropertyCommand.TryGetValueFromPropertyBag<PersonId>(propertyBag, PersonSchema.Id, out personId))
			{
				string id = IdConverter.PersonIdToEwsId(idAndSession.Session.MailboxGuid, personId);
				serviceObject.PropertyBag[propertyInformation] = this.CreateServiceObjectId(id, null);
			}
		}

		public void ToXml()
		{
			ToXmlCommandSettings commandSettings = base.GetCommandSettings<ToXmlCommandSettings>();
			StoreObject storeObject = commandSettings.StoreObject;
			XmlElement serviceItem = commandSettings.ServiceItem;
			StoreSession session = storeObject.Session;
			IdAndSession idAndSession = commandSettings.IdAndSession;
			PersonId personId = PropertyCommand.GetPropertyValueFromStoreObject(storeObject, PersonSchema.Id) as PersonId;
			IdConverter.CreatePersonIdXml(serviceItem, personId, idAndSession, this.xmlLocalName);
		}

		public void ToXmlForPropertyBag()
		{
			ToXmlForPropertyBagCommandSettings commandSettings = base.GetCommandSettings<ToXmlForPropertyBagCommandSettings>();
			IDictionary<PropertyDefinition, object> propertyBag = commandSettings.PropertyBag;
			XmlElement serviceItem = commandSettings.ServiceItem;
			IdAndSession idAndSession = commandSettings.IdAndSession;
			PersonId personId;
			PropertyCommand.TryGetValueFromPropertyBag<PersonId>(propertyBag, PersonSchema.Id, out personId);
			IdConverter.CreatePersonIdXml(serviceItem, personId, idAndSession, this.xmlLocalName);
		}
	}
}
