using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class ConversationIdProperty : ComplexPropertyBase, IToXmlCommand, IToXmlForPropertyBagCommand, IToServiceObjectCommand, IToServiceObjectForPropertyBagCommand, IPropertyCommand
	{
		private ConversationIdProperty(CommandContext commandContext) : base(commandContext)
		{
			this.conversationIdDefinition = this.propertyDefinitions[0];
		}

		public static ConversationIdProperty CreateCommand(CommandContext commandContext)
		{
			return new ConversationIdProperty(commandContext);
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
			ConversationId conversationId = PropertyCommand.GetPropertyValueFromStoreObject(storeObject, this.conversationIdDefinition) as ConversationId;
			MailboxSession mailboxSession = commandSettings.IdAndSession.Session as MailboxSession;
			Guid guid;
			if (mailboxSession != null && mailboxSession.IsUnified)
			{
				guid = (Guid)PropertyCommand.GetPropertyValueFromStoreObject(storeObject, ConversationItemSchema.MailboxGuid);
				if (guid == Guid.Empty)
				{
					throw new RequiredPropertyMissingException(ResponseCodeType.ErrorRequiredPropertyMissing);
				}
			}
			else
			{
				guid = commandSettings.IdAndSession.Session.MailboxGuid;
			}
			string id = IdConverter.ConversationIdToEwsId(guid, conversationId);
			serviceObject.PropertyBag[propertyInformation] = this.CreateServiceObjectId(id, null);
		}

		public void ToServiceObjectForPropertyBag()
		{
			ToServiceObjectForPropertyBagCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectForPropertyBagCommandSettings>();
			IDictionary<PropertyDefinition, object> propertyBag = commandSettings.PropertyBag;
			ServiceObject serviceObject = commandSettings.ServiceObject;
			IdAndSession idAndSession = commandSettings.IdAndSession;
			PropertyInformation propertyInformation = this.commandContext.PropertyInformation;
			ConversationId conversationId = null;
			if (PropertyCommand.TryGetValueFromPropertyBag<ConversationId>(propertyBag, this.conversationIdDefinition, out conversationId))
			{
				MailboxSession mailboxSession = commandSettings.IdAndSession.Session as MailboxSession;
				Guid mailboxGuid;
				if (mailboxSession != null && mailboxSession.IsUnified)
				{
					if (!PropertyCommand.TryGetValueFromPropertyBag<Guid>(propertyBag, ConversationItemSchema.MailboxGuid, out mailboxGuid))
					{
						throw new RequiredPropertyMissingException(ResponseCodeType.ErrorRequiredPropertyMissing);
					}
				}
				else
				{
					mailboxGuid = commandSettings.IdAndSession.Session.MailboxGuid;
				}
				string id = IdConverter.ConversationIdToEwsId(mailboxGuid, conversationId);
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
			ConversationId conversationId = PropertyCommand.GetPropertyValueFromStoreObject(storeObject, this.conversationIdDefinition) as ConversationId;
			IdConverter.CreateConversationIdXml(serviceItem, conversationId, idAndSession, this.xmlLocalName);
		}

		public void ToXmlForPropertyBag()
		{
			ToXmlForPropertyBagCommandSettings commandSettings = base.GetCommandSettings<ToXmlForPropertyBagCommandSettings>();
			IDictionary<PropertyDefinition, object> propertyBag = commandSettings.PropertyBag;
			XmlElement serviceItem = commandSettings.ServiceItem;
			IdAndSession idAndSession = commandSettings.IdAndSession;
			ConversationId conversationId = null;
			PropertyCommand.TryGetValueFromPropertyBag<ConversationId>(propertyBag, this.conversationIdDefinition, out conversationId);
			IdConverter.CreateConversationIdXml(serviceItem, conversationId, idAndSession, this.xmlLocalName);
		}

		private PropertyDefinition conversationIdDefinition;
	}
}
