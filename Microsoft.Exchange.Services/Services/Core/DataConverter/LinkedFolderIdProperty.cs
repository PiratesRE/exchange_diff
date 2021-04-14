using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class LinkedFolderIdProperty : FolderIdProperty
	{
		private LinkedFolderIdProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		public static LinkedFolderIdProperty Create(CommandContext commandContext)
		{
			return new LinkedFolderIdProperty(commandContext);
		}

		public override void ToServiceObjectForPropertyBag()
		{
			ToServiceObjectForPropertyBagCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectForPropertyBagCommandSettings>();
			IDictionary<PropertyDefinition, object> propertyBag = commandSettings.PropertyBag;
			ServiceObject serviceObject = commandSettings.ServiceObject;
			IdAndSession idAndSession = commandSettings.IdAndSession;
			PropertyInformation propertyInformation = this.commandContext.PropertyInformation;
			byte[] entryId = null;
			if (PropertyCommand.TryGetValueFromPropertyBag<byte[]>(propertyBag, this.propertyDefinitions[0], out entryId))
			{
				StoreId storeId = StoreObjectId.FromProviderSpecificId(entryId, StoreObjectType.Folder);
				if (storeId != null && IdConverter.GetAsStoreObjectId(storeId).ProviderLevelItemId.Length > 0)
				{
					ConcatenatedIdAndChangeKey concatenatedId = IdConverter.GetConcatenatedId(storeId, idAndSession, null);
					serviceObject[propertyInformation] = this.CreateServiceObjectId(concatenatedId.Id, concatenatedId.ChangeKey);
				}
			}
		}

		protected override StoreId GetIdFromObject(StoreObject storeObject)
		{
			StoreId result = null;
			object obj = storeObject.TryGetProperty(this.propertyDefinitions[0]);
			if (obj != null)
			{
				byte[] array = obj as byte[];
				if (array != null)
				{
					result = StoreObjectId.FromProviderSpecificId(array, StoreObjectType.Folder);
				}
			}
			return result;
		}
	}
}
