using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class DistinguishedFolderIdProperty : ComplexPropertyBase, IToXmlCommand, IToXmlForPropertyBagCommand, IToServiceObjectCommand, IToServiceObjectForPropertyBagCommand, IPropertyCommand
	{
		public DistinguishedFolderIdProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		private void ToServiceObject(StoreId storeId, StoreSession session, ServiceObject serviceObject)
		{
			if (EWSSettings.DistinguishedFolderIdNameDictionary == null)
			{
				EWSSettings.DistinguishedFolderIdNameDictionary = new DistinguishedFolderIdNameDictionary();
			}
			StoreObjectId asStoreObjectId = IdConverter.GetAsStoreObjectId(storeId);
			string value = EWSSettings.DistinguishedFolderIdNameDictionary.Get(asStoreObjectId, session);
			if (!string.IsNullOrEmpty(value))
			{
				PropertyInformation propertyInformation = this.commandContext.PropertyInformation;
				serviceObject[propertyInformation] = value;
			}
		}

		public static DistinguishedFolderIdProperty CreateCommand(CommandContext commandContext)
		{
			return new DistinguishedFolderIdProperty(commandContext);
		}

		public void ToXml()
		{
			throw new InvalidOperationException("DistinguishedFolderIdProperty.ToXml should not be called.");
		}

		public void ToXmlForPropertyBag()
		{
			throw new InvalidOperationException("DistinguishedFolderIdProperty.ToXml should not be called.");
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
			StoreId id = commandSettings.StoreObject.Id;
			this.ToServiceObject(id, commandSettings.IdAndSession.Session, commandSettings.ServiceObject);
		}

		public void ToServiceObjectForPropertyBag()
		{
			ToServiceObjectForPropertyBagCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectForPropertyBagCommandSettings>();
			IDictionary<PropertyDefinition, object> propertyBag = commandSettings.PropertyBag;
			StoreId storeId = null;
			if (PropertyCommand.TryGetValueFromPropertyBag<StoreId>(propertyBag, this.propertyDefinitions[0], out storeId))
			{
				this.ToServiceObject(storeId, commandSettings.IdAndSession.Session, commandSettings.ServiceObject);
			}
		}
	}
}
